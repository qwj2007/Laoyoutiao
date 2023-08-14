using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
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
    }
}
