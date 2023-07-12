/**
 * 引入路由
 */
import router from './index0'
import routes from './staticRoutes'

import { MenuModel } from '../view/admin/menu/class/MenuModel'
import Tool from '../global'
import { UserInfo } from '../view/index/class/UserInfo'
import { getUserMenus, getToken } from '../http/index'
const toolObj = new Tool();
// const handleRoutes = (menuList: List) => {
//     if (!menuList || menuList.length === 0) {
//         return false
//     }
// }
// function handleRoutes1(menuList) {
//     if (!menuList || menuList.length === 0) {
//         return false
//     }
//     let whiteList = ['55555', '12']
//     let userId = localStorage.getItem('wx')
//     for (let i in whiteList) {
//         if (whiteList[i] === userId) {　　　　　　// 按照自己项目逻辑做不同的处理
//             menuList.push({
//                 path: '/tem',
//                 name: 'Tem',
//                 component: () => import('../pages/Tem/index.vue')
//             })
//             break
//         }
//     }
//     return [...menuList]
// }
debugger
router.beforeEach(async (to, from, next) => {
    // if (localStorage["nickname"] != undefined) {
    //     const user: UserInfo = JSON.parse(new Tool().FormatToken(localStorage["token"]))
    //     const expDate = toolObj.FormatDate(user.exp)
    //     const currDate = toolObj.GetDate()
    //     if (expDate >= currDate) {
    //         //读取webapi，获取路由列表
    //         const list: MenuModel[] = await getUserMenus() as any as MenuModel[]
    //         let data = []
    //         if (list.length > 0) {
    //             for (let i = 0; i < list.length; i++) {
    //                 //PS1：这里import动态路由导入，需要通过string+变量的方式导入，如果直接传入一个变量，编译器无法成功解析！ 
    //                 //PS2：动态导入提示警告 需要加上/* @vite-ignore */才能消除警告
    //                 data.push({ name: list[i].name, path: list[i].index, component: () => import(/* @vite-ignore */`${list[i].filePath}.vue`) })
    //             }
    //             routes[0].children = routes[0].children?.concat(data as []) as []
    //         }
    //     }
    // }

    if (localStorage["token"] != undefined && localStorage["token"] != null) {
        const user: UserInfo = JSON.parse(new Tool().FormatToken(localStorage["token"]));
        const list: MenuModel[] = await getUserMenus() as any as MenuModel[]       

        let expDate: string = toolObj.FormatDate(user.exp);
        const currDate = toolObj.GetDate();
        //判断过期时间如果和现在的时间差小于5分钟，就重新获取一遍token        
        const nowDate = new Date().getTime() / 1000; //获取当前时间(从1970.1.1开始的毫秒数)
        if (user.exp - nowDate > 0 && user.exp - nowDate < 5 * 60) {
            const token = await (getToken(user.Name, user.Password)) as any as string;
            const userNew: UserInfo = JSON.parse(new Tool().FormatToken(localStorage["token"]));
            localStorage["token"] = token;
            //store.commit("SettingToken", token);
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
        }
    } else {
        //避免无限重定向，因此要做个判断
        if (to.path !== "/login") {
            return { path: "/login" }
        }
    }
});

export default router;