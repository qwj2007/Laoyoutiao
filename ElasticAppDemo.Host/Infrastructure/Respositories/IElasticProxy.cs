using Nest;

namespace ElasticAppDemo.Host.Infrastructure.Respositories
{
    public interface IElasticProxy
    {
        IElasticClient GetClient(string indexName=null);
    }
}
