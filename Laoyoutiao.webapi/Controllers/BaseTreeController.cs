using Laoyoutiao.Common;
using Laoyoutiao.IService;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Controllers
{
    /// <summary>
    /// 列表为树形列表的要继承这个controller
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TRes"></typeparam>
    /// <typeparam name="TReq"></typeparam>
    /// <typeparam name="TEdit"></typeparam>

    public class BaseTreeController<T, TRes, TReq, TEdit> : BaseController<T, TRes, TReq, TEdit> where T : BaseTreeEntity<T>, new()
        where TRes : class where TReq : Pagination where TEdit : class
    {
        private readonly IBaseTreeService<T> _baseService;
        private ISysDicService sysDicService;

        public BaseTreeController(IBaseTreeService<T> baseService) : base(baseService)
        {
            _baseService = baseService;
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
