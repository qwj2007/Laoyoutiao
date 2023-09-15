using Laoyoutiao.IService;
using Laoyoutiao.Models.Dto.WF;
using Laoyoutiao.Models.Entitys.WF;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Area.WF.Controllers
{
    [Area("WF")]
    public class WorkFlowController : BaseController<WF_WorkFlow, WFRes, WFReq, WFEdit>
    {
        public WorkFlowController(IBaseService<WF_WorkFlow> baseService) : base(baseService)
        {
        }
    }
}
