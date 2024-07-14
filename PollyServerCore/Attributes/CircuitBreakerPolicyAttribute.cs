using Polly;

namespace PollyServerCore.Attributes
{
    /// <summary>
    /// Circuit Breaker Attribute
    /// </summary>
    public class CircuitBreakerAttribute : CustomBaseAttribute
    {

        public override Action<ISyncPolicy> DO(Action<ISyncPolicy> action)
        {
            var circuitBreakerPolicy = Policy.Handle<Exception>()
               .CircuitBreaker(
                   // 熔断前允许出现几次错误
                   1,
                   // 熔断时间
                   TimeSpan.FromSeconds(1),
                   // 熔断时触发 OPEN
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
                   }
               );
            return new Action<ISyncPolicy>(s =>
            {
                Policy policy = null;
                if (s != null)
                {
                    policy = Policy.Wrap(s, circuitBreakerPolicy);
                }
                else
                {
                    policy = circuitBreakerPolicy;
                }
                action.Invoke(policy);
            });
        }
    }
}
