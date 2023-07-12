/**
 * 存放静态路由
 */

import LoginPage from '../view/index/LoginPage.vue'
import RootPage from '../view/index/RootPage.vue'
import DeskTop from '../view/index/DeskTop.vue'
import PersonCenter from '../view/index/PersonCenter.vue'
export let route = [
    {
        path: "/", component: RootPage,
        children: [
            { name: "工作台", path: "/desktop", component: DeskTop },
            { name: "个人信息", path: "/personinfo", component: PersonCenter }
        ]
    },
    { path: "/login", component: LoginPage }
]
export default route;