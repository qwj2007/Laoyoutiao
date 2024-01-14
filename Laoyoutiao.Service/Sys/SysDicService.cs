using AutoMapper;
using Laoyoutiao.Caches;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Service.Sys
{
    public class SysDicService : BaseService<SysDic>, ISysDicService
    {
        public SysDicService(IMapper mapper, CustomCache cache) : base(mapper, cache)
        {
        }
    }
}
