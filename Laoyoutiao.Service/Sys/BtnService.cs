using AutoMapper;
using Laoyoutiao.Caches;
using Laoyoutiao.Enums;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Service.Sys
{
    public class BtnService : BaseService<SysButton>, IBtnService
    {
        public BtnService(IMapper mapper, CurrentUserCache cache) : base(mapper, cache)
        {

        }
        public override Task<bool> AddAsync(SysButton t)
        {
            t.BtnType = BtnEnum.SysButton;
            return base.AddAsync(t);
        }
    }
}
