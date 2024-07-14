using Polly;
using System;


namespace PollyServerCore.Attributes
{
    /// <summary>
    /// 重试策略
    /// </summary>
    public class CustomRetryAttribute : CustomBaseAttribute
    {
        public override Action<ISyncPolicy> DO(Action<ISyncPolicy> action)
        {
            var retryPolicy = Policy.Handle<Exception>().WaitAndRetry(
                       3,
                       retryAttempt => TimeSpan.FromSeconds(1),
                       (exception, timespan, retryCount, context) =>
                       {
                           Console.WriteLine($"{DateTime.Now} - 重试 {retryCount} 次 - 抛出{exception.GetType()}");
                       });
            return new Action<ISyncPolicy>(s =>
            {
                Policy policy = null;
                if (s != null)
                {
                    policy = Policy.Wrap(s, retryPolicy);
                }
                else
                {
                    policy = retryPolicy;
                }
                action.Invoke(policy);
            });
        }
    }
}
