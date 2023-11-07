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
    public class UserDeptService : BaseService<UserDept>, IUserDeptService
    {
        private readonly IMapper _mapper;
        public UserDeptService(IMapper mapper, CustomCache cache) : base(mapper, cache)
        {
            _mapper = mapper;
        }

        public async Task<List<long>> GetSelectDeptIdByUserId(long userid)
        {
         return await   _db.Queryable<UserDept>().Where(a => a.IsDeleted == 0 && a.UserId == userid).Select(a=>a.DeptId).ToListAsync();
        }

        public async Task<bool> SaveUserDept(List<UserDeptEdit> list,long userId, long operatorId)
        {
          

            //先删除再添加
            var result = await _db.Ado.UseTranAsync(async () =>
            {
                Expression<Func<UserDept, bool>> expression = a => a.UserId == userId;
                await _db.Deleteable(expression).ExecuteCommandAsync();
                if (list != null && list.Any()) {
                    List<UserDept> infos = _mapper.Map<List<UserDept>>(list);
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
