using AutoMapper;
using Laoyoutiao.Caches;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Service.Sys
{
    public class SystemsService : BaseService<Systems>, ISystemsService
    {
        private readonly IMapper _mapper;
        public SystemsService(IMapper mapper, CustomCache cache) : base(mapper, cache)
        {
            _mapper = mapper;
        }
        public override async Task<PageInfo> GetPagesAsync<TReq, TRes>(TReq req)
        {          
            var reqs = req as SystemsReq;
            PageInfo pageInfo = new PageInfo();
            var exp = await _db.Queryable<Systems>()
                .WhereIF(!string.IsNullOrEmpty(reqs.SystemName), u => u.SystemName.Contains(reqs.SystemName))              
                .OrderByDescending((u) => u.CreateDate)
                .Select((u) => new SystemsRes
                {
                    Id = u.Id,
                    SystemName = u.SystemName,
                    Memo = u.Memo,
                    Status = u.isEnable == 1 ? "可用" : "不可用",
                    CreateDate = u.CreateDate,  
                    SystemCode=u.SystemCode
                }).ToListAsync();
            var res = exp
                .Skip((req.PageIndex - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToList();
            pageInfo.data = _mapper.Map<List<SystemsRes>>(res);
            pageInfo.total = exp.Count();
            return pageInfo;
        }

        //public override bool Add(SystemsEdit t)
        //{
        //    if (t.Id == 0) {
        //        t.SystemCode = Guid.NewGuid().ToString();
        //    }
        //    return base.Add(t);
        //}

        public override async Task<bool> Add<TEdit>(TEdit input)
        {
            var edit = input as SystemsEdit;
            //if (edit.Id == 0) {
            //    edit.SystemCode = Guid.NewGuid().ToString();
            //}
           
            Systems info = _mapper.Map<Systems>(input);            

            if (info.Id == 0)
            {
                info.CreateUserId = _currentUser.loginUser.Id;
                info.CreateDate = DateTime.Now;
                info.SystemCode = Guid.NewGuid().ToString();
                //info.isEnable = edit.Status;
                return await _db.Insertable(info).ExecuteCommandAsync() > 0;

            }
            else
            {
                info.ModifyUserId = _currentUser.loginUser.Id;
                info.ModifyDate = DateTime.Now;              
                return await _db.Updateable(info).ExecuteCommandAsync() > 0;
            }

          
        }
    }
}
