using Laoyoutiao.Models.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Entitys.Sys
{
    [SugarTable("sys_user_role")]
    [Tenant("0")]
    public class SysUserRole: BaseEntity
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long UserId { get; set; }
        /// <summary>
        /// 角色Id
        /// </summary>

        [SugarColumn(IsNullable = false)]
        public long RoleId { get; set; }

    }
}
