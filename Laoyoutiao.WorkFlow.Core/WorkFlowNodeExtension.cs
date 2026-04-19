using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    /// <summary>
    /// 流程节点扩展方法
    /// </summary>
    public static class WorkFlowNodeExtension
    {
        /// <summary>
        /// 根据节点类型判断是否为结束节点
        /// </summary>
        public static int ToIsFinish(this WorkFlowNodeType nodeType)
        {
            return nodeType == WorkFlowNodeType.EndNode ? 1 : 0;
        }
    }

    /// <summary>
    /// 流程节点类型
    /// </summary>
    public enum WorkFlowNodeType
    {
        /// <summary>
        /// 开始节点
        /// </summary>
        StartNode = 0,

        /// <summary>
        /// 审批节点
        /// </summary>
        AuditNode = 1,

        /// <summary>
        /// 条件节点
        /// </summary>
        ConditionNode = 2,

        /// <summary>
        /// 结束节点
        /// </summary>
        EndNode = 9
    }
}
