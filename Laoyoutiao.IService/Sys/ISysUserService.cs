namespace Laoyoutiao.IService.Sys;

using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;

public interface ISysUserService : IBaseService<SysUser>
{
    /// <summary>
        /// 登录用
        /// </summary>
        /// <param name="userName"></param>
        /// /// <param name="password"></param>
        /// <returns></returns>
        SysUserRes GetUser(string userName, string password);  
}
