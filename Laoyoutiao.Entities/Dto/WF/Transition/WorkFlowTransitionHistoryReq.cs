using Laoyoutiao.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.WF.Transition
{
    public class WorkFlowTransitionHistoryReq: Pagination
    {
        public string InstanceId { get; set; }
    }
}
