using AutoMapper;
using Laoyoutiao.IService.WF;
using Laoyoutiao.Models.Dto.WF.Transition;
using Laoyoutiao.Models.Entitys.WF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Service.WF
{
    public class WorkFlowTransitionHistoryService : BaseService<WF_WorkFlow_Transition_History>, IWorkFlowTransitionHistory
    {
        private readonly IMapper _mapper;
        public WorkFlowTransitionHistoryService(IMapper mapper) : base(mapper)
        {
            this._mapper = mapper;
        }


        /// <summary>
        ///根据InstanceId得到历史记录
        /// </summary>
        /// <param name="InstanceId"></param>
        /// <returns></returns>  

        public async Task<List<WorkFlowTransitionHistoryRes>> GetWorkFlowTransitionHistorySetp(string InstanceId)
        {
            var list = await _db.Queryable<WF_WorkFlow_Transition_History>().Select(it => new WF_WorkFlow_Transition_History
            {
                FromNodeId = it.FromNodeId,
                FromNodeName = it.FromNodeName
            })
                .WhereIF(!string.IsNullOrEmpty(InstanceId), a => a.InstanceId == InstanceId && a.IsDeleted == 0)
                .Distinct().ToListAsync();
            return _mapper.Map<List<WorkFlowTransitionHistoryRes>>(list);
        }
    }
}
