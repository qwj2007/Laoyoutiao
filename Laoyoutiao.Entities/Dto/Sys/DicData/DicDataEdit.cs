using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Sys.DicData
{
    [TypeMapper(SourceType = typeof(SysDicData))]
    public class DicDataEdit:BaseDto
    {
        public string? DicCode { get; set; }
        public long? ParentId { get; set; }
        public int? Is_System { get; set; }
        public string Name { get; set; }
    }
}
