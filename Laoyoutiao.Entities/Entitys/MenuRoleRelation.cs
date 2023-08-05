
using Laoyoutiao.Models.Common;
using SqlSugar;

namespace Laoyoutiao.Models.Entitys
{
    /// <summary>
    /// 菜单角色关系
    /// </summary>
    //[SugarTable("MenuRoleRelation")]
    //[Tenant("1")]
    public class MenuRoleRelation : BaseKey
    {
 
        /// <summary>
        /// 菜单主键
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long MenuId { get; set; }
        /// <summary>
        /// 角色主键
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long RoleId { get; set; }
    }
}
