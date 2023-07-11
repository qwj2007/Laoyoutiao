using Laoyoutiao.Common;
using Laoyoutiao.IService;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.User;
using Laoyoutiao.Models.Entitys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UserController : BaseController<Users,UserRes,UserReq,UserEdit>
    {
        private readonly IUserService _users;
        public UserController(IUserService Users):base(Users)
        {
            _users = Users;
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult>  GetUsers(UserReq req)
        {
            long userId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);

            //return ResultHelper.Success(_users.GetUsers(req));
            var result = await _users.GetPagesAsync<UserReq, UserRes>(req);
            return ResultHelper.Success(result);
        }

        #region
        //[HttpGet]
        //public ApiResult GetUsersById(long id)
        //{
        //    return ResultHelper.Success(_users.GetModelById<UserRes>(id));
        //}
        //[HttpPost]
        //public ApiResult Add(UserAdd req)
        //{
        //    //获取当前登录人信息 
        //    long userId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
        //    return ResultHelper.Success(_users.Add(req, userId));
        //}
        #endregion

        //[HttpPost]
        //public ApiResult Edit(UserEdit req)
        //{
        //    //获取当前登录人信息
        //    long userId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
        //    return ResultHelper.Success(_users.Edit(req, userId));
        //}
        
        [HttpGet]
        public ApiResult SettingRole(string pid, string rids)
        {
            return ResultHelper.Success(_users.SettingRole(pid, rids));
        }
        [HttpGet]
        public ApiResult EditNickNameOrPassword(string nickName, string? password)
        {
            //获取当前登录人信息
            long userId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
            return ResultHelper.Success(_users.EditNickNameOrPassword(userId, nickName, password));
        }
    }
}
