using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.IService.Sys
{
    public interface IRoleMenuService : IBaseService<RoleMenu>
    {
        Task<List<RoleMenuRes>> GetExistMenuByRoleId(long roleId);

        Task<bool> SaveRoleMenu(List<RoleMenuEdit> list, long roleId, long operatorId);
    }
}
