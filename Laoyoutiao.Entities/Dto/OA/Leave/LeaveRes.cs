using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.OA;
using Laoyoutiao.Common;
using Laoyoutiao.WorkFlow.Core;

namespace Laoyoutiao.Models.Dto.OA.Leave
{
    [TypeMapper(SourceType = typeof(OALeave))]
    public class LeaveRes
    {
       public long Id { get; set; }
        public string LeaveCode { get; set; }
        
        public string Title { get; set; }
        
        public long UserId { get; set; }
       
        public long AgentId { get; set; } = 0;
      
        public int LeaveType { get; set; }
    
        public string Reason { get; set; }
      
        public decimal Days { get; set; }
      
        public DateTime StartTime { get; set; }
     
        public DateTime EndTime { get; set; }
      
        public int FlowStatus { get; set; }
       
        public string FlowStatusName { get; set; }
        
        //public string FlowStatusName { get {
        //        return EnumHelper.GetEnumDescription<WorkFlowStatus>(FlowStatus);
        //    }}
    
        public DateTime FlowTime { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
