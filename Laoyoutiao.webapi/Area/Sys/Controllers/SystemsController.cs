using Laoyoutiao.IService;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Area.Sys.Controllers
{ 
    [Area("sys")]
    public class SystemsController : BaseController<Systems, SystemsRes, SystemsReq, SystemsEdit>
    {
        public SystemsController(IBaseService<Systems> baseService) : base(baseService)
        {
        }
    }
}
