using Laoyoutiao.Models.Common;
using SqlSugar;

namespace Laoyoutiao.Models.Entitys.Sys
{

    [SugarTable("sys_menu")]
    [Tenant("0")]
    public class Menus : BaseEntity
    {
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "菜单名称")]
        public string MenuName { get; set; }
        [SugarColumn(IsNullable = false, ColumnDescription = "所属系统")]
        public long SystemId { get; set; }
        [SugarColumn(IsNullable = false, ColumnDescription = "上级菜单")]
        public long ParentId { get; set; }
        [SugarColumn(IsNullable = false, ColumnDescription = "路由地址")]
        public string MenuUrl{get;set;}
        [SugarColumn(IsNullable = false, ColumnDescription = "排序")]
        public int Sort { get; set; }
        [SugarColumn(IsNullable = false, ColumnDescription = "按钮样式")]
        public string ButtonClass { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "按钮图标")]
        public string Icon { get; set; }

        [SugarColumn(IsNullable = true, ColumnDescription = "是否显示")]
        public int IsShow { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "是否是按钮")]
        public int IsButton { get; set; }
        [SugarColumn(IsNullable = false, ColumnDescription = "全路径")]
        public string Path { get; set; }
    }
}
