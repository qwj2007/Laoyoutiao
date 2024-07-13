using ConsulServerCore.ConsuleServiceDiscovery.LoadBalance;

namespace ConsulServerCore.ConsuleServiceDiscovery.ServiceBuilder
{
    /// <summary>
    /// 服务构建
    /// </summary>
    public class ServiceBuilder : IServiceBuilder
    {
        public IServiceProvider ServiceProvider { get; set; }
        public string ServiceName { get; set; }
        public string? UriScheme { get; set; }
        public ILoadBalance? LoadBalancer { get; set; }

        public async Task<Uri> BuildAsync(string path)
        {
            //获取服务列表
            var serviceList = await ServiceProvider.GetService(ServiceName);
            //根据服务列表，负载均衡解析服务
            var service = LoadBalancer.Resolve(serviceList);
            //构建服务地址
            var baseUri = new Uri($"{UriScheme}://{service}");
            var uri = new Uri(baseUri, path);
            return uri;

        }
        public ServiceBuilder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}
