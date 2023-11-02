using Quartz;
using SqlSugar;

namespace Laoyoutiao.Tasks.Core
{
    [DisallowConcurrentExecution] //此属性防止Quartz.NET试图同时运行相同的作业
    public class Demo : IJob
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public Demo(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }
        public System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            //处理的逻辑
            throw new NotImplementedException();
        }
    }
}