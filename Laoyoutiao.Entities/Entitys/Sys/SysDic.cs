using Laoyoutiao.Models.Common;
using SqlSugar;

namespace Laoyoutiao.Models.Entitys.Sys
{
    [SugarTable("sys_dic")]
    [Tenant("0")]
    public class SysDic : BaseTreeEntity<SysDic>
    {
        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "样式")]
        public string IconCls { get; set; }
        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "名称")]
        public string Name { get; set; }
        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "编码")]
        public string DicCode { get; set; }
        [SugarColumn(IsNullable = false, Length = 500, ColumnDescription = "全路径")]
        public string FullId { get; set; }
        [SugarColumn(IsNullable = false, Length = 500, ColumnDescription = "全路径名称")]
        public string FullName { get; set; }
        //[SugarColumn(IsNullable = false, ColumnDescription = "全路径名称")]
        ////public long Pid { get; set; }
        [SugarColumn(IsNullable = false, Length = 20, ColumnDescription = "字典类型")]
        public string DicType { get; set; }
        [SugarColumn(IsNullable = false, Length = 500, ColumnDescription = "备注")]
        public string? Remark { get; set; }
    }
}
