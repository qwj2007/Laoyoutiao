using Laoyoutiao.Models.Common;
using SqlSugar;

namespace Laoyoutiao.Models.Entitys.Sys
{
    [SugarTable("sys_user_dept")]
    [Tenant("0")]
    public class UserDept : BaseEntity
    {
        public long UserId { get; set; }
        public long DeptId { get; set; }
    }
}
