using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.WF;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.WF.Urge
{
    [TypeMapper(SourceType = typeof(WF_WorkFlow_Urge))]
    public class UrgeEdit:BaseDto
    {    
        public string? InstanceId { get; set; }
        
        public string? NodeId { get; set; }

        
        public string? NodeName { get; set; }
      
        public long Sender { get; set; }

       
        public string UrgeUser { get; set; }

     
        public string UrgeType { get; set; }

        public string UrgeContent { get; set; }
    }
}
