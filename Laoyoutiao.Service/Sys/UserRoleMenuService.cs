using AutoMapper;
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
        public UserRoleMenuService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// 根据用户查找到菜单权限
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<PromiseMenu>> GetPromiseMenus(long userId, int isButton = 0)
        {
            //throw new NotImplementedException();
            //PageInfo pageInfo = new PageInfo();
            ////影响构造树的条件过滤
            //var exp = _db.Queryable<Menus>();
            //exp = GetMappingExpression(req, exp);
            //var res = await exp.ToListAsync();
            //object[] inIds = (await exp.Select(it => it.Id).ToListAsync()).Cast<object>().ToArray();


            string sql = @"";
            //1为超级管理员的账号
            if (userId > 1)
            {
                sql = @"select menu.Id, Path , Component,Icon,ParentId, Title,IsButton from view_menu  menu where userid>0";
                sql += " and  userId=" + userId;
            }
            else
            {
                sql = @"select menu.Id,menu.MenuUrl Path ,menu.ComponentUrl Component,menu.Icon,menu.ParentId,menu.Name Title,menu.IsButton from sys_menu menu where menu.isdeleted=0 ";
            }
            sql += " and IsButton=" + isButton;
            //var exp = await _db.SqlQueryable<Menus>(sql).ToListAsync();
            //object[] inIds = (exp.Select(it => it.Id).ToList()).Cast<object>().ToArray();
            var listTree = await _db.SqlQueryable<Menus>(sql).ToTreeAsync(it => it.Children, it => it.ParentId, 0);

            var list = _mapper.Map<List<PromiseMenu>>(listTree);
            return list;

        }
    }
}
