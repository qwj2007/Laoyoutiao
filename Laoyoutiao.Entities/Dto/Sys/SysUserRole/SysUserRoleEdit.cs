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
    [TypeMapper(SourceType = typeof(SysUserRole))]
    public class SysUserRoleEdit:BaseDto
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }
    }
}
