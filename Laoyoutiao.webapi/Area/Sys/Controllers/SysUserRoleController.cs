using Laoyoutiao.Common;
using Laoyoutiao.IService;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.Service;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Laoyoutiao.webapi.Area.Sys.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Area("Sys")]
    [Route("api/[controller]/[action]")]
    public class SysUserRoleController : BaseController<SysUserRole, SysUserRoleRes, SysUserRoleReq, SysUserRoleEdit>
    {

        private readonly ISysUserRoleService _sysUserRoleService;
        public SysUserRoleController(ISysUserRoleService sysUserRoleService) : base(sysUserRoleService)
        {
            _sysUserRoleService = sysUserRoleService;
        }
        /// <summary>
        /// 保存，用户一对多
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult> SaveUserRoles(List<SysUserRoleEdit> list,long userId)
        {
            long uId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);           
            var result = await _sysUserRoleService.SaveSysUserRole(list, userId, uId);
            if (result) {
                return ResultHelper.Success(result);
            }
            return ResultHelper.Error();


        }
        /// <summary>
        /// 保存,角色一对多
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult> SaveUserRoleByRoles(List<SysUserRoleEdit> list,long roleId)
        {
            long userId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
            var result = await _sysUserRoleService.SaveUserRoleByRoles(list, roleId,userId);
            if (result)
            {
                return ResultHelper.Success(result);
            }
            return ResultHelper.Error();
        }

        

        /// <summary>
        /// 根据角色Id查找用户信息
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult> GetExistUserByRoleId(long roleId)
        {          
            var result = await _sysUserRoleService.GetExistUserByRoleId(roleId);
            return ResultHelper.Success(result);
        }


    }
}
