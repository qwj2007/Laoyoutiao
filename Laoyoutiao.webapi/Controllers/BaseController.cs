using Laoyoutiao.Common;
using Laoyoutiao.IService;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.User;
using Laoyoutiao.Models.Entitys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace Laoyoutiao.webapi.Controllers
{
    /// <summary>
    /// 控制器基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TRes"></typeparam>
    /// <typeparam name="TReq"></typeparam>
    /// <typeparam name="TEdit"></typeparam>

    [Authorize]
    public class BaseController<T, TRes, TReq, TEdit> : ControllerBase where T : BaseEntity, new()
          where TRes : class where TReq : Pagination where TEdit : class
    {
        private readonly IBaseService<T> _baseService;
        /// <summary>
        /// 基础controller
        /// </summary>
        /// <param name="baseService"></param>
        public BaseController(IBaseService<T> baseService)
        {
            this._baseService = baseService;
        }


        /// <summary>
        /// 根据Id查找一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual ApiResult GetModelById(long id)
        {
            return ResultHelper.Success(_baseService.GetModelById<TRes>(id));
        }
        /// <summary>
        /// 插入或修改一条数据
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<ApiResult> Add(TEdit req)
        {
            //获取当前登录人信息 
            long userId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
            return ResultHelper.Success(await _baseService.Add(req, userId));
        }

        //[HttpPost]
        //public async Task<ApiResult> Edit(UserEdit req)
        //{
        //    //获取当前登录人信息
        //    long userId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
        //    return ResultHelper.Success(await _baseService(req, userId));
        //}
        /// <summary>
        /// 根据Id删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual ApiResult Del(long id)
        {
            return ResultHelper.Success(_baseService.Del(id));
        }

        /// <summary>
        ///   ///获取所有信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]      
        public virtual async Task<ApiResult> GetAll()
        {
            Expression<Func<T, bool>> expression = a => a.IsDeleted == 0;           
            return ResultHelper.Success(await _baseService.GetListByQueryAsync<TRes>(expression));
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual ApiResult BatchDel(string ids)
        {
            return ResultHelper.Success(_baseService.BatchDel(ids.Split(',')));
        }
        /// <summary>
        ///  批量删除
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost]
        public virtual ApiResult BatchDelByIdArray(string[] ids)
        {
            return ResultHelper.Success(_baseService.BatchDel(ids));
        }

        /// <summary>
        /// 获取列表信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<ApiResult> GetPages(TReq req)
        {
            long userId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
            var result = await _baseService.GetPagesAsync<TReq, TRes>(req);
            return ResultHelper.Success(result);
        }
        /// <summary>
        /// 数列表
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<ApiResult> GetTree(TReq req)
        {
            long userId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
            var result = await _baseService.GetTreeAsync<TReq, TRes>(req);
            return ResultHelper.Success(result);
        }
    }
}


