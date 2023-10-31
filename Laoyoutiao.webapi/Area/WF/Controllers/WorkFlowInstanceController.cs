using Laoyoutiao.Common;
using Laoyoutiao.IService;
using Laoyoutiao.IService.WF;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.WF;
using Laoyoutiao.Models.Dto.WF.Instance;
using Laoyoutiao.Models.Entitys.WF;
using Laoyoutiao.Service.WF;
using Laoyoutiao.Service;
using Laoyoutiao.webapi.Controllers;
using Laoyoutiao.WorkFlow.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Area.WF.Controllers
{
    /// <summary>
    /// 工作流操作
    /// </summary>
    [Area("WF")]
    public class WorkFlowInstanceController : BaseController<WF_WorkFlow_Instance, WorkFlowInstanceRes, WorkFlowInstanceReq, WorkFlowInstanceEdit>
    {
        public readonly IWorkFlowInstanceService _workFlowInstanceService;
        public WorkFlowInstanceController(IWorkFlowInstanceService workFlowInstanceService) : base(workFlowInstanceService)
        {
            _workFlowInstanceService = workFlowInstanceService;
        }

        /// <summary>
        /// 获取用户的待办信息
        /// </summary>
        /// <param name="req">传递的参数</param>
        /// <returns></returns>
        [HttpGet, HttpPost]
        public async Task<ApiResult> GetUserTodoListAsync(WorkFlowInstanceReq req)
        {
            var result = await _workFlowInstanceService.GetUserTodoListAsync(req);
            return ResultHelper.Success(result);
        }
        #region 审批流程


        /// <summary>
        /// 同意操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult> WorkFlowAgree(WorkFlowProcessTransition model)
        {
            var result = await _workFlowInstanceService.WorkFlowAgreeAsync(model);
            return ResultHelper.Success(result);
        }

        /// <summary>
        /// 不同意操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult> WorkFlowNoAgree(WorkFlowProcessTransition model)
        {
            var result = await _workFlowInstanceService.WorkFlowDeprecateAsync(model);
            return ResultHelper.Success(result);
        }

        /// <summary>
        /// 工作流程提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<ApiResult> WrokFlowCommit(WorkFlowProcessTransition model)
        {
            //提交先保存，在提交
            var result = await _workFlowInstanceService.CreateInstanceAsync(model);
            return ResultHelper.Success(result);
        }
        /// <summary>
        /// 撤销
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<ApiResult> WorkFlowWithdrawAsync(WorkFlowProcessTransition model)
        {
            //提交先保存，在提交
            var result = await _workFlowInstanceService.WorkFlowWithdrawAsync(model);
            return ResultHelper.Success(result);
        }

        /// <summary>
        /// 退回
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<ApiResult> WorkFlowBackAsync(WorkFlowProcessTransition model)
        {
            var result = await _workFlowInstanceService.WorkFlowBackAsync(model);
            return ResultHelper.Success(result);
        }

        /// <summary>
        /// 获取流程图信息
        /// </summary>
        /// <param name="flowid"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<ApiResult> GetFlowImageAsync(string? flowid, string? instanceId)
        {
            var result = await _workFlowInstanceService.GetFlowImageAsync(flowid, instanceId);
            return ResultHelper.Success(result);
        }

        /// <summary>
        /// 获取审批意见
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult> GetFlowApprovalAsync(string instanceId)
        {
            var result = await _workFlowInstanceService.GetFlowApprovalAsync(instanceId);
            return ResultHelper.Success(result);
        }
        #endregion

    }
}
