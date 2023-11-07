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
    ///��ȡ���û���Ϣ
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
            #region ��ȡ��ǰ��¼�˵�����Ȩ����Ϣ�����浽�����С�
            LoginUserInfo loginUser = new LoginUserInfo();

            //loginUser.deptDataIds
            loginUser.loginUser = user;
            //���ҵ�¼��Ա�Ĳ�����Ϣ
            loginUser.deptMents = _db.Queryable<DeptMent>().InnerJoin<UserDept>((dept, userdept) => dept.Id == userdept.DeptId)
                   .Where((dept, userdept) => dept.IsDeleted == 0 && userdept.IsDeleted == 0 && userdept.UserId == user.Id && dept.Status == 1)
                   .Select(dept => dept).ToList();

            //��ǰ��¼�����еĽ�ɫ
            loginUser.roles = _db.Queryable<SysRole>().InnerJoin<SysUserRole>((role, userrole) => role.Id == userrole.RoleId)
                  .Where((role, userrole) => role.IsDeleted == 0 && userrole.IsDeleted == 0 && userrole.UserId == user.Id).Select(role => role).ToList();

            //var sysurs = _db.Queryable<SysUserRole>().Where(a => a.UserId == user.Id && a.IsDeleted == 0).ToListAsync();
            var admin = loginUser.roles.Where(a => a.Id == 1).FirstOrDefault();
            //����ǹ���Ա
            if (admin != null)
            {
                loginUser.isPower = loginUser.isAdmin = true;
            }
            else
            {
                //������ǹ���Ա,������Ȩ�ޱ��������Ȩ��
                if (loginUser.roles != null && loginUser.roles.Count > 0)
                {
                    var rolesIds = loginUser.roles.Select(a => a.Id).ToList();
                    string rids = string.Join(',', rolesIds.ToArray());
                    var dataList = _db.Queryable<DataPermission>().Where(a => rids.Contains(a.DataId.ToString())).ToList();

                    var result = dataList.Where(a => a.DataRange == (int)DataPermissionEnum.������).ToList();
                    if (result != null && result.Count > 0)
                    {
                        loginUser.isPower = true;
                    }
                    else
                    {
                        #region ����
                        result = dataList.Where(a => a.DataRange == (int)DataPermissionEnum.����).ToList();
                        if (result != null && result.Count > 0 && dataList.Count == result.Count)
                        {
                            loginUser.isOnlySelf = true;
                        }
                        #endregion

                        #region ���ڹ�˾,�ܲ鿴���ڹ�˾�����в��ŵ�����
                        result = dataList.Where(a => a.DataRange == (int)DataPermissionEnum.���ڹ�˾).ToList();
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

                        #region ֱ���ϼ����ż���������
                        result = dataList.Where(a => a.DataRange == (int)DataPermissionEnum.����).ToList();
                        if (result != null)
                        {

                            foreach (var item in loginUser.deptMents)
                            {
                                loginUser.deptDataIds.Add(item.Id);
                                //ֱ���ϼ�����
                                loginUser.deptDataIds.Add(item.ParentId);
                                //��������
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

                        #region �����ż���������
                        result = dataList.Where(a => a.DataRange == (int)DataPermissionEnum.�����ż���������).ToList();
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

                        #region ������
                        result = dataList.Where(a => a.DataRange == (int)DataPermissionEnum.������).ToList();
                        if (result != null)
                        {
                            foreach (var item in loginUser.deptMents)
                            {
                                loginUser.deptDataIds.Add(item.Id);
                            }
                        }
                        #endregion

                        #region �Զ���
                        result = dataList.Where(a => a.DataRange == (int)DataPermissionEnum.�Զ���).ToList();
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
            item.Status = item.Status == "0" ? "����" : "������";
        }
        pageInfo.data = _mapper.Map<List<SysUserRes>>(res);
        pageInfo.total = exp.Count();
        return pageInfo;

    }
}
