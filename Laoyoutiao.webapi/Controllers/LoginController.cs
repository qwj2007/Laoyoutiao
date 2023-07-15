using Laoyoutiao.Common;
using Laoyoutiao.IService;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.User;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICustomJWTService _jwtService;
        private readonly IConfiguration _configuration;

        public LoginController(IUserService userService, ICustomJWTService customJWTService,IConfiguration configuration)
        {
            _userService = userService;
            _jwtService = customJWTService;
            _configuration = configuration;
        }

/// <summary>
/// 测试一下Apollo配置中心
/// </summary>
/// <returns></returns>
        [HttpGet]
        public async Task<ApiResult> GetApollo()
        {
            string name =  _configuration.GetSection("JWTTokenOptions:SecurityKey").Value;
            return ResultHelper.Success( _configuration.GetSection("JWTTokenOptions:SecurityKey"));
        }

            [HttpGet]
        public async Task<ApiResult> GetToken(string name, string password)
        {
            var result = Task.Run(() =>
            {

                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
                {
                    return ResultHelper.Error("参数不能为空");
                }
                UserRes users = _userService.GetUser(name, password) as UserRes;
                if (string.IsNullOrEmpty(users.Name))
                {
                    return ResultHelper.Error("账号不存在，用户名或密码错误！");
                }

                return ResultHelper.Success(_jwtService.GetToken(users));
            });
            return await result;
        }
    }
}
