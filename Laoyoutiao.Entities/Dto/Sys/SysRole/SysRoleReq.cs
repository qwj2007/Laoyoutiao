using Laoyoutiao.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Sys
{
    public class SysRoleReq:Pagination
    {
        public string? RoleName { get; set; }
        public int IsEnable { get; set; }

        public long SystemId { get; set; }
    }
}
