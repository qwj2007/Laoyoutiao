namespace Laoyoutiao.Extends
{
    public class TimeHelper
    {
        /// <summary>
        /// 将秒换成时间
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static string TimeTo(string duration) {
            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(duration));
            string str = "";
            if (ts.Hours > 0)
            {
                str = ts.Hours.ToString() + "小时 " + ts.Minutes.ToString() + "分钟 " + ts.Seconds + "秒";
            }
            if (ts.Hours == 0 && ts.Minutes > 0)
            {
                str = ts.Minutes.ToString() + "分钟 " + ts.Seconds + "秒";
            }
            if (ts.Hours == 0 && ts.Minutes == 0)
            {
                str = ts.Seconds + "秒";
            }
            return str;

        }


    }
}
