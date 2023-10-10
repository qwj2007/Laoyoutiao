using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    /// <summary>
    /// 会签数据
    /// </summary>
    // 摘要:
    //     会签数据
    public class ChatData
    {
        /// <summary>
        /// 会签类型
        /// </summary>
        // 摘要:
        //     会签类型
        public ChatType ChatType { get; set; }

        /// <summary>
        /// 会签计算方式
        /// </summary>
        // 摘要:
        //     会签计算方式
        public ChatParallelCalcType ParallelCalcType { get; set; }
    }
    /// <summary>
    /// 会签并行计算方式
    /// </summary>

    public enum ChatParallelCalcType
    {
        /// <summary>
        /// 百分百
        /// </summary>
        // 摘要:
        //     百分百
        [Description("百分百")]
        OneHundredPercent,
        /// <summary>
        /// 超过一半
        /// </summary>
        // 摘要:
        //     超过一半
        [Description("超过一半")]
        MoreThenHalf
    }

    //
    // 摘要:
    //     会签方式
    public enum ChatType
    {
        //
        // 摘要:
        //     并行
        [Description("并行")]
        Parallel,
        //
        // 摘要:
        //     串行
        [Description("串行")]
        Serial
    }
}
