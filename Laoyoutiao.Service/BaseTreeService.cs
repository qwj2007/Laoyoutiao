using AutoMapper;
using Laoyoutiao.IService;
using Laoyoutiao.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Service
{
    public class BaseTreeService<T> : BaseService<T> ,IBaseTreeService<T> where T : BaseTreeEntity<T>, new()
    {
        private readonly IMapper _mapper;
        
        public BaseTreeService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
        }

        public virtual async Task<PageInfo> GetTreeAsync<TReq, TRes>(TReq req)
            where TReq : Pagination
            where TRes : class
        {
            PageInfo pageInfo = new PageInfo();
            //影响构造树的条件过滤
            var exp = _db.Queryable<T>();
            exp = GetMappingExpression(req, exp);
            var res = await exp.ToListAsync();
            object[] inIds = (await exp.Select(it => it.Id).ToListAsync()).Cast<object>().ToArray();

            //查找到所有数据转换成树形结构
            var listTree = _db.Queryable<T>().Where(a => a.IsDeleted == 0).ToTree(it => it.Children, it => it.ParentId, 0, inIds);
            var parentList = _mapper.Map<List<TRes>>(listTree);
            pageInfo.total = res.Count;
            pageInfo.data = parentList;
            return pageInfo;
        }
    }
}
