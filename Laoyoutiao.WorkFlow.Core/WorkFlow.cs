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
        /// 流程名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 流程Id
        /// </summary>
        public Guid FlowId { get; set; }
        /// <summary>
        /// 流程实例Id
        /// </summary>
        public Guid InstanceId { get; set; }
        /// <summary>
        /// 开始节点Id
        /// </summary>
        public Guid StartNodeId { get; set; }
        /// <summary>
        /// 上一个节点Id
        /// </summary>
        public Guid PreviousId { get; set; }
        /// <summary>
        /// 当前节点Id
        /// </summary>
        public Guid ActivityNodeId { get; set; }
        /// <summary>
        /// 当前节点类型
        /// </summary>
        public WorkFlowInstanceNodeType ActivityNodeType { get; set; }
        public WorkFlowNode ActivityNode => Nodes[ActivityNodeId];
        /// <summary>
        /// 下一个节点Id
        /// </summary>
        public Guid NextNodeId { get; set; }
        /// <summary>
        /// 下个节点类型
        /// </summary>
        public WorkFlowInstanceNodeType NextNodeType { get; set; }
        /// <summary>
        /// 下一节点对象
        /// </summary>
        public WorkFlowNode NextNode => (NextNodeId != default(Guid)) ? Nodes[NextNodeId] : null;
        /// <summary>
        /// 全部节点
        /// </summary>
        public Dictionary<Guid, WorkFlowNode> Nodes { get; set; }
        /// <summary>
        /// 全部连线
        /// </summary>
        public Dictionary<Guid, List<WorkFlowEdge>> Edges { get; set; }
        /// <summary>
        /// 流程图Json
        /// </summary>
        public string FlowJson { get; set; }
        /// <summary>
        /// 节点集合
        /// </summary>
        //public List<WorkFlowNode> nodes { get; set; }
        ///// <summary>
        ///// 连线集合
        ///// </summary>
        //public List<WorkFlowEdge> edges { get; set; }
    }
}
