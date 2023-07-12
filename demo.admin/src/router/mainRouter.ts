/**
 * 存放动态路由
 */

import routes from './staticRoutes'
import { MenuModel } from '../view/admin/menu/class/MenuModel'
import Tool from '../global'
import { UserInfo } from '../view/index/class/UserInfo'
import { getUserMenus, getToken } from '../http/index'
const toolObj = new Tool();
const route=routes;
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

export default route;