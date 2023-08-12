using Laoyoutiao.Models.CustomAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laoyoutiao.Models.Entitys.Sys;
using SqlSugar;

namespace Laoyoutiao.Models.Dto.Sys
{
    [TypeMapper(SourceType = typeof(Systems))]
    public class SystemsRes
    {

        public long Id { get; set; }
        public string? SystemName { get; set; }
    
        public string? SystemCode { get; set; }
   
        public string? Memo { get; set; }
        public DateTime CreateDate { get; set; }

        // public int Sort { get; set; } = 0;
        public string? Status { get; set; }
       
    }
}
