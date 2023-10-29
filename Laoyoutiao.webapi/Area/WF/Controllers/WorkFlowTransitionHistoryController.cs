using Laoyoutiao.Common;
using Laoyoutiao.IService;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.WF.Transition;
using Laoyoutiao.Models.Entitys.WF;
using Laoyoutiao.Service.WF;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Area.WF.Controllers
{
    [Area("WF")]
    public class WorkFlowTransitionHistoryController : BaseController<WF_WorkFlow_Transition_History, WorkFlowTransitionHistoryRes, WorkFlowTransitionHistoryReq, WorkFlowTransitionHistoryEdit>
    {
        private readonly WorkFlowTransitionHistoryService _service;
        public WorkFlowTransitionHistoryController(IBaseService<WF_WorkFlow_Transition_History> baseService) : base(baseService)
        {
            this._service = baseService as WorkFlowTransitionHistoryService;
        }

       

        /// <summary>
        /// 获取用户的部门Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult> GetWorkFlowTransitionHistroyByInstanceId(string instanceId)
        {
            return ResultHelper.Success(await _service.GetWorkFlowHistorySetp(instanceId));

        }
    }
}
