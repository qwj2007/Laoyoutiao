using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    /// <summary>
    /// 委托实体
    /// </summary>
    public class FlowAssign
    {
        //
        // 摘要:
        //     委托人id
        public string AssignUserId { get; set; }

        //
        // 摘要:
        //     委托人姓名
        public string AssignUserName { get; set; }

        //
        // 摘要:
        //     委托信息
        public string AssignContent { get; set; }
    }
}
