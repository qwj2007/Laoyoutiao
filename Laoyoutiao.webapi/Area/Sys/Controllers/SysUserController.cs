using Laoyoutiao.IService;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Mvc;


namespace Laoyoutiao.webapi.Area.Sys.Controllers;


/// <summary>
/// 
/// </summary>

[ApiController]
[Area("Sys")]
[Route("api/[controller]/[action]")]
public class SysUserController : BaseController<SysUser, SysUserRes, SysUserReq, SysUserEdit>
{
    /// <summary>
    /// 获取系统中的用户信息
    /// </summary>
    /// <param name="sysUserService"></param> <summary>
    /// 
    private readonly ISysUserService _userService;
    private readonly ICustomJWTService _jwtService;
    private readonly IConfiguration _configuration;


    public SysUserController(ISysUserService sysUserService, IConfiguration configuration, ICustomJWTService customJWTService) : base(sysUserService)
    {
        _userService = sysUserService;
        _jwtService = customJWTService;
        _configuration = configuration;
    }
    //[HttpGet]
    //public async Task<ApiResult> GetTokens(string account, string password)
    //{
    //    var result = Task.Run(() =>
    //    {

    //        if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
    //        {
    //            return ResultHelper.Error("参数不能为空");
    //        }
    //        var users = _userService.GetUser(account, password) as SysUserRes;
    //        if (string.IsNullOrEmpty(users.UserName))
    //        {
    //            return ResultHelper.Error("账号不存在，用户名或密码错误！");
    //        }

    //        return ResultHelper.Success(_jwtService.GetToken(users));
    //    });
    //    return await result;
    //}
}