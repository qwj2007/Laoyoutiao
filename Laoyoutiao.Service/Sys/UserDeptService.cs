using AutoMapper;
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
        public UserDeptService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
        }

        public async Task<List<long>> GetSelectDeptIdByUserId(long userid)
        {
         return await   _db.Queryable<UserDept>().Where(a => a.IsDeleted == 0 && a.UserId == userid).Select(a=>a.DeptId).ToListAsync();
        }

        public async Task<bool> SaveUserDept(List<UserDeptEdit> list, long operatorId)
        {
            List<UserDept> infos = _mapper.Map<List<UserDept>>(list);

            long userId = list[0].UserId;
            infos.ForEach((item) =>
            {
                item.CreateDate = DateTime.Now;
                item.CreateUserId = operatorId;
            });

            //先删除再添加
            var result = await _db.Ado.UseTranAsync(async () =>
            {
                Expression<Func<UserDept, bool>> expression = a => a.UserId == userId;
                await _db.Deleteable(expression).ExecuteCommandAsync();
                await base.InsertAsync(infos);
            });
            return result.IsSuccess;

        }
    }
}
