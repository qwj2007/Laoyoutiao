using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Sys
{
    [TypeMapper(SourceType = typeof(SysRole))]
    public class SysRoleRes
    {
        public long Id { get; set; }

       public long SystemId { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        /// <value></value>
        public string? RoleName { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        /// <value></value>
        public string? Memo { get; set; }     
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

    }
}
