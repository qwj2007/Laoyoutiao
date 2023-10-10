using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    //
    // 摘要:
    //     工作流上下文
    public abstract class WorkFlowContext
    {
        //
        // 摘要:流程实体
        //     flow entity
        public WorkFlow WorkFlow { get; set; }
    }
}


