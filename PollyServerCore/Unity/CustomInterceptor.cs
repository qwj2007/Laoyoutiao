using Castle.Core.Interceptor;
using Polly;
using PollyServerCore.Attributes;
using System.Reflection;

namespace PollyServerCore.Unity
{
    public class CustomInterceptor : StandardInterceptor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="invocation"></param>

        protected override void PerformProceed(IInvocation invocation)
        {
            Polly.Policy policy = null;
            Action<ISyncPolicy> action = policy =>
            {
                policy.Execute(() =>
                {
                    base.PerformProceed(invocation);
                });
            };

            //if (invocation.Method.IsDefined(typeof(CustomRetryAttribute), true))
            //{
            //    CustomRetryAttribute customRetryAttribute = invocation.Method.GetCustomAttribute<CustomRetryAttribute>();

            //    action = customRetryAttribute.DO(action);
            //}

            foreach (var attribute in invocation.Method.GetCustomAttributes<CustomBaseAttribute>().Reverse())
            {
                action = attribute.DO(action);
            }
            action.Invoke(policy);

        }
    }
}
