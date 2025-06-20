using ElasticAppDemo.Host.Models;

namespace ElasticAppDemo.Host.Infrastructure.Respositories
{
    public class AppLogRepository:ElasticRepositoryBase<AppLog>,IAppLogRepository
    {
        public AppLogRepository(IElasticProxy elasticProxy):base(elasticProxy) { }
        protected override string IndexName => "app-logs";
    }

    public interface IAppLogRepository:IElasticRepositoryBase<AppLog>
    {
    }
}
