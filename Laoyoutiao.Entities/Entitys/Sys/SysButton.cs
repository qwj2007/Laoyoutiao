using Laoyoutiao.Models.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Entitys.Sys
{
    [SugarTable("sys_button")]
    [Tenant("0")]
    public class SysButton:BaseEntity
    {
        [SugarColumn(IsNullable=false,Length =50,ColumnDescription ="按钮名称")]
        public string? BtnName { get; set; }
        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "描述")]
        public string? Memo { get; set; }
        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "图标")]
        public string? Icon { get; set; }
    }
}
