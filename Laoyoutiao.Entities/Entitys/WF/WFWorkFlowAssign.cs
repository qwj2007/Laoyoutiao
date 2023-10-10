using Laoyoutiao.Models.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Entitys.WF
{
    [SugarTable("wf_workflow_assign")]
    [Tenant("0")]
    public class WFWorkFlowAssign:BaseEntity
    {
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "流程Id")]
        public string FlowId { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "流程实例Id")]
        public string InstanceId { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "流程节点Id")]
        public string NodeId { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "流程节点名称")]
        public string NodeName { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "请求人Id")]
        public long UserId { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "请求人姓名")]
        public string UserName { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "委托人Id")]
        public long AssignUserId { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "委托人姓名")]
        public string AssignUserName { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "委托信息")]
        public string Content { get; set; }

    }
}
