using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Sys
{
    [TypeMapper(SourceType = typeof(SysRole))]
    public class SysRoleEdit:BaseDto
    {
        public string RoleName { get; set; }
        public long SystemId { get; set; }
        public string Memo { get; set; }

    }
}
