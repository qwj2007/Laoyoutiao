using AutoMapper;
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
    }
}
