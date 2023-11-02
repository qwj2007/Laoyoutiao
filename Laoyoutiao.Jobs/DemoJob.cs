using Quartz;
using SqlSugar;

namespace Laoyoutiao.Jobs
{
    [DisallowConcurrentExecution] //此属性防止Quartz.NET试图同时运行相同的作业]
    public class DemoJob : IJob
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public DemoJob(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }
        public Task Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}