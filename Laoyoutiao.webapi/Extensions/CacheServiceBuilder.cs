using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Laoyoutiao.webapi.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class CacheServiceBuilder
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        public static IServiceCollection AddCache(this IServiceCollection services, Action<CacheService> configure)
        {
            var builder = new CacheService(services);
            configure(builder);
            return services;
        }
       
    }
}
