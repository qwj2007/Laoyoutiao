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
        /// 提交审批
        /// </summary>
        /// <param name="req">请求参数</param>
        ///  /// <param name="url">路由地址</param>
        ///   /// <param name="userId">当前用户Id</param>
        /// <returns></returns>(string url,long userId,string instanceId,string SourceTable,long KeyValue)
        [HttpPost]
        public virtual async Task<ApiResult> Commit(TEdit req, string url, long userId,string sourceTable,long keyValue,string businessName)
        {
            //提交先保存，在提交
            var result = await _baseService.AddOrUpdateReturnEntity(req, userId);
            bool isOk = false;
            if (result.Id>0)
            {
                if (keyValue == 0) {
                    keyValue = result.Id;
                }
                //提交操作
                isOk = await _workFlowInstanceService.CreateInstanceAsync(url, userId, sourceTable, keyValue,businessName,result.Code);
            }
            return ResultHelper.Success(isOk);
        }

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

        
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <param name="url"></param>
        /// <param name="userId"></param>
        /// <param name="sourceTable"></param>
        /// <param name="keyValue"></param>
        /// <param name="businessName"></param>
        /// <returns></returns>
        #region 撤回操作，只有本人才能撤回没有被审核的数据
        //public virtual async Task<ApiResult> WithDraw(string url, long userId, string sourceTable, long keyValue, string businessName)
        //{
        //    //提交先保存，在提交
        //    var result = await _baseService.AddOneRerunKeyValue(req, userId);
        //    bool isOk = false;
        //    if (result > 0)
        //    {
        //        if (keyValue == 0)
        //        {
        //            keyValue = result;
        //        }
        //        //提交操作
        //        isOk = await _workFlowInstanceService.CreateInstanceAsync(url, userId, sourceTable, keyValue, businessName);
        //    }
        //    return ResultHelper.Success(isOk);
        //}
        #endregion
    }
}
