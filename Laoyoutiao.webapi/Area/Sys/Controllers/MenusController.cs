using Laoyoutiao.Common;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.Service.Sys;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Area.Sys.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Area("Sys")]
    public class MenusController : BaseTreeController<Menus, MenusRes, MenusReq, MenusEdit>
    {
        private readonly IMenusService _menuService;
        public MenusController(IMenusService menuService) : base(menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// 获取页面的按钮信息
        /// </summary>
        /// <returns></returns>
        ///       
        [HttpGet]
        public async Task<ApiResult> GetChildButtons(long parentId)
        {
            return ResultHelper.Success(await _menuService.GetChildButtons(parentId));
        }
    }
}
