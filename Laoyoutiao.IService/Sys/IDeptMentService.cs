using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.IService.Sys
{
    public interface IDeptMentService:IBaseService<DeptMent>
    {
        Task<bool> IsExitChildList(long Id);
    }
}
