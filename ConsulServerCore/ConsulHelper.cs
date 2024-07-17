using ConsulServerCore.ConsuleServiceDiscovery.LoadBalance;
using ConsulServerCore.ConsuleServiceDiscovery;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsulServerCore
{
    public class ConsulHelper
    {
 
        #region 旧版代码
        public static string GetMicroService(string microServiceAddress, string apipath, LoadTypeBalance loadType = LoadTypeBalance.RoundRobin)
        {
            // 获取服务配置项

            var config = new ConfigurationBuilder().AddJsonFile("service.config.json").Build();
            //var serviceOptions = app.ApplicationServices.GetRequiredService<IOptions<ConsulServiceOptions>>().Value;
            // var consulAddress = serviceOptions.ConsulAddress;
            var httpClient = new HttpClient();
            var consulAddress = config["ConsulAddress"];// options.Value.ConsulAddress;
            var serviceProvider = new ConsulServiceProvider(new Uri(consulAddress));
            var microService = serviceProvider.CreateServiceBuilder(builder =>
            {

                builder.ServiceName = microServiceAddress;
                builder.UriScheme = Uri.UriSchemeHttp;
                switch (loadType)
                {
                    case LoadTypeBalance.RoundRobin:
                        builder.LoadBalancer = TypeLoadBalance.RoundRobin; break;
                    case LoadTypeBalance.Random:
                        builder.LoadBalancer = TypeLoadBalance.Random; break;
                    default:
                        throw new Exception("未找到负载类型");
                }
            });

            //apipath = "/api/Order/Getxx";
            var uri = microService.BuildAsync(apipath).Result;
            //Console.WriteLine($"{DateTime.Now} - 正在调用:{uri}");
            //var content = httpClient.GetStringAsync(uri).Result;
            //return content;
            return uri.ToString();
        }
        #endregion

        /// <summary>
        /// 异步方式
        /// </summary>
        /// <param name="microServiceAddress"></param>
        /// <param name="apipath"></param>
        /// <param name="loadType"></param>
        /// <returns></returns>

        #region 异步
        public static async Task<string> GetMicroServiceAsync(string microServiceName, string apipath, LoadTypeBalance loadType = LoadTypeBalance.RoundRobin)
        {
            // 获取服务配置项
            // 读取服务配置文件
            var config = new ConfigurationBuilder().AddJsonFile("service.config.json").Build();
            var consulAddress = config["ConsulAddress"];// options.Value.ConsulAddress;
            //var consulAddress =  options.Value.ConsulAddress;
            var httpClient = new HttpClient();
            var serviceProvider = new ConsulServiceProvider(new Uri(consulAddress));
            var microService = serviceProvider.CreateServiceBuilder(builder =>
            {

                builder.ServiceName = microServiceName;
                builder.UriScheme = Uri.UriSchemeHttp;
                switch (loadType)
                {
                    case LoadTypeBalance.RoundRobin:
                        builder.LoadBalancer = TypeLoadBalance.RoundRobin; break;
                    case LoadTypeBalance.Random:
                        builder.LoadBalancer = TypeLoadBalance.Random; break;
                    default:
                        throw new Exception("未找到负载类型");

                }
            });

            //apipath = "/api/Order/Getxx";
            var uri = microService.BuildAsync(apipath).Result;
            Console.WriteLine($"{DateTime.Now} - 正在调用:{uri}");
            return uri.ToString();
            // var content = new RequestApi().InvokeApi(uri.ToString());
            //var content = RequestApiProxy.InvokeApi(uri.ToString());
            //var content = httpClient.GetStringAsync(uri).Result;
            //return content;
        }
        #endregion

    }
}
