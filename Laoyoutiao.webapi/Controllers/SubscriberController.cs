using DotNetCore.CAP;
using Laoyoutiao.IService.WF;
using Laoyoutiao.WorkFlow.Core;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Controllers
{
    /// <summary>
    /// CAP订阅模式
    /// </summary>
    public class SubscriberController : ControllerBase
    {
        private readonly IWorkFlowService workFlowService;

        public SubscriberController(IWorkFlowService workFlowService)
        {
            this.workFlowService = workFlowService;
        }

        /// <summary>
        /// CAP 改变OA系统表单流程状态，CapSubscribe是CAP订阅模式
        /// </summary>
        /// <param name="statusChange"></param>
        /// <returns></returns>
        [NonAction]
        [CapSubscribe("WorkFlowStatusChanged")]
        public async Task ChangeTableStatusAsync(WorkFlowStatusChange statusChange)
        {
            await workFlowService.ChangeTableStatusAsync(statusChange);
        }
    }
}
