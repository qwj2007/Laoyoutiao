using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    //     工作流实例节点类型
    public enum WorkFlowInstanceNodeType
    {
        //
        // 摘要:
        //     无法运行
        [Description("无法运行")]
        NotRun = -1,
        //
        // 摘要:
        //     分流开始
        [Description("分流")]
        ForkNode,
        //
        // 摘要:
        //     合流分流会和
        [Description("合流")]
        JoinNode,
        //
        // 摘要:
        //     正常节点
        [Description("正常节点")]
        Normal,
        //
        // 摘要:
        //     开始节点
        [Description("开始节点")]
        Start,
        //
        // 摘要:
        //     结束节点
        [Description("结束节点")]
        End,
        //
        // 摘要:
        //     会签节点
        [Description("会签")]
        ChatNode,
        //
        // 摘要:
        //     条件节点
        [Description("条件节点")]
        ConditionNode,
        //
        // 摘要:
        //     通知节点
        [Description("通知节点")]
        ViewNode
    }

    public static class FlowNodeExtension
    {
        //
        // 摘要:
        //     判断节点是否结束
        //
        // 参数:
        //   nodeType:
        public static int ToIsFinish(this WorkFlowInstanceNodeType nodeType)
        {
            return (nodeType == WorkFlowInstanceNodeType.End) ? 1 : 0;
        }
    }

    //
    // 摘要:
    //     流程流转状态
    public enum WorkFlowTransitionStateType
    {
        //
        // 摘要:
        //     正常通过
        Normal,
        //
        // 摘要:
        //     节点拒绝
        Reject
    }
}
