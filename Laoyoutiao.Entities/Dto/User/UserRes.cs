using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.User
{
    [TypeMapper(SourceType = typeof(Users))]
    public class UserRes
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary> 
        [PropertyMapper(SourceName = "NickName")]
        public string? Name { get; set; }
        /// <summary>
        /// 昵称
        /// </summary> 
        public string? NickName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string? Password { get; set; }
        /// <summary>
        /// 用户类型
        /// </summary>
        public int UserType { get; set; } = 1;
        /// <summary>
        /// 角色名
        /// </summary>
        public string? RoleName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        
        public string? Description { get; set; }
    }
}
