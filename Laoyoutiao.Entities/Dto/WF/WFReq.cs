using Laoyoutiao.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.WF
{
    public class WFReq: Pagination
    {
        public string? FlowName { get; set; }
    }
}
