using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.WorkFlow.Core
{
    //    "properties":{
    //"CustomRect":"矩形",
    //"approveType":"roles",
    //"roles":"7",
    //"users":"",
    //"isActived":true
    //},
    /// <summary>
    /// 审批节点的属性
    /// </summary>
    public class NodeProperties
    {
        /// <summary>
        /// 审批类型
        /// </summary>
        public string approveType { get; set; }
        /// <summary>
        /// 当前节点那些角色审批
        /// </summary>
        public string roles { get; set; }
        /// <summary>
        /// 当前节点哪些人员可以审批
        /// </summary>
        public string users { get; set; }

    }
}
