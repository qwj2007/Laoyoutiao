using Laoyoutiao.WorkFlow.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Service.WF
{
    public class MsWorkFlowContext : WorkFlowContext
    {
        public MsWorkFlowContext(WorkFlow.Core.WorkFlow workFlow)
        {
            if (workFlow.FlowId == default(Guid))
            {
                throw new ArgumentNullException("FlowId", "input workflow flowid is null");
            }
            if (string.IsNullOrEmpty(workFlow.FlowJson))
            {
                throw new ArgumentException("FlowJSON", "input workflow json is null");
            }
            if (workFlow.ActivityNodeId == null)
            {
                throw new ArgumentException("ActivityNodeId", "input workflow ActivityNodeId is null");
            }

            this.WorkFlow = workFlow;

            dynamic jsonobj = JsonConvert.DeserializeObject(this.WorkFlow.FlowJson);
            //获取节点
            this.WorkFlow.Nodes = this.GetNodes(jsonobj.nodes);
            //获取连线
            this.WorkFlow.Edges = this.GetFromLines(jsonobj.edges);
            //if (workFlow.NextNodeType != WorkFlowInstanceNodeType.End)
            //{
            this.WorkFlow.ActivityNodeId = workFlow.ActivityNodeId == default(Guid) ? this.WorkFlow.StartNodeId : workFlow.ActivityNodeId;
            this.WorkFlow.ActivityNodeType = this.GetNodeType(this.WorkFlow.ActivityNodeId);
            //}
            //else {
            //    this.WorkFlow.ActivityNodeId = workFlow.Nodes.Where(a => a.Value.Type == "END".ToLower()).FirstOrDefault().Key;
            //}


            //会签会签节点和流程结束节点没有下一步
            if (this.WorkFlow.ActivityNodeType == WorkFlowInstanceNodeType.ChatNode || this.WorkFlow.ActivityNodeType == WorkFlowInstanceNodeType.End)
            {
                this.WorkFlow.NextNodeId = default(Guid);//未找到节点
                this.WorkFlow.NextNodeType = WorkFlowInstanceNodeType.NotRun;
            }
            else
            {
                //查找下一个执行节点
                var nodeids = this.GetNextNodeIdsNotSpecialNode(this.WorkFlow.ActivityNodeId, WorkFlowInstanceNodeType.ViewNode);
                if (WorkFlow.NextNodeType != WorkFlowInstanceNodeType.End)
                {
                    if (nodeids.Count == 1)
                    {
                        this.WorkFlow.NextNodeId = nodeids[0];
                        this.WorkFlow.NextNodeType = this.GetNodeType(this.WorkFlow.NextNodeId);
                    }
                    else
                    {
                        //多个下个节点情况
                        this.WorkFlow.NextNodeId = default(Guid);
                        this.WorkFlow.NextNodeType = WorkFlowInstanceNodeType.NotRun;
                    }
                }
                else
                {//如果是不同意操作，直接结束流程
                    this.WorkFlow.NextNodeId = workFlow.Nodes.Where(a => a.Value.Type == "END".ToLower()).FirstOrDefault().Key;
                    this.WorkFlow.NextNodeType = this.GetNodeType(this.WorkFlow.NextNodeId);
                }

            }
        }
        /// <summary>
        /// 从工作流中获取与指定节点不同类型的目标节点的列表
        /// </summary>
        /// <param name="nodeId">当前节点</param>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        public List<Guid> GetNextNodeIdsNotSpecialNode(Guid nodeId, WorkFlowInstanceNodeType nodeType)
        {
            List<Guid> list = new List<Guid>();
            List<WorkFlowEdge> lines = this.WorkFlow.Edges[nodeId];
            var nodeids = lines.Select(m => m.targetNodeId).ToList();
            foreach (var item in nodeids)
            {
                var _thisnode = this.WorkFlow.Nodes[item];
                if (_thisnode.NodeType() != nodeType)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// 下个正常节点是否是多个(包含结束节点)
        /// </summary>
        public bool IsMultipleNextNode()
        {
            List<WorkFlowNode> nodes = new List<WorkFlowNode>();
            List<WorkFlowEdge> lines = this.WorkFlow.Edges[this.WorkFlow.ActivityNodeId];
            var nodeids = lines.Select(m => m.targetNodeId).ToList();
            foreach (var item in nodeids)
            {
                var _thisnode = this.WorkFlow.Nodes[item];
                if (_thisnode.NodeType() == WorkFlowInstanceNodeType.Normal || _thisnode.NodeType() == WorkFlowInstanceNodeType.End)
                {
                    nodes.Add(_thisnode);
                }
            }
            return nodes.Count >= 2;
        }

        /// <summary>
        /// 根据节点ID获取节点类型
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public WorkFlowInstanceNodeType GetNodeType(Guid nodeId)
        {
            var _thisnode = this.WorkFlow.Nodes[nodeId];
            return _thisnode.NodeType();
        }

        /// <summary>
        /// 获取节点集合
        /// </summary>
        /// <param name="nodesobj"></param>
        /// <returns></returns>
        private Dictionary<Guid, WorkFlowNode> GetNodes(dynamic nodesobj)
        {
            Dictionary<Guid, WorkFlowNode> nodes = new Dictionary<Guid, WorkFlowNode>();

            foreach (JObject item in nodesobj)
            {
                WorkFlowNode node = item.ToObject<WorkFlowNode>();
                if (!nodes.ContainsKey(node.Id))
                {
                    nodes.Add(node.Id, node);
                }
                if (node.Type == WorkFlowNode.START)
                {
                    this.WorkFlow.StartNodeId = node.Id;
                }
            }
            return nodes;
        }

        /// <summary>
        /// 获取工作流节点及以节点为出发点的流程
        /// </summary>
        /// <param name="linesobj"></param>
        /// <returns></returns>
        private Dictionary<Guid, List<WorkFlowEdge>> GetFromLines(dynamic linesobj)
        {
            Dictionary<Guid, List<WorkFlowEdge>> lines = new Dictionary<Guid, List<WorkFlowEdge>>();

            foreach (JObject item in linesobj)
            {
                WorkFlowEdge line = item.ToObject<WorkFlowEdge>();

                if (!lines.ContainsKey(line.sourceNodeId))
                {
                    lines.Add(line.sourceNodeId, new List<WorkFlowEdge> { line });
                }
                else
                {
                    lines[line.sourceNodeId].Add(line);
                }
            }

            return lines;
        }


        /// <summary>
        /// 获取全部流程线
        /// </summary>
        /// <returns></returns>
        public List<WorkFlowEdge> GetAllLines()
        {
            dynamic jsonobj = JsonConvert.DeserializeObject(this.WorkFlow.FlowJson);
            List<WorkFlowEdge> lines = new List<WorkFlowEdge>();
            foreach (JObject item in jsonobj.edges)
            {
                WorkFlowEdge line = item.ToObject<WorkFlowEdge>();
                lines.Add(line);
            }
            return lines;
        }

        /// <summary>
        /// 根据节点ID获取From（流入的线条）
        /// </summary>
        /// <param name="nodeid"></param>
        /// <returns></returns>
        public List<WorkFlowEdge> GetLinesForFrom(string nodeid)
        {
            var lines = GetAllLines().Where(m => m.targetNodeId == Guid.Parse(nodeid)).ToList();
            return lines;
        }

        /// <summary>
        /// 根据节点ID获取该节点与下个节点的连线
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        /// <param name="nodeType">要获取的节点类型,默认正常节点（包含结束节点）</param>
        /// <returns></returns>
        public List<WorkFlowEdge> GetLinesForTo(Guid nodeid)
        {
            List<WorkFlowEdge> list = new List<WorkFlowEdge>();
            var lines = GetAllLines().Where(m => m.sourceNodeId == nodeid).ToList();
            foreach (var item in lines)
            {
                var _nodeType = this.GetNodeType(item.targetNodeId);
                if (_nodeType == WorkFlowInstanceNodeType.Normal || _nodeType == WorkFlowInstanceNodeType.End)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取全部节点
        /// </summary>
        /// <returns></returns>
        public List<WorkFlowNode> GetAllNodes()
        {
            dynamic jsonobj = JsonConvert.DeserializeObject(this.WorkFlow.FlowJson);
            List<WorkFlowNode> nodes = new List<WorkFlowNode>();
            foreach (JObject item in jsonobj.nodes)
            {
                WorkFlowNode node = item.ToObject<WorkFlowNode>();
                nodes.Add(node);
            }
            return nodes;
        }

        /// <summary>
        /// 根据节点id获取下个节点id
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public List<Guid> GetNextNodeIds(Guid nodeId)
        {
            List<WorkFlowEdge> lines = this.WorkFlow.Edges[nodeId];
            return lines.Select(m => m.targetNodeId).ToList();
        }


        /// <summary>
        /// 根据节点id获取下个节点id
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        public List<Guid> GetNextNodeIds(Guid nodeId, WorkFlowInstanceNodeType nodeType)
        {
            List<Guid> list = new List<Guid>();
            List<WorkFlowEdge> lines = this.WorkFlow.Edges[nodeId];
            var nodeids = lines.Select(m => m.targetNodeId).ToList();
            foreach (var item in nodeids)
            {
                var _thisnode = this.WorkFlow.Nodes[item];
                if (_thisnode.NodeType() == nodeType)
                {
                    list.Add(item);
                }
            }
            return list;
        }


        /// <summary>
        /// 获取该节点的下面节点
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        public List<WorkFlowNode> GetNextNodes(Guid? nodeId = null, WorkFlowInstanceNodeType? nodeType = WorkFlowInstanceNodeType.Normal)
        {
            if (nodeId == null)
            {
                nodeId = this.WorkFlow.ActivityNodeId;
            }
            List<WorkFlowEdge> lines = this.WorkFlow.Edges[nodeId.Value];
            List<WorkFlowNode> list = new List<WorkFlowNode>();
            var nodeids = lines.Select(m => m.targetNodeId).ToList();
            foreach (var item in nodeids)
            {
                var _thisnode = this.WorkFlow.Nodes[item];
                if (_thisnode.NodeType() == nodeType)
                {
                    list.Add(_thisnode);
                }
            }
            return list;
        }

        /// <summary>
        /// 节点驳回
        /// </summary>
        /// <param name="rejectType">驳回节点类型</param>
        /// <param name="rejectNodeid">要驳回到的节点</param>
        /// <returns></returns>
        public Guid RejectNode(NodeRejectType rejectType, Guid? rejectNodeid)
        {
            switch (rejectType)
            {
                case NodeRejectType.PreviousStep:
                    return this.WorkFlow.PreviousId;
                case NodeRejectType.FirstStep:
                    return this.GetNextNodeIds(this.WorkFlow.StartNodeId).First();
                case NodeRejectType.ForOneStep:
                    if (rejectNodeid == null || rejectNodeid == default(Guid))
                    {
                        throw new Exception("驳回节点没有值！");
                    }
                    var fornode = this.WorkFlow.Nodes[rejectNodeid.Value];
                    return fornode.Id;
                case NodeRejectType.UnHandle:
                default:
                    return this.WorkFlow.PreviousId;
            }
        }
    }
}