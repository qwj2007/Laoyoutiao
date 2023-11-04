using System.Security.Cryptography;
using System.Text;

namespace Laoyoutiao.Extends
{
    public static class HashHlelper
    {
        /// <summary>
        /// 获得加盐的hash值
        /// </summary>
        /// <param name="gameid"></param>
        /// <returns></returns>
        public static string ToHash(this string para)
        {
            return GetHash_C(para);
        }
      
        /// <summary>
        /// 获取hash最就根本方法
        /// </summary>
        /// <param name="str"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string GetHash_C(string str, string salt = "CFJ")
        {
            if (salt == null)
            {
                salt = "";
            }
            str = str + salt;

            byte[] pwdAndSalt = Encoding.UTF8.GetBytes(str);
            byte[] hashBytes = new SHA256Managed().ComputeHash(pwdAndSalt);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            return sb.ToString();
        }
        


      
        ///// <summary>
        ///// 获取指定日期，在为一年中为第几周
        ///// </summary>
        ///// <param name="dt">指定时间</param>
        ///// <reutrn>返回第几周</reutrn>
        //public static int GetWeekOfYear(DateTime dt)
        //{
        //    GregorianCalendar gc = new GregorianCalendar();
        //    int weekOfYear = gc.GetWeekOfYear(dt, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        //    return weekOfYear;
        //}
    }
}
