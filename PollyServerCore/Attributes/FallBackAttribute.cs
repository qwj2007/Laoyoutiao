using Polly;


namespace PollyServerCore.Attributes
{
    /// <summary>
    /// FallBackAttribute
    /// </summary>
    public class FallBackAttribute : CustomBaseAttribute
    {
        public override Action<ISyncPolicy> DO(Action<ISyncPolicy> action)
        {  
            var fallBack = Policy.Handle<Exception>().Fallback(() =>
            {
                Console.WriteLine("这是一个服务回退策略");               
               
            });
            return new Action<ISyncPolicy>(s =>
            {
                Policy policy = null;
                if (s != null)
                {
                   // policy = Policy.Wrap(s, fallBack);
                    policy= fallBack.Wrap(s);
                }
                else
                {
                    policy = fallBack;
                }
                action.Invoke(policy);
            });
        }
    }
}
