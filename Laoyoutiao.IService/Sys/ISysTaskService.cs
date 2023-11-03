using Laoyoutiao.Common;
using Laoyoutiao.Models.Entitys.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.IService.Sys
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public interface ISysTaskService : IBaseService<SysTask>
    {
        ///// <summary>
        /////删除某一个任务
        ///// </summary>
        ///// <returns></returns>
        //Task<bool> RemoveJob(string jobName, string jobGroup);
        ///// <summary>
        ///// 删除某一个任务
        ///// </summary>
        ///// <param name="jobName"></param>
        ///// <returns></returns>
        //Task<bool> RemoveJob(string jobName);
        ///// <summary>
        ///// 暂停某一个任务
        ///// </summary>
        ///// <param name="jobName"></param>
        ///// <returns></returns>
        //Task<bool> StopJobAsync(string jobName);
        ///// <summary>
        ///// 暂停某一个任务
        ///// </summary>
        ///// <param name="jobName"></param>
        ///// <param name="jobGroups"></param>
        ///// <returns></returns>
        //Task<bool> StopJobAsync(string jobName, string jobGroups);

        ///// <summary>
        ///// 启动
        ///// </summary>
        ///// <param name="JobName"></param>
        ///// <param name="JobGroup"></param>
        ///// <param name="JobNamespaceAndClassName"></param>
        ///// <param name="JobAssemblyName"></param>
        ///// <param name="CronExpress"></param>
        ///// <returns></returns>
        //Task<bool> AddJobAsync(string JobName, string JobGroup, string JobNamespaceAndClassName, string JobAssemblyName, string CronExpress);

        //Task<bool> AddDataJob(string jobName, Type jobType, string cron);
    }
}
