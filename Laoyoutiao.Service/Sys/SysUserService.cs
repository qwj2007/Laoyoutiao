using AutoMapper;
using Laoyoutiao.IService;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Menu;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys;
using Laoyoutiao.Models.Entitys.Sys;
namespace Laoyoutiao.Service.Sys;


public class SysUserService : BaseService<SysUser>, ISysUserService
{
    private readonly IMapper _mapper;    
    public SysUserService(IMapper mapper) : base(mapper)
    {
        _mapper = mapper;
        
    }
    /// <summary>
    ///获取到用户信息
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public SysUserRes GetUser(string Account, string password)
    {
        var user = _db.Queryable<SysUser>().Where(u => u.Account == Account && u.Password == password).First();
        if (user != null)
        {
            return _mapper.Map<SysUserRes>(user);
        }
        return new SysUserRes();
    }
    public override async Task<PageInfo> GetPagesAsync<TReq, TRes>(TReq req)
    {
        var reqs = req as SysUserReq;
        PageInfo pageInfo = new PageInfo();
        var exp = await _db.Queryable<SysUser>()
            .WhereIF(!string.IsNullOrEmpty(reqs.UserName), u => u.UserName.Contains(reqs.UserName))
            .WhereIF(reqs.IsDeleted > -1, u => u.IsDeleted == reqs.IsDeleted)
            .OrderByDescending((u) => u.CreateDate)
            .Select((u) => new SysUserRes
            {
                Id = u.Id,
                UserName = u.UserName,
                Account = u.Account,
                Status = u.IsDeleted.ToString(),
                JobNumber = u.JobNumber,
                CreateDate = u.CreateDate,
                Password=u.Password
            }).ToListAsync();
        var res = exp
            .Skip((req.PageIndex - 1) * req.PageSize)
            .Take(req.PageSize)
            .ToList();
        foreach (var item in res)
        {
            item.Status = item.Status == "0" ? "可用" : "不可用";
        }
        pageInfo.data = _mapper.Map<List<SysUserRes>>(res);
        pageInfo.total = exp.Count();
        return pageInfo;

    }
}
