using Laoyoutiao.Models.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Entitys.Sys
{
    /// <summary>
    /// 字典数据
    /// </summary>
    [SugarTable("sys_dicdata")]
    [Tenant("0")]
    public class SysDicData: BaseTreeEntity<SysDicData>
    {
        [SugarColumn(IsNullable = false,  ColumnDescription = "是否是系统预设")]
        public int Is_System { get; set; } = 0;
        [SugarColumn(IsNullable = false,Length =50, ColumnDescription = "名称")]
        public string? Name { get; set; }
        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "编码")]
        public string? DicCode { get; set; }
        [SugarColumn(IsNullable = false, Length = 500, ColumnDescription = "备注")]
        public string? Remark { get; set; }
    }
}
