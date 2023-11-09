using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    //"id":"3b295f35-dd38-4b12-915b-c5cd396138c7",
    //"type":"edge",
    //"sourceNodeId":"618d1660-3ae6-4125-96a1-710e41345bb3",
    //"targetNodeId":"72653a4e-c1dc-4c41-8e73-0564f579844d",
    public class WorkFlowEdge
    {
        /// <summary>
        /// 节点Id
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// 节点类型
        /// </summary>
        public string type { get; set; } = "edge";
        /// <summary>
        /// 连线开始节点
        /// </summary>
        public Guid sourceNodeId { get; set; }
        /// <summary>
        /// 连线目标节点
        /// </summary>
        public Guid targetNodeId { get; set; }
        /// <summary>
        /// 开始坐标
        /// </summary>
        public WFPoint startPoint { get; set; }
        /// <summary>
        /// 结束坐标
        /// </summary>
        public WFPoint endPoint { get; set; }
        /// <summary>
        /// 显示坐标集合
        /// </summary>
        public List<WFPoint> pointsList { get; set; } = new List<WFPoint>();
        /// <summary>
        /// 显示属性
        /// </summary>
        public EdgeProperties properties { get; set; } = new EdgeProperties();

        public string statu { get; set; }


    }
}
