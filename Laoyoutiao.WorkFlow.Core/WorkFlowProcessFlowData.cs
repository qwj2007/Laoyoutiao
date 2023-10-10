using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    /// <summary>
    /// 流程信息
    /// </summary>
       
    public class WorkFlowProcessFlowData
    {
        /// <summary>
        /// 流程是否结束
        /// </summary>
           
        public int? IsFinish { get; set; }

        /// <summary>
        /// 审批状态
        /// </summary>
            
        public int Status { get; set; }

        /// <summary>
        ///   当前节点
        /// </summary>
        public WorkFlowNode CurrentNode { get; set; }
    }
}
