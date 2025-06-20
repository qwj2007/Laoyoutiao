using ElasticAppDemo.Host.Infrastructure.Respositories;

namespace ElasticAppDemo.Host.Infrastructure.Extensions
{
    public static class InfraServiceExtensions
    {
        public static void AddApplicationInfrastructure(this IServiceCollection services) {
            services.AddSingleton<IElasticProxy, ElasticProxy>();
            services.AddSingleton<IAppLogRepository, AppLogRepository>();
            services.AddSingleton<INoteRepository, NoteRepository>();
        }
    }
}
