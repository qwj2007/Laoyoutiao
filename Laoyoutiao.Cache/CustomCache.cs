using Laoyoutiao.Enums;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.Models.Entitys;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using SqlSugar;
using Laoyoutiao.Common;
using System.Reflection;
using SqlSugar.IOC;

namespace Laoyoutiao.Caches
{
    /// <summary>
    /// 常用缓存
    /// </summary>
    public class CustomCache
    {

        /// <summary>
        /// 请求资源
        /// </summary>
        protected readonly IHttpContextAccessor _httpContext;
        protected readonly IEnumerable<Claim> Claims;
        protected readonly ICache _cache;
        private readonly object lockobj = new object();
        private ISqlSugarClient? _db = null;

        public CustomCache()
        {
            _cache = ServiceProviderInstance.Instance.GetService<ICache>();
            _httpContext = ServiceProviderInstance.Instance.GetRequiredService<IHttpContextAccessor>();
            if (_httpContext != null && _httpContext.HttpContext != null)
            {
                Claims = _httpContext.HttpContext.User.Claims;
            }
            string ConnectionConfigs = "ConnectionConfigs";
            List<IocConfig> connectionConfigs = AppSettings.App<IocConfig>(new string[] { ConnectionConfigs });
            var attr = typeof(SysUser).GetCustomAttribute<TenantAttribute>();
            if (attr != null)
            {
                var attrConfigId = attr.configId ?? "0";
                _db = DbScoped.SugarScope.GetConnection(attrConfigId);
            }

        }



        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns></returns>
        public LoginUserInfo GetUserInfo()
        {
            if (Claims != null)
            {
                // LoginUserInfo loginUser = new LoginUserInfo();
             
                string account = Claims.FirstOrDefault(t => t.Type == "Account")?.Value;
                var loginUser = _cache.GetCache<LoginUserInfo>(CacheInfo.LoginUserInfo + account, out Boolean hascache);

                if (!hascache)
                {
                    string userId = Claims.FirstOrDefault(t => t.Type == "Id")?.Value;
                    GetUserDataInfos(userId);
                }
                //
                return loginUser;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取用户相关信息
        /// </summary>
        /// <param name="userId"></param>
        public void GetUserDataInfos( string userId)
        {           
            #region 获取当前登录人的数据权限信息，保存到缓存中。
            LoginUserInfo loginUser = new LoginUserInfo();
            loginUser.deptDataIds = new SortedSet<long>();
            //loginUser.deptDataIds
            loginUser.loginUser = _db.Queryable<SysUser>().Where(a => a.Id == long.Parse(userId) && a.IsDeleted == 0 ).FirstAsync().GetAwaiter().GetResult();
            //查找登录人员的部门信息
            loginUser.deptMents = _db.Queryable<DeptMent>().InnerJoin<UserDept>((dept, userdept) => dept.Id == userdept.DeptId)
                   .Where((dept, userdept) => dept.IsDeleted == 0 && userdept.IsDeleted == 0 && userdept.UserId == long.Parse(userId) && dept.Status == 1)
                   .Select(dept => dept).ToList();

            //当前登录人所有的角色
            loginUser.roles = _db.Queryable<SysRole>().InnerJoin<SysUserRole>((role, userrole) => role.Id == userrole.RoleId)
                  .Where((role, userrole) => role.IsDeleted == 0 && userrole.IsDeleted == 0 && userrole.UserId == long.Parse(userId)).Select(role => role).ToList();

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
                    var dataList = _db.Queryable<DataPermission>().Where(a => rids.Contains(a.DataId.ToString())&&a.DataType=="roles").ToList();
                    var dataUserList = _db.Queryable<DataPermission>().Where(a => rids.Contains(a.DataId.ToString()) && a.DataType == "users").ToList();
                    dataList.AddRange(dataUserList);

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
                        result = dataList.Where(a => a.DataRange == (int)DataPermissionEnum.直属上级部门及下属部门).ToList();
                        if (result != null && result.Count > 0)
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
                        if (result != null && result.Count > 0)
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
                        if (result != null && result.Count > 0)
                        {
                            foreach (var item in loginUser.deptMents)
                            {
                                loginUser.deptDataIds.Add(item.Id);
                            }
                        }
                        #endregion

                        #region 自定义
                        result = dataList.Where(a => a.DataRange == (int)DataPermissionEnum.自定义).ToList();
                        if (result != null && result.Count > 0)
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
            _cache.WriteCache(CacheInfo.LoginUserInfo + loginUser.loginUser.Account, loginUser, TimeSpan.FromDays(0.5));
        }
    }
}