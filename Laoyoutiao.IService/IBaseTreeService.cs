using Laoyoutiao.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.IService
{
    public interface IBaseTreeService<T>:IBaseService<T> where T:BaseEntity,new()
    {
        Task<PageInfo> GetTreeAsync<TReq, TRes>(TReq req) where TReq : Pagination where TRes : class;
    }
}
