namespace ConsulServerCore.ConsuleServiceDiscovery
{
    public interface IServiceProvider
    {
        /// <summary>
        /// 查询服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        Task<List<string>> GetService(string serviceName);
    }
}
