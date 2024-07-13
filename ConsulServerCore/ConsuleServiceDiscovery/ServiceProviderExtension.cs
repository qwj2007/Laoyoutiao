using ConsulServerCore.ConsuleServiceDiscovery.ServiceBuilder;


namespace ConsulServerCore.ConsuleServiceDiscovery
{
    /// <summary>
    /// 创建服务构建器扩展
    /// </summary>
    public static class ServiceProviderExtension
    {
        /// <summary>
        /// 创建服务构建器
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceBuilder CreateServiceBuilder(this IServiceProvider serviceProvider, Action<IServiceBuilder> config)
        {
            var builder = new ServiceBuilder.ServiceBuilder(serviceProvider);
            config(builder);
            return builder;
        }
    }
}
