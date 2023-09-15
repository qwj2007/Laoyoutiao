using Laoyoutiao.Models.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Entitys.WF
{
    [SugarTable("wf_workflow")]
    [Tenant("0")]
    public class WF_WorkFlow:BaseEntity
    {
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "编号")]
        public string FlowId { get; set; }
        
        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "编号")]
        public string FlowCode { get; set; }
        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "类别Id")]

        public string CategoryId { get; set; }
        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "表单Id")]

        public string FormId { get; set; }
        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "流程名称")]
        public string FlowName { get; set; }
        [SugarColumn(IsNullable = true, Length = 500, ColumnDescription = "流程JSON")]
        public string FlowContent { get; set; }
        [SugarColumn(IsNullable = true, Length = 500, ColumnDescription = "备注")]
        public string Memo { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "是否旧版")]
        public int IsOld { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "是否启用")]
        public int Enable { get; set; }

    }
}
