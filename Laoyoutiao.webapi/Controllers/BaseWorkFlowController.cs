using AutoMapper;
using DotNetCore.CAP;
using Laoyoutiao.Common;
using Laoyoutiao.IService;
using Laoyoutiao.IService.WF;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Service.WF;
using Laoyoutiao.WorkFlow.Core;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BaseWorkFlowController<T, TRes, TReq, TEdit> : BaseController<T, TRes, TReq, TEdit> where T : BaseEntity, new()
        where TRes : class where TReq : Pagination where TEdit : class
    {
        private readonly IBaseService<T> _baseService;
        private readonly IWorkFlowInstanceService _workFlowInstanceService;      

        //private readonly IMapper _mapper;
        //private readonly ICapPublisher capPublisher;
        public BaseWorkFlowController(IBaseService<T> baseService, IWorkFlowInstanceService workFlowInstanceService) : base(baseService)
        {
            _baseService = baseService;
            //this._workFlowInstanceService = new WorkFlowInstanceService(_mapper, capPublisher);
            this._workFlowInstanceService = workFlowInstanceService;
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
        #endregion
    }
}
