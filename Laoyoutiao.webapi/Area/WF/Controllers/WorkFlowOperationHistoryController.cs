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
    /// <summary>
    /// 流程操作历史
    /// </summary>
    [Area("WF")]
    public class WorkFlowOperationHistoryController : BaseController<WF_WorkFlow_Operation_History, WorkFlowOperationHistoryRes, WorkFlowOperationHistoryReq, WorkFlowOperationHistoryEdit>
    {
        private readonly WorkFlowOperationHistoryService _service;
        public WorkFlowOperationHistoryController(IBaseService<WF_WorkFlow_Operation_History> baseService) : base(baseService)
        {
            this._service = baseService as WorkFlowOperationHistoryService;
        }

       

        /// <summary>
        /// 获取用户的部门Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult> GetWorkFlowOperationByInstanceId(string instanceId)
        {
            return ResultHelper.Success(await _service.GetWorkFlowHistorySetp(instanceId));

        }
    }
}
