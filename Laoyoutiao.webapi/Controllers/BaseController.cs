using AutoMapper;
using DotNetCore.CAP;
using Laoyoutiao.Common;
using Laoyoutiao.IService;
using Laoyoutiao.IService.WF;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.User;
using Laoyoutiao.Models.Entitys;
using Laoyoutiao.Service.WF;
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
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BaseController<T, TRes, TReq, TEdit> : ControllerBase where T : BaseEntity, new()
          where TRes : class where TReq : Pagination where TEdit : class
    {
        private readonly IBaseService<T> _baseService;
        private readonly IWorkFlowInstanceService _workFlowInstanceService;
        //private readonly IMapper _mapper;
        //private readonly ICapPublisher capPublisher;

        /// <summary>
        /// 基础controller
        /// </summary>
        /// <param name="baseService"></param>
        public BaseController(IBaseService<T> baseService)
        {
            this._baseService = baseService;
            //this._workFlowInstanceService = new WorkFlowInstanceService(_mapper, capPublisher);
        }


        /// <summary>
        /// 根据Id查找一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual ApiResult GetModelById(long id)
        {
            var res = _baseService.GetModelById<TRes>(id);
            if (res != null)
            {
                return ResultHelper.Success(res);
            }
            else
            {
                return ResultHelper.Error("未查找到数据");
            }


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
            var result = await _baseService.Add(req, userId);
            if (result)
            {
                return ResultHelper.Success(result);
            }
            else
            {
                return ResultHelper.Error("操作失败");
            }

        }

        /// <summary>
        /// 保存修改返回主键
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<ApiResult> AddOneRerunKeyValue(TEdit req)
        {
            //获取当前登录人信息 
            long userId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
            var result = await _baseService.AddOneRerunKeyValue(req, userId);
            if (result > 0)
            {
                return ResultHelper.Success(result);
            }
            return ResultHelper.Error("操作失败");

        }
        /// <summary>
        /// 保存并返回实体
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<ApiResult> AddOrUpdateReturnEntity(TEdit req)
        {
            //获取当前登录人信息 
            long userId = Convert.ToInt32(HttpContext.User.Claims.ToList()[0].Value);
            var result = await _baseService.AddOrUpdateReturnEntity(req, userId);
            if (result != null)
            {
                return ResultHelper.Success(result);
            }
            return ResultHelper.Error();

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
            var res = _baseService.Del(id);
            if (res) {
                return ResultHelper.Success(res);
            }
            return ResultHelper.Error();
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
        public virtual async Task<ApiResult> BatchDel(string ids)
        {
            var res = await _baseService.BatchDelAsync(ids.Split(','));
            if (res) { return ResultHelper.Success(res); }
            return ResultHelper.Error("批量删除失败");
           
        }
        /// <summary>
        ///  批量删除
        /// </summary>
        /// <param name="ids">主键数组</param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<ApiResult> BatchDelByIdArray(string[] ids)
        {
            var res = await _baseService.BatchDelAsync(ids);
            if (res) { return ResultHelper.Success(res); }
            return ResultHelper.Error("批量删除失败");          
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



    }
}


