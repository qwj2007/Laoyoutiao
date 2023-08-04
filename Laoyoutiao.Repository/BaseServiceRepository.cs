using Laoyoutiao.IRespository;
using Laoyoutiao.Models.Common;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;
using System.Reflection;

namespace Laoyoutiao.Repository
{
    /// <summary>
    /// 基类仓储
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseServiceRepository<T> : SimpleClient<T>,IBaseServiceRepository<T> where T : BaseEntity, new()
    {
        public ITenant? itenant = null;//多租户事务
        public ISqlSugarClient _db
        {

            get
            {
                return Context;
            }
        }
        public BaseServiceRepository(ISqlSugarClient? context = null) : base(context)
        {
            //通过特性拿到ConfigId
            var configId = typeof(T).GetCustomAttribute<TenantAttribute>()?.configId;
            if (configId != null)
            {
                Context = DbScoped.SugarScope.GetConnectionScope(configId);//根据类传入的ConfigId自动选择
            }
            else
            {
                Context = context ?? DbScoped.SugarScope.GetConnectionScope(0);//没有默认db0
            }
            itenant = DbScoped.SugarScope;//处理多租户事务、GetConnection、IsAnyConnection等功能
            //CreateDB(Context, configId.ToString());
        }
        private void CreateDB(ISqlSugarClient client, string configID)
        {
            client.DbMaintenance.CreateDatabase();//没有数据库的时候创建数据库
            //var tableLists = client.DbMaintenance.GetTableInfoList();
            var files = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Laoyoutiao.Models.dll");
            if (files.Length > 0)
            {

                //Type[] types = Assembly.LoadFrom(files[0]).GetTypes().Where(it => it.BaseType == typeof(BaseEntity)).ToArray();
                //更新数据库字段，如果有修改就更新
                Type[] types = Assembly.LoadFrom(files[0]).GetTypes().Where(it => it.BaseType == typeof(BaseEntity) 
                && it.GetCustomAttribute<TenantAttribute>().configId.ToString() == configID).ToArray();
                client.CodeFirst.SetStringDefaultLength(200).InitTables(types);

                // foreach (var entityType in types)
                // {
                //     //创建数据表
                //     string tableName = entityType.GetCustomAttribute<SugarTable>().TableName;//根据特性获取表名称
                //     //var configid = entityType.GetCustomAttribute<TenantAttribute>()?.configId;//根据特性获取租户id
                //     //configid = configid == null ? "0" : configid.ToString();
                //     if (!tableLists.Any(p => p.Name == tableName))
                //     {
                //         //创建数据表包括字段更新
                //         client.CodeFirst.SetStringDefaultLength(200).InitTables(entityType);
                //     }
                // }
            }
        }

        #region 基础方法
        /// <summary>
        /// 新增一条数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual bool Add(T t)
        {
            int rowsAffect = Context.Insertable(t).IgnoreColumns(true).ExecuteCommand();
            return rowsAffect > 0;
        }

        /// <summary>
        /// 新增一条数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual async Task<bool> AddAsync(T t)
        {
            int rowsAffect = await Context.Insertable(t).IgnoreColumns(true).ExecuteCommandAsync();
            return rowsAffect > 0;
        }
        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual bool Insert(List<T> t)
        {
            int rowsAffect = Context.Insertable(t).ExecuteCommand();
            return rowsAffect > 0;
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual async Task<bool> InsertAsync(List<T> t)
        {
            int rowsAffect = await Context.Insertable(t).ExecuteCommandAsync();
            return rowsAffect > 0;
        }
        /// <summary>
        /// 插入设置列数据
        /// </summary>
        /// <param name="parm"></param>
        /// <param name="iClumns"></param>
        /// <param name="ignoreNull"></param>
        /// <returns></returns>
        public virtual bool Insert(T parm, Expression<Func<T, object>>? iClumns = null, bool ignoreNull = true)
        {

            int rowsAffect = Context.Insertable(parm).InsertColumns(iClumns).IgnoreColumns(ignoreNullColumn: ignoreNull).ExecuteCommand();
            return rowsAffect > 0;

        }

        /// <summary>
        /// 插入设置列数据
        /// </summary>
        /// <param name="parm"></param>
        /// <param name="iClumns"></param>
        /// <param name="ignoreNull"></param>
        /// <returns></returns>
        public virtual async Task<bool> InsertAsync(T parm, Expression<Func<T, object>>? iClumns = null, bool ignoreNull = true)
        {

            int rowsAffect = await Context.Insertable(parm).InsertColumns(iClumns).IgnoreColumns(ignoreNullColumn: ignoreNull).ExecuteCommandAsync();
            return rowsAffect > 0;

        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignoreNullColumns">是否更新null的字段</param>
        /// <returns></returns>
        public virtual bool Update(T entity, bool ignoreNullColumns = false)
        {
            int rowsAffect = Context.Updateable(entity).IgnoreColumns(ignoreNullColumns).ExecuteCommand();
            return rowsAffect >= 0;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignoreNullColumns">是否更新null的字段</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(T entity, bool ignoreNullColumns = false)
        {
            int rowsAffect = await Context.Updateable(entity).IgnoreColumns(ignoreNullColumns).ExecuteCommandAsync();
            return rowsAffect >= 0;
        }
        /// <summary>
        /// 根据实体类更新指定列 eg：Update(dept, it => new { it.Status });只更新Status列，条件是包含
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="expression"></param>
        /// <param name="ignoreAllNull"></param>
        /// <returns></returns>
        public virtual bool Update(T entity, Expression<Func<T, object>> expression, bool ignoreAllNull = false)
        {
            int rowsAffect = Context.Updateable(entity).UpdateColumns(expression).IgnoreColumns(ignoreAllNull).ExecuteCommand();
            return rowsAffect >= 0;
        }
        /// <summary>
        /// 根据实体类更新指定列 eg：Update(dept, it => new { it.Status });只更新Status列，条件是包含
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="expression"></param>
        /// <param name="ignoreAllNull"></param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(T entity, Expression<Func<T, object>> expression, bool ignoreAllNull = false)
        {
            int rowsAffect = await Context.Updateable(entity).UpdateColumns(expression).IgnoreColumns(ignoreAllNull).ExecuteCommandAsync();
            return rowsAffect >= 0;
        }
        /// <summary>
        /// 根据实体类更新指定列 eg：Update(dept, it => new { it.Status }, f => depts.Contains(f.DeptId));只更新Status列，条件是包含
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="expression"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual bool Update(T entity, Expression<Func<T, object>> expression, Expression<Func<T, bool>> where)
        {
            int rowsAffect = Context.Updateable(entity).UpdateColumns(expression).Where(where).ExecuteCommand();
            return rowsAffect >= 0;
        }

        /// <summary>
        /// 根据实体类更新指定列 eg：Update(dept, it => new { it.Status }, f => depts.Contains(f.DeptId));只更新Status列，条件是包含
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="expression"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(T entity, Expression<Func<T, object>> expression, Expression<Func<T, bool>> where)
        {
            int rowsAffect = await Context.Updateable(entity).UpdateColumns(expression).Where(where).ExecuteCommandAsync();
            return rowsAffect >= 0;
        }
        /// <summary>
        /// 更新指定列 eg：Update(w => w.NoticeId == model.NoticeId, it => new SysNotice(){ UpdateTime = DateTime.Now, Title = "通知标题" });
        /// </summary>
        /// <param name="where"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public virtual bool Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> columns)
        {
            int rowsAffect = Context.Updateable<T>().SetColumns(columns).Where(where).RemoveDataCache().ExecuteCommand();
            return rowsAffect >= 0;

        }
        /// <summary>
        /// 更新指定列 eg：Update(w => w.NoticeId == model.NoticeId, it => new SysNotice(){ UpdateTime = DateTime.Now, Title = "通知标题" });
        /// </summary>
        /// <param name="where"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(Expression<Func<T, bool>> where, Expression<Func<T, T>> columns)
        {
            int rowsAffect = await Context.Updateable<T>().SetColumns(columns).Where(where).RemoveDataCache().ExecuteCommandAsync();
            return rowsAffect >= 0;

        }
        /// <summary>
        /// 事务 eg:var result = UseTran(() =>{SysRoleRepository.UpdateSysRole(sysRole);DeptService.DeleteRoleDeptByRoleId(sysRole.ID);DeptService.InsertRoleDepts(sysRole);});
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual bool UseTran(Action action)
        {
            try
            {
                var result = Context.Ado.UseTran(() => action());
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                Context.Ado.RollbackTran();
                throw ex;
            }
        }
        public virtual async Task<bool> UseTranAsync(Func<Task> action)
        {
            try
            {
                var result = await Context.Ado.UseTranAsync(() => action());
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                Context.Ado.RollbackTran();
                throw ex;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids">主键id</param>
        /// <param name="IsDelete">是否真删除</param>
        /// <returns></returns>
        public virtual bool Delete(object[] ids, bool IsDelete = true)
        {
            int rowsAffect = 0;
            try
            {
                if (IsDelete)
                {
                    rowsAffect = Context.Deleteable<T>().In(ids).ExecuteCommand();
                }
                else
                {
                    //假删除 实体属性有isdelete或者isdeleted 请升级到5.0.4.9+，（5.0.4.3存在BUG）
                    rowsAffect = Context.Deleteable<T>().In(ids).IsLogic().ExecuteCommand();
                }
                return rowsAffect >= 0;
            }
            catch (Exception ex)
            {
                throw ex;
                //return false;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids">主键id</param>
        /// <param name="IsDelete">是否真删除</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(object[] ids, bool IsDelete = false)
        {
            int rowsAffect = 0;
            try
            {
                if (IsDelete)
                {
                    rowsAffect = await Context.Deleteable<T>().In(ids).ExecuteCommandAsync();
                }
                else
                {
                    //假删除 实体属性有isdelete或者isdeleted 请升级到5.0.4.9+，（5.0.4.3存在BUG）
                    rowsAffect = await Context.Deleteable<T>().In(ids).IsLogic().ExecuteCommandAsync();
                }
                return rowsAffect >= 0;
            }
            catch (Exception ex)
            {
                throw ex;
                //return false;
            }
        }
        /// <summary>
        /// 根据id获取数据
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns>泛型实体</returns>
        public virtual T GetEntityById(long id)
        {
            return Context.Queryable<T>().First(p => p.Id == id);
        }
        /// <summary>
        /// 根据id获取数据异步
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns>泛型实体</returns>
        public virtual async Task<T> GetEntityByIdAsync(long id)
        {
            return await Context.Queryable<T>().FirstAsync(p => p.Id == id);
        }


        /// <summary>
        /// 数据是否存在
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual bool IsExists(Expression<Func<T, bool>> expression)
        {
            return Context.Queryable<T>().Where(expression).Any();
        }
        /// <summary>
        /// 数据是否存在(异步)
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<bool> IsExistsAsync(Expression<Func<T, bool>> expression)
        {
            return await Context.Queryable<T>().Where(expression).AnyAsync();
        }
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public virtual List<T> GetAll()
        {
            return Context.Queryable<T>().ToList();
        }
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<T>> GetAllAsync()
        {
            return await Context.Queryable<T>().ToListAsync();
        }
        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual List<T> GetListByWhere(Expression<Func<T, bool>> expression)
        {
            return Context.Queryable<T>().Where(expression).ToList();
        }
        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListByWhereAsync(Expression<Func<T, bool>> expression)
        {
            return await Context.Queryable<T>().Where(expression).ToListAsync();
        }
        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="orderFiled">排序字段</param>
        /// <param name="orderEnum">排序方式</param>
        /// <returns></returns>
        public virtual List<T> GetList(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderFiled, OrderByType orderEnum = OrderByType.Asc)
        {
            return Context.Queryable<T>().Where(expression).OrderByIF(orderEnum == OrderByType.Asc, orderFiled, OrderByType.Asc).OrderByIF(orderEnum == OrderByType.Desc, orderFiled, OrderByType.Desc).ToList();
        }
        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="orderFiled">排序字段</param>
        /// <param name="orderEnum">排序方式</param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderFiled, OrderByType orderEnum = OrderByType.Asc)
        {
            return await Context.Queryable<T>().Where(expression).OrderByIF(orderEnum == OrderByType.Asc, orderFiled, OrderByType.Asc).OrderByIF(orderEnum == OrderByType.Desc, orderFiled, OrderByType.Desc).ToListAsync();
        }
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="pageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public virtual PageInfo GetPageList(Expression<Func<T, bool>> expression, int pageIndex, int PageSize)
        {
            int totalCount = 0;
            var result = Context.Queryable<T>().Where(expression).ToPageList(pageIndex, PageSize, ref totalCount);
            var pageResult = new PageInfo();
            pageResult.data = result;
            pageResult.total = totalCount;
            return pageResult;
        }
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="pageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public virtual async Task<PageInfo> GetPageListAsync(Expression<Func<T, bool>> expression, int pageIndex, int PageSize)
        {
            RefAsync<int> totalCount = 0;
            var result = await Context.Queryable<T>().Where(expression).ToPageListAsync(pageIndex, PageSize, totalCount);
            var pageResult = new PageInfo();
            pageResult.data = result;
            pageResult.total = totalCount;
            return pageResult;
        }
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="pageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="orderFiled"></param>
        /// <param name="orderEnum"></param>
        /// <returns></returns>
        public virtual PageInfo GetPageList(Expression<Func<T, bool>> expression, int pageIndex, int PageSize, Expression<Func<T, object>> orderFiled, OrderByType orderEnum = OrderByType.Asc)
        {
            int totalCount = 0;
            var result = Context.Queryable<T>().Where(expression).OrderByIF(orderEnum == OrderByType.Asc, orderFiled, OrderByType.Asc).OrderByIF(orderEnum == OrderByType.Desc, orderFiled, OrderByType.Desc)
                .ToPageList(pageIndex, PageSize, ref totalCount);
            var pageResult = new PageInfo();
            pageResult.data = result;
            pageResult.total = totalCount;
            return pageResult;
        }
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="pageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="orderFiled"></param>
        /// <param name="orderEnum"></param>
        /// <returns></returns>
        public virtual async Task<PageInfo> GetPageListAsync(Expression<Func<T, bool>> expression, int pageIndex, int PageSize, Expression<Func<T, object>> orderFiled, OrderByType orderEnum = OrderByType.Asc)
        {
            RefAsync<int> totalCount = 0;
            var result = await Context.Queryable<T>().Where(expression).OrderByIF(orderEnum == OrderByType.Asc, orderFiled, OrderByType.Asc).OrderByIF(orderEnum == OrderByType.Desc, orderFiled, OrderByType.Desc)
                .ToPageListAsync(pageIndex, PageSize, totalCount);
            var pageResult = new PageInfo();
            pageResult.data = result;
            pageResult.total = totalCount;
            return pageResult;
        }

        //*/

        #endregion
    }
}
