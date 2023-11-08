using SqlSugar;

namespace Laoyoutiao.Models.Common
{
    public class BaseEntity: BaseKey
    {
        public BaseEntity()
        {
            CreateDate = DateTime.Now;
            IsDeleted = 0;         
        }

        /// <summary>
        /// 创建人
        /// </summary>
        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public long? CreateUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public long? ModifyUserId { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [SugarColumn(IsOnlyIgnoreInsert = true, IsNullable = true)]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDeleted { get; set; }
    
        [SugarColumn(IsNullable = true, ColumnDescription = "数据状态")]
        public int Status { get; set; } = 1;
        [SugarColumn(IsNullable = true, ColumnDescription = "流程状态,99没有流程")]
        public int FlowStatus { set; get; } = 99;

        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "编码")]
        public string? Code { get; set; }
        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "部门Id")]
        public string? DeptId { get; set; }

       
        

    }
}
