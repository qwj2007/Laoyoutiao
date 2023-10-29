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
        public async Task<List<WorkFlowTransitionHistoryRes>> GetWorkFlowHistorySetp(string InstanceId)
        {
            var list = await _db.Queryable<WF_WorkFlow_Transition_History>().WhereIF(!string.IsNullOrEmpty(InstanceId), a => a.InstanceId == InstanceId)
                    .OrderBy(a => a.CreateDate).Distinct().ToListAsync();
            return _mapper.Map<List<WorkFlowTransitionHistoryRes>>(list);
        }
    }
}
