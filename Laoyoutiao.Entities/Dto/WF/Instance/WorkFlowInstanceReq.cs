using Laoyoutiao.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.WF.Instance
{
    public class WorkFlowInstanceReq: Pagination
    {
        public string? FlowName { get; set; }
        public string? UserName { get; set; }
        public long? LoginUserId { get; set; }
        public string? BusinessName { get; set; }
    }
}
