import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue()],
  base:"/dist",

  //运用代理处理跨域问题,这段代码只在开发服务器中器作用，发布到服务器将不会器作用，用nginx来处理。
  server:{
    port:3001,
    open:true,
    proxy:{ 
      '/api':{
       // target:'http://127.0.0.1:5041/api',      
        target:'http://localhost:5041/api',      
        changeOrigin:true,//是否跨域       
        rewrite:(path) => path.replace(/^\/api/,''),        
        ws : false//如果要代理 websockets，配置这个参数
      }
    },
  },
  build:{
    target:['edge90','chrome90','firefox90','safari15']
  }
})
