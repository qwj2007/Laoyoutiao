using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    /// <summary>
    /// 工作流实例运行状态
    /// </summary>
      
    public enum WorkFlowInstanceStatus
    {
        /// <summary>
        /// 审核中
        /// </summary>
         
        [Description("审核中")]
        Running,
        /// <summary>
        /// 已结束,流程实例已结束
        /// </summary>
         
        [Description("已结束")]
        Finish
    }
}
