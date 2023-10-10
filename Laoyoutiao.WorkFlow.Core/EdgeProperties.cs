using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class EdgeProperties
    {
        /// <summary>
        /// 条件
        /// </summary>
        public string conditional { get; set; }
        /// <summary>
        /// 条件判断值
        /// </summary>
        public double conditionalValue { get; set; }

    }

    /// <summary>
    /// 条件判断
    /// </summary>
    public enum EdgeConditionEnum
    {
        大于,
        大于等于,
        小于,
        小于等于,
        等于
    }
}
