using AutoMapper;
using Laoyoutiao.Caches;
using Laoyoutiao.Enums;
using Laoyoutiao.IService;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Dto;
using Laoyoutiao.Repository;
using Microsoft.AspNetCore.Http;
using SqlSugar;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;

namespace Laoyoutiao.Service
{
    public class BaseService<T> : BaseServiceRepository<T>, IBaseService<T> where T : BaseEntity, new()
    {
        private readonly IMapper _mapper;
        //private readonly ICache _cache;
        private readonly CustomCache _customcache;
        public ICache _cache { get; set; }
        public IHttpContextAccessor _httpContext { get; set; }
        /// <summary>
        /// 当前用户信息  用_cache 获取
        /// </summary>
        public LoginUserInfo _currentUser
        {
            get
            {
                return _customcache.GetUserInfo();
                //if (_httpContext != null && _httpContext.HttpContext != null)
                //{
                //    var Claims = _httpContext.HttpContext.User.Claims;
                //    if (Claims.Count() > 0)
                //    {
                //        var id = Claims.FirstOrDefault(t => t.Type == ClaimTypes.PrimarySid)?.Value;
                //        return _cache.GetCache<CurrentUserInfo>($"selfinfo{id}", out bool has);
                //    }
                //}
                //return null;
            }
        }

        public BaseService(IMapper mapper, CustomCache cache)
        {
            this._mapper = mapper;
            _customcache = cache;
        }

        public virtual bool BatchDel(string[] ids)
        {
            return Delete(ids);
        }

        public virtual async Task<bool> BatchDelAsync(string[] ids)
        {
            return await DeleteAsync(ids);

        }

        public virtual bool Del(long id)
        {
            return DeleteById(id);
        }

        public virtual async Task<bool> DelAsync(long id)
        {
            return await DeleteByIdAsync(id);
        }
        public virtual bool Edit<TEdit>(TEdit input, long userId) where TEdit : BaseDto
        {
            var info = _db.Queryable<T>().First(p => p.Id == input.Id);
            _mapper.Map(input, info);
            info.ModifyUserId = userId;
            info.ModifyDate = DateTime.Now;
            return _db.Updateable(info).ExecuteCommand() > 0;
        }



        /// <summary>
        /// 添加或修改一条数据
        /// </summary>
        /// <typeparam name="TAdd"></typeparam>
        /// <param name="input"></param>
        /// <param name="userId"></param>
        /// <returns></returns>

        public virtual async Task<bool> Add<TEdit>(TEdit input, long userId)
        {
            T info = _mapper.Map<T>(input);
            if (_currentUser.deptDataIds.Count > 0)
            {
                info.DeptId = string.Join(",", _currentUser.deptDataIds.ToArray());
            }
            if (_currentUser.isAdmin)
            {
                info.DeptId = "1";
            }
            if (info.Id == 0)
            {
                info.Code = SnowFlakeSingle.Instance.NextId().ToString();
                info.CreateUserId = userId;
                info.CreateDate = DateTime.Now;

                //info.UserType = 1;//0=炒鸡管理员，系统内置的
                info.IsDeleted = 0;
                return await _db.Insertable(info).ExecuteCommandAsync() > 0;
            }
            else
            {
                info.ModifyUserId = userId;
                info.ModifyDate = DateTime.Now;
                return await _db.Updateable(info).ExecuteCommandAsync() > 0;
            }
        }
        /// <summary>
        /// 保存并返回主键
        /// </summary>
        /// <typeparam name="TEdit"></typeparam>
        /// <param name="input"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<long> AddOneRerunKeyValue<TEdit>(TEdit input, long userId)
        {
            T info = _mapper.Map<T>(input);
            if (_currentUser.deptDataIds.Count > 0)
            {
                info.DeptId = string.Join(",", _currentUser.deptDataIds.ToArray());
            }
            if (_currentUser.isAdmin)
            {
                info.DeptId = "1";
            }
            if (info.Id == 0)
            {
                info.Code = SnowFlakeSingle.Instance.NextId().ToString();
                info.CreateUserId = userId;
                info.CreateDate = DateTime.Now;
                //info.UserType = 1;//0=炒鸡管理员，系统内置的
                info.IsDeleted = 0;
                return await _db.Insertable(info).ExecuteReturnIdentityAsync();


            }
            else
            {
                info.ModifyUserId = userId;
                info.ModifyDate = DateTime.Now;
                var result = await _db.Updateable(info).ExecuteCommandAsync();
                return info.Id;
            }
        }
        /// <summary>
        /// 保存并返回实体
        /// </summary>
        /// <typeparam name="TEdit"></typeparam>
        /// <param name="input"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<T> AddOrUpdateReturnEntity<TEdit>(TEdit input, long userId)
        {
            T info = _mapper.Map<T>(input);
            if (_currentUser.deptDataIds.Count > 0)
            {
                info.DeptId = string.Join(",", _currentUser.deptDataIds.ToArray());
            }
            if (_currentUser.isAdmin)
            {
                info.DeptId = "1";
            }
            if (info.Id == 0)
            {
                info.Code = SnowFlakeSingle.Instance.NextId().ToString();
                info.CreateUserId = userId;
                info.CreateDate = DateTime.Now;
                //info.UserType = 1;//0=炒鸡管理员，系统内置的
                info.IsDeleted = 0;
                return await _db.Insertable(info).ExecuteReturnEntityAsync();
            }
            else
            {
                info.ModifyUserId = userId;
                info.ModifyDate = DateTime.Now;
                await _db.Updateable(info).ExecuteCommandAsync();
                return info;
            }
        }

        /// <summary>
        /// 根据id查找实体
        /// </summary>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TRes GetModelById<TRes>(long id)
        {
            var info = GetEntityById(id);
            return _mapper.Map<TRes>(info);
        }

        public virtual async Task<TRes> GetModelByIdAsync<TRes>(long id)
        {
            var info = await GetEntityByIdAsync(id);
            return _mapper.Map<TRes>(info);
        }
        /// <summary>
        /// 分页列表
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="req"></param>
        /// <returns></returns>
        public virtual async Task<PageInfo> GetPagesAsync<TReq, TRes>(TReq req) where TReq : Pagination where TRes : class
        {

            var exp = _db.Queryable<T>();
            #region 加载数据权限
            exp = GetCurrentUserDataRange(exp);
            #endregion
            exp = GetMappingExpression(req, exp);


            var res = await exp
                .Skip((req.PageIndex - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync();
            PageInfo pageInfo = new PageInfo();
            pageInfo.data = _mapper.Map<List<TRes>>(res);
            pageInfo.total = exp.Count();
            return pageInfo;
        }

        public ISugarQueryable<T> GetCurrentUserDataRange(ISugarQueryable<T> exp)
        {
            var userInfo = _currentUser;
            if (userInfo != null)
            {
                if (userInfo.isAdmin || userInfo.isPower)
                {//若果是管理员或没有限制，不做任何处理
                }
                else
                {
                    //如果只能查看自己的数据
                    if (userInfo.isOnlySelf)
                    {
                        exp = exp.Where(a => a.CreateUserId == userInfo.loginUser.Id);
                    }
                    if (userInfo.deptDataIds.Count > 0)
                    {
                        string deptIds = string.Join(",", userInfo.deptDataIds.ToArray());
                        exp = exp.Where(a => a.DeptId.Contains(deptIds) || deptIds.Contains(a.DeptId));
                    }
                }
            }

            return exp;
        }

        //public ISugarQueryable<TRes> GetCurrentUserDataRange<TRes>(ISugarQueryable<TRes> exp) where TRes : class
        //{
        //    var userInfo = _currentUser;
        //    if (userInfo != null)
        //    {
        //        if (userInfo.isAdmin || userInfo.isPower)
        //        {//若果是管理员或没有限制，不做任何处理
        //        }
        //        else
        //        {
        //            //如果只能查看自己的数据
        //            if (userInfo.isOnlySelf)
        //            {
        //                exp = exp.Where(a => a.CreateUserId == userInfo.loginUser.Id);
        //            }
        //            if (userInfo.deptDataIds.Count > 0)
        //            {
        //                string deptIds = string.Join(",", userInfo.deptDataIds.ToArray());
        //                exp = exp.Where(a => a.DeptId.Contains(deptIds) || deptIds.Contains(a.DeptId));
        //            }
        //        }
        //    }

        //    return exp;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <param name="req"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ISugarQueryable<T> GetMappingExpression<TReq>(TReq req, ISugarQueryable<T> exp) where TReq : Pagination
        {
            PropertyInfo[] propertyInfos = req.GetType().GetProperties();
            //判断是否加SugarTable属性，如果加就以SugarTable标记的表名
            string tableName = typeof(T).Name;
            bool tabAtrr = typeof(T).IsDefined(typeof(SugarTable));
            if (tabAtrr)
            {
                var sugarTableAttr = typeof(T).GetCustomAttribute<SugarTable>();
                tableName = sugarTableAttr.TableName;
            }

            PropertyInfo[] propertyInfosSource = typeof(T).GetProperties();
            foreach (var property in propertyInfos)
            {
                //判断这个字段是否有【PropertyMapperAttribute】标记，如果有就以标记的属性名为准，
                var properMaapered = property.IsDefined(typeof(PropertyMapperAttribute));
                var propertyName = "";
                var values = property.GetValue(req);//得到值
                if (properMaapered)
                {
                    var propertyAttribute = property.GetCustomAttributes(typeof(PropertyMapperAttribute)).FirstOrDefault() as PropertyMapperAttribute;
                    //得到
                    propertyName = propertyAttribute?.SourceName;
                    if (propertyName == null)
                    {
                        propertyName = property.Name;
                    }
                }
                else
                {
                    //如果没有，就检测源T是否有这个属性。
                    var souceType = propertyInfosSource.Where(a => a.Name == property.Name).FirstOrDefault();
                    if (souceType != null)
                    {
                        propertyName = property.Name;
                    }
                    else
                    {
                        continue;
                    }
                }

                //判断是否有这个属性
                //var ty = propertyInfosSource.Where(a => a.Name == property.Name).FirstOrDefault();
                //if (ty != null)
                //{
                //    var values = property.GetValue(req);
                switch (property.PropertyType.Name.ToLower())
                {
                    case "string":
                        exp = exp.WhereIF(!string.IsNullOrWhiteSpace(Convert.ToString(values)), $"{tableName}.{propertyName} like '%{values}%' ");
                        break;
                    case "int32":
                    case "int64":
                    case "decimal":
                        exp = exp.Where($"{tableName}.{propertyName} = {values} ");
                        break;
                    case "boolean":
                        exp = exp.Where($"{tableName}.{propertyName} = {Convert.ToInt16(values)} ");
                        break;
                    case "datetime":
                        if (propertyName.StartsWith("begin".ToLower()) || propertyName.EndsWith("begin".ToLower())
                            || propertyName.StartsWith("start".ToLower()) || propertyName.EndsWith("start".ToLower()))

                        {
                            exp = exp.Where($"{tableName}.{propertyName} >= {values} ");
                        }
                        else if (propertyName.StartsWith("end".ToLower()) || propertyName.EndsWith("end".ToLower()))
                        {
                            exp = exp.Where($"{tableName}.{propertyName} <= {values} ");
                        }
                        break;
                    default:
                        throw new Exception("类型不存在，请联系管理员");
                }

            }
            // }
            exp = exp.OrderBy((u) => u.CreateDate, OrderByType.Desc);//根据创建时间

            return exp;
        }


        public PageInfo GetPages<TReq, TRes>(TReq req) where TReq : Pagination
        {
            PageInfo pageInfo = new PageInfo();
            var exp = _db.Queryable<T>();
            #region 加载数据权限
            exp = GetCurrentUserDataRange(exp);
            #endregion
            var res = exp
                .Skip((req.PageIndex - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToList();
            pageInfo.data = _mapper.Map<List<TRes>>(res);
            pageInfo.total = exp.Count();
            return pageInfo;
        }

        public PageInfo GetPages<TReq>(TReq req) where TReq : class
        {
            throw new NotImplementedException();
        }

        public virtual async Task<List<TRes>> GetListAllAsync<TRes>() where TRes : class
        {
            var exp = await base.GetAllAsync();
            #region 加载数据权限
            exp = GetCurrentUserDataRange(exp);
            #endregion
            return _mapper.Map<List<TRes>>(exp);
        }

        public List<TRes> GetListAll<TRes>() where TRes : class
        {
            var exp = base.GetAll();
            #region 加载数据权限
            exp = GetCurrentUserDataRange(exp);
            #endregion
            return _mapper.Map<List<TRes>>(exp);
        }

        public List<TRes> GetListByQuery<TRes>(Expression<Func<T, bool>> expression) where TRes : class
        {
            var exp = base.GetListByWhere(expression);
            exp = GetCurrentUserDataRange(exp);
            return _mapper.Map<List<TRes>>(exp);
        }

        public async Task<List<TRes>> GetListByQueryAsync<TRes>(Expression<Func<T, bool>> expression) where TRes : class
        {
            var exp = await base.GetListByWhereAsync(expression);
            exp = GetCurrentUserDataRange(exp); 
            return _mapper.Map<List<TRes>>(exp);
        }

        /// <summary>
        /// 获取当前登录人的权限
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public List<T> GetCurrentUserDataRange(List<T> exp)
        {
            #region 加载数据权限
            var userInfo = _currentUser;
            if (userInfo != null)
            {
                if (userInfo.isAdmin || userInfo.isPower)
                {//若果是管理员或没有限制，不做任何处理
                }
                else
                {
                    //如果只能查看自己的数据
                    if (userInfo.isOnlySelf)
                    {
                        exp = exp.Where(a => a.CreateUserId == userInfo.loginUser.Id).ToList();
                    }
                    if (userInfo.deptDataIds.Count > 0)
                    {
                        string deptIds = string.Join(",", userInfo.deptDataIds.ToArray());
                        exp = exp.Where(a => deptIds.Contains(a.DeptId) || a.DeptId.Contains(deptIds)).ToList();
                        //exp = exp.Where(a => a.DeptId.Contains(deptIds)).ToList();
                    }
                }
            }
            #endregion
            return exp;
        }



        //public virtual async Task<PageInfo> GetTreeAsync<TReq, TRes>(TReq req)
        //    where TReq : Pagination
        //    where TRes : class
        //{
        //    PageInfo pageInfo = new PageInfo();
        //    //影响构造树的条件过滤
        //    var exp = _db.Queryable<T>();
        //    exp = GetMappingExpression(req, exp);
        //    var res = await exp.ToListAsync();
        //    object[] inIds = (await exp.Select(it => it.Id).ToListAsync()).Cast<object>().ToArray();

        //    //查找到所有数据转换成树形结构
        //    var listTree = _db.Queryable<T>().Where(a => a.IsDeleted == 0).ToTree(it => it.Children, it => it.ParentId, 0, inIds);
        //    var parentList = _mapper.Map<List<TRes>>(listTree);
        //    pageInfo.total = res.Count;
        //    pageInfo.data = parentList;
        //    return pageInfo;
        //}
    }
}
