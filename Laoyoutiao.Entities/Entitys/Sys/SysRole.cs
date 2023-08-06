using Laoyoutiao.Models.Common;
using SqlSugar;

namespace Laoyoutiao.Models.Entitys.Sys
{
    [SugarTable("sys_role")]
    [Tenant("0")]
    public class SysRole: BaseEntity
    {
        /// <summary>
        /// 系统Id
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public long SystemId { get; set; } = 0;
        [SugarColumn(IsNullable = false, Length = 50)]
        /// <summary>
        /// 角色名称
        /// </summary>
        /// <value></value>
        public string? RoleName { get; set; }      

        /// <summary>
        /// 是否可用
        /// </summary>
        /// <value></value>
        public int IsEnable { get; set; } = 1;

        [SugarColumn(IsNullable = false,Length =200)]

        /// <summary>
        /// 描述
        /// </summary>
        /// <value></value>
        public string? Memo { get; set; }

    }
}
