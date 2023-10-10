using Laoyoutiao.Models.Entitys.WF;
using Laoyoutiao.WorkFlow.Core;

namespace Laoyoutiao.IService.WF
{
    public interface IWorkFlowService : IBaseService<WF_WorkFlow>
    {
        /// <summary>
        /// 更新表的流程状态
        /// </summary>
        /// <param name="statusChange"></param>
        /// <returns></returns>
        Task<bool> ChangeTableStatusAsync(WorkFlowStatusChange statusChange);
    }
}
