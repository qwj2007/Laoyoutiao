using Quartz;
using Quartz.Spi;

namespace Laoyoutiao.Tasks.Core
{
    /// <summary>
    /// Job 构造函数注入 重写工厂类 需要给调度器 jobfactory 赋值
    /// </summary>
    public class CustomJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CustomJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            //Job类型
            Type jobType = bundle.JobDetail.JobType;

            //返回jobType对应类型的实例
          
            return _serviceProvider.GetService(jobType) as IJob;
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }

    
    }
}
