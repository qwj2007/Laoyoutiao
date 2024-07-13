using ConsulServerCore.ConsuleServiceDiscovery.LoadBalance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsulServerCore.ConsuleServiceDiscovery.ServiceBuilder
{
    /// <summary>
    /// 服务构建器接口
    /// </summary>
    public interface IServiceBuilder
    {
        /// <summary>
        /// 服务提供者
        /// </summary>
        IServiceProvider ServiceProvider { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        string ServiceName { get; set; }
        /// <summary>
        /// 服务地址
        /// </summary>
        string UriScheme { get; set; }
        /// <summary>
        /// 负载均衡
        /// </summary>
        ILoadBalance LoadBalancer { get; set; }
        /// <summary>
        /// 构建服务
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<Uri> BuildAsync(string path);

    }
}
