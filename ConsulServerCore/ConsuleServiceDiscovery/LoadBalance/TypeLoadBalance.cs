using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsulServerCore.ConsuleServiceDiscovery.LoadBalance
{
    /// <summary>
    /// 负载均衡类型
    /// </summary>
    public class TypeLoadBalance
    {
        /// <summary>
        /// 随机
        /// </summary>
        public static ILoadBalance Random = new RandomLoadBalance();
        /// <summary>
        /// 轮询
        /// </summary>
        public static ILoadBalance RoundRobin = new RoundRoinBalance();
    }
    /// <summary>
    /// 负载均衡枚举
    /// </summary>
    public enum LoadTypeBalance
    {
        Random, RoundRobin
    }
}
