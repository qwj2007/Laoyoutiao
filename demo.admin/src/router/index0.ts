
/**
 * 存放基本的路由设置
 */

import { createRouter, createWebHistory } from 'vue-router'
import routes from './mainRouter'
import { MenuModel } from '../view/admin/menu/class/MenuModel'
import Tool from '../global'
import { UserInfo } from '../view/index/class/UserInfo'
import { getUserMenus, getToken } from '../http/index'
const toolObj = new Tool();
//创建路由
const router = createRouter({
    history: createWebHistory(),
    routes: routes
});
console.log(routes);
router.beforeEach(async (to, from, next) => {  
    
    // if (localStorage["token"] != undefined && localStorage["token"] != null) {
    //     const user: UserInfo = JSON.parse(new Tool().FormatToken(localStorage["token"]));
    //     const list: MenuModel[] = await getUserMenus() as any as MenuModel[]       

    //     let expDate: string = toolObj.FormatDate(user.exp);
    //     const currDate = toolObj.GetDate();
    //     //判断过期时间如果和现在的时间差小于5分钟，就重新获取一遍token        
    //     const nowDate = new Date().getTime() / 1000; //获取当前时间(从1970.1.1开始的毫秒数)
    //     if (user.exp - nowDate > 0 && user.exp - nowDate < 5 * 60) {
    //         const token = await (getToken(user.Name, user.Password)) as any as string;
    //         const userNew: UserInfo = JSON.parse(new Tool().FormatToken(localStorage["token"]));
    //         localStorage["token"] = token;
    //         //store.commit("SettingToken", token);
    //         expDate = toolObj.FormatDate(userNew.exp);
    //     }
    
    //     if (to.path == "/login" || to.path == "/") {
    //         if (expDate >= currDate) {
    //             return { path: "/desktop" }
    //         } else {
    //             toolObj.ClearLocalStorage()
    //         }
    //     } else {
    //         if (expDate < currDate) {
    //             toolObj.ClearLocalStorage()
    //             return { path: "/login" }
    //         }
    //     }
    // } else {
    //     //避免无限重定向，因此要做个判断
    //     if (to.path !== "/login") {
    //         return { path: "/login" }
    //     }
    // }
});


export default router;