using Laoyoutiao.Common;
using Laoyoutiao.IService;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.Service.Sys;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Area.Sys.Controllers
{
    [Area("Sys")]
    public class RoleMenuController : BaseController<RoleMenu, RoleMenuRes, RoleMenuReq, RoleMenuEdit>
    {
        private readonly IRoleMenuService _roleMenuService;
        public RoleMenuController(IRoleMenuService roleMenuService) : base(roleMenuService)
        {
            _roleMenuService = roleMenuService;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult> SaveRoleMenu(List<RoleMenuEdit> list, long roleId)
        {
            long uId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
            var result = await _roleMenuService.SaveRoleMenu(list, roleId, uId);
            return ResultHelper.Success(result);
        }   

        /// <summary>
        /// 根据角色Id查找信息
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult> GetExistMenuByRoleId(long roleId)
        {
            var result = await _roleMenuService.GetExistMenuByRoleId(roleId);
            return ResultHelper.Success(result);
        }
    }
}
