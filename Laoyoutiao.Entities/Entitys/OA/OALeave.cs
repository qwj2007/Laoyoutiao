using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Entitys.Sys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Entitys.OA
{
    [SugarTable("oa_leave")]
    [Tenant("0")]
    public class OALeave : BaseEntity
    {
        //[SugarColumn(IsNullable = false, Length = 50,ColumnDescription ="请假编码")]
        //public string LeaveCode { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "请假标题")]
        public string Title { get; set; }
        [SugarColumn(IsNullable = false, ColumnDescription = "请假人")]
        public long UserId { get; set; }
        [SugarColumn(IsNullable = false, ColumnDescription = "请假代理人")]
        public long AgentId { get; set; } = 0;
        [SugarColumn(IsNullable = false, ColumnDescription = "请假类型")]
        public int LeaveType { get; set; }
        [SugarColumn(IsNullable = false, Length = 500, ColumnDescription = "请假原因")]
        public string Reason { get; set; }
        [SugarColumn(IsNullable = false, ColumnDescription = "请假天数")]
        public decimal Days { get; set; }
        [SugarColumn(IsNullable = false, ColumnDescription = "请假开始时间")]
        public DateTime StartTime { get; set; }
        [SugarColumn(IsNullable = false, ColumnDescription = "请假结束时间")]
        public DateTime EndTime { get; set; }
        [SugarColumn(IsNullable = false, ColumnDescription = "流程状态")]
        public int FlowStatus { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "流程时间")]
        public DateTime FlowTime { get; set; }
    }
}
