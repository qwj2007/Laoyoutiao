import { createRouter, createWebHistory } from 'vue-router'
import LoginPage from '../view/index/LoginPage.vue'
import RootPage from '../view/index/RootPage.vue'
import DeskTop from '../view/index/DeskTop.vue'
import PersonCenter from '../view/index/PersonCenter.vue'
import Tabs from '../components/tabsCom.vue'
import { MenuModel } from '../view/admin/menu/class/MenuModel'
import Tool from '../global'
import { UserInfo } from '../view/index/class/UserInfo'
import { getUserMenus, getToken } from '../http/index'
import { useStore } from "vuex";
import { nextTick } from 'vue'
let route = [
    {
        name: "root",
        path: "/",
        component: RootPage,        
        children: [
            { name: "工作台", path: "/desktop", component: DeskTop}
           // { name: "个人信息", path: "/personinfo", component: PersonCenter},
            //{ name: "tab标签", path: "/tabs", component: Tabs }
        ]
    },
    { path: "/login", component: LoginPage, name: '登录'}
]
const store = useStore();
const toolObj = new Tool();
//当前存在用户信息且有效，才会读取动态路由

console.log('----------------开始------------');
//当前存在用户信息且有效，才会读取动态路由,
//防止浏览器刷新页面空白
if (localStorage["nickname"] != undefined) {
    const user: UserInfo = JSON.parse(new Tool().FormatToken(localStorage["token"]))
    const expDate = toolObj.FormatDate(user.exp)
    const currDate = toolObj.GetDate()
    if (expDate >= currDate) {
        //读取webapi，获取路由列表
        const list: MenuModel[] = await getUserMenus() as any as MenuModel[]
        let data = []
        if (list.length > 0) {
            for (let i = 0; i < list.length; i++) {
                //PS1：这里import动态路由导入，需要通过string+变量的方式导入，如果直接传入一个变量，编译器无法成功解析！ 
                //PS2：动态导入提示警告 需要加上/* @vite-ignore */才能消除警告
                data.push({ name: list[i].name, path: list[i].index, component: () => import(/* @vite-ignore */`${list[i].filePath}.vue`) })
            }
            route[0].children = route[0].children?.concat(data as []) as []
        }
    }
}
//递归查找到所有路由的children，并添加到路由中去
const getChildMenuToRouter = async (item: MenuModel) => {
    if (item.children != null && item.children != undefined && item.children != '[]') {
        const menutoroute = (item.children) as any as MenuModel[];
        if (menutoroute.length > 0) {
            menutoroute.forEach(async a => {
                router.addRoute('root', {
                    path: '/' + a.index,
                    name: a.name,                   
                    component: () => import(/* @vite-ignore */`${a.filePath}.vue`)
                });
                await getChildMenuToRouter(a);
            });
        }
    }
}

//创建路由
const router = createRouter({
    history: createWebHistory(),
    routes: route
});
// //路由守卫
router.beforeEach(async (to, form) => {
    //判断是否已经加上路由了,如果没有把路由加上
    if (localStorage["menus"] != undefined && localStorage["menus"] != null) {
        if (localStorage["addRouter"] == undefined || localStorage["addRouter"] == null) {
            const menutoroute = JSON.parse(localStorage["menus"]) as any as MenuModel[];
            if (menutoroute.length > 0) {
                menutoroute.forEach(async item => {
                    router.addRoute('root', {
                        path: '/' + item.index,
                        name: item.name,                      
                        component: () => import(/* @vite-ignore */`${item.filePath}.vue`)
                    });
                    //查找当前children
                    await getChildMenuToRouter(item);
                });
                localStorage["addRouter"] = 'true';
            }
        }
    }
    if (localStorage["token"] != undefined && localStorage["token"] != null) {
        const user: UserInfo = JSON.parse(new Tool().FormatToken(localStorage["token"]));
        let expDate: string = toolObj.FormatDate(user.exp);
        const currDate = toolObj.GetDate();
        //判断过期时间如果和现在的时间差小于5分钟，就重新获取一遍token        
        const nowDate = new Date().getTime() / 1000; //获取当前时间(从1970.1.1开始的毫秒数)
        if (user.exp - nowDate > 0 && user.exp - nowDate < 5 * 60) {
            const token = await (getToken(user.Name, user.Password)) as any as string;
            const userNew: UserInfo = JSON.parse(new Tool().FormatToken(localStorage["token"]));
            localStorage["token"] = token;
            store.commit("SettingToken", token);
            expDate = toolObj.FormatDate(userNew.exp);
        }

        if (to.path == "/login" || to.path == "/") {
            if (expDate >= currDate) {
                return { path: "/desktop" }
            } else {
                toolObj.ClearLocalStorage()                
            }
        } else {
            if (expDate < currDate) {
                toolObj.ClearLocalStorage()
                return { path: "/login" }
            }
            else{
                
            }
        }
    } else {
        //避免无限重定向，因此要做个判断
        if (to.path !== "/login") {
            return { path: "/login" }
        }
    }
//return{path:to.path}
});

export default router;