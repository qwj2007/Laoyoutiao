using Laoyoutiao.Common;
using Laoyoutiao.IService;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.Service.Sys;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Laoyoutiao.webapi.Area.Sys.Controllers
{

    [ApiController]
    [Area("Sys")]
    [Route("api/[controller]/[action]")]
    public class DeptMentController : BaseTreeController<DeptMent, DeptRes, DeptReq, DeptEdit>
    {
        private readonly IDeptMentService _deptMentService;
        public DeptMentController(IDeptMentService deptMentService) : base(deptMentService)
        {
            _deptMentService = deptMentService;
        }
        /// <summary>
        /// 判断是不存在子部门
        /// </summary>
        /// <returns></returns>
        ///       
        [HttpGet]
        public  async Task<ApiResult> IsExitChildList(long Id)
        {          
            return ResultHelper.Success(await _deptMentService.IsExitChildList(Id));
        }        
    }
}
