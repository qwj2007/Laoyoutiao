using Nest;

namespace ElasticAppDemo.Host.Infrastructure.Respositories
{
    public class ElasticProxy : IElasticProxy
    {
        private readonly string ElasticServerUrl;
        private readonly string DefaultIndexName;
        public ElasticProxy(IConfiguration configuration) {
            ElasticServerUrl = configuration["Elastic:ServerUrl"] ?? throw new ArgumentNullException();
            DefaultIndexName = configuration["Elastic:DefaultIndex"] ?? throw new ArgumentNullException();
        }
        public IElasticClient GetClient(string indexName = null)
        {
            var settings = new ConnectionSettings(new Uri(ElasticServerUrl))
      .DefaultIndex(indexName ?? DefaultIndexName);
            return new ElasticClient(settings);
        }
    }
}
