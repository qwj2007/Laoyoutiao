using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    //
    // 摘要:
    //     流程进程实体
    public class WorkFlowProcess
    {
        //
        // 摘要:
        //     流程id
        public string FlowId { get; set; }

        //
        // 摘要:
        //     流程名称
        public string FlowName { get; set; }

        //
        // 摘要:
        //     实例id
        public string InstanceId { get; set; }

        //
        // 摘要:
        //     用户id
        public string UserId { get; set; }

        //
        // 摘要:
        //     用户名
        public string UserName { get; set; }

        //
        // 摘要:
        //     可操作按钮集合 JadeFramework.WorkFlow.WorkFlowMenu集合
        public List<int> Menus { get; set; }

        //
        // 摘要:
        //     表单id
        public string FormId { get; set; }

        //
        // 摘要:
        //     表单类型
        public WorkFlowFormType FormType { get; set; }

        //
        // 摘要:
        //     表单内容
        public string FormContent { get; set; }

        //
        // 摘要:
        //     表单数据
        public string FormData { get; set; }

        //
        // 摘要:
        //     表单地址
        public string FormUrl { get; set; }

        //
        // 摘要:
        //     执行过的任务节点
        public List<WorkFlowNode> ExecutedNode { get; set; }

        //
        // 摘要:
        //     流程信息
        public WorkFlowProcessFlowData FlowData { get; set; }
    }

    //
    // 摘要:
    //     流程表单类型
    public enum WorkFlowFormType
    {
        //
        // 摘要:
        //     自定义表单
        [Description("自定义表单")]
        Custom,
        //
        // 摘要:
        //     系统表单
        [Description("系统表单")]
        System
    }
}
