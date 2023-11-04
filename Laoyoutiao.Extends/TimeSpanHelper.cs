namespace Laoyoutiao.Extends
{
    /// <summary>
    /// 设置timespan
    /// </summary>
    public class TimeSpanHelper
    {
        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <returns></returns>
        public static TimeSpan setTimeOut(DateTime start,DateTime end) {
            
            var sbuTime = end - start;
            return new TimeSpan(sbuTime.Days, sbuTime.Hours, sbuTime.Minutes, sbuTime.Seconds, sbuTime.Milliseconds);
        }
    }
}
