using AutoMapper;
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
        public BtnService(IMapper mapper) : base(mapper)
        {

        }
        public override Task<bool> AddAsync(SysButton t)
        {
            t.BtnType = BtnEnum.SysButton;
            return base.AddAsync(t);
        }
    }
}
