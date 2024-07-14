using Polly;
using System;


namespace PollyServerCore.Attributes
{
    /// <summary>
    /// 自定义BaseAttribute
    /// </summary>
    public abstract class CustomBaseAttribute : System.Attribute
    {
        public abstract Action<ISyncPolicy> DO(Action<ISyncPolicy> action);
    }
}
