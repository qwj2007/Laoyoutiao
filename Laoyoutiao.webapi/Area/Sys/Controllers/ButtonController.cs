using Laoyoutiao.IService;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Dto.Sys.Buttom;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Area.Sys.Controllers
{
    [Area("Sys")]
    public class ButtonController : BaseController<SysButton, BtnRes, BtnReq, BtnEdit>
    {
        public ButtonController(IBaseService<SysButton> baseService) : base(baseService)
        {
        }
    }
}
