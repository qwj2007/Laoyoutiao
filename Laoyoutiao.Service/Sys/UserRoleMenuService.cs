﻿using AutoMapper;
using AutoMapper.Internal;
using Laoyoutiao.Caches;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Service.Sys
{
    public class UserRoleMenuService : BaseTreeService<Menus>, IUserRoleMenuService
    {
        private readonly IMapper _mapper;
        public UserRoleMenuService(IMapper mapper, CustomCache cache) : base(mapper, cache)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// 根据用户查找到菜单权限
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<PromiseMenu>> GetPromiseMenus(long userId, int isButton = 0,int isShow=-1)
        {
            string sql = @"select sr.RoleName from sys_user us inner join sys_user_role role on us.Id=role.UserId
inner JOIN sys_role sr on sr.Id=role.RoleId where us.IsDeleted=0" ;
            if (userId > 0) {
                sql += " and us.Id =" + userId;
            }
            string?[] roleInfos = await _db.SqlQueryable<SysRole>(sql).Select(a => a.RoleName).ToArrayAsync();

            //超级管理员的账号
            if (roleInfos != null && roleInfos.Contains("超级管理员"))
            {
                sql = @"select menu.Id,menu.MenuUrl Path ,menu.ComponentUrl Component,menu.Icon,menu.ParentId,menu.Name Title,menu.IsButton,menu.Code,menu.ButtonClass,menu.IsShow  
from sys_menu menu where menu.isdeleted=0  ";
            }
            else
            {
                sql = @"select menu.Id, Path , Component,Icon,ParentId, Title,IsButton,Code,ButtonClass,IsShow  from view_menu  menu where menu.Id>0 ";

                
                if (userId > 0)
                {
                    sql += " and  userId=" + userId;
                }
            }
            if (isButton >= 0)
            {
                sql += " and IsButton=" + isButton;
            }
            if (isShow >= 0)
            {
                sql += " and IsShow=" + isShow;
            }
            var listTree = await _db.SqlQueryable<Menus>(sql).ToTreeAsync(it => it.Children, it => it.ParentId, 0);
            var list = _mapper.Map<List<PromiseMenu>>(listTree);
            return list;
        }
    }
}
