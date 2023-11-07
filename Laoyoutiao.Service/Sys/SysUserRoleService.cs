using AutoMapper;
using Laoyoutiao.Caches;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace Laoyoutiao.Service.Sys
{
    public class SysUserRoleService : BaseService<SysUserRole>, ISysUserRoleService
    {
        private readonly IMapper _mapper;
        public SysUserRoleService(IMapper mapper, CustomCache cache) : base(mapper, cache)
        {
            _mapper = mapper;
        }
        /// <summary>
        /// 根据角色Id查找所有的用户信息
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<List<SysUserRoleRes>> GetExistUserByRoleId(long roleId)
        {
          var infos= await  _db.Queryable<SysUserRole>().Where(a => a.IsDeleted == 0 && a.RoleId == roleId).ToListAsync();
          return  _mapper.Map<List<SysUserRoleRes>>(infos);
        }

        public async Task<bool> SaveSysUserRole(List<SysUserRoleEdit> list,long userId,long operatorId)
        {
           
           
            //先删除再添加
            var result = await _db.Ado.UseTranAsync(async () =>
              {
                  Expression<Func<SysUserRole, bool>> expression = a => a.UserId == userId;
                  await _db.Deleteable(expression).ExecuteCommandAsync();
                  if (list != null && list.Count > 0)
                  {
                      List<SysUserRole> infos = _mapper.Map<List<SysUserRole>>(list);
                      //long userId = list[0].UserId;
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
        public async Task<bool> SaveUserRoleByRoles(List<SysUserRoleEdit> list,long roleId, long operatorId)
        {
           

            //先删除再添加
            var result = await _db.Ado.UseTranAsync(async () =>
            {
                Expression<Func<SysUserRole, bool>> expression = a => a.RoleId == roleId;
                await _db.Deleteable(expression).ExecuteCommandAsync();
                if (list != null && list.Count > 0) {
                    List<SysUserRole> infos = _mapper.Map<List<SysUserRole>>(list);
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
