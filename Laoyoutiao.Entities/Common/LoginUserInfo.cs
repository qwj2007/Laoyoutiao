using Laoyoutiao.Models.Entitys;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Common
{
    /// <summary>
    /// 当前登录人员信息
    /// </summary>
    public class LoginUserInfo
    {
        public SysUser loginUser { get; set; }
        /// <summary>
        /// 是否是管理员角色
        /// </summary>
        public bool isAdmin { get; set; } = false;
        
        /// <summary>
        /// 登录人所有角色
        /// </summary>
        public List<SysRole> roles { get; set; }
        /// <summary>
        /// 登录人所在的部门
        /// </summary>
        public List<DeptMent> deptMents { get; set; }

        /// <summary>
        /// 登录人所能看到的部门数据的集合
        /// </summary>
        //public List<long> deptDataIds { get; set; }
        public SortedSet<long> deptDataIds { get; set; }

        /// <summary>
        /// 是否能看所有的数据
        /// </summary>
        public bool isPower { get; set; } = false;
        /// <summary>
        /// 只能查看自己本人的数据
        /// </summary>
        public bool isOnlySelf { get; set; } = false;
    }
}
