using Quartz;
using SqlSugar;

namespace Laoyoutiao.Jobs
{
    [DisallowConcurrentExecution] //此属性防止Quartz.NET试图同时运行相同的作业]
    public class DemoJob : IJob
    {
        //private readonly ISqlSugarClient _sqlSugarClient;

        public DemoJob()
        {
            //_sqlSugarClient = sqlSugarClient;
        }
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("程序开始执行啊........." + DateTime.Now);
            return Task.CompletedTask;
        }
    }
}