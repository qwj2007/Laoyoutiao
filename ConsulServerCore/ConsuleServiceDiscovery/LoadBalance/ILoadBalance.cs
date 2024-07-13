using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsulServerCore.ConsuleServiceDiscovery.LoadBalance
{
    /// <summary>
    /// 负载均衡接口
    /// </summary>
    public interface ILoadBalance
    {
        /// <summary>
        /// 负载均衡算法
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        string Resolve(List<string> services);
    }
}
