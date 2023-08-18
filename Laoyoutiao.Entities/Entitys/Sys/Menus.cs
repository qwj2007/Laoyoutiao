using Laoyoutiao.Enums;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using SqlSugar;

namespace Laoyoutiao.Models.Entitys.Sys
{
    [SugarTable("sys_menu")]
    [Tenant("0")]
    public class Menus : BaseTreeEntity<Menus>
    {
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "菜单名称")]
        public string? Name { get; set; }
        //[SugarColumn(IsNullable = false, ColumnDescription = "所属系统")]
        //public long SystemId { get; set; }
        // [SugarColumn(IsNullable = false, ColumnDescription = "上级菜单")]
        //new public long ParentId { get; set; }
        [SugarColumn(IsNullable = false, ColumnDescription = "上级菜单")]
        public override long ParentId { get => base.ParentId; set => base.ParentId = value; }

        [SugarColumn(IsNullable = false, ColumnDescription = "路由地址")]
        public string? MenuUrl { get; set; }
        [SugarColumn(IsNullable = false, ColumnDescription = "排序")]
        public int Sort { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "按钮样式")]
        public string? ButtonClass { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "按钮图标")]
        public string? Icon { get; set; }

        [SugarColumn(IsNullable = true, ColumnDescription = "是否显示在菜单中")]
        public int IsShow { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "是否是按钮")]
        public int IsButton { get; set; }

        [SugarColumn(IsNullable = true,Length =20, ColumnDescription = "系统按钮还是自定义按钮")]
        public string? BtnType { get; set; }

        [SugarColumn(IsNullable = true, ColumnDescription = "全路径")]
   
        public string? Path { get; set; }
        [SugarColumn(IsNullable = false, ColumnDescription = "页面路径")]
        public string? ComponentUrl { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "编码")]
        public string? Code { get; set; }

        [SugarColumn(IsNullable = true,Length =200, ColumnDescription = "描述")]
        public string? Memo { get; set; }

        //[SugarColumn(IsIgnore = true)]
        //public List<MenuButton> MenuBtns { get; set; }
        [SugarColumn(IsIgnore =true)]
        public override List<Menus> Children { get => base.Children; set => base.Children = value; }


    }

   
}
