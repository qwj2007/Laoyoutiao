using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsulServerCore.ConsuleServiceDiscovery.LoadBalance
{
    /// <summary>
    /// 轮询负载均衡算法
    /// </summary>
    public class RoundRoinBalance : ILoadBalance
    {
        private readonly object _lock = new object();
        private int _index;
        public string Resolve(List<string> services)
        {
            lock (_lock) {
                if (_index >= services.Count)
                {
                    _index = 0;
                }
                return services[_index++];
            }            
        }
    }
}
