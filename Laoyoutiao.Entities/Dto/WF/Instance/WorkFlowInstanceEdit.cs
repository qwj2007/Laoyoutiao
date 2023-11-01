using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.WF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.WF.Instance
{
    [TypeMapper(SourceType = typeof(WF_WorkFlow_Instance))]
    public class WorkFlowInstanceEdit:BaseDto
    {
        public string? BusinessName { get; set; }
    }
}
