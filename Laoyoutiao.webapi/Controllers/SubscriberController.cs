using DotNetCore.CAP;
using Laoyoutiao.IService.WF;
using Laoyoutiao.WorkFlow.Core;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Controllers
{
    public class SubscriberController : ControllerBase
    {
        private readonly IWorkFlowService workFlowService;

        public SubscriberController(IWorkFlowService workFlowService)
        {
            this.workFlowService = workFlowService;
        }

        /// <summary>
        /// CAP 改变OA系统表单流程状态
        /// </summary>
        /// <param name="statusChange"></param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.NonAction]
        [CapSubscribe("WorkFlowStatusChangedOA")]
        public async Task ChangeTableStatusAsync(WorkFlowStatusChange statusChange)
        {
            await workFlowService.ChangeTableStatusAsync(statusChange);
        }
    }
}
