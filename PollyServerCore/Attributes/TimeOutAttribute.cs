using Polly;



namespace PollyServerCore.Attributes
{
    /// <summary>
    /// 超时策略
    /// </summary>
    public class TimeOutAttribute : CustomBaseAttribute { 
        public override Action<ISyncPolicy> DO(Action<ISyncPolicy> action)
        {
            var retryTimeOut = Policy.Timeout(10, Polly.Timeout.TimeoutStrategy.Pessimistic, (context, timespan, task) => 
            {
                Console.WriteLine("执行超时，抛出TimeoutRejectedException异常");
            });
            return new Action<ISyncPolicy>(s =>
            {
                Policy policy = null;
                if (s != null)
                {
                    policy = Policy.Wrap(s, retryTimeOut);
                }
                else
                {
                    policy = retryTimeOut;
                }
                action.Invoke(policy);
            });
        }
    }
}
