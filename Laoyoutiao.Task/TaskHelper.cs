using System.Reflection;
using System.Text.RegularExpressions;
using Laoyoutiao.Models.Entitys.Sys;
using Quartz;
using SqlSugar;

namespace Laoyoutiao.Tasks.Core
{

    public class TaskHelper
    {
        public TaskHelper(ISchedulerFactory schedulerFactory, ISqlSugarClient sqlSugarClient)
        {
            
            _schedulerFactory = schedulerFactory;
            //IScheduler scheduler，
            _sqlSugarClient = sqlSugarClient;
        }

        //private readonly IScheduler _scheduler;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ISqlSugarClient _sqlSugarClient;

        private IScheduler _scheduler
        {
            get { return _schedulerFactory.GetScheduler().GetAwaiter().GetResult(); }
        }
        /// <summary>
        /// 校验cron 并获取下次可执行时间
        /// </summary>
        /// <param name="cron"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public  bool GetTime(string cron, out DateTime? next)
        {
            var result = true;
            next = null;
            if (string.IsNullOrWhiteSpace(cron))
            {
                return false;
            }
            var crons = cron.Split(' ');
            if (!(crons.Length == 6 || crons.Length == 7))
            {
                return false;
            }

            bool isValid = Quartz.CronExpression.IsValidExpression(cron);
            if (!isValid)
            {
                return false;
            }

            Quartz.CronExpression exp = new Quartz.CronExpression(cron);
            DateTime dd = DateTime.Now;
            DateTimeOffset ddo = DateTime.SpecifyKind(dd, DateTimeKind.Local);

            next = ((DateTimeOffset)exp.GetNextValidTimeAfter(ddo)).LocalDateTime;

            return result;
        }
        private string GetTime(double times)
        {
            if (times < 1000)
            {
                return $"{Math.Round(times, 2)}ms";
            }
            else
            {
                return $"{Math.Round(times / 1000, 2) }s";
            }
        }
        /// <summary>
        /// 更新数据状态
        /// </summary>
        /// <param name="sqlSugarClient"></param>
        /// <param name="jobid"></param>
        /// <param name="runtime"></param>
        /// <param name="NextTime"></param>
        public  async System.Threading.Tasks.Task UpdateTime(IJobExecutionContext context, ISqlSugarClient sqlSugarClient)
        {
            var jobid = context.JobDetail.Key.Name;
            //Console.WriteLine(context.JobDetail.Key.Name);
            var runtime = GetTime(context.JobRunTime.TotalMilliseconds);
            var NextTime = context.NextFireTimeUtc?.LocalDateTime;
            var RunStartTime = context.FireTimeUtc.LocalDateTime;

            Regex _regex = new Regex("[0-9]");
            if (_regex.IsMatch(jobid))
            {
                if(long.TryParse(jobid, out long joid))
                {
                    await sqlSugarClient.Updateable<SysTask>().SetColumns(t => new SysTask()
                    {
                        Id = joid,
                        RunTime = runtime,
                        NextTime = NextTime,
                        //RunStartTime = RunStartTime,

                    }).SetColumns(t => t.Counts == t.Counts + 1).Where(t => t.Id == joid).ExecuteCommandAsync();
                }
               
            }
             
        }
        /// <summary>
        /// 添加普通定时任务
        /// </summary>
        /// <param name="jobName"></param>     
        /// <param name="jobClass"></param>
        /// <param name="cron"></param>
        /// <returns></returns>
        public async Task<string> AddJob(String jobName, IJob jobClass, String cron)
        {

            try
            {

                //  var jobs=  await _scheduler.GetCurrentlyExecutingJobs();

                //if (!_scheduler.IsShutdown){

                //    await _scheduler.Start();
                //}
                // 任务名，任务组，任务执行类

                var jobDetail = JobBuilder.Create(jobClass.GetType()).WithIdentity(jobName).Build();

                ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(jobName)
                .StartNow()
                .WithCronSchedule(cron).Build();


                await _scheduler.ScheduleJob(jobDetail, trigger);

                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;

            }

        }
        /// <summary>
        /// 添加普通定时任务
        /// </summary>
        /// <param name="jobName"></param>     
        /// <param name="jobClass"></param>
        /// <param name="cron"></param>
        /// <returns></returns>
        public async Task<string> AddJob(String jobName, Type jobType, String cron)
        {

            try
            {
                var jobDetail = JobBuilder.Create(jobType)
                    .WithIdentity(jobName)
                    .UsingJobData("jobid", jobName)//添加参数
                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(jobName)
                .StartNow()
                .WithCronSchedule(cron).Build();


                await _scheduler.ScheduleJob(jobDetail, trigger);

                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;

            }

        }

        /// <summary>
        /// 数据库数据任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobType"></param>
        /// <param name="cron"></param>
        /// <returns></returns>
        public async Task<string> AddDataJob(String jobName, Type jobType, String cron)
        {

            try
            {
                var jobDetail = JobBuilder.Create(jobType)
                    .WithIdentity(jobName, "datajob")

                    .Build();

                ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(jobName)
                .StartNow()
                .WithCronSchedule(cron).Build();


                await _scheduler.ScheduleJob(jobDetail, trigger);

                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;

            }

        }
        public async Task<string> AddDataJob(long jobid, string jobname, String cron)
        {

            try
            {
                var Jobdll = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "/Laoyoutiao.Jobs.dll");
                var job = Jobdll.GetTypes()
                    .FirstOrDefault(t => t.IsClass && t.Name.Equals(jobname, StringComparison.OrdinalIgnoreCase) && t.GetInterfaces()
                       .Contains(typeof(IJob))
                       )
                    ;
                if (job != null)
                {
                    var jobDetail = JobBuilder.Create(job)
                  .WithIdentity(jobid.ToString(), "datajob")

                  .Build();

                    ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(jobid.ToString())
                    .StartNow()
                    .WithCronSchedule(cron).Build();


                    await _scheduler.ScheduleJob(jobDetail, trigger);
                }
                else
                {
                    return "未找到该任务";
                }


                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;

            }

        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobid">jobid</param>
        /// <param name="date"></param>
        /// <param name="para">需要携带的参数</param>
        /// <returns></returns>
        public async Task<string> AddDataJob<T>(long jobid, DateTime date, Dictionary<string,dynamic> para=null) where T : IJob
        {

            try
            {
                if (date < DateTime.Now.AddMinutes(1))
                {
                    return "时间间隔不能少于一分钟";
                }
                var jobDetail = JobBuilder.Create<T>()
                  .WithIdentity(jobid.ToString())
                  .Build();

                 TriggerBuilder  tbulider= TriggerBuilder.Create()
                .WithIdentity(jobid.ToString());
                if (para != null)
                {
                    foreach (var key in para.Keys)
                    {
                        tbulider = tbulider.UsingJobData("para", para[key]);
                    }
                    
                }
                ITrigger trigger = tbulider.StartNow()
                .WithCronSchedule($"{date.Second} {date.Minute} {date.Hour} {date.Day} {date.Month + 1} ? {date.Year}")
                 .Build();

                await _scheduler.ScheduleJob(jobDetail, trigger);

                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;

            }

        }
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroupName"></param>
        /// <returns></returns>
        public async Task<string> removeJob(String jobName)
        {

            try
            {
                TriggerKey triggerKey = new TriggerKey(jobName);

                await _scheduler.PauseTrigger(triggerKey);// 停止触发器 

                await _scheduler.UnscheduleJob(triggerKey);// 移除触发器 

                await _scheduler.DeleteJob(new JobKey(jobName));// 删除任务 
                return null;

            }
            catch (Exception e)
            {

                return e.Message;

            }

        }



        ///// <summary>
        ///// 启动任务
        ///// </summary>
        ///// <returns></returns>
        //public async Task<string> startJobs()
        //{
        //    try
        //    {
        //      //  Console.WriteLine(_scheduler.IsShutdown);
        //        if (_scheduler.IsShutdown)
        //        {
        //            await _scheduler.Start();
        //        }

        //        return null;
        //    }
        //    catch (Exception e)
        //    {

        //        return e.Message;

        //    }

        //}



        ///// <summary>
        ///// 关闭全部任务
        ///// </summary>
        ///// <returns></returns>
        //public async Task<string> shutdownJobs()
        //{
        //    try
        //    {

        //        if (!_scheduler.IsShutdown)
        //        {

        //            await _scheduler.Shutdown();

        //        }
        //        return null;
        //    }
        //    catch (Exception e)
        //    {
        //        return e.Message;

        //    }

        //}

        ///// <summary>
        ///// 基础任务
        ///// </summary>
        //public async Task BaseTasks()
        //{

        //    #region 自带定时任务
        //    //工作日3点清理3天前的多余上传的附件
        //     await AddJob(nameof(FuJianJob), typeof(FuJianJob), "0 0 3 ? * 1-5");
        //    //每周日0点备份数据库
        //    await AddJob(nameof(DBJob), typeof(DBJob), "0 0 0 ? * 7");
        //   // await AddJob(nameof(DBJob), typeof(DBJob), "0 43 * ? * *");
        //    // await AddJob(nameof(BaseJob),typeof( BaseJob), "0/5 * * * * ?");
        //    #endregion

        //    #region 数据库配置的定时任务
        //    var list = _sqlSugarClient.Queryable<C_Base_Tasks>().Where(t => t.IsDelete == 0).ToList();
        //    if (list.Count > 0)
        //    {
        //        var Jobdll = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "/CG.Jobs.dll");
        //        var jobs = Jobdll.GetTypes()
        //            .Where(t => t.IsClass && t.GetInterfaces()
        //               .Contains(typeof(IJob))
        //               )
        //            .ToArray();
        //        foreach (var data in list)
        //        {
        //            var job = jobs.FirstOrDefault(t => t.Name.Equals(data.TaskName, StringComparison.OrdinalIgnoreCase));
        //            if (job != null)
        //            {
        //                await AddDataJob(data.Id.ToString(), job, data.Cron);
        //            }
        //        }
        //    }

        //    #endregion


        //    //AddJob(nameof(HelloJob),  new HelloJob(), "0/10 * * * * ?").Wait();
        //    //var types = AppDomain.CurrentDomain.GetAssemblies()
        //    //   .SelectMany(a => a.GetTypes().Where(t => t.IsClass && t.GetInterfaces()
        //    //       .Contains(typeof(IJob))))
        //    //   .ToArray();

        //}
    }
}
