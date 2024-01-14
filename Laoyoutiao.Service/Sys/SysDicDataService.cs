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
    public class SysDicDataService : BaseService<SysDicData>, ISysDicDataService
    {
        public SysDicDataService(IMapper mapper, CustomCache cache) : base(mapper, cache)
        {
        }
    }
}
