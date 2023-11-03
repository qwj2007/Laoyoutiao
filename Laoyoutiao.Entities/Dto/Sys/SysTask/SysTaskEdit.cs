using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Sys.SysTask
{
    [TypeMapper(SourceType = typeof(Laoyoutiao.Models.Entitys.Sys.SysTask))]
    public class SysTaskEdit:BaseDto
    {
        public string? TaskName { get; set; }
        public string? Groups { get; set; }
        public string? Cron { get; set; }
        public string? TaskDesc { get; set; }

    }
}
