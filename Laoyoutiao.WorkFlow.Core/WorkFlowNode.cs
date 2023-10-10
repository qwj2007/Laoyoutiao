using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    /// <summary>
    /// 流程节点
    /// </summary>
    public class WorkFlowNode
    {

        //
        // 摘要:
        //     开始
        public const string START = "start";

        //
        // 摘要:
        //     结束
        public const string END = "end";

        //
        // 摘要:
        //     自动节点 =>条件判断 SQL、结果值判断
        public const string NODE = "node";

        /// <summary>
        /// 审批节点
        /// </summary>
        public const string APPROVE = "approve";

        //
        // 摘要:
        //     任务节点
        public const string Task = "task";

        //
        // 摘要:
        //     分流 =>代表一个任务可以走多个分支
        public const string FORK = "fork";

        //
        // 摘要:
        //     合流 =>代表合并多个分支得出最终的结果
        public const string JOIN = "join";

        //
        // 摘要:
        //     会签 串行/并行两种方式
        public const string Chat = "chat";

        //
        // 摘要:
        //     通知节点
        public const string View = "view";

        /// <summary>
        /// 节点Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 节点类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// x坐标
        /// </summary>
        public int x { get; set; }
        /// <summary>
        /// y坐标
        /// </summary>
        public int y { get; set; }
        /// <summary>
        /// node自定义属性
        /// </summary>
        public NodeProperties properties { get; set; }

        public  NodeText text{get;set;}
        //
        // 摘要:
        //     获取节点枚举类型
        public WorkFlowInstanceNodeType NodeType()
        {
            return Type switch
            {
                "start" => WorkFlowInstanceNodeType.Start,//开始节点
                "end" => WorkFlowInstanceNodeType.End,//结束节点
                "task" => WorkFlowInstanceNodeType.Normal,//正常节点
                "fork" => WorkFlowInstanceNodeType.ForkNode,//分流
                "join" => WorkFlowInstanceNodeType.JoinNode,//合流分流会和
                "chat" => WorkFlowInstanceNodeType.ChatNode,//会签节点
                "node" => WorkFlowInstanceNodeType.ConditionNode,//条件节点
                "view" => WorkFlowInstanceNodeType.ViewNode,//通知节点
                _ => WorkFlowInstanceNodeType.NotRun,//无法运行
            };
        }

    }
}
