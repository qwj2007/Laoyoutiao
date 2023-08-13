using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Dto.Menu;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Area.Sys.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Area("sys")]
    public class MenusController : BaseController<Menus, MenuRes, MenuReq, MenuEdit>
    {
        private readonly IMenusService _menuService;
        public MenusController(IMenusService menuService) : base(menuService)
        {
            _menuService = menuService;
        }
    }
}
