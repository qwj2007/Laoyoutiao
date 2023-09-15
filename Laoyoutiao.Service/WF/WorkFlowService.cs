using AutoMapper;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.IService.WF;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.Models.Entitys.WF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Service.WF
{
    public class WorkFlowService : BaseService<WF_WorkFlow>, IWorkFlowService
    {
        public WorkFlowService(IMapper mapper) : base(mapper)
        {
        }
    }
}
