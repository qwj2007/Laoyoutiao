using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.webapi.Controllers;

namespace Laoyoutiao.webapi.Area.Sys.Controllers
{
    public class SysUserRoleController :  BaseController<SysUserRole, SysUserRoleRes, SysUserRoleReq, SysUserRoleEdit>
  
    {
        public SysUserRoleController(IService.IBaseService<SysUserRole> baseService) : base(baseService)
        {
        }
    }
}
