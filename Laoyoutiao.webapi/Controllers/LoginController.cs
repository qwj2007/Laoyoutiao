using Laoyoutiao.Common;
using Laoyoutiao.Enums;
using Laoyoutiao.IService;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys;
using Laoyoutiao.webapi.Filter;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Laoyoutiao.webapi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[CustomerActionFilters]
    //[TypeFilter(typeof(CustomerActionFilters))] //这两种方式局部注册在类上，里边的方法都能filrer
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
        public ApiResult GetApollo()
        {
            //string name = _configuration.GetSection("JWTTokenOptions:SecurityKey").Value;
            return ResultHelper.Success(_configuration.GetSection("JWTTokenOptions:SecurityKey"));
        }

        /// <summary>
        /// 获取SysUser用户token
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>

        [HttpGet]
        //[CustomerActionFilters]
        //[TypeFilter(typeof(CustomerActionFilters))] 这两种方式局部注册到方法上，只有这个方法可以filter都可以
        public async Task<ApiResult> GetTokens(string account, string password)
        {
            //throw new Exception("出现错误了12121。。。。。。。。");

            var result = System.Threading.Tasks.Task.Run(() =>
            {

                if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
                {
                    return ResultHelper.Error("参数不能为空");
                }
                var users = _sysUserService.GetUser(account, password) as SysUserRes;

                if (string.IsNullOrEmpty(users.Account))
                {
                    return ResultHelper.Error("账号不存在，用户名或密码错误！");
                }
                return ResultHelper.Success(_jwtService.GetToken(users));
            });

            return await result;
        }
        /// <summary>
        /// 验证token是否有效
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult> ValidationToken(string token)
        {
            var result = System.Threading.Tasks.Task.Run(() =>
            {
                string code =Convert.ToInt64( TokenValidCodeEnum.Valid).ToString();
                var tokenResult = _jwtService.ValidateToken(token);
                if (tokenResult == null) {
                    //token失效Code
                    code =Convert.ToInt64(  TokenValidCodeEnum.UnValid).ToString();
                }
                return ResultHelper.Success(code);
            });

            return await result;
        }
        /// <summary>
        /// 根据RefreshToken获取新的token
        /// </summary>
        /// <param name="RefreshToken"></param>
        /// <returns></returns>
        //public async Task<ApiResult> TokenRenewalByRefreshToken(string RefreshToken) { 
        
        //}
        /// <summary>
        /// 对密码进行加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<string> GetEncodePassword(string password)
        {
            var result = Task.Run(() =>
            {
                return Encrypt.Encode(password);
            });

            return await result;

        }
    }
}
