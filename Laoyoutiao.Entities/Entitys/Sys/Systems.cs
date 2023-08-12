using Laoyoutiao.Models.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Entitys.Sys
{    
    [SugarTable("sys_system")]
    [Tenant("0")]
    public class Systems:BaseEntity
    {
        [SugarColumn(IsNullable = false, Length = 50,ColumnDescription ="系统名称")]
        public string SystemName { get; set; }

        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "系统代码")]
        public string SystemCode { get; set; }

        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "描述")]
        public string Memo { get; set; }
        [SugarColumn(IsNullable = false, ColumnDescription = "排序")]
        public int Sort { get; set; } = 0;
        [SugarColumn(IsNullable = false, ColumnDescription = "是否启用")]
        public int isEnable { get; set; } = 1;
    }
}
