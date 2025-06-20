using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Laoyoutiao.Caches
{
    public class RedisHelper : IDisposable
    {
        private static readonly object objlock = new object();
        private static RedisHelper _instance;
        private ConnectionMultiplexer redis { get; set; }
        public IDatabase db { get; set; }

        private RedisHelper()
        {

        }
        public bool isOpen { get; set; } = false;

        public void InitRedisConnect(IConfiguration configration)
        {
            //try
            //{
            var isOpenRedis = configration.GetSection("IsOpenRedis").Value;
            if (!string.IsNullOrEmpty(isOpenRedis) && isOpenRedis.Trim().ToLower() == "true")
            {
                var RedisConnection = configration.GetSection("RedisConnectionString").Value;
                redis = ConnectionMultiplexer.Connect(RedisConnection);
                db = redis.GetDatabase();
                isOpen = true;
            }

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    redis = null;
            //    db = null;
            //}
        }
        public static RedisHelper redisClient
        {
            get
            {
                if (_instance == null)
                {
                    lock (objlock)
                    {
                        if (_instance == null)
                        {
                            _instance = new RedisHelper();

                        }
                    }
                }
                return _instance;
            }
        }



        #region string类型操作
        /// <summary>
        /// set or update the value for string key 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetStringValue(string key, string value)
        {
            return db.StringSet(key, value);
        }
        /// <summary>
        /// 保存单个key value
        /// </summary>
        /// <param name="key">Redis Key</param>
        /// <param name="value">保存的值</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public bool SetStringKey(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            return db.StringSet(key, value, expiry);
        }
        /// <summary>
        /// 保存一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool SetStringKey<T>(string key, T obj, TimeSpan? expiry = default(TimeSpan?))
        {

            string json = JsonConvert.SerializeObject(obj);// obj.ToJson();
            return db.StringSet(key, json, expiry);
        }
        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetStringKey<T>(string key) where T : class
        {
            var result = db.StringGet(key);
            if (string.IsNullOrEmpty(result))
            {
                return null;
            }
            try
            {

                return JsonConvert.DeserializeObject<T>(result);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// get the value for string key 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetStringValue(string key)
        {
            return db.StringGet(key);
        }

        /// <summary>
        /// Delete the value for string key 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool DeleteStringKey(string key)
        {
            return db.KeyDelete(key);
        }
        #endregion

        #region 哈希类型操作
        /// <summary>
        /// set or update the HashValue for string key 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashkey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetHashValue(string key, string hashkey, string value)
        {
            return db.HashSet(key, hashkey, value);
        }
        /// <summary>
        /// set or update the HashValue for string key 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="hashkey"></param>
        /// <param name="t">defined class</param>
        /// <returns></returns>
        public bool SetHashValue<T>(String key, string hashkey, T t) where T : class
        {
            var json = JsonConvert.SerializeObject(t);
            return db.HashSet(key, hashkey, json);
        }
        /// <summary>
        /// 保存一个集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Redis Key</param>
        /// <param name="list">数据集合</param>
        /// <param name="getModelId"></param>
        public void HashSet<T>(string key, List<T> list, Func<T, string> getModelId)
        {
            List<HashEntry> listHashEntry = new List<HashEntry>();
            foreach (var item in list)
            {
                string json = JsonConvert.SerializeObject(item);
                listHashEntry.Add(new HashEntry(getModelId(item), json));
            }
            db.HashSet(key, listHashEntry.ToArray());
        }
        /// <summary>
        /// 获取hashkey所有的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> HashGetAll<T>(string key) where T : class
        {
            List<T> result = new List<T>();
            HashEntry[] arr = db.HashGetAll(key);
            foreach (var item in arr)
            {
                if (!item.Value.IsNullOrEmpty)
                {
                    T t = JsonConvert.DeserializeObject<T>(item.Value);

                    result.Add(t);

                }
            }
            return result;
            //result =JsonConvert.DeserializeJsonToList<T>(arr.ToString());                        
            //return result;
        }
        /// <summary>
        /// get the HashValue for string key  and hashkey
        /// </summary>
        /// <param name="key">Represents a key that can be stored in redis</param>
        /// <param name="hashkey"></param>
        /// <returns></returns>
        public RedisValue GetHashValue(string key, string hashkey)
        {
            //#if DEBUG
            //            return 1;
            //#endif
            RedisValue result = db.HashGet(key, hashkey);
            return result;
        }
        /// <summary>
        /// get the HashValue for string key  and hashkey
        /// </summary>
        /// <param name="key">Represents a key that can be stored in redis</param>
        /// <param name="hashkey"></param>
        /// <returns></returns>
        public T GetHashValue<T>(string key, string hashkey) where T : class
        {
            RedisValue result = db.HashGet(key, hashkey);
            if (string.IsNullOrEmpty(result))
            {
                return null;
            }

            T t = JsonConvert.DeserializeObject<T>(result);

            return t;


        }
        /// <summary>
        /// delete the HashValue for string key  and hashkey
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashkey"></param>
        /// <returns></returns>
        public bool DeleteHashValue(string key, string hashkey)
        {
            return db.HashDelete(key, hashkey);
        }

        /// <summary>
        /// 在列表头部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public long ListLeftPush(string redisKey, string redisValue)
        {
            return db.ListLeftPush(redisKey, redisValue);
        }
        public async Task<long> ListLeftPushAsync(string redisKey, string redisValue)
        {
            return await db.ListLeftPushAsync(redisKey, redisValue);
        }
        /// <summary>
        /// 在列表尾部插入值。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public long ListRightPush(string redisKey, string redisValue)
        {
            return db.ListRightPush(redisKey, redisValue);
        }
        public async Task<long> ListRightPushAsync(string redisKey, string redisValue)
        {
            return await db.ListRightPushAsync(redisKey, redisValue);
        }
        /// <summary>
        /// 在列表尾部插入数组集合。如果键不存在，先创建再插入值
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public long ListRightPush(string redisKey, IEnumerable<string> redisValue)
        {

            var redislist = new List<RedisValue>();
            foreach (var item in redisValue)
            {
                redislist.Add(item);
            }
            return db.ListRightPush(redisKey, redislist.ToArray());
        }


        /// <summary>
        /// 移除并返回存储在该键列表的第一个元素  反序列化
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public T ListLeftPop<T>(string redisKey) where T : class
        {
            return JsonConvert.DeserializeObject<T>(db.ListLeftPop(redisKey));
        }

        /// <summary>
        /// 移除并返回存储在该键列表的最后一个元素   反序列化
        /// 只能是对象集合
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public T ListRightPop<T>(string redisKey) where T : class
        {
            return JsonConvert.DeserializeObject<T>(db.ListRightPop(redisKey));
        }

        /// <summary>
        /// 移除并返回存储在该键列表的第一个元素   
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public string ListLeftPop(string redisKey)
        {
            return db.ListLeftPop(redisKey);
        }

        /// <summary>
        /// 移除并返回存储在该键列表的第一个元素   
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<string> ListLeftPopAsync(string redisKey)
        {
            return await db.ListLeftPopAsync(redisKey);
        }

        /// <summary>
        /// 移除并返回存储在该键列表的最后一个元素   
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public string ListRightPop(string redisKey)
        {
            return db.ListRightPop(redisKey);
        }

        /// <summary>
        /// 移除并返回存储在该键列表的最后一个元素   
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<string> ListRightPopAsync(string redisKey)
        {
            return await db.ListRightPopAsync(redisKey);
        }

        /// <summary>
        /// 列表长度
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public long ListLength(string redisKey)
        {
            return db.ListLength(redisKey);

        }

        /// <summary>
        /// 列表长度
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public async Task<long> ListLengthAsync(string redisKey)
        {
            return await db.ListLengthAsync(redisKey);

        }
        /// <summary>
        /// 返回在该列表上键所对应的元素
        /// </summary>
        /// <param name="redisKey"></param>
        /// <returns></returns>
        public IEnumerable<string> ListRange(string redisKey)
        {

            var result = db.ListRange(redisKey);
            return result.Select(o => o.ToString());
        }

        /// <summary>
        /// 根据索引获取指定位置数据
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public IEnumerable<string> ListRange(string redisKey, int start, int stop)
        {
            var result = db.ListRange(redisKey, start, stop);
            return result.Select(o => o.ToString());
        }

        /// <summary>
        /// 删除List中的元素 并返回删除的个数
        /// </summary>
        /// <param name="redisKey">key</param>
        /// <param name="redisValue">元素</param>
        /// <param name="type">大于零 : 从表头开始向表尾搜索，小于零 : 从表尾开始向表头搜索，等于零：移除表中所有与 VALUE 相等的值</param>
        /// <param name="db"></param>
        /// <returns></returns>
        public long ListDelRange(string redisKey, string redisValue, long type = 0)
        {
            return db.ListRemove(redisKey, redisValue, type);
        }

        /// <summary>
        /// 清空List
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="db"></param>
        public void ListClear(string redisKey)
        {
            db.ListTrim(redisKey, 1, 0);
        }
        public void Dispose()
        {
            this.Dispose();
            GC.SuppressFinalize(this);

        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        //public ISubscriber GetSubscriber()
        //{
        //    ISubscriber sub = redis.GetSubscriber();
        //    return sub;
        //    ////订阅名为 messages 的通道
        //    //sub.Subscribe(messages, (channel, message) =>
        //    //{
        //    //    //输出收到的消息
        //    //    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
        //    //});

        //    //Console.WriteLine("已订阅 messages");
        //    //Console.ReadKey();
        //}







        #endregion

        #region Redis发布订阅

        public delegate void RedisDeletegate(string str);
        public event RedisDeletegate RedisSubMessageEvent;//订阅事件

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="subChannel"></param>
        public void RedisSub(string subChannel)
        {

            redis.GetSubscriber().Subscribe(subChannel, (channel, message) =>
            {
                RedisSubMessageEvent?.Invoke(message); //触发事件

            });

        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public long RedisPub<T>(string channel, T msg)
        {

            return redis.GetSubscriber().Publish(channel, JsonConvert.SerializeObject(msg));
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="channel"></param>
        public void Unsubscribe(string channel)
        {
            redis.GetSubscriber().Unsubscribe(channel);
        }

        /// <summary>
        /// 取消全部订阅
        /// </summary>
        public void UnsubscribeAll()
        {
            redis.GetSubscriber().UnsubscribeAll();
        }

        #endregion
        /// <summary>
        /// 执行Lua脚本文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>

        public RedisResult ExecuteLuaScriptFile(string filePath, RedisKey[] keys = null, RedisValue[] values = null)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Lua 脚本文件未找到", filePath);

            string script = File.ReadAllText(filePath);
            return db.ScriptEvaluate(script, keys ?? Array.Empty<RedisKey>(), values ?? Array.Empty<RedisValue>());
        }

        #region 自定义分布式锁
        /// <summary>
        /// 获取锁
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="timeoutSeconds"></param>
        /// <returns></returns>
        public async Task<(bool Success, string LockValue)> LockAsync(string cacheKey, int timeoutSeconds = 5)
        {
            var lockKey = GetLockKey(cacheKey);
            var lockValue = Guid.NewGuid().ToString();
            var timeoutMilliseconds = timeoutSeconds * 1000;
            var expiration = TimeSpan.FromMilliseconds(timeoutMilliseconds);
            bool flag = await db.StringSetAsync(lockKey, lockValue, expiration, When.NotExists);


            return (flag, flag ? lockValue : string.Empty);
        }
        public string GetLockKey(string cacheKey)
        {
            return $"locker:{cacheKey}";
        }
        /// <summary>
        /// 删除锁
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="lockValue"></param>
        /// <returns></returns>
        public async Task<bool> UnLockAsync(string cacheKey, string lockValue)
        {            
            var lockKey = GetLockKey(cacheKey);
            var script = @"local invalue = @value
                                    local currvalue = redis.call('get',@key)
                                    if(invalue==currvalue) then redis.call('del',@key)
                                        return 1
                                    else
                                        return 0
                                    end";
            var parameters = new { key = lockKey, value = lockValue };
            var prepared = LuaScript.Prepare(script);
            var result = (int)await db.ScriptEvaluateAsync(prepared, parameters);

            return result == 1;
        }
        /// <summary>
        /// 自动续期
        /// </summary>
        /// <param name="redisDb"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="milliseconds">续期的时间</param>
        /// <returns></returns>
        public async Task Delay( string key, string value, int milliseconds)
        {
            if (!AutoDelayHandler.Instance.ContainsKey(key))
                return;

            var script = @"local val = redis.call('GET', @key)
                                    if val==@value then
                                        redis.call('PEXPIRE', @key, @milliseconds)
                                        return 1
                                    end
                                    return 0";
            object parameters = new { key, value, milliseconds };
            var prepared = LuaScript.Prepare(script);
            var result = await db.ScriptEvaluateAsync(prepared, parameters, CommandFlags.None);
            if ((int)result == 0)
            {
                AutoDelayHandler.Instance.CloseTask(key);
            }
            return;
        }
        /// <summary>
        /// 获取锁(带有自动续期功能)
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="timeoutSeconds">超时时间</param>
        /// <param name="autoDelay">是否自动续期</param>
        /// <returns></returns>
        public async Task<(bool Success, string LockValue)> LockAsync(string cacheKey, int timeoutSeconds = 5, bool autoDelay = false)
        {
            var lockKey = GetLockKey(cacheKey);
            var lockValue = Guid.NewGuid().ToString();
            var timeoutMilliseconds = timeoutSeconds * 1000;
            var expiration = TimeSpan.FromMilliseconds(timeoutMilliseconds);
            bool flag = await db.StringSetAsync(lockKey, lockValue, expiration, When.NotExists);
            if (flag && autoDelay)
            {
                //需要自动续期，创建后台任务
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                var autoDelaytask = new Task(async () =>
                {
                    while (!cancellationTokenSource.IsCancellationRequested)
                    {
                        await Task.Delay(timeoutMilliseconds / 2);
                        await Delay(lockKey, lockValue, timeoutMilliseconds);
                    }
                }, cancellationTokenSource.Token);
                var result = AutoDelayHandler.Instance.TryAdd(lockKey, autoDelaytask, cancellationTokenSource);

                if (!result)
                {
                    autoDelaytask.Dispose();
                    await UnLockAsync(cacheKey, lockValue);
                    return (false, string.Empty);
                }
            }
            return (flag, flag ? lockValue : string.Empty);
        }
        #endregion


    }
    /// <summary>
    /// 自动续期任务的处理器
    /// </summary>
    public class AutoDelayHandler
    {
        private static readonly Lazy<AutoDelayHandler> lazy = new Lazy<AutoDelayHandler>(() => new AutoDelayHandler());
        private static ConcurrentDictionary<string, (Task, CancellationTokenSource)> _tasks = new ConcurrentDictionary<string, (Task, CancellationTokenSource)>();

        public static AutoDelayHandler Instance => lazy.Value;

        /// <summary>
        /// 任务令牌添加到集合中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool TryAdd(string key, Task task, CancellationTokenSource token)
        {
            if (_tasks.TryAdd(key, (task, token)))
            {
                task.Start();

                return true;
            }
            else
            {
                return false;
            }
        }


        public void CloseTask(string key)
        {
            if (_tasks.ContainsKey(key))
            {
                if (_tasks.TryRemove(key, out (Task, CancellationTokenSource) item))
                {
                    item.Item2?.Cancel();
                    item.Item1?.Dispose();
                }
            }
        }

        public bool ContainsKey(string key)
        {
            return _tasks.ContainsKey(key);
        }
    }
}
