using Laoyoutiao.Common;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Laoyoutiao.webapi.Area.Sys.Controllers
{
    [Area("Sys")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PromiseController : ControllerBase
    {
        private readonly IUserRoleMenuService _userRoleMenuService;
        public PromiseController(IUserRoleMenuService userRoleMenuService) 
        {
            _userRoleMenuService = userRoleMenuService;
        }
        /// <summary>
        /// 获得菜单权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult> GetPromise(long userId) {
            var ser = await _userRoleMenuService.GetPromiseMenus(userId);
            return  ResultHelper.Success(ser);
        }
    }
}
