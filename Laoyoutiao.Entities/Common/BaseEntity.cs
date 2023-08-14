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
      
      
    }
}
