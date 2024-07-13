using Consul;

namespace ConsulServerCore.ConsuleServiceDiscovery
{
    /// <summary>
    /// Consul服务发现
    /// </summary>
    public class ConsulServiceProvider : IServiceProvider
    {
        private ConsulClient _consulClient;
       
        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public async Task<List<string>> GetService(string serviceName)
        {
            var queryResult = await _consulClient.Health.Service(serviceName, null, true);

            var result = new List<string>();

            foreach (var serviceEntry in queryResult.Response)
            {
                result.Add(serviceEntry.Service.Address + ":" + serviceEntry.Service.Port);
            }

            return result;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="uri"></param>
        public ConsulServiceProvider(Uri uri) {
            _consulClient = new ConsulClient(configuration =>
            {
                configuration.Address = uri;
            });
        }
    }
}
