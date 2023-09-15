using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    /// <summary>
    /// 流程节点
    /// </summary>
    public class WorkFlowNode
    {
        /// <summary>
        /// 节点Id
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// 节点类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// x坐标
        /// </summary>
        public int x { get; set; }
        /// <summary>
        /// y坐标
        /// </summary>
        public int y { get; set; }
        /// <summary>
        /// node自定义属性
        /// </summary>
        public NodeProperties properties { get; set; }

    }
}
