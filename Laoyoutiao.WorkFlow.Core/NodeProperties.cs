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

        public ChatData ChatData { get; set; }

    }
    /// <summary>
    /// 审批人员类型，指定人员或角色
    /// </summary>
    public class ApproveType
    {
        //
        // 摘要:
        //     指定用户
        public const string SPECIAL_USER = "USERS";

        //
        // 摘要:
        //     所有用户
        public const string ALL_USER = "ALL_USER";

        //
        // 摘要:
        //     制定角色
        public const string SPECIAL_ROLE = "ROLES";

        //
        // 摘要:
        //     SQL自动获取
        public const string SQL = "SQL";

        //
        // 摘要:
        //     流程发起人
        public const string CREATEUSER = "CREATEUSER";
    }
}
