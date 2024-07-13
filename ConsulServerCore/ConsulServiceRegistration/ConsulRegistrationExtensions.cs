using Consul;
using ConsulServerCore.ConsulServiceRegistration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ConsulServiceRegistration
{
    /// <summary>
    /// Consul服务注册类的封装
    /// </summary>
    public static class ConsulRegistrationExtensions
    {
        
        public static void AddConsul(this IServiceCollection service)
        {
            // 读取服务配置文件
            var config = new ConfigurationBuilder().AddJsonFile("service.config.json").Build();

            service.Configure<ConsulServiceOptions>(config);

        }

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
        {
            // 获取主机生命周期管理接口
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            // 获取服务配置项
            var serviceOptions = app.ApplicationServices.GetRequiredService<IOptions<ConsulServiceOptions>>().Value;
            //获取微服务的地址
            //var clientOptions = app.ApplicationServices.GetRequiredService<IOptions<ClientOptions>>().Value;

            // 服务ID必须保证唯一
            serviceOptions.ServiceId = Guid.NewGuid().ToString();

            var consulClient = new ConsulClient(configuration =>
            {
                //服务注册的地址，集群中任意一个地址
                configuration.Address = new Uri(serviceOptions.ConsulAddress);
            });

            // 获取当前服务地址和端口，配置方式，也可以用自动获取
            // 192.168.0.2:7000\在某些情况获取不了\也用配置文件\命令行参数
            //Console.WriteLine("获取当前服务地址和端口-----------------------开始");
            // var features = app.Properties["server.Features"] as FeatureCollection;
            // Console.WriteLine("获取当前服务地址和端口-----------------------结束");          

            // var address = features.Get<IServerAddressesFeature>().Addresses.FirstOrDefault();

            // var uri = new Uri(address);
            var uri = new Uri(serviceOptions.ServerAddress);
            // 节点服务注册对象
            var registration = new AgentServiceRegistration()
            {
                ID = serviceOptions.ServiceId,
                Name = serviceOptions.ServiceName,// 服务名
                Address = uri.Host,
                Port = uri.Port, // 服务端口
                Check = new AgentServiceCheck
                {
                    // 超时，访问健康检测的API接口
                    Timeout = TimeSpan.FromSeconds(5),
                    // 服务停止多久后注销服务
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    // 健康检查地址
                    HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}{serviceOptions.HealthCheck}",
                    // 健康检查时间间隔
                    Interval = TimeSpan.FromSeconds(10),
                }
            };

            // 注册服务
            consulClient.Agent.ServiceRegister(registration).Wait();

            // 应用程序终止时，注销服务
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(serviceOptions.ServiceId).Wait();
            });

            return app;
        }
    }
}