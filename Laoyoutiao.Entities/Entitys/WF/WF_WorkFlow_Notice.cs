using Laoyoutiao.Models.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Entitys.WF
{
    [SugarTable("wf_workflow_notice")]
    [Tenant("0")]
    public class WF_WorkFlow_Notice:BaseEntity
    {
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "编号")]
        public string InstanceId { get; set; }

        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "节点Id")]
        public string NodeId { get; set; }
        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "节点名称")]

        public string  NodeName { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "执行人")]

        public string Maker { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "是否已经流转过")]
        public int IsTransition { get; set; }

        [SugarColumn(IsNullable = true, ColumnDescription = "状态，退回时候用")]
        public int Status { get; set; } = 1;
        [SugarColumn(IsNullable = true, ColumnDescription = "是否已阅")]
        public int IsRead { get; set; } = 0;
    }
}
