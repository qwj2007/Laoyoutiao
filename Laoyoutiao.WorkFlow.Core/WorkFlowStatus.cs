﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    /// <summary>
    /// 流程状态
    /// </summary>
    public enum WorkFlowStatus
    {
        //
        // 摘要:
        //     未提交
        [Description("未提交")]
        UnSubmit = -1,
        /// <summary>
        /// 提交
        /// </summary>
        [Description("提交")]
        Submit = 0,
        //
        // 摘要:
        //     审核中
        [Description("审核中")]
        Running=1,
        //
        // 摘要:
        //     已结束 => 通过
        [Description("已结束")]
        IsFinish=2,
        //
        // 摘要:
        //     不同意
        [Description("不同意")]
        Deprecate=3,
        //
        // 摘要:
        //     流程被退回
        [Description("已退回")]
        Back=4,
        //
        // 摘要:
        //     终止，流程被暂停,流程意外报错终止
        [Description("终止")]
        Stop=5,
        //
        // 摘要:
        //     撤回
        [Description("撤回")]
        Withdraw=6
    }
}
