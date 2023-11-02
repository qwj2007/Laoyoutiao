using Laoyoutiao.Common;
using Laoyoutiao.IService;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Dto.User;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    
    public class LoginController : ControllerBase
    {
        //private readonly IUserService _userService;
        private readonly ICustomJWTService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly ISysUserService _sysUserService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="customJWTService"></param>
        /// <param name="configuration"></param>
        public LoginController(//IUserService userService, 
            ICustomJWTService customJWTService,
            IConfiguration configuration,
            ISysUserService sysUserService
            )
        {
            //_userService = userService;
            _jwtService = customJWTService;
            _configuration = configuration;
            _sysUserService = sysUserService;
        }

/// <summary>
/// 测试一下Apollo配置中心
/// </summary>
/// <returns></returns>
        [HttpGet]
        public  ApiResult GetApollo()
        {
            //string name = _configuration.GetSection("JWTTokenOptions:SecurityKey").Value;
            return ResultHelper.Success( _configuration.GetSection("JWTTokenOptions:SecurityKey"));
        }
        /// <summary>
        /// 获取token值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
            [HttpGet]
        //public async Task<ApiResult> GetToken(string name, string password)
        //{
        //    var result = Task.Run(() =>
        //    {

        //        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
        //        {
        //            return ResultHelper.Error("参数不能为空");
        //        }
        //        SysUserRes users = _sysUserService.GetUser(name, password) as SysUserRes;
        //        if (string.IsNullOrEmpty(users.UserName))
        //        {
        //            return ResultHelper.Error("账号不存在，用户名或密码错误！");
        //        }

        //        return ResultHelper.Success(_jwtService.GetToken(users));
        //    });
        //    return await result;
        //}
        /// <summary>
        /// 获取SysUser用户token
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>

        [HttpGet]
        public async Task<ApiResult> GetTokens(string account, string password)
        {
            var result = System.Threading.Tasks.Task.Run(() =>
            {

                if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
                {
                    return ResultHelper.Error("参数不能为空");
                }
                var users = _sysUserService.GetUser(account, password) as SysUserRes;
                if (string.IsNullOrEmpty(users.UserName))
                {
                    return ResultHelper.Error("账号不存在，用户名或密码错误！");
                }

                return ResultHelper.Success(_jwtService.GetToken(users));
            });
            return await result;
        }
    }
}
