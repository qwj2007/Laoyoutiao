
using Autofac.Extras.DynamicProxy;
using Castle.Core;
using ConsulServerCore;
using Newtonsoft.Json;
using PollyServerCore.PollyService;

namespace ServiceA.Hystrix.Impl
{
    /// <summary>
    /// 调用ServiceB的接口
    /// </summary>
    [Intercept(typeof(PollyPolicyInterceptor))]//表示要polly生效
    public class TestHystrix : ITestHystrix, IHystrix
    {
        public virtual async Task<string> Fallback()
        {
            Console.WriteLine("调用过程出现错误");
            return "调用过程出现错误。。。。";
        }


        [HystrixCommand(nameof(Fallback),
        MaxRetryTimes = 3,
        TimeOutMilliseconds = 100,           
            EnableCircuitBreaker = true,
            MillisecondsOfBreak = 100,
            CacheTTLMilliseconds = 1000)]

        public async Task<string> getDemo(Users users)
        {
           await Task.Delay(10000);
            return  await HttpMicroService.HttpPostAsync("ServiceB", "api/test/postServiceB1", JsonConvert.SerializeObject(users));
        }
    }
}
