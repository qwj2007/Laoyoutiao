using Laoyoutiao.Models.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Entitys.WF
{
    /// <summary>
    /// 流程流转记录
    /// </summary>
    [SugarTable("wf_workflow_transition_history")]
    [Tenant("0")]
    public class WF_WorkFlow_Transition_History: BaseEntity
    {
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "流程流转Id")]
        public string? transitionId { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "流程实例Id")]
        public string? InstanceId { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "节点Id来源")]
        public string? FromNodeId { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "节点类型")]
        public int FromNodeType { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "来源节点名称")]
        public string FromNodeName { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "目标节点")]
        public string ToNodeId { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "目标节点类型")]
        public int ToNodeType { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "目标节点名称")]
        public string ToNodeName { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "创建人")]
        public string CreateUserName { get; set; }
        [SugarColumn(IsNullable = false,  ColumnDescription = "是否完成")]
        public int IsFinish { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "流程流转的状态")]
        public int TransitionState { get; set; } = 0;
    }
}
