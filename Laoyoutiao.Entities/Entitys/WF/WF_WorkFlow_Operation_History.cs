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
    /// 工作流历史记录
    /// </summary>
    [SugarTable("wf_workflow_operation_history")]
    [Tenant("0")]
    public class WF_WorkFlow_Operation_History:BaseEntity
    {
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "操作Id")]
        public string? OperationId { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "流程实例Id")]
        public string? InstanceId { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "节点Id")]
        public string? NodeId { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "审批节点名称")]
      
        public int NodeName { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "审批意见")]
        
        public int Content { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "创建人")]
        public string CreateUserName { get; set; }

        [SugarColumn(IsNullable = false, ColumnDescription = "操作按钮类型")]
        public int TransitonType { get; set; }
      
    }
}
