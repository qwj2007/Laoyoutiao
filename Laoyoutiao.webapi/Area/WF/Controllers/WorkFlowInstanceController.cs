﻿using Laoyoutiao.Common;
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
using Laoyoutiao.Models.Views;

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
            if (result)
            {
                return ResultHelper.Success(result);
            }
            return ResultHelper.Error("同意操作失败");
        }

        /// <summary>
        /// 不同意操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult> WorkFlowNoAgree(WorkFlowProcessTransition model)
        {
            var result = await _workFlowInstanceService.WorkFlowDeprecateAsync(model);
            if (result)
            {
                return ResultHelper.Success(result);
            }
            return ResultHelper.Error("不同意操作失败");
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
            if (result)
            {
                return ResultHelper.Success(result);
            }
            return ResultHelper.Error("流程提交失败");
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
            if (result)
            {
                return ResultHelper.Success(result);
            }
            return ResultHelper.Error("流程撤销失败");
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
            if (result)
            {
                return ResultHelper.Success(result);
            }
            return ResultHelper.Error("流程退回失败");
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

        /// <summary>
        /// 获取执行过的节点
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="currentNodeId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult> GetExcuteNodes(string instanceId,string currentNodeId)
        {
            var result = await _workFlowInstanceService.GetExcuteNodes(instanceId, currentNodeId);
            return ResultHelper.Success(result);
        }

        /// <summary>
        /// 获取执行过的线路
        /// </summary>
        /// <param name="instanceid"></param>
        /// <param name="currentNodeId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult> GetExcuteEdges(string instanceId, string currentNodeId) {
            var result = await _workFlowInstanceService.GetExcuteEdges(instanceId, currentNodeId);
            return ResultHelper.Success(result);
        }
        #endregion

    }
}
