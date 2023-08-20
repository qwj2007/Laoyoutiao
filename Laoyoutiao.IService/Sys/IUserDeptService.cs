using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.IService.Sys
{
    public interface IUserDeptService : IBaseService<UserDept>
    {
        Task<List<long>> GetSelectDeptIdByUserId(long userid);
        Task<bool> SaveUserDept(List<UserDeptEdit> list,long userId, long operatorId);
    }
}
