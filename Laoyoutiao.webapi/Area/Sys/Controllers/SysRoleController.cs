using Laoyoutiao.IService;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Area.Sys.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Area("Sys")]
    [Route("api/[controller]/[action]")]
    public class SysRoleController : BaseController<SysRole, SysRoleRes, SysRoleReq, SysRoleEdit>
   
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseService"></param>
        public SysRoleController(IBaseService<SysRole> baseService) : base(baseService)
        {
        }
    }
}
