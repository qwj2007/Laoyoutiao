using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.Sys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Sys
{
    [TypeMapper(SourceType = typeof(SysButton))]
    public class BtnEdit : BaseDto
    {
        public long Id { get; set; }
        public string? BtnName { get; set; }

        public string? Memo { get; set; }

        public string? Icon { get; set; }
    }
}
