using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.WF;
using Laoyoutiao.Models.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.WF.Transition
{

    [TypeMapper(SourceType = typeof(WF_WorkFlow_Operation_History))]
    public class WorkFlowOperationHistoryRes
    {
       // public string InstanceId { get; set; }
        public string FromNodeId { get; set; }
        public string FromNodeName { get; set; }
        public string CreateDate { get; set; }
        public string Content { get; set; }
        public string CreateUserName { get; set; }
        public string NodeName { get; set; }
    }
}
