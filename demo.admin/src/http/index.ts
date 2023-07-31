
import axios from 'axios';


//需要拦截器的地方使用instance对象， 
//有自定义返回逻辑的地方沿用axios，在组件内部处理返回结果即可

import instance from './filter';
//getMenuDataNew, settingMenu 
//const http="http://localhost:3000/api";
//运用代理处理跨域问题
const http = "/api";
export const getMenuDataNew = async (parms: {}) => {
    //instance.defaults.headers.common['Authorization'] = "Bearer " + localStorage["token"];

    return instance.post(http + "/Menu/GetPages", parms);
}
export const addMenu =async (params: {}) => {
    //instance.defaults.headers.common['Authorization'] = "Bearer " + localStorage["token"];
    return instance.post(http + "/Menu/Add", params);
}
export const editMenu = async (params: {}) => {
    //instance.defaults.headers.common['Authorization'] = "Bearer " + localStorage["token"];
    return instance.post(http + "/Menu/Add", params);
}

//删除菜单
export const delMenu = async (id: number) => {
    console.log(id);
    //instance.defaults.headers.common["Authorization"] = "Bearer " + localStorage["token"];
    return instance.get(http + "/Menu/Del?id="+id);
}
export const batchDelMenu=async(ids:string)=>{
    //instance.defaults.headers.common["Authorization"] = "Bearer " + localStorage["token"];
    return instance.get(http + "/Menu/BatchDel?ids="+ids);
}


export const settingMenu = (rid: string, mids: string) => {  
   // instance.defaults.headers.common['Authorization'] = "Bearer " + localStorage["token"];
return instance.get(`${http}/Menu/SettingMenu?rid=${rid}&mids=${mids}`)}
//角色模块
//获取列表
export const getRoleData = async (parms: {}) => {
    //instance.defaults.headers.common['Authorization'] = "Bearer " + localStorage["token"];
    return instance.post(http + "/Role/GetPages", parms)
}
//添加
export const addRole = async (parms: {}) => {
    //instance.defaults.headers.common['Authorization'] = "Bearer " + localStorage["token"];
    return instance.post(http + "/Role/Add", parms)
}
//修改
export const editRole = async (parms: {}) => {
    //instance.defaults.headers.common['Authorization'] = "Bearer " + localStorage["token"];
    return instance.post(http + "/Role/Add", parms)
}
//删除
export const delRole = async (id: number) => {
    //instance.defaults.headers.common['Authorization'] = "Bearer " + localStorage["token"];
    return instance.get(http + "/Role/Del?id=" + id)
}
//BatchDel
export const batchDelRole = async (ids: string) => {
    //instance.defaults.headers.common['Authorization'] = "Bearer " + localStorage["token"];
    return instance.get(http + "/Role/BatchDel?ids=" + ids)
}

//用户模块
//获取列表
export const getUserData = async (parms: {}) => {
    //instance.defaults.headers.common['Authorization'] = "Bearer " + localStorage["token"];
    //return instance.post(http + "/User/GetUsers", parms)
     return instance.post(http + "/User/GetPages", parms)    
}
//添加
export const addUsers = async (parms: {}) => {
    //instance.defaults.headers.common['Authorization'] = "Bearer " + localStorage["token"];
    return instance.post(http + "/User/Add", parms)
}
//修改
export const editUsers = async (parms: {}) => {
    //instance.defaults.headers.common['Authorization'] = "Bearer " + localStorage["token"];
    return instance.post(http + "/User/Add", parms)
}
//删除
export const delUsers = async (id: number) => {
   // instance.defaults.headers.common['Authorization'] = "Bearer " + localStorage["token"];
    return instance.get(http + "/User/Del?id=" + id)
}
//BatchDel
export const batchDelUsers = async (ids: string) => {
    //instance.defaults.headers.common['Authorization'] = "Bearer " + localStorage["token"];
    return instance.get(http + "/User/BatchDel?ids=" + ids)
}
//分配
export const settingRole = async (pid: string,rids: string) => {
    //instance.defaults.headers.common['Authorization'] = "Bearer " + localStorage["token"];
    return instance.get(`${http}/User/SettingRole?pid=${pid}&rids=${rids}`)
}
//根据角色获取菜单
export const getUserMenus = async () => {
    //instance.defaults.headers.common['Authorization'] = "Bearer " + localStorage["token"];
    return instance.get(`${http}/Menu/GetUserMenus`)
}
//个人中心修改用户昵称和密码
export const editNickNameOrPassword = async (nickName: string,password: string) => {
    //instance.defaults.headers.common['Authorization'] = "Bearer " + localStorage["token"];
    return instance.get(`${http}/User/EditNickNameOrPassword?nickName=${nickName}&password=${password}`)
}




//const http="http://192.168.1.106:5041/api";
//获取token
export const getToken = (name: string, password: string) => {
    //return instance.get(http + "/Login/GetToken?name=" + name + "&password=" + password);
    //需要拦截器的地方使用instance对象， 
    //有自定义返回逻辑的地方沿用axios，在组件内部处理返回结果即可
    /**
     * 登录不需要拦截器，所以用axios
     */
    debugger
    const re=instance.get(http + "/Login/GetToken?name=" + name + "&password=" + password);
    return re;
    //return instance.get(http + "/Login/GetToken?name=" + name + "&password=" + password);
    
}
