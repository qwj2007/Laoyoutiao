

using Laoyoutiao.Models.Common;
using SqlSugar;

namespace Laoyoutiao.Models.Entitys
{
    //[SugarTable("Menu")]
    //[Tenant("1")]
    /// <summary>
    /// 菜单表
    /// </summary>
    public class Menu : BaseEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string? Name { get; set; }
        /// <summary>
        /// 路由地址
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string? Index { get; set; }
        /// <summary>
        /// 项目中的页面路径
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string? FilePath { get; set; }
        /// <summary>
        /// 父级
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long ParentId { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int Order { get; set; }
        /// <summary>
        /// 是否启用（0=未启用，1=启用）
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public bool IsEnable { get; set; }

        /// <summary>
        /// 是否启用（0=未启用，1=启用）
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string CMP { get; set; } = "123";

        [SugarColumn(IsNullable = true)]
        public string? Description { get; set; }
    }
}
