using Quartz;
using SqlSugar;

namespace Laoyoutiao.Jobs
{
    [DisallowConcurrentExecution] //此属性防止Quartz.NET试图同时运行相同的作业]
    public class DemoJob22 : IJob
    {
        //private readonly ISqlSugarClient _sqlSugarClient;

        public DemoJob22()
        {
            //_sqlSugarClient = sqlSugarClient;
        }
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("DemoJob22DemoJob22DemoJob22程序开始执行啊........." + DateTime.Now);
            return Task.CompletedTask;
        }
    }
}