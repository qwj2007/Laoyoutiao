using AutoMapper;
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
        public SysUserRoleService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
        }

        public async Task<bool> SaveSysUserRole(List<SysUserRoleEdit> list,long operatorId)
        {
            List<SysUserRole> infos = _mapper.Map<List<SysUserRole>>(list);
          
            long userId = list[0].UserId;
            infos.ForEach((item) =>
            {
                item.CreateDate = DateTime.Now;
                item.CreateUserId = operatorId;
            });
           
            //先删除再添加
            var result = await _db.Ado.UseTranAsync(async () =>
              {
                  Expression<Func<SysUserRole, bool>> expression = a => a.UserId == userId;
                  await _db.Deleteable(expression).ExecuteCommandAsync();
                  await base.InsertAsync(infos);
              });
            return result.IsSuccess;

        }
    }
}
