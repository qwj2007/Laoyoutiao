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
    /// 
    /// </summary>
    /// <param name="sysUserService"></param>
    public SysUserController(ISysUserService sysUserService) : base(sysUserService)
    {

    }

}