using Laoyoutiao.IRespository;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.User;
using Laoyoutiao.Models.Entitys;
using System.Linq.Expressions;

namespace Laoyoutiao.IService
{
    public interface IBaseService<T> : IBaseServiceRepository<T> where T : BaseKey, new()
    {

        

        /// <summary>
        /// 添加或修改一条记录
        /// </summary>
        /// <typeparam name="TAdd"></typeparam>
        /// <param name="input">DTO</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        Task<bool> Add<TAdd>(TAdd input, long userId);
        /// <summary>
        /// 根据Id获取一个实体
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        TRes GetModelById<TRes>(long id);
        /// <summary>
        /// 根据Id删除一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Del(long id);
        /// <summary>
        /// 根据Id批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool BatchDel(string[] ids);

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <typeparam name="TReq">请求参数</typeparam>
        /// <param name="req"></param>
        /// <returns></returns>

        PageInfo GetPages<TReq>(TReq req) where TReq : class;

        /// <summary>
        /// 获取所有的数据
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <returns></returns>
        Task<List<TRes>> GetListAllAsync<TRes>() where TRes : class;
        List<TRes> GetListAll<TRes>() where TRes : class;
        /// <summary>
        /// 条件查找数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        List<TRes> GetListByQuery<TRes>(Expression<Func<T, bool>> expression) where  TRes : class;
        Task<List<TRes>> GetListByQueryAsync<TRes>(Expression<Func<T, bool>> expression) where  TRes : class;
        /// <summary>
        /// 根据Id获取一个实体异步方法
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TRes> GetModelByIdAsync<TRes>(long id);
        /// <summary>
        /// 根据Id删除一条数据 异步方法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DelAsync(long id);
        /// <summary>
        /// 根据Id批量删除异步
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<bool> BatchDelAsync(string[] ids);

        /// <summary>
        /// 获取分页数据异步
        /// </summary>
        /// <typeparam name="TReq">请求参数</typeparam>
        /// <param name="req"></param>
        /// <returns></returns>

        Task<PageInfo> GetPagesAsync<TReq, TRes>(TReq req) where TReq : Pagination where TRes:class;

        //Task<PageInfo> GetTreeAsync<TReq, TRes>(TReq req) where TReq : Pagination where TRes : class;

    }
}
