using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.WF
{
    public class WFRes
    {     
        public string FlowId { get; set; }    
        public string FlowCode { get; set; }   
        public string CategoryId { get; set; }
        public string FormId { get; set; }       
        public string FlowName { get; set; }       
        public string FlowContent { get; set; }     
        public string Memo { get; set; }      
        public int IsOld { get; set; }      
        public int Enable { get; set; }
    }
}
