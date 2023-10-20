using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.OA;
using SqlSugar;

namespace Laoyoutiao.Models.Dto.OA.Leave
{
    [TypeMapper(SourceType = typeof(OALeave))]
    public class LeaveEdit:BaseDto
    {
     
        public string LeaveCode { get; set; }

        public string Title { get; set; }

        public long UserId { get; set; }
      
        public long AgentId { get; set; } = 0;

        public int LeaveType { get; set; } = 1;
        public string Reason { get; set; }

        public decimal Days { get; set; } = 0;
       
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int FlowStatus { get; set; } = -1;//未提交，保存
      
        public DateTime FlowTime { get; set; }
    }
}

