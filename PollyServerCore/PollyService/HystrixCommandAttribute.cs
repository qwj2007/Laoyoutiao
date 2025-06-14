﻿using AspectCore.DynamicProxy;
using Microsoft.Extensions.Caching.Memory;

using Polly;
using System.Collections.Concurrent;
using System.Reflection;

namespace PollyServerCore.PollyService
{    /// <summary>
     /// 对Polly的封装
     /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HystrixCommandAttribute : AbstractInterceptorAttribute
    {/// <summary>
     /// 最多重试几次，如果为0则不重试
     /// </summary>
        public int MaxRetryTimes { get; set; } = 0;

        /// <summary>
        /// 重试间隔的毫秒数
        /// </summary>
        public int RetryIntervalMilliseconds { get; set; } = 1000;

        /// <summary>
        /// 是否启用熔断
        /// </summary>
        public bool EnableCircuitBreaker { get; set; } = false;

        /// <summary>
        /// 熔断前出现允许错误几次
        /// </summary>
        public int ExceptionsAllowedBeforeBreaking { get; set; } = 1;

        /// <summary>
        /// 熔断多长时间（毫秒）
        /// </summary>
        public int MillisecondsOfBreak { get; set; } = 100;

        /// <summary>
        /// 执行超过多少毫秒则认为超时（0表示不检测超时）
        /// </summary>
        public int TimeOutMilliseconds { get; set; } = 0;

        /// <summary>
        /// 缓存多少毫秒（0表示不缓存），用“类名+方法名+所有参数ToString拼接”做缓存Key
        /// </summary>

        public int CacheTTLMilliseconds { get; set; } = 0;

        private static ConcurrentDictionary<MethodInfo, IAsyncPolicy> policies
            = new ConcurrentDictionary<MethodInfo, IAsyncPolicy>();

        private static readonly IMemoryCache memoryCache
            = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// HystrixCommandAttribute
        /// </summary>
        /// <param name="fallBackMethod">降级的方法名</param>
        public HystrixCommandAttribute(string fallBackMethod)
        {
            this.FallBackMethod = fallBackMethod;
        }

        public string FallBackMethod { get; set; }
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            //一个HystrixCommand中保持一个policy对象即可
            //其实主要是CircuitBreaker要求对于同一段代码要共享一个policy对象
            //根据反射原理，同一个方法的MethodInfo是同一个对象，但是对象上取出来的HystrixCommandAttribute
            //每次获取的都是不同的对象，因此以MethodInfo为Key保存到policies中，确保一个方法对应一个policy实例
            policies.TryGetValue(context.ServiceMethod, out IAsyncPolicy policy);
            lock (policies)//因为Invoke可能是并发调用，因此要确保policies赋值的线程安全
            {
                if (policy == null)
                {

                    policy = Policy.NoOpAsync();//创建一个空的Policy

                    //超时
                    if (TimeOutMilliseconds > 0)
                    {

                        var timeOutPolicy = Policy.TimeoutAsync(
                                TimeSpan.FromMilliseconds(TimeOutMilliseconds),
                            Polly.Timeout.TimeoutStrategy.Pessimistic,
                            async (context, timespan, task) =>
                            {
                                //Console.WriteLine("执行超时，抛出TimeoutRejectedException异常");
                                //await Task.Run(() =>
                                //{
                                Console.WriteLine($"{DateTime.Now} - 执行超时，抛出TimeoutRejectedException异常");
                                //});
                            });
                        policy = policy.WrapAsync(timeOutPolicy);
                        // policy = timeOutPolicy.WrapAsync(policy);
                        // policy = Policy.WrapAsync(policy, timeOutPolicy);

                    }
                    //重试
                    if (MaxRetryTimes > 0)
                    {
                        var retryTimePolicy = Policy.Handle<Exception>().WaitAndRetryAsync(
                                MaxRetryTimes,
                                i => TimeSpan.FromMilliseconds(RetryIntervalMilliseconds),
                            async (exception, timespan, retryCount, context) =>
                            {
                                //await Task.Run(() =>
                                //{
                                Console.WriteLine($"{DateTime.Now} - 重试 {retryCount} 次 - 抛出{exception.GetType()}");
                                // });

                            });


                        policy = policy.WrapAsync(retryTimePolicy);
                        //policy = retryTimePolicy.WrapAsync(policy);
                        //policy = Policy.WrapAsync(policy, retryTimePolicy);
                    }
                    //是否启用断路器
                    if (EnableCircuitBreaker)
                    {
                        var cicuitBreakPolicy = (Policy.Handle<Exception>().CircuitBreakerAsync(
                            ExceptionsAllowedBeforeBreaking,
                            TimeSpan.FromMilliseconds(MillisecondsOfBreak),// 熔断时触发 OPEN
                    onBreak: (ex, breakDelay) =>
                    {
                        Console.WriteLine($"{DateTime.Now} - 断路器：开启状态（熔断时触发）");
                    },
                    // 熔断恢复时触发 // CLOSE
                    onReset: () =>
                    {
                        Console.WriteLine($"{DateTime.Now} - 断路器：关闭状态（熔断恢复时触发）");
                    },
                    // 熔断时间到了之后触发，尝试放行少量（1次）的请求，
                    onHalfOpen: () =>
                    {
                        Console.WriteLine($"{DateTime.Now} - 断路器：半开启状态（熔断时间到了之后触发）");
                    }));

                        // policy = Policy.WrapAsync(policy, cicuitBreakPolicy);
                        // policy = cicuitBreakPolicy.WrapAsync(policy);


                        //policy = timeOutPolicy.WrapAsync(policy);
                        policy = policy.WrapAsync(cicuitBreakPolicy);

                    }

                    //回退

                    var policyFallBack = Policy
                   .Handle<Exception>()
                   .FallbackAsync(async (ctx, t) =>
                   {
                       AspectContext aspectContext = (AspectContext)ctx["aspectContext"];
                       //var fallBackMethod = context.ServiceMethod.DeclaringType.GetMethod(this.FallBackMethod);
                       //merge this issue: https://github.com/yangzhongke/RuPeng.HystrixCore/issues/2
                       var fallBackMethod = context.ImplementationMethod.DeclaringType.GetMethod(this.FallBackMethod);
                       Object fallBackResult = fallBackMethod.Invoke(context.Implementation, context.Parameters);
                       //不能如下这样，因为这是闭包相关，如果这样写第二次调用Invoke的时候context指向的
                       //还是第一次的对象，所以要通过Polly的上下文来传递AspectContext
                       //context.ReturnValue = fallBackResult;
                       aspectContext.ReturnValue = fallBackResult;
                   }, async (ex, t) =>
                   {
                       //Console.WriteLine(ex.Message.ToString());

                   });

                    //policy = timeOutPolicy.WrapAsync(policy);
                    policy = policyFallBack.WrapAsync(policy);
                    //放入
                    policies.TryAdd(context.ServiceMethod, policy);
                }
            }

            //把本地调用的AspectContext传递给Polly，主要给FallbackAsync中使用，避免闭包的坑
            Context pollyCtx = new Context();
            pollyCtx["aspectContext"] = context;

            //Install-Package Microsoft.Extensions.Caching.Memory
            if (CacheTTLMilliseconds > 0)
            {
                //用类名+方法名+参数的下划线连接起来作为缓存key
                string cacheKey = "HystrixMethodCacheManager_Key_" + context.ServiceMethod.DeclaringType
                                                                   + "." + context.ServiceMethod + string.Join("_", context.Parameters);
                //尝试去缓存中获取。如果找到了，则直接用缓存中的值做返回值
                if (memoryCache.TryGetValue(cacheKey, out var cacheValue))
                {
                    context.ReturnValue = cacheValue;
                }
                else
                {
                    //如果缓存中没有，则执行实际被拦截的方法
                    await policy.ExecuteAsync(ctx => next(context), pollyCtx);
                    //存入缓存中
                    using (var cacheEntry = memoryCache.CreateEntry(cacheKey))
                    {
                        cacheEntry.Value = context.ReturnValue;
                        cacheEntry.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMilliseconds(CacheTTLMilliseconds);
                    }
                }
            }
            else//如果没有启用缓存，就直接执行业务方法
            {
                await policy.ExecuteAsync(ctx => next(context), pollyCtx);

            }
        }
    }
}
