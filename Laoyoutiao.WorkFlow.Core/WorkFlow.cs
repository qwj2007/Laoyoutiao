using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    public class WorkFlow
    {
        /// <summary>
        /// 节点集合
        /// </summary>
        public List<WorkFlowNode> nodes { get; set; }
        /// <summary>
        /// 连线集合
        /// </summary>
        public List<WorkFlowEdge> edges { get; set; }
    }
}
