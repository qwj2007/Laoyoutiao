using AutoMapper;
using Laoyoutiao.IService;
using Laoyoutiao.Models.Entitys;
using Laoyoutiao.Service;

namespace demo.Service
{
    public class RoleService : BaseService<Role>, IRoleService
    {
        private readonly IMapper _mapper;
        public RoleService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
        }
        //public bool Add(RoleAdd req, long userId)
        //{
        //    Role info = _mapper.Map<Role>(req);
        //    info.CreateUserId = userId;
        //    info.CreateDate = DateTime.Now;
        //    info.IsDeleted = 0;
        //    return _db.Insertable(info).ExecuteCommand() > 0;
        //}

        //public bool Del(long id)
        //{
        //    var info = _db.Queryable<Role>().First(p => p.Id == id);
        //    return _db.Deleteable(info).ExecuteCommand() > 0;
        //}
        //public bool BatchDel(string ids)
        //{
        //    return _db.Ado.ExecuteCommand($"DELETE Role WHERE Id IN({ids})") > 0;
        //}

        //public bool Edit(RoleEdit req, long userId)
        //{
        //    var role = _db.Queryable<Role>().First(p => p.Id == req.Id);
        //    _mapper.Map(req, role);
        //    role.ModifyUserId = userId;
        //    role.ModifyDate = DateTime.Now;
        //    return _db.Updateable(role).ExecuteCommand() > 0;
        //}

        //public RoleRes GetRoleById(long id)
        //{
        //    var info = _db.Queryable<Role>().First(p => p.Id == id);
        //    return _mapper.Map<RoleRes>(info);
        //}

        //public PageInfo GetRoles(RoleReq req)
        //{
        //    PageInfo pageInfo = new PageInfo();
        //    var exp = _db.Queryable<Role>().WhereIF(!string.IsNullOrEmpty(req.Name), p => p.Name.Contains(req.Name))
        //         .OrderBy(p => p.Order)
        //        .Skip((req.PageIndex - 1) * req.PageSize)
        //        .Take(req.PageSize)
        //        .ToList();
        //    ;
        //    //var res = exp

        //    pageInfo.data = _mapper.Map<List<RoleRes>>(exp);
        //    pageInfo.total = exp.Count();
        //    return pageInfo;
        //}

        //public override async Task<PageInfo> GetPagesAsync<RoleReq, RoleRes>(RoleReq req)
        //{
        //    var roleReq = req as Laoyoutiao.Models.Role.RoleReq;
        //    PageInfo pageInfo = new PageInfo();
        //    var exp =  await _db.Queryable<Role>().WhereIF(!string.IsNullOrEmpty(roleReq.Name), p => p.Name.Contains(roleReq.Name))
        //         .OrderBy(p => p.Order)
        //        .Skip((req.PageIndex - 1) * req.PageSize)
        //        .Take(req.PageSize)
        //        .ToListAsync();

        //    pageInfo.data = _mapper.Map<List<RoleRes>>(exp);
        //    pageInfo.total = exp.Count();
        //    return pageInfo;
        //}
    }
}
