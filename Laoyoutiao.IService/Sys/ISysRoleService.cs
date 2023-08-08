using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;

namespace Laoyoutiao.IService.Sys
{
    public interface ISysRoleService : IBaseService<SysRole>
    {
        /// <summary>
        /// 根据用户名查找这个用户已经存在的角色名称
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<SysRoleRes>> GetRolesByUserId(long userId);

        /// <summary>
        /// 查找这个用户没有指定的角色名称
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
       // Task<List<SysRoleRes>> GetNoExistRolesByUserId(long userId);

    }
}
