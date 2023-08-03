using AutoMapper;
using Laoyoutiao.IService;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Dto;
using Laoyoutiao.Models.Dto.User;
using Laoyoutiao.Models.Entitys;
using Laoyoutiao.Repository;
using Microsoft.AspNetCore.Http;
using SqlSugar;
using System.ComponentModel.Design;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Laoyoutiao.Service
{
    public class BaseService<T> : BaseServiceRepository<T>, IBaseService<T> where T : BaseEntity, new()
    {
        private IMapper _mapper;
        public BaseService(IMapper mapper)
        {
            this._mapper = mapper;
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
            if (info.Id == 0)
            {
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
            PageInfo pageInfo = new PageInfo();
            var exp = _db.Queryable<T>();
            exp = getMappingExpression(req, exp);
            var res = await exp
                .Skip((req.PageIndex - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync();
            pageInfo.data = _mapper.Map<List<TRes>>(res);
            pageInfo.total = exp.Count();
            return pageInfo;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <param name="req"></param>
        /// <param name="exp"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static ISugarQueryable<T> getMappingExpression<TReq>(TReq req, ISugarQueryable<T> exp) where TReq : Pagination
        {
            PropertyInfo[] propertyInfos = req.GetType().GetProperties();

            PropertyInfo[] propertyInfosSource = typeof(T).GetProperties();
            foreach (var property in propertyInfos)
            {
                //判断这个字段是否有【PropertyMapperAttribute】标记，如果有就以标记的属性名为准，
                var properMaapered = property.IsDefined(typeof(PropertyMapperAttribute));
                var propertyName = "";
                var values = property.GetValue(req);//得到值
                if (properMaapered)
                {
                    var propertyAttribute = (PropertyMapperAttribute)property.GetCustomAttributes(typeof(PropertyMapperAttribute)).FirstOrDefault();
                    //得到
                    propertyName = propertyAttribute.SourceName;
                }
                else
                {
                    //如果没有，就检测源T是否有这个属性。
                    var souceType = propertyInfosSource.Where(a => a.Name == property.Name).FirstOrDefault();
                    if (souceType != null)
                    {
                        propertyName = property.Name;
                    }
                    else {
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
                            exp = exp.WhereIF(!string.IsNullOrWhiteSpace(Convert.ToString(values)), $"{typeof(T).Name}.{propertyName} like '%{values}%' ");
                            break;
                        case "int32":
                        case "int64":
                        case "decimal":
                            exp = exp.Where($"{typeof(T).Name}.{propertyName} = {values} ");
                            break;
                        case "boolean":
                            exp = exp.Where($"{typeof(T).Name}.{propertyName} = {Convert.ToInt16(values)} ");
                            break;
                        case "datetime":
                            if (propertyName.StartsWith("begin".ToLower()) || propertyName.EndsWith("begin".ToLower())
                                || propertyName.StartsWith("start".ToLower()) || propertyName.EndsWith("start".ToLower())
                                )
                            {
                                exp = exp.Where($"{typeof(T).Name}.{propertyName} >= {values} ");
                            }
                            else if (propertyName.StartsWith("end".ToLower()) || propertyName.EndsWith("end".ToLower()))
                            {
                                exp = exp.Where($"{typeof(T).Name}.{propertyName} <= {values} ");
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
    }
}
