using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    /// <summary>
    /// 驳回类型
    /// </summary>
    public enum NodeRejectType
    {
        /// <summary>
        ///  前一步
        /// </summary>
       
        [Description("前一步")]
        PreviousStep,
        /// <summary>
        /// 第一步
        /// </summary>
        [Description("第一步")]
        FirstStep,
        /// <summary>
        /// 某一步
        /// </summary>
        [Description("某一步")]
        ForOneStep,
        /// <summary>
        /// 不处理
        /// </summary>
        [Description("不处理")]
        UnHandle
    }
}
