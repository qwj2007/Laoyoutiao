using AutoMapper;
using Laoyoutiao.Caches;
using Laoyoutiao.Common;
using Laoyoutiao.Enums;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;

namespace Laoyoutiao.Service.Sys;


public class SysUserService : BaseService<SysUser>, ISysUserService
{
    private readonly IMapper _mapper;
    protected readonly CustomCache _customcache;
    private readonly ICache _cache;

    public SysUserService(IMapper mapper, CustomCache cache, ICache cache1) : base(mapper, cache)
    {
        _mapper = mapper;
        _customcache = cache;
        _cache = cache1;

    }
    /// <summary>
    ///获取到用户信息
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public SysUserRes GetUser(string Account, string password)
    {
        var login = _cache.GetCache<LoginUserInfo>(CacheInfo.LoginUserInfo + Account, out bool hascache);
        if (hascache)
        {
            //_cache.WriteCache(CacheInfo.LoginUserInfo + , login.loginUser, TimeSpan.FromDays(0.5));
            return _mapper.Map<SysUserRes>(login.loginUser);
        }
        //var user = _db.Queryable<SysUser>().Where(u => u.Account == Account && u.Password == password).First();
        var user = _db.Queryable<SysUser>().Where(u => u.Account == Account&&u.IsDeleted==0).First();
        
        if (user != null&&user.Password== Encrypt.Encode(password))
        {
            _customcache.GetUserDataInfos(user.Id.ToString());       
            return _mapper.Map<SysUserRes>(user);
        }
        return new SysUserRes();
    }
    public override async Task<PageInfo> GetPagesAsync<TReq, TRes>(TReq req)
    {
        var reqs = req as SysUserReq;
        PageInfo pageInfo = new PageInfo();
        var exp = await _db.Queryable<SysUser>().WhereIF(!string.IsNullOrEmpty(reqs.UserName), u => u.UserName.Contains(reqs.UserName))
            .WhereIF(reqs.IsDeleted > -1, u => u.IsDeleted == reqs.IsDeleted)
            .OrderByDescending((u) => u.CreateDate)
            .Select((u) => new SysUser
            {
                Id = u.Id,
                UserName = u.UserName,
                Account = u.Account,
                Status = u.IsDeleted,
                JobNumber = u.JobNumber,
                CreateDate = u.CreateDate,
                Password = u.Password,
                CreateUserId = u.CreateUserId,
                DeptId = u.DeptId
            }).Distinct().ToListAsync();

        #region 加载数据权限       
        exp = base.GetCurrentUserDataRange(exp);
        #endregion

        var res = exp
            .Skip((req.PageIndex - 1) * req.PageSize)
            .Take(req.PageSize)
            .ToList();

        pageInfo.data = _mapper.Map<List<SysUserRes>>(res);
        foreach (var item in pageInfo.data as List<SysUserRes>)
        {
            item.Status = item.Status == "0" ? "可用" : "不可用";
        }
        pageInfo.total = exp.Count();
        return pageInfo;

    }


    /// <summary>
    /// 新增用户
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
   

    public override Task<bool> Add<TEdit>(TEdit input)
    {
        var entity = _mapper.Map<SysUserEdit>(input);
        entity.Password = Encrypt.Encode(entity.Password);
        return base.Add(input);
    }
}
