using Laoyoutiao.Models.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Laoyoutiao.IRespository
{

    public interface IBaseServiceRepository<T>
    {
        /// <summary>
        /// 新增一条数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        bool Add(T t);

        /// <summary>
        /// 新增一条数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<bool> AddAsync(T t);
        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        bool Insert(List<T> t);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<bool> InsertAsync(List<T> t);
        /// <summary>
        /// 插入设置列数据
        /// </summary>
        /// <param name="parm"></param>
        /// <param name="iClumns"></param>
        /// <param name="ignoreNull"></param>
        /// <returns></returns>
        bool Insert(T parm, Expression<Func<T, object>> iClumns = null, bool ignoreNull = true);

        /// <summary>
        /// 插入设置列数据
        /// </summary>
        /// <param name="parm"></param>
        /// <param name="iClumns"></param>
        /// <param name="ignoreNull"></param>
        /// <returns></returns>
        Task<bool> InsertAsync(T parm, Expression<Func<T, object>> iClumns = null, bool ignoreNull = true);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignoreNullColumns">是否更新null的字段</param>
        /// <returns></returns>
        bool Update(T entity, bool ignoreNullColumns = false);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignoreNullColumns">是否更新null的字段</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T entity, bool ignoreNullColumns = false);
        /// <summary>
        /// 根据实体类更新指定列 eg：Update(dept, it => new { it.Status });只更新Status列，条件是包含
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="expression"></param>
        /// <param name="ignoreAllNull"></param>
        /// <returns></returns>
        bool Update(T entity, Expression<Func<T, object>> expression, bool ignoreAllNull = false);
        /// <summary>
        /// 根据实体类更新指定列 eg：Update(dept, it => new { it.Status });只更新Status列，条件是包含
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="expression"></param>
        /// <param name="ignoreAllNull"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T entity, Expression<Func<T, object>> expression, bool ignoreAllNull = false);
        /// <summary>
        /// 根据实体类更新指定列 eg：Update(dept, it => new { it.Status }, f => depts.Contains(f.DeptId));只更新Status列，条件是包含
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="expression"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        bool Update(T entity, Expression<Func<T, object>> expression, Expression<Func<T, bool>> where);

        /// <summary>
        /// 根据实体类更新指定列 eg：Update(dept, it => new { it.Status }, f => depts.Contains(f.DeptId));只更新Status列，条件是包含
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="expression"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T entity, Expression<Func<T, object>> expression, Expression<Func<T, bool>> where);
        /// <summary>
        /// 更新指定列 eg：Update(w => w.NoticeId == model.NoticeId, it => new SysNotice(){ UpdateTime = DateTime.Now, Title = "通知标题" });
        /// </summary>
        /// <param name="where"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        bool Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> columns);
        /// <summary>
        /// 更新指定列 eg：Update(w => w.NoticeId == model.NoticeId, it => new SysNotice(){ UpdateTime = DateTime.Now, Title = "通知标题" });
        /// </summary>
        /// <param name="where"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(Expression<Func<T, bool>> where, Expression<Func<T, T>> columns);
        /// <summary>
        /// 事务 eg:var result = UseTran(() =>{SysRoleRepository.UpdateSysRole(sysRole);DeptService.DeleteRoleDeptByRoleId(sysRole.ID);DeptService.InsertRoleDepts(sysRole);});
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        bool UseTran(Action action);
        Task<bool> UseTranAsync(Func<Task> action)
       ;
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids">主键id</param>
        /// <param name="IsDelete">是否真删除</param>
        /// <returns></returns>
        bool Delete(object[] ids, bool IsDelete = false);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids">主键id</param>
        /// <param name="IsDelete">是否真删除</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(object[] ids, bool IsDelete = false)
       ;
        /// <summary>
        /// 根据id获取数据
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns>泛型实体</returns>
        T GetEntityById(long id);
        /// <summary>
        /// 根据id获取数据异步
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns>泛型实体</returns>
        Task<T> GetEntityByIdAsync(long id);


        /// <summary>
        /// 数据是否存在
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        bool IsExists(Expression<Func<T, bool>> expression);
        /// <summary>
        /// 数据是否存在(异步)
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<bool> IsExistsAsync(Expression<Func<T, bool>> expression)
       ;
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        List<T> GetAll();
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        Task<List<T>> GetAllAsync()
       ;
        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        List<T> GetListByWhere(Expression<Func<T, bool>> expression);
        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<List<T>> GetListByWhereAsync(Expression<Func<T, bool>> expression);
        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="orderFiled">排序字段</param>
        /// <param name="orderEnum">排序方式</param>
        /// <returns></returns>
        List<T> GetList(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderFiled, OrderByType orderEnum = OrderByType.Asc);
        /// <summary>
        /// 根据查询条件获取数据
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="orderFiled">排序字段</param>
        /// <param name="orderEnum">排序方式</param>
        /// <returns></returns>
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> expression, Expression<Func<T, object>> orderFiled, OrderByType orderEnum = OrderByType.Asc);
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="pageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        PageInfo GetPageList(Expression<Func<T, bool>> expression, int pageIndex, int PageSize);
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="pageIndex"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        Task<PageInfo> GetPageListAsync(Expression<Func<T, bool>> expression, int pageIndex, int PageSize)
      ;
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="pageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="orderFiled"></param>
        /// <param name="orderEnum"></param>
        /// <returns></returns>
        PageInfo GetPageList(Expression<Func<T, bool>> expression, int pageIndex, int PageSize, Expression<Func<T, object>> orderFiled, OrderByType orderEnum = OrderByType.Asc);

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="pageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="orderFiled"></param>
        /// <param name="orderEnum"></param>
        /// <returns></returns>
        Task<PageInfo> GetPageListAsync(Expression<Func<T, bool>> expression, int pageIndex, int PageSize, Expression<Func<T, object>> orderFiled, OrderByType orderEnum = OrderByType.Asc);


    }

}
