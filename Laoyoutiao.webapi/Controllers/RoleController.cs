using Laoyoutiao.Common;
using Laoyoutiao.IService;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Menu;
using Laoyoutiao.Models.Dto.Role;
using Laoyoutiao.Models.Entitys;
using Laoyoutiao.Models.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RoleController : BaseController<Role, RoleRes, RoleReq, RoleEdit>
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService):base(roleService)
        {
            _roleService = roleService;
        }
        //[HttpPost]
        //public ApiResult GetRoles(RoleReq req)
        //{
        //    return ResultHelper.Success(_role.GetRoles(req));
        //}
        //[HttpGet]
        //public ApiResult GetRole(long id)
        //{
        //    return ResultHelper.Success(_role.GetRoleById(id));
        //}

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        //[HttpPost]
        //public ApiResult Add(RoleAdd req)
        //{
        //    //获取当前登录人信息
        //    long userId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
        //    return ResultHelper.Success(_role.Add(req, userId));
        //}
        //[HttpPost]
        //public ApiResult Edit(RoleEdit req)
        //{
        //    //获取当前登录人信息
        //    long userId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
        //    return ResultHelper.Success(_role.Edit(req, userId));
        //}
        //[HttpGet]
        //public ApiResult Del(long id)
        //{
        //    //获取当前登录人信息 
        //    return ResultHelper.Success(_role.Del(id));
        //}
        //[HttpGet]
        //public ApiResult BatchDel(string ids)
        //{
        //    //获取当前登录人信息 
        //    return ResultHelper.Success(_role.BatchDel(ids));
        //}
    }
}
