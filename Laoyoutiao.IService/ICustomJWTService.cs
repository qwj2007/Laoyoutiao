using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.IService
{
    public interface ICustomJWTService
    {
        //获取token
        //string GetToken(UserRes user);
        //获取token
        string GetToken(SysUserRes user);
        /// <summary>
        /// 获取刷新token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        string GetRefreshToken(SysUserRes user);

        /// <summary>
        /// 获取access_token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        string GetAccessToken(SysUserRes user);

        /// <summary>
        /// 验证token是否有效
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        ClaimsPrincipal ValidateToken(string token);
    }
}
