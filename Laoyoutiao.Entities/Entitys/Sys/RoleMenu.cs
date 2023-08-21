using Laoyoutiao.Models.Common;
using SqlSugar;

namespace Laoyoutiao.Models.Entitys.Sys
{
    [SugarTable("sys_role_menu")]
    [Tenant("0")]
    public class RoleMenu:BaseEntity
    {
        public long RoleId { get; set; }
        public long MenuId { get; set; }
    }
}
