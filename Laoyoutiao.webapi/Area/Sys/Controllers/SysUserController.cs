using Laoyoutiao.Caches;
using Laoyoutiao.Common;
using Laoyoutiao.Enums;
using Laoyoutiao.IService;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Security.Principal;


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
    /// 
    /// </summary>
    /// <param name="sysUserService"></param>
    private readonly ICache _cache;
    public SysUserController(ISysUserService sysUserService,ICache cache) : base(sysUserService)
    {
        _cache = cache;
    }

    /// <summary>
    /// 退出登录，把缓存中的信息一块清除
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ApiResult> LogOut(string account) {
        
        _cache.RemoveCache(CacheInfo.LoginUserInfo + account);
        return ResultHelper.Success(true) ;
    }

}