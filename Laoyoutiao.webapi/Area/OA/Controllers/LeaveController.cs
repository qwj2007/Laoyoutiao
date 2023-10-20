using Laoyoutiao.IService;
using Laoyoutiao.Models.Dto.OA.Leave;
using Laoyoutiao.Models.Entitys.OA;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Area.OA.Controllers
{
    [Area("OA")]
    public class LeaveController : BaseController<OALeave, LeaveRes, LeaveReq, LeaveEdit>
    {
        public LeaveController(IBaseService<OALeave> baseService) : base(baseService)
        {
        }
    }
}
