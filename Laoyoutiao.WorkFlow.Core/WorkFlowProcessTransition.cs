using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    //
    // 摘要:
    //     流程流转实体
    public class WorkFlowProcessTransition
    {
        /// <summary>
        /// 流程实例Id，真正的主键
        /// </summary>
        public long Id { get; set; }
        ///// <summary>
        ///// 表单Id
        ///// </summary>
        //public long FormId { get; set; }
        //
        // 摘要:
        //     流程id
        public Guid FlowId { get; set; }

        //
        // 摘要:
        //     实例id
        public Guid InstanceId { get; set; }

        //
        // 摘要:
        //     用户id
        public string UserId { get; set; }

        //
        // 摘要:
        //     用户姓名
        public string UserName { get; set; }

        //
        // 摘要:
        //     菜单类型 操作类型
        public WorkFlowMenu MenuType { get; set; }

        //
        // 摘要:
        //     内容
        public string ProcessContent { get; set; }

        //
        // 摘要:
        //     驳回类型
        public NodeRejectType? NodeRejectType { get; set; }

        //
        // 摘要:
        //     当驳回类型为JadeFramework.WorkFlow.NodeRejectType.ForOneStep时候的那个节点ID
        public Guid? RejectNodeId { get; set; }

        //
        // 摘要:
        //     流程状态改变实体
        public WorkFlowStatusChange StatusChange { get; set; }

        //
        // 摘要:
        //     流程委托实体
        public FlowAssign Assign { get; set; }

        //
        // 摘要:
        //     参数信息 用于节点获取、条件判断
        public Dictionary<string, object>? OptionParams { get; set; }

        //
        // 摘要:
        //     扩展字段
        public object Extend { get; set; }


    }
}
