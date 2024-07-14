using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PollyServerCore.PollyService
{
    public static class PolicyBuilder
    {
        public static ISyncPolicy CreatePolly()
        {
            // 超时1秒
            var timeoutPolicy = Policy.Timeout(1, Polly.Timeout.TimeoutStrategy.Pessimistic, (context, timespan, task) =>
            {
                Console.WriteLine("执行超时，抛出TimeoutRejectedException异常");
            });


            // 重试2次
            var retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetry(
                    2,
                    retryAttempt => TimeSpan.FromSeconds(2),
                    (exception, timespan, retryCount, context) =>
                    {
                        Console.WriteLine($"{DateTime.Now} - 重试 {retryCount} 次 - 抛出{exception.GetType()}");
                    });

            // 连续发生两次故障，就熔断3秒
            var circuitBreakerPolicy = Policy.Handle<Exception>()
                .CircuitBreaker(
                    // 熔断前允许出现几次错误
                    2,
                    // 熔断时间
                    TimeSpan.FromSeconds(3),
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

            // 回退策略，降级！
            var fallbackPolicy = Policy.Handle<Exception>()
                .Fallback(() =>
                {
                    Console.WriteLine("这是一个服务降级");
                }, exception =>
                {
                    Console.WriteLine($"Fallback异常：{exception.GetType()}");
                });

            // 策略从右到左依次进行调用
            // 首先判断调用是否超时，如果超时就会触发异常，发生超时故障，然后就触发重试策略；
            // 如果重试两次中只要成功一次，就直接返回调用结果
            // 如果重试两次都失败，第三次再次失败，就会发生故障
            // 重试之后是断路器策略，所以这个故障会被断路器接收，当断路器收到两次故障，就会触发熔断，也就是说断路器开启
            // 断路器开启的3秒内，任何故障或者操作，都会通过断路器到达回退策略，触发降级操作
            // 3秒后，断路器进入到半开启状态，操作可以正常执行
            return Policy.Wrap(fallbackPolicy, circuitBreakerPolicy, retryPolicy, timeoutPolicy);
        }
    }
}
