using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    /// <summary>
    /// 摘要:
    //     流程状态修改实体
    /// </summary>
    
    public class WorkFlowStatusChange
    {
        /// <summary>
        /// 主键名称
        /// </summary>

        public string? KeyName { get; set; } = "Id";
        /// <summary>
        /// 主键值
        /// </summary>
      
        public string? KeyValue { get; set; }
        /// <summary>
        /// 流程状态
        /// </summary>
        
        public WorkFlowStatus? Status { get; set; }

        /// <summary>
        /// 表名称
        /// </summary>
       
        public string? TableName { get; set; }

        /// <summary>
        /// 流程操作时间
        /// </summary>
       
        public DateTime? FlowTime { get; set; }

        /// <summary>
        ///  CAP订阅名称
        /// </summary>        
        public string? TargetName { get; set; } = "WorkFlowStatusChanged";
    }
}
