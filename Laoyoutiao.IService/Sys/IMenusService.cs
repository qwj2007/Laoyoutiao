using Laoyoutiao.Models.Dto.Sys;

using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.IService.Sys
{
    public interface IMenusService:IBaseTreeService<Menus>
    {
        Task<bool> IsExitChildList(long Id);
        /// <summary>
        /// 查找这个页面的按钮信息
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        Task<List<MenusRes>> GetChildButtons(long parentId);
        Task GetButtonOprate(List<MenusRes> list,List<Menus> allMenus);
    }
}
