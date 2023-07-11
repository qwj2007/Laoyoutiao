using Laoyoutiao.Models.Common;
using SqlSugar;

namespace Laoyoutiao.Models.Entitys
{
    [SugarTable("UserRoleRelation")]
    [Tenant("1")]
    /// <summary>
    /// 用户角色关系
    /// </summary>
    public class UserRoleRelation : BaseKey
    {
        /// <summary>
        /// 用户主键
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long UserId { get; set; }
        /// <summary>
        /// 角色主键
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long RoleId { get; set; }
    }
}
