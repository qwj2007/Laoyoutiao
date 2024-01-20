using Laoyoutiao.Models.Common;
using SqlSugar;

namespace Laoyoutiao.Models.Entitys.Sys
{
    [SugarTable("sys_dic")]
    [Tenant("0")]
    public class SysDic : BaseTreeEntity<SysDic>
    {
       
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "名称")]
        public string? Title { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "编码")]
        public string? DicCode { get; set; }
        [SugarColumn(IsNullable = false, Length = 500, ColumnDescription = "全路径")]
        public string? FullId { get; set; }
        [SugarColumn(IsNullable = true, Length = 500, ColumnDescription = "全路径名称")]
        public string? FullName { get; set; }        
        [SugarColumn(IsNullable = false, Length = 20, ColumnDescription = "字典类型")]
        public string? DicType { get; set; }
        [SugarColumn(IsNullable = true, Length = 500, ColumnDescription = "备注")]
        public string? Remark { get; set; }
        [SugarColumn(IsNullable = false, ColumnDescription = "排序")]
        public int? Sort { get; set; } = 0;
    }
}
