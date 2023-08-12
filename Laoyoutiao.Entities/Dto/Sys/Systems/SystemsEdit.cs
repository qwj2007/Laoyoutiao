using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Sys
{
    [TypeMapper(SourceType =typeof(Systems))]
    public class SystemsEdit:BaseDto
    {
        public string? SystemName { get; set; }
        public string? Memo { get; set; }
        public string? Status { get; set; }
        public string? SystemCode { get; set; }
    }
}
