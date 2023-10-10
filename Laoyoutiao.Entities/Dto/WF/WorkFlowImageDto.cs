using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.WF
{
    public class WorkFlowImageDto
    {
        /// <summary>
        /// 工作流ID
        /// </summary>
        public string FlowId { get; set; }

        /// <summary>
        /// 实例ID
        /// </summary>
        public string InstanceId { get; set; }

        /// <summary>
        /// 当前节点ID
        /// </summary>
        public string CurrentNodeId { get; set; }

        /// <summary>
        /// 流程JSON内容
        /// </summary>
        public string FlowContent { get; set; }
    }
}
