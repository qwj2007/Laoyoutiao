using SqlSugar;

namespace Laoyoutiao.Models.Common
{
    public class BaseKey
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }
    }
}
