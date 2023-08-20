using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;

namespace Laoyoutiao.IService.Sys
{
    public interface ISysUserRoleService : IBaseService<SysUserRole>
    {
        /// <summary>
        /// 添加用户和角色的关系，用户一对多
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<bool> SaveSysUserRole(List<SysUserRoleEdit> list,long userId, long operatorId);

        /// <summary>
        /// 根据角色ＩＤ查找所有的用户信息
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<List<SysUserRoleRes>> GetExistUserByRoleId(long roleId);
        /// <summary>
        /// 添加用户和角色的关系，角色一对多
        /// </summary>
        /// <param name="list"></param>
        /// <param name="operatorId"></param>
        /// <returns></returns>
        Task<bool> SaveUserRoleByRoles(List<SysUserRoleEdit> list,long roleId, long operatorId);
    }
}
