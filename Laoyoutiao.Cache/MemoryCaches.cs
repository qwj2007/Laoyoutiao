using Microsoft.Extensions.Caching.Memory;

using Newtonsoft.Json;

namespace Laoyoutiao.Caches
{

    /// <summary>
    /// 内存缓存
    /// </summary>
    public class MemoryCaches : ICache
    {
        private static IMemoryCache _memoryCache;
       

        public MemoryCaches()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
          
        }
        //public static MemoryCaches GetInstance()
        //{
        //    return childclass.memoryCaches;
        //}
        //private class childclass
        //{
        //    public static MemoryCaches memoryCaches = null;
        //    static childclass()
        //    {
        //        memoryCaches = new MemoryCaches();
        //    }
        //}
        public T GetCache<T>(string cacheKey, out Boolean hascache) 
        {
            hascache = _memoryCache.TryGetValue(cacheKey, out T value);
            if (hascache)
            {               
               //内存缓存获取后修改值 会影响原缓存，在此序列化返回
               return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(value));
              
            }
            return value;
        }

        public void RemoveCache(string cacheKey)
        {
            _memoryCache.Remove(cacheKey);
        }



        public void WriteCache<T>(string cacheKey, T value, TimeSpan? timeSpan = null)
        {
            if (value == null) return;
            if (timeSpan != null)
            {
                _memoryCache.Set(cacheKey, value, new MemoryCacheEntryOptions()
                  .SetAbsoluteExpiration((TimeSpan)timeSpan));
            }
            else
            {
                _memoryCache.Set(cacheKey, value);

            }
        }
    }
}
