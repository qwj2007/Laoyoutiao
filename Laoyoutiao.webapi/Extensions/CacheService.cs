using Laoyoutiao.Caches;

namespace Laoyoutiao.webapi.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public class CacheService
    {
        public IServiceCollection ServiceCollection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public CacheService(IServiceCollection services)
        {
            ServiceCollection = services;
        }
        public void UseCache(ConfigurationManager Configuration)
        {
            var isOpenRedis = Configuration.GetSection("IsOpenRedis").Value;
            //判断用什么缓存
            if (!string.IsNullOrEmpty(isOpenRedis) && isOpenRedis.Trim().ToLower() == "true")
            {
                ServiceCollection.AddSingleton<ICache, RedisCache>();//Redis缓存
            }
            else
            {
                ServiceCollection.AddSingleton<ICache, MemoryCaches>();//内存缓存
            }
        }
    }
}
