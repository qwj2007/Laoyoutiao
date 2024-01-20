using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Sys.Dic
{
    [TypeMapper(SourceType = typeof(SysDic))]
    public class DicEdit:BaseDto
    {     
        public string? DicCode { get; set; }
        public string? DicType { get; set; }
        public long ParentId { get; set; }
        public string? Title { get; set; }
    }
}
