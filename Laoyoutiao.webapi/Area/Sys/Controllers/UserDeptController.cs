using Laoyoutiao.Common;
using Laoyoutiao.IService;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Dto.Sys.UserDept;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.Service.Sys;
using Laoyoutiao.webapi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace Laoyoutiao.webapi.Area.Sys.Controllers
{
    [Area("Sys")]
    public class UserDeptController : BaseController<UserDept, UserDeptEdit, UserDeptReq, UserDeptRes>
    {
        private readonly IUserDeptService _userDeptService;
        public UserDeptController(IUserDeptService userDeptService) : base(userDeptService)
        {
            _userDeptService = userDeptService;
        }
        /// <summary>
        /// 获取用户的部门Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ApiResult>  GetSelectDeptIdByUserId(long userId) {
            return ResultHelper.Success(await _userDeptService.GetSelectDeptIdByUserId(userId));

        }

     

        [HttpPost]
        public async Task<ApiResult> SaveUserDepts(List<UserDeptEdit> list,long userId)
        {
            long uid = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
            var result = await _userDeptService.SaveUserDept(list, userId,uid);
            return ResultHelper.Success(result);
        }
    }
}
