using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.Models.Entitys.WF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.WF
{
    [TypeMapper(SourceType = typeof(WF_WorkFlow))]
    public class WFEdit: BaseDto
    {
        //public long? Id { get; set; }
        public string? FlowId { get; set; }
        public string? FlowCode { get; set; }
        public int? CategoryId { get; set; }
        public long? FormId { get; set; }
        public string? FlowName { get; set; }
        public string? FlowContent { get; set; }
        public string? Memo { get; set; }
        public int? IsOld { get; set; } = 0;
        public int? Enable { get; set; } = 1;
    }
}
