using AutoMapper;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Service.Sys
{
    public class SysRoleService : BaseService<SysRole>, ISysRoleService
    {
        private readonly IMapper _mapper;
        public SysRoleService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
        }

        public override async Task<PageInfo> GetPagesAsync<TReq, TRes>(TReq req)
        {
            var reqs = req as SysRoleReq;
            PageInfo pageInfo = new PageInfo();
            var exp = await _db.Queryable<SysRole>()
                .WhereIF(!string.IsNullOrEmpty(reqs.RoleName), u => u.RoleName.Contains(reqs.RoleName))
                .WhereIF(reqs.IsEnable > -1, u => u.IsEnable == reqs.IsEnable)
                .WhereIF(reqs.SystemId > -1, u => u.SystemId == reqs.SystemId)
                .OrderByDescending((u) => u.CreateDate)
                .Select((u) => new SysRoleRes
                {
                    Id = u.Id,
                    RoleName = u.RoleName,
                    Memo = u.Memo,
                    Status = u.IsEnable == 1 ? "可用" : "不可用",
                    CreateDate=u.CreateDate,
                    SystemId=u.SystemId


        }).ToListAsync();
            var res = exp
                .Skip((req.PageIndex - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToList();           
            pageInfo.data = _mapper.Map<List<SysRoleRes>>(res);
            pageInfo.total = exp.Count();
            return pageInfo;
        }
    }
}
