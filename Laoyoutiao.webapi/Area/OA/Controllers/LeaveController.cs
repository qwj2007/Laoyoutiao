using Laoyoutiao.IService;
using Laoyoutiao.IService.OA;
using Laoyoutiao.IService.WF;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.OA.Leave;
using Laoyoutiao.Models.Entitys.OA;
using Laoyoutiao.Service.OA;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Area.OA.Controllers
{
    [Area("OA")]
    //[Authorize]
    //[ApiController]
    //[Route("api/[controller]/[action]")]
    public class LeaveController : BaseController<OALeave, LeaveRes, LeaveReq, LeaveEdit>
    {
        //private readonly ILeaveService _leaveService;
        //private readonly IWorkFlowInstanceService _workFlowInstanceService;
        //public LeaveController(ILeaveService leaveService) : base(leaveService)
        //{
        //    this._leaveService = leaveService;
        //    //this._workFlowInstanceService = workFlowInstanceService;
        //}
        //[HttpPost]
        //public override Task<ApiResult> GetPages(LeaveReq req)
        //{
        //    return base.GetPages(req);
        //}
        public LeaveController(IBaseService<OALeave> baseService) : base(baseService)
        {
        }
    }
}
