using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsulServerCore.ConsulServiceRegistration
{
    /// <summary>
    /// Consul服务注册选项
    /// </summary>
    public class ConsulServiceOptions
    {

        // 服务注册地址（Consul的地址）
        public string ConsulAddress { get; set; }

        // 服务ID
        public string ServiceId { get; set; }

        // 服务名称
        public string ServiceName { get; set; }

        // 健康检查地址
        public string HealthCheck { get; set; } = "/Health";
        /// <summary>
        /// 微服务地址
        /// </summary>
        public string ServerAddress { get; set; }
    }
}
