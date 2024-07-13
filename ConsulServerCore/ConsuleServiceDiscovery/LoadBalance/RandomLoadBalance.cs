using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsulServerCore.ConsuleServiceDiscovery.LoadBalance
{
    /// <summary>
    /// 随机负载均衡算法
    /// </summary>
    public class RandomLoadBalance : ILoadBalance
    {
        private static int seed = 0;//随机种子
        private readonly Random _random = new Random(DateTime.Now.Microsecond+seed++);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string Resolve(List<string> services)
        {
            var index = _random.Next(services.Count);
            return services[index];
        }
    }
}
