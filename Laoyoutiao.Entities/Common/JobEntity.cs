namespace Laoyoutiao.Models.Common
{
    public class JobEntity
    {
        /// <summary>
        /// 任务ID 需要唯一
        /// </summary>
        public string JobId { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public Type JobType { get; set; }

        /// <summary>
        /// 执行时间
        /// </summary>
        public string Cron { get; set; }
    }
}
