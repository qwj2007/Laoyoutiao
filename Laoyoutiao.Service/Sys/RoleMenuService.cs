using AutoMapper;
using Laoyoutiao.Caches;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Service.Sys
{
    public class RoleMenuService : BaseService<RoleMenu>, IRoleMenuService
    {
        private readonly IMapper _mapper;
        public RoleMenuService(IMapper mapper, CurrentUserCache cache) : base(mapper, cache)
        {
            _mapper = mapper;
        }
        
        public async Task<List<RoleMenuRes>> GetExistMenuByRoleId(long roleId)
        {
            var infos = await _db.Queryable<RoleMenu>().Where(a => a.IsDeleted == 0 && a.RoleId == roleId).ToListAsync();
            return _mapper.Map<List<RoleMenuRes>>(infos);
        }

        public async Task<bool> SaveRoleMenu(List<RoleMenuEdit> list, long roleId, long operatorId)
        {
            //先删除再添加
            var result = await _db.Ado.UseTranAsync(async () =>
            {
                Expression<Func<RoleMenu, bool>> expression = a => a.RoleId == roleId&&a.IsDeleted==0;
                await _db.Deleteable(expression).ExecuteCommandAsync();
                if (list != null && list.Count > 0)
                {
                    List<RoleMenu> infos = _mapper.Map<List<RoleMenu>>(list);
                    infos.ForEach((item) =>
                    {
                        item.CreateDate = DateTime.Now;
                        item.CreateUserId = operatorId;
                    });
                    await base.InsertAsync(infos);
                }

            });
            return result.IsSuccess;
        }
    }
}
