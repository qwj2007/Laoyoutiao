using AutoMapper;
using Laoyoutiao.Caches;
using Laoyoutiao.Enums;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using System.Security.AccessControl;

namespace Laoyoutiao.Service.Sys;


public class SysUserService : BaseService<SysUser>, ISysUserService
{
    private readonly IMapper _mapper;
    protected readonly CustomCache _customcache;
    private readonly ICache _cache;
    
    public SysUserService(IMapper mapper, CustomCache cache,ICache cache1) : base(mapper, cache)
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
        var user = _db.Queryable<SysUser>().Where(u => u.Account == Account && u.Password == password).First();
        //bool hascache = false;
       
        var login = _cache.GetCache<LoginUserInfo>(CacheInfo.LoginUserInfo + user.Id, out bool hascache);
        if (hascache) {
            //_cache.WriteCache(CacheInfo.LoginUserInfo + , login.loginUser, TimeSpan.FromDays(0.5));
            return _mapper.Map<SysUserRes>(login.loginUser);
        }
      
        if (user != null)
        {
            #region 获取当前登录人的数据权限信息，保存到缓存中。
            LoginUserInfo loginUser = new LoginUserInfo();

            //loginUser.deptDataIds
            loginUser.loginUser = user;
            //查找登录人员的部门信息
            loginUser.deptMents = _db.Queryable<DeptMent>().InnerJoin<UserDept>((dept, userdept) => dept.Id == userdept.DeptId)
                   .Where((dept, userdept) => dept.IsDeleted == 0 && userdept.IsDeleted == 0 && userdept.UserId == user.Id && dept.Status == 1)
                   .Select(dept => dept).ToList();

            //当前登录人所有的角色
            loginUser.roles = _db.Queryable<SysRole>().InnerJoin<SysUserRole>((role, userrole) => role.Id == userrole.RoleId)
                  .Where((role, userrole) => role.IsDeleted == 0 && userrole.IsDeleted == 0 && userrole.UserId == user.Id).Select(role => role).ToList();

            //var sysurs = _db.Queryable<SysUserRole>().Where(a => a.UserId == user.Id && a.IsDeleted == 0).ToListAsync();
            var admin = loginUser.roles.Where(a => a.Id == 1).FirstOrDefault();
            //如果是管理员
            if (admin != null)
            {
                loginUser.isPower = loginUser.isAdmin = true;
            }
            else
            {
                //如果不是管理员,从数据权限表查找数据权限
                if (loginUser.roles != null && loginUser.roles.Count > 0)
                {
                    var rolesIds = loginUser.roles.Select(a => a.Id).ToList();
                    string rids = string.Join(',', rolesIds.ToArray());
                    var dataList = _db.Queryable<DataPermission>().Where(a => rids.Contains(a.DataId.ToString())).ToList();

                    var result = dataList.Where(a => a.DataRange == (int)DataPermissionEnum.无限制).ToList();
                    if (result != null && result.Count > 0)
                    {
                        loginUser.isPower = true;
                    }
                    else
                    {
                        #region 个人
                        result = dataList.Where(a => a.DataRange == (int)DataPermissionEnum.个人).ToList();
                        if (result != null && result.Count > 0 && dataList.Count == result.Count)
                        {
                            loginUser.isOnlySelf = true;
                        }
                        #endregion

                        #region 所在公司,能查看所在公司的所有部门的数据
                        result = dataList.Where(a => a.DataRange == (int)DataPermissionEnum.所在公司).ToList();
                        if (result != null && result.Count > 0)
                        {
                            var usrDept = loginUser.deptMents;
                            foreach (var usr in usrDept)
                            {
                                var did = usr.Path.Split(",")[0];
                                var list = _db.Queryable<DeptMent>().Where(a => a.Path.Contains(did) && a.IsDeleted == 0 && a.Status == 1).ToList();
                                foreach (var item1 in list)
                                {
                                    loginUser.deptDataIds.Add(item1.Id);
                                }
                            }
                        }
                        #endregion

                        #region 直属上级部门及下属部门
                        result = dataList.Where(a => a.DataRange == (int)DataPermissionEnum.个人).ToList();
                        if (result != null)
                        {

                            foreach (var item in loginUser.deptMents)
                            {
                                loginUser.deptDataIds.Add(item.Id);
                                //直属上级部门
                                loginUser.deptDataIds.Add(item.ParentId);
                                //下属部门
                                var childDepts = _db.Queryable<DeptMent>().Where(a => a.Path.Contains(item.Path) && a.Status == 1 && a.IsDeleted == 0).ToList();
                                if (childDepts != null)
                                {
                                    foreach (var item1 in childDepts)
                                    {
                                        loginUser.deptDataIds.Add(item1.Id);
                                    }
                                }

                            }

                        }
                        #endregion

                        #region 本部门及下属部门
                        result = dataList.Where(a => a.DataRange == (int)DataPermissionEnum.本部门及下属部门).ToList();
                        if (result != null)
                        {
                            foreach (var item in loginUser.deptMents)
                            {
                                loginUser.deptDataIds.Add(item.Id);
                                var childDepts = _db.Queryable<DeptMent>().Where(a => a.Path.Contains(item.Path) && a.Status == 1 && a.IsDeleted == 0).ToList();
                                if (childDepts != null)
                                {
                                    foreach (var item1 in childDepts)
                                    {
                                        loginUser.deptDataIds.Add(item1.Id);
                                    }
                                }
                            }

                        }
                        #endregion

                        #region 本部门
                        result = dataList.Where(a => a.DataRange == (int)DataPermissionEnum.本部门).ToList();
                        if (result != null)
                        {
                            foreach (var item in loginUser.deptMents)
                            {
                                loginUser.deptDataIds.Add(item.Id);
                            }
                        }
                        #endregion

                        #region 自定义
                        result = dataList.Where(a => a.DataRange == (int)DataPermissionEnum.自定义).ToList();
                        if (result != null)
                        {
                            foreach (var item in result)
                            {
                                foreach (var item1 in item.Depts.Split(','))
                                {
                                    loginUser.deptDataIds.Add(long.Parse(item1));
                                }

                            }
                        }
                        #endregion
                    }


                }


            }
            #endregion
            _cache.WriteCache(CacheInfo.LoginUserInfo + user.Id, loginUser, TimeSpan.FromDays(0.5));
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
                Password = u.Password
            }).Distinct().ToListAsync();
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
