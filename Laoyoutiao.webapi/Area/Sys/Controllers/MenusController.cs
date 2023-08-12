using Laoyoutiao.IService;
using Laoyoutiao.Models.Dto.Menu;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Area.Sys.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Area("sys")]
    public class MenusController : BaseController<Menus, MenuRes, MenuReq, MenuEdit>
    {
        public MenusController(IBaseService<Menus> baseService) : base(baseService)
        {
        }
    }
}
