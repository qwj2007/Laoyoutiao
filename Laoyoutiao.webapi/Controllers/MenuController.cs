using demo.Model.Dto.Menu;
using Laoyoutiao.Common;
using Laoyoutiao.IService;
using Laoyoutiao.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _Menu;
        public MenuController(IMenuService Menu)
        {
            _Menu = Menu;
        }
        [HttpPost]
        public ApiResult Add(MenuAdd req)
        {
            long userId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
            //获取当前登录人信息 
            return ResultHelper.Success(_Menu.Add(req, userId));
        }
        [HttpPost]
        public ApiResult Edit(MenuEdit req)
        {
            long userId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
            //获取当前登录人信息
            return ResultHelper.Success(_Menu.Edit(req, userId));
        }
        [HttpGet]
        public ApiResult Del(long id)
        {
            //获取当前登录人信息 
            return ResultHelper.Success(_Menu.Del(id));
        }
        [HttpGet]
        public ApiResult BatchDel(string ids)
        {
            //获取当前登录人信息 
            return ResultHelper.Success(_Menu.BatchDel(ids));
        }
        [HttpPost]
        public ApiResult GetMenus(MenuReq req)
        {
            return ResultHelper.Success(_Menu.GetMenus(req));
        }
        [HttpGet]
        public ApiResult SettingMenu(long rid, string mids)
        {
            return ResultHelper.Success(_Menu.SettingMenu(rid, mids));
        }
        /// <summary>
        /// 根据当前登录用户获取用户菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResult GetUserMenus()
        {
            long userId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
            var list = _Menu.GetMenusByUserId(userId);
            return ResultHelper.Success(list);
        }
    }
}
