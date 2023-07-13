using AutoMapper;
using Laoyoutiao.IService;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.User;
using Laoyoutiao.Models.Entitys;
using Microsoft.Extensions.Configuration;
using SqlSugar;

namespace Laoyoutiao.Service
{

    public class UsersService : BaseService<Users>, IUserService
    {
        private readonly IMapper _mapper;
        public UsersService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserRes GetUser(string userName, string password)
        {
            var user = _db.Queryable<Users>().Where(u => u.Name == userName && u.Password == password).First();
            if (user != null)
            {
                return _mapper.Map<UserRes>(user);
            }
            return new UserRes();
        }

        public override async Task<PageInfo> GetPagesAsync<UserReq, UserRes>(UserReq req)
        {        
            var userReq = req as Laoyoutiao.Models.Dto.User.UserReq;
            PageInfo pageInfo = new PageInfo();
            var exp = await _db.Queryable<Users>()
                //默认只查询非炒鸡管理员的用户
                .Where(u => u.UserType == 1)
                .WhereIF(!string.IsNullOrEmpty(userReq.Name), u => u.Name.Contains(userReq.Name))
                .WhereIF(!string.IsNullOrEmpty(userReq.NickName), u => u.NickName.Contains(userReq.NickName))
                .OrderBy((u) => u.CreateDate, OrderByType.Desc)
                .Select((u) => new Laoyoutiao.Models.Dto.User.UserRes
                {
                    Id = u.Id
                ,
                    Name = u.Name
                ,
                    NickName = u.NickName
                ,
                    Password = u.Password
                ,
                    UserType = u.UserType
                //,
                //    RoleName = GetRolesByUserId(u.Id)
                ,
                    CreateDate = u.CreateDate
                ,
                    IsEnable = u.IsEnable
                ,
                    Description = u.Description
                }).ToListAsync();
            var res = exp
                .Skip((req.PageIndex - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToList();
            res.ForEach(p =>
            {
                p.RoleName = GetRolesByUserId(p.Id);
            });
            pageInfo.data = _mapper.Map<List<UserRes>>(res);
            pageInfo.total = exp.Count();
            return pageInfo;

        }

        private string GetRolesByUserId(long uid)
        {
            return _db.Ado.SqlQuery<string>($@"SELECT STUFF((SELECT ','+R.Name FROM dbo.Role R
                    LEFT JOIN dbo.UserRoleRelation UR ON R.Id=UR.RoleId
                    WHERE UR.UserId={uid} FOR XML PATH('')),1,1,'') RoleNames")[0];
        }
        public UserRes GetUsersById(long id)
        {
            var info = GetEntityById(id);// _db.Queryable<Users>().First(p => p.Id == id);
            return _mapper.Map<UserRes>(info);
        }
        public bool SettingRole(string pid, string rids)
        {
            //1,2,3,4,5
            List<UserRoleRelation> list = (from string it in rids.Split(',')
                                           let info = new UserRoleRelation() { UserId = Convert.ToInt64(pid), RoleId = Convert.ToInt64(it.Replace("'", "")) }
                                           select info).ToList();
            //删除之前的角色
            _db.Ado.ExecuteCommand($"DELETE UserRoleRelation WHERE UserId = {pid}");
            return _db.Insertable(list).ExecuteCommand() > 0;
        }
        public bool EditNickNameOrPassword(long userId, string nickName, string password)
        {
            var info = _db.Queryable<Users>().Where(p => p.Id == userId).First();
            if (info != null)
            {
                if (!string.IsNullOrEmpty(nickName))
                {
                    info.NickName = nickName;
                }
                if (!string.IsNullOrEmpty(password))
                {
                    info.Password = password;
                }
            }
            return _db.Updateable(info).ExecuteCommand() > 0;
        }

    }
}
