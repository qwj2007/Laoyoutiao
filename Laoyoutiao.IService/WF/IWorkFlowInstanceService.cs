using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.WF;
using Laoyoutiao.Models.Dto.WF.Instance;
using Laoyoutiao.Models.Dto.WF.Urge;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.Models.Entitys.WF;
using Laoyoutiao.WorkFlow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.IService.WF
{
    public interface IWorkFlowInstanceService : IBaseService<WF_WorkFlow_Instance>
    {
        /// <summary>
        /// 创建实例
        /// 注意事项：
        /// 1、流程开始节点不可添加任何条件分支（不符合逻辑，故人为规定）,即开始节点之后必须只能有一个任务节点，否则整个逻辑就错误了
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> CreateInstanceAsync(WorkFlowProcessTransition model);
        //Task<bool> CreateInstanceAsync(string url, long userId,  string sourceTable, long keyValue, string businessName = "",string businessCode="");
       // Task<bool> CreateInstanceAsync(string url, long userId, string sourceTable, long keyValue, string businessName = "", string businessCode = "")
        Task<WorkFlowProcess> GetProcessAsync(WorkFlowProcess process);
        Task<WorkFlowProcess> GetProcessForSystemAsync(SystemFlowDto model);
        Task<bool> ProcessTransitionFlowAsync(WorkFlowProcessTransition model);
        Task<IEnumerable<WF_WorkFlow_Operation_History>> GetFlowApprovalAsync(WorkFlowProcessTransition model);
        Task<WorkFlowImageDto> GetFlowImageAsync(string flowid, string? instanceId);
        Task<bool> UrgeAsync(UrgeEdit urge);
        Task<bool> WorkFlowWithdrawAsync(WorkFlowProcessTransition model);
        /// <summary>
        /// 获取用户待办事项
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<PageInfo> GetUserTodoListAsync(WorkFlowInstanceReq req);
        /// <summary>
        /// 同意操作
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> WorkFlowAgreeAsync(WorkFlowProcessTransition model);
        /// <summary>
        /// 不同意操作
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> WorkFlowDeprecateAsync(WorkFlowProcessTransition model);
        /// <summary>
        /// 退回操作
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> WorkFlowBackAsync(WorkFlowProcessTransition model);

    }
}
