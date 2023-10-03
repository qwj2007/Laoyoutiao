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
    //     流程按钮
    public enum WorkFlowMenu
    {
        //
        // 摘要:
        //     返回
        [Description("返回")]
        Return = -1,
        //
        // 摘要:
        //     保存
        [Description("保存")]
        Save = 0,
        //
        // 摘要:
        //     提交
        [Description("提交")]
        Submit = 1,
        //
        // 摘要:
        //     同意
        [Description("同意")]
        Agree = 2,
        //
        // 摘要:
        //     不同意
        [Description("不同意")]
        Deprecate = 3,
        //
        // 摘要:
        //     弃权
        [Description("弃权")]
        Cancel = 4,
        //
        // 摘要:
        //     直送
        [Description("直送")]
        Throgh = 5,
        //
        // 摘要:
        //     委托
        [Description("委托")]
        Assign = 6,
        //
        // 摘要:
        //     退回
        [Description("退回")]
        Back = 7,
        //
        // 摘要:
        //     终止，流程被暂停,流程意外报错终止
        [Description("终止")]
        Stop = 8,
        //
        // 摘要:
        //     已阅，用通知节点
        [Description("已阅")]
        View = 9,
        //
        // 摘要:
        //     流程图
        [Description("流程图")]
        FlowImage = 10,
        //
        // 摘要:
        //     审批意见
        [Description("审批意见")]
        Approval = 11,
        //
        // 摘要:
        //     抄送
        [Description("抄送")]
        CC = 12,
        //
        // 摘要:
        //     挂起
        [Description("挂起")]
        Suspend = 13,
        //
        // 摘要:
        //     回复
        [Description("回复")]
        Resume = 14,
        //
        // 摘要:
        //     撤回,即取消流程 刚开始提交，下一个节点未审批情况，流程发起人可以终止 终止时：流程实例，操作历史，审批历史将被物理删除
        [Description("撤回")]
        Withdraw = 0xF,
        //
        // 摘要:
        //     表单打印
        [Description("打印")]
        Print = 0x10,
        //
        // 摘要:
        //     再次提交
        [Description("再次提交")]
        ReSubmit = 101
    }
}
