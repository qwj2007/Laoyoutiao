using Laoyoutiao.Common;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Laoyoutiao.webapi.Area.Sys.Controllers
{
    /// <summary>
    /// 
    /// </summary>
   
    [Area("Sys")]
 
    public class SysRoleController : BaseController<SysRole, SysRoleRes, SysRoleReq, SysRoleEdit>
   
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseService"></param>
        private readonly ISysRoleService _sysRoleService;
        public SysRoleController(ISysRoleService sysRoleService) : base(sysRoleService)
        {
            _sysRoleService = sysRoleService;
        }
        /// <summary>
        /// 获取已经存在的用户角色
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public async Task<ApiResult>  GetRolesByUserId(long userId)
        {
            //long userId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
            var list = await _sysRoleService.GetRolesByUserId(userId);           
            return ResultHelper.Success(list);
        }

        /// <summary>
        /// 获取所有的角色
        /// </summary>
        /// <returns></returns>
        ///       
        [HttpGet]
        public override async Task<ApiResult> GetAll()
        {
            Expression<Func<SysRole, bool>> expression = a => a.IsDeleted == 0&&a.IsEnable==1;
            return ResultHelper.Success(await _sysRoleService.GetListByQueryAsync<SysRoleRes>(expression));           
        }      

    }
}
