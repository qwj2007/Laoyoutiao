using Newtonsoft.Json;

namespace Laoyoutiao.Caches
{
    public class RedisCache : ICache
    {
        public T GetCache<T>(string cacheKey, out bool hascache)
        {
            hascache = false;
            string str = RedisHelper.redisClient.GetStringValue(cacheKey);
            if (!string.IsNullOrEmpty(str))
            {
                hascache = true;
            }
            if (hascache)
            {
                //内存缓存获取后修改值 会影响原缓存，在此序列化返回
                return JsonConvert.DeserializeObject<T>(str);
            }
            return default(T);
        }

        public void RemoveCache(string cacheKey)
        {
            RedisHelper.redisClient.DeleteStringKey(cacheKey);
        }

        public void WriteCache<T>(string cacheKey, T value, TimeSpan? timeSpan = null)
        {
            RedisHelper.redisClient.SetStringKey<T>(cacheKey, value, timeSpan);
        }
    }
}
