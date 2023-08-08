using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;

namespace Laoyoutiao.IService.Sys
{
    public interface ISysUserRoleService : IBaseService<SysUserRole>
    {
        /// <summary>
        /// 添加用户和角色的关系
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<bool> SaveSysUserRole(List<SysUserRoleEdit> list, long operatorId);
    }
}
