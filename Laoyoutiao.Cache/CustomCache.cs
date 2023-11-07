using Laoyoutiao.Enums;
using Laoyoutiao.Models.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

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

        public CustomCache()
        {           
            _cache = ServiceProviderInstance.Instance.GetService<ICache>();
            _httpContext = ServiceProviderInstance.Instance.GetRequiredService<IHttpContextAccessor>();
            if (_httpContext != null && _httpContext.HttpContext != null)
            {
                Claims = _httpContext.HttpContext.User.Claims;
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
                string userId = Claims.FirstOrDefault(t => t.Type == "Id")?.Value;
                return _cache.GetCache<LoginUserInfo>(CacheInfo.LoginUserInfo+ userId, out Boolean result);
            }
            else
            {
                return null;
            }
        }

       
    }
}