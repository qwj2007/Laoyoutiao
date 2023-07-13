
using Laoyoutiao.Models.Common;
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace Laoyoutiao.Models.Entitys
{
    [SugarTable("Users")]
    [Tenant("1")]
    /// <summary>
    /// 用户
    /// </summary>
    public class Users: BaseEntity
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Name { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string NickName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public string Password { get; set; }
        /// <summary>
        /// 用户类型（0=超级管理员，1=普通用户）
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int UserType { get; set; } = 1;
        /// <summary>
        /// 是否启用（0=未启用，1=启用）
        /// </summary>
        [SugarColumn(IsNullable = false,ColumnDataType ="boolean")]
        public bool IsEnable { get; set; }

        [SugarColumn(IsNullable = false)]
        public string Description { get; set; }

    }   
}
