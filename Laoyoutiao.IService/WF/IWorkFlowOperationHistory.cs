using Laoyoutiao.Models.Dto.WF.Transition;
using Laoyoutiao.Models.Entitys.WF;

namespace Laoyoutiao.IService.WF
{
    public interface IWorkFlowOperationHistory : IBaseService<WF_WorkFlow_Operation_History>
    {
        /// <summary>
        /// 返回历史步骤
        /// </summary>
        /// <param name="InstanceId"></param>
        /// <returns></returns>
        Task<List<WorkFlowOperationHistoryRes>> GetWorkFlowHistorySetp(string InstanceId);
    }
}
