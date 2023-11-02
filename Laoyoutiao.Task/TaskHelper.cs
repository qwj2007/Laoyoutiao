using System.Reflection;
using System.Text.RegularExpressions;
using Laoyoutiao.Common;
using Laoyoutiao.Models.Entitys.Sys;
using Quartz;
using SqlSugar;
using SqlSugar.IOC;

namespace Laoyoutiao.Tasks.Core
{

    public class TaskHelper
    { //private readonly IScheduler _scheduler;
        private readonly ISchedulerFactory _schedulerFactory;
        private ISqlSugarClient _sqlSugarClient;
        public TaskHelper(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
            string configId = "0";
            string ConnectionConfigs = "ConnectionConfigs";
            List<IocConfig> connectionConfigs = AppSettings.App<IocConfig>(new string[] { ConnectionConfigs });
            var attr = typeof(SysTask).GetCustomAttribute<TenantAttribute>();
            if (attr != null)
            {
                var attrConfigId = attr.configId ?? "0";
                configId = attrConfigId.ToString();
            }
            _sqlSugarClient = DbScoped.SugarScope.GetConnection(configId ?? "0");
        }
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
        public bool GetTime(string cron, out DateTime? next)
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
                return $"{Math.Round(times / 1000, 2)}s";
            }
        }
        /// <summary>
        /// 更新数据状态
        /// </summary>
        /// <param name="sqlSugarClient"></param>
        /// <param name="jobid"></param>
        /// <param name="runtime"></param>
        /// <param name="NextTime"></param>
        public async Task UpdateTime(IJobExecutionContext context)
        {
            var jobid = context.JobDetail.Key.Name;
            //Console.WriteLine(context.JobDetail.Key.Name);
            var runtime = GetTime(context.JobRunTime.TotalMilliseconds);
            var NextTime = context.NextFireTimeUtc?.LocalDateTime;
            var RunStartTime = context.FireTimeUtc.LocalDateTime;

            Regex _regex = new Regex("[0-9]");
            if (_regex.IsMatch(jobid))
            {
                if (long.TryParse(jobid, out long joid))
                {
                    await _sqlSugarClient.Updateable<SysTask>().SetColumns(t => new SysTask()
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
        public async Task<ScheduleResult> AddJob(String jobName, IJob jobClass, String cron)
        {
            ScheduleResult result = new ScheduleResult();
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

              
            }
            catch (Exception ex)
            {
                result.ResultCode = ResultCodeAddMsgKeys.CommonExceptionCode;
                result.ResultMsg = ResultCodeAddMsgKeys.CommonExceptionMsg + "," + ex.ToString();


            }
            return result;

        }
        /// <summary>
        /// 添加普通定时任务
        /// </summary>
        /// <param name="jobName"></param>     
        /// <param name="jobClass"></param>
        /// <param name="cron"></param>
        /// <returns></returns>
        public async Task<ScheduleResult> AddJob(string jobName, Type jobType, string cron)
        {
            ScheduleResult result = new ScheduleResult();
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
            }
            catch (Exception ex)
            {
                result.ResultCode = ResultCodeAddMsgKeys.CommonExceptionCode;
                result.ResultMsg = ResultCodeAddMsgKeys.CommonExceptionMsg + "," + ex.ToString();
            }
            return result;
        }

        /// <summary>
        /// 数据库数据任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobType"></param>
        /// <param name="cron"></param>
        /// <returns></returns>
        public async Task<ScheduleResult> AddDataJob(string jobName, Type jobType, string cron)
        {
            ScheduleResult result = new ScheduleResult();
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

               
            }
            catch (Exception ex)
            {
                result.ResultCode = ResultCodeAddMsgKeys.CommonExceptionCode;
                result.ResultMsg = ResultCodeAddMsgKeys.CommonExceptionMsg + "," + ex.ToString();

            }
            return result;

        }
        public async Task<ScheduleResult> AddDataJob(long jobid, string jobname, String cron)
        {
            ScheduleResult result = new ScheduleResult();
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
            }
            catch (Exception ex)
            {
                result.ResultCode = ResultCodeAddMsgKeys.CommonExceptionCode;
                result.ResultMsg = ResultCodeAddMsgKeys.CommonExceptionMsg + "," + ex.ToString();

            }
            return result;

        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobid">jobid</param>
        /// <param name="date"></param>
        /// <param name="para">需要携带的参数</param>
        /// <returns></returns>
        public async Task<ScheduleResult> AddDataJob<T>(long jobid, DateTime date, Dictionary<string, dynamic> para = null) where T : IJob
        {
            ScheduleResult result = new ScheduleResult();
            try
            {
                if (date < DateTime.Now.AddMinutes(1))
                {
                    result.ResultCode = -10;
                    result.ResultMsg = "时间间隔不能少于一分钟";
                    return result;
                }
                var jobDetail = JobBuilder.Create<T>()
                  .WithIdentity(jobid.ToString())
                  .Build();

                TriggerBuilder tbulider = TriggerBuilder.Create()
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
               
            }
            catch (Exception ex)
            {
                result.ResultCode = ResultCodeAddMsgKeys.CommonExceptionCode;
                result.ResultMsg = ResultCodeAddMsgKeys.CommonExceptionMsg + "," + ex.ToString();
            }
            return result;

        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroupName"></param>
        /// <returns></returns>
        public async Task<ScheduleResult> RemoveJob(string jobName)
        {
            ScheduleResult result = new ScheduleResult();
            try
            {

                TriggerKey triggerKey = new TriggerKey(jobName);

                await _scheduler.PauseTrigger(triggerKey);// 停止触发器 

                await _scheduler.UnscheduleJob(triggerKey);// 移除触发器 

                await _scheduler.DeleteJob(new JobKey(jobName));// 删除任务                

            }
            catch (Exception ex)
            {
                result.ResultCode = ResultCodeAddMsgKeys.CommonExceptionCode;
                result.ResultMsg = ResultCodeAddMsgKeys.CommonExceptionMsg+","+ ex.ToString();

            }
            return result;
        }
        public async Task<ScheduleResult> RemoveJob(string jobName, string jobGroup )
        {
            ScheduleResult result = new ScheduleResult();
            try
            {

                TriggerKey triggerKey = new TriggerKey(jobName, jobGroup);

                await _scheduler.PauseTrigger(triggerKey);// 停止触发器 

                await _scheduler.UnscheduleJob(triggerKey);// 移除触发器 

                await _scheduler.DeleteJob(new JobKey(jobName,jobGroup));// 删除任务                

            }
            catch (Exception ex)
            {
                result.ResultCode = ResultCodeAddMsgKeys.CommonExceptionCode;
                result.ResultMsg = ResultCodeAddMsgKeys.CommonExceptionMsg + "," + ex.ToString();

            }
            return result;
        }

        /// <summary>
        /// 暂停指定任务计划
        /// </summary>
        /// <param name="jobName">任务名</param>
        /// <param name="jobGroup">任务分组</param>
        /// <returns></returns>
        public async Task<ScheduleResult> StopJobAsync(string jobName)
        {
            ScheduleResult result = new ScheduleResult();
            try
            {
                JobKey jobKey = new JobKey(jobName);
                if (await _scheduler.CheckExists(jobKey))
                {
                    await _scheduler.PauseJob(jobKey);
                }               

            }
            catch (Exception ex)
            {
                result.ResultCode = ResultCodeAddMsgKeys.CommonExceptionCode;
                result.ResultMsg = ResultCodeAddMsgKeys.CommonExceptionMsg + "," + ex.ToString();
            }
            return result;
        }
        public async Task<ScheduleResult> StopJobAsync(string jobName,string jobGroup)
        {
            ScheduleResult result = new ScheduleResult();
            try
            {
                JobKey jobKey = new JobKey(jobName, jobGroup);
                if (await _scheduler.CheckExists(jobKey))
                {
                    await _scheduler.PauseJob(jobKey);
                }

            }
            catch (Exception ex)
            {
                result.ResultCode = ResultCodeAddMsgKeys.CommonExceptionCode;
                result.ResultMsg = ResultCodeAddMsgKeys.CommonExceptionMsg + "," + ex.ToString();
            }
            return result;
        }

        //public async Task<ScheduleResult> DeleteJobAsync(string jobName, string jobGroup)
        //{
        //    ScheduleResult result = new ScheduleResult();
        //    try
        //    {
        //        JobKey jobKey = new JobKey(jobName, jobGroup);

        //        if (await Scheduler.CheckExists(jobKey))
        //        {
        //            //先暂停，再移除
        //            await Scheduler.PauseJob(jobKey);
        //            await Scheduler.DeleteJob(jobKey);
        //        }
        //        else
        //        {
        //            result.ResultCode = -1;
        //            result.ResultMsg = "任务不存在";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, nameof(ResumeJobAsync));
        //        result.ResultCode = -4;
        //        result.ResultMsg = ex.ToString();
        //    }
        //    return result;
        //}



        ///// <summary>
        ///// 启动任务
        ///// </summary>
        ///// <returns></returns>
        public async Task<ScheduleResult> StartJobs()
        {
            ScheduleResult result = new ScheduleResult();
            try
            {
                //  Console.WriteLine(_scheduler.IsShutdown);
                if (_scheduler.IsShutdown)
                {
                    await _scheduler.Start();
                }
            }
            catch (Exception ex)
            {

                result.ResultCode = ResultCodeAddMsgKeys.CommonExceptionCode;
                result.ResultMsg = ResultCodeAddMsgKeys.CommonExceptionMsg + "," + ex.ToString();

            }
            return result;

        }       


        ///// <summary>
        ///// 关闭全部任务
        ///// </summary>
        ///// <returns></returns>
        public async Task<ScheduleResult> ShutdownJobs()
        {
            ScheduleResult result = new ScheduleResult();
            try
            {
                if (!_scheduler.IsShutdown)
                {
                    await _scheduler.Shutdown();

                }
              
            }
            catch (Exception ex)
            {
                result.ResultCode = ResultCodeAddMsgKeys.CommonExceptionCode;
                result.ResultMsg = ResultCodeAddMsgKeys.CommonExceptionMsg + "," + ex.ToString();

            }
            return result;
        }
     
    }
}
