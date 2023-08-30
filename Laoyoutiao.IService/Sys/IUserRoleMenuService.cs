using Laoyoutiao.Models.Dto.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.IService.Sys
{
    public interface IUserRoleMenuService
    {
        /// <summary>
        /// 根据用户名查找到菜单权限
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        Task<List<PromiseMenu>> GetPromiseMenus(long userId, int isButton = 0, int isShow = -1);
    }
}
