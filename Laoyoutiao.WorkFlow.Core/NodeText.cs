using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    /// <summary>
    /// 节点显示文本值和坐标
    /// </summary>
    public class NodeText:WFPoint
    {       
        /// <summary>
        /// 显示的值
        /// </summary>
        public string? value { get; set; }
    }
}
