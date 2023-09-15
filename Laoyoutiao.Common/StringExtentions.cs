using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Laoyoutiao.Common
{
    public static class StringExtentions
    {
        private static string[] strs = new string[52]
        {
            "a", "b", "c", "d", "e", "f", "g", "h", "i", "j",
            "k", "l", "m", "n", "o", "p", "q", "r", "s", "t",
            "u", "v", "w", "x", "y", "z", "A", "B", "C", "D",
            "E", "F", "G", "H", "I", "J", "K", "L", "M", "N",
            "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X",
            "Y", "Z"
        };

        private static string[] nums = new string[10] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        //
        // 摘要:
        //     驼峰命名
        //
        // 参数:
        //   text:
        public static string ToHump(this string text)
        {
            string[] array = text.Split('_');
            string text2 = string.Empty;
            for (int i = 0; i < array.Length; i++)
            {
                text2 += array[i].ToUpperHead();
            }

            return text2;
        }

        //
        // 摘要:
        //     驼峰命名（不移除 _ ）
        //
        // 参数:
        //   text:
        public static string ToOnlyHump(this string text)
        {
            string[] array = text.Split('_');
            List<string> list = new List<string>();
            for (int i = 0; i < array.Length; i++)
            {
                list.Add(array[i].ToUpperHead());
            }

            return list.Join("_");
        }

        //
        // 摘要:
        //     转拼音
        //
        // 参数:
        //   text:
        public static string ToPinYin(this string text)
        {
            return PinYin.GetPinyin(text);
        }

        //
        // 摘要:
        //     换为首字母大写的字符串
        //
        // 参数:
        //   str:
        //     源字符串
        //
        // 返回结果:
        //     首字母大写的字符串
        public static string ToUpperHead(this string str)
        {
            if (str.IsNullOrEmptyOrWhiteSpace() || (str[0] >= 'A' && str[0] <= 'Z'))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return str.ToUpper();
            }

            return $"{str[0].ToString().ToUpper()}{str.Substring(1).ToLower()}";
        }

        //
        // 摘要:
        //     删除字符串头部和尾部的回车/换行/空格
        //
        // 参数:
        //   str:
        //     字符串
        //
        // 返回结果:
        //     清除回车/换行/空格之后的字符串
        public static string TrimBlank(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                throw new NullReferenceException("字符串不可为空");
            }

            return str.TrimLeft().TrimRight();
        }

        //
        // 摘要:
        //     删除字符串尾部的回车/换行/空格
        //
        // 参数:
        //   str:
        //     字符串
        //
        // 返回结果:
        //     清除回车/换行/空格之后的字符串
        public static string TrimRight(this string str)
        {
            if (!str.IsNullOrEmpty())
            {
                int num = 0;
                while ((num = str.Length) > 0 && (str[num - 1].Equals(' ') || str[num - 1].Equals('\r') || str[num - 1].Equals('\n')))
                {
                    str = str.Substring(0, num - 1);
                }
            }

            return str;
        }

        //
        // 摘要:
        //     删除字符串头部的回车/换行/空格
        //
        // 参数:
        //   str:
        //     字符串
        //
        // 返回结果:
        //     清除回车/换行/空格之后的字符串
        public static string TrimLeft(this string str)
        {
            if (!str.IsNullOrEmpty())
            {
                while (str.Length > 0 && (str[0].Equals(' ') || str[0].Equals('\r') || str[0].Equals('\n')))
                {
                    str = str.Substring(1);
                }
            }

            return str;
        }

        //
        // 摘要:
        //     相同字符串的数量
        //
        // 参数:
        //   source:
        //     字符串
        //
        //   pattern:
        //     相比较字符串
        public static int MatchesCount(this string source, string pattern)
        {
            return (!source.IsNullOrEmpty()) ? Regex.Matches(source, pattern).Count : 0;
        }

        //
        // 摘要:
        //     获取字符串长度，按中文2位，英文1位进行计算
        //
        // 参数:
        //   source:
        //     字符串
        public static int CharCodeLength(string source)
        {
            int num = 0;
            char[] array = source.ToCharArray();
            foreach (char c in array)
            {
                num = ((c >= '\u0080') ? (num + 2) : (num + 1));
            }

            return num;
        }

        //
        // 摘要:
        //     字符串转换成bool类型
        //
        // 参数:
        //   source:
        //     字符串
        public static bool ToBoolean(this string source)
        {
            bool.TryParse(source, out var result);
            return result;
        }

        //
        // 摘要:
        //     转化为Byte型
        //
        // 返回结果:
        //     Byte
        public static byte ToByte(this string source)
        {
            byte.TryParse(source, out var result);
            return result;
        }

        //
        // 摘要:
        //     转化为Short型
        //
        // 返回结果:
        //     Short
        public static short ToShort(this string source)
        {
            short.TryParse(source, out var result);
            return result;
        }

        //
        // 摘要:
        //     转化为Short型
        //
        // 返回结果:
        //     Short
        public static short ToInt16(this string source)
        {
            short.TryParse(source, out var result);
            return result;
        }

        //
        // 摘要:
        //     转化为int32型
        //
        // 返回结果:
        //     int32
        public static int ToInt32(this string source)
        {
            int.TryParse(source, out var result);
            return result;
        }

        //
        // 摘要:
        //     转化为int64型
        //
        // 返回结果:
        //     int64
        public static long ToInt64(this string source)
        {
            long.TryParse(source, out var result);
            return result;
        }

        //
        // 摘要:
        //     转化为Double型
        //
        // 返回结果:
        //     decimal
        public static double ToDouble(this string source)
        {
            double.TryParse(source, out var result);
            return result;
        }

        //
        // 摘要:
        //     转化为decimal型
        //
        // 返回结果:
        //     decimal
        public static decimal ToDecimal(this string source)
        {
            decimal.TryParse(source, out var result);
            return result;
        }

        //
        // 摘要:
        //     转化为数字类型的日期
        //
        // 返回结果:
        //     DateTime
        public static decimal ToDateTimeDecimal(this string source)
        {
            DateTime result;
            return DateTime.TryParse(source, out result) ? result.ToString("yyyyMMddHHmmss").ToDecimal() : 0m;
        }

        //
        // 摘要:
        //     HDF 2009-3-12 将时间转换成数字
        //
        // 参数:
        //   source:
        public static decimal ToDateTimeDecimal(this DateTime source)
        {
            return source.ToString("yyyyMMddHHmmss").ToDecimal();
        }

        //
        // 摘要:
        //     转换成TextArea保存的格式；（textarea中的格式保存显示的时候会失效）
        //
        // 参数:
        //   source:
        public static string ToTextArea(this string source)
        {
            return source.IsNullOrEmpty() ? source : source.Replace("\n\r", "<br/>").Replace("\r", "<br>").Replace("\t", "\u3000\u3000");
        }

        //
        // 摘要:
        //     SubString方法扩展
        //
        // 参数:
        //   str:
        //     截取字符串
        //
        //   length:
        //     要截取的长度
        //
        // 返回结果:
        //     string
        public static string Substring(this string str, int length)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= length)
            {
                return str;
            }

            return str.Substring(0, length);
        }

        //
        // 摘要:
        //     截取字符并显示...符号
        //
        // 参数:
        //   str:
        //     截取字符串
        //
        //   length:
        //     要截取的长度
        //
        // 返回结果:
        //     string
        public static string SubstringToSx(this string str, int length)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= length)
            {
                return str;
            }

            return str.Substring(0, length) + "...";
        }

        //
        // 摘要:
        //     根据某个字符截取
        //
        // 参数:
        //   text:
        //     要截取的字符串
        //
        //   delimiter:
        //     字符
        public static string SubstringUpToFirst(this string text, char delimiter)
        {
            if (text == null)
            {
                return null;
            }

            int num = text.IndexOf(delimiter);
            if (num >= 0)
            {
                return text.Substring(0, num);
            }

            return text;
        }

        //
        // 摘要:
        //     字符串拼接成的数组转换成集合
        //
        // 参数:
        //   arrStr:
        //     要转换的字符串
        //
        //   splitchar:
        //     分离字符(默认,)
        public static List<int> ToIntList(this string arrStr, char splitchar = ',')
        {
            if (arrStr.IsNullOrEmpty())
            {
                return new List<int>();
            }

            try
            {
                return (from m in arrStr.Split(splitchar)
                        select m.ToInt32()).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //
        // 摘要:
        //     将字符串转换成int类型的数组
        //
        // 参数:
        //   arrStr:
        //     要转换的字符串
        //
        //   splitchar:
        //     分离字符(默认,)
        public static int[] ToIntArray(this string arrStr, char splitchar = ',')
        {
            if (arrStr.IsNullOrEmpty())
            {
                return new int[0];
            }

            try
            {
                return (from m in arrStr.Split(splitchar)
                        select m.ToInt32()).ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //
        // 摘要:
        //     反射获取属性值
        //
        // 参数:
        //   t:
        //     匿名对象集合
        //
        //   propertyname:
        //     属性名
        //
        // 类型参数:
        //   T:
        //     匿名对象
        public static string GetPropertyValue<T>(this T t, string propertyname)
        {
            Type typeFromHandle = typeof(T);
            PropertyInfo property = typeFromHandle.GetProperty(propertyname);
            if (property == null)
            {
                return string.Empty;
            }

            object value = property.GetValue(t, null);
            if (value == null)
            {
                return string.Empty;
            }

            return value.ToString();
        }

        //
        // 摘要:
        //     是否为空
        //
        // 参数:
        //   source:
        //     字符串
        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        //
        // 摘要:
        //     是否为空
        //
        // 参数:
        //   source:
        //     字符串
        public static bool IsNotNullOrEmpty(this string source)
        {
            return !string.IsNullOrEmpty(source);
        }

        //
        // 摘要:
        //     指示指定的字符串是 null、空还是仅由空白字符组成
        //
        // 参数:
        //   source:
        //     要测试的字符串
        //
        // 返回结果:
        //     如果 value 参数为 null 或 System.String.Empty，或者如果 value 仅由空白字符组成，则为 true。
        public static bool IsNullOrWhiteSpace(this string source)
        {
            return string.IsNullOrWhiteSpace(source);
        }

        //
        // 摘要:
        //     字符串是否为Null或Empty或WhiteSpace
        //
        // 参数:
        //   source:
        //     字符串
        //
        // 返回结果:
        //     是否为Null或Empty或WhiteSpace
        public static bool IsNullOrEmptyOrWhiteSpace(this string source)
        {
            return string.IsNullOrEmpty(source) || string.IsNullOrWhiteSpace(source);
        }

        //
        // 摘要:
        //     是否匹配相等
        //
        // 参数:
        //   source:
        //     字符串
        //
        //   pattern:
        //     相比较字符串
        public static bool IsMatch(this string source, string pattern)
        {
            return !source.IsNullOrEmpty() && Regex.IsMatch(source, pattern);
        }

        //
        // 摘要:
        //     相同的字符串
        //
        // 参数:
        //   source:
        //     字符串
        //
        //   pattern:
        //     相比较字符串
        //
        // 返回结果:
        //     相同的字符串
        public static string Match(this string source, string pattern)
        {
            return source.IsNullOrEmpty() ? "" : Regex.Match(source, pattern).Value;
        }

        //
        // 摘要:
        //     是否是url地址
        //
        // 参数:
        //   checkStr:
        //     字符串
        public static bool IsUrlAddress(this string checkStr)
        {
            return !checkStr.IsNullOrEmpty() && Regex.IsMatch(checkStr, "[a-zA-z]+://[^s]*", RegexOptions.Compiled);
        }

        //
        // 摘要:
        //     判断是否是正确的电子邮件格式
        //
        // 参数:
        //   source:
        //     字符串
        //
        // 返回结果:
        //     bool
        public static bool IsEmail(this string source)
        {
            return Regex.IsMatch(source, "^\\w+((-\\w+)|(\\.\\w+))*\\@[A-Za-z0-9]+((\\.|-)[A-Za-z0-9]+)*\\.[A-Za-z0-9]+$", RegexOptions.Compiled);
        }

        //
        // 摘要:
        //     判断是否是正确的身份证编码格式
        //
        // 参数:
        //   source:
        //     字符串
        //
        // 返回结果:
        //     bool
        public static bool IsIdCard(this string source)
        {
            return Regex.IsMatch(source, "^\\d{17}(\\d|x)$|^\\d{15}$", RegexOptions.Compiled);
        }

        //
        // 摘要:
        //     判断是否是15位身份证号
        //
        // 参数:
        //   id:
        //     身份证号
        //
        //   mesage:
        //     返回结果信息
        public static bool IsIdCard15(this string id, out string mesage)
        {
            long result = 0L;
            if (!long.TryParse(id, out result) || (double)result < Math.Pow(10.0, 14.0))
            {
                mesage = "不是有效的身份证号";
                return false;
            }

            string text = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (text.IndexOf(id.Remove(2)) == -1)
            {
                mesage = "省份不合法";
                return false;
            }

            string s = id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime result2 = default(DateTime);
            if (!DateTime.TryParse(s, out result2))
            {
                mesage = "生日不合法";
                return false;
            }

            mesage = "正确";
            return true;
        }

        //
        // 摘要:
        //     判断是否是正确的邮政编码格式
        //
        // 参数:
        //   source:
        //     字符串
        public static bool IsPostcode(this string source)
        {
            return Regex.IsMatch(source, "^[1-9]{1}(\\d){5}$", RegexOptions.Compiled);
        }

        //
        // 摘要:
        //     判断是否是正确的中国移动或联通电话
        //
        // 参数:
        //   source:
        //     字符串
        public static bool IsMobilePhone(this string source)
        {
            return Regex.IsMatch(source, "^(86)*0*13\\d{9}$", RegexOptions.Compiled);
        }

        //
        // 摘要:
        //     判断是否是正确的中国固定电话
        //
        // 参数:
        //   source:
        //     字符串
        public static bool IsTelephone(this string source)
        {
            return Regex.IsMatch(source, "^((\\d{3,4})|\\d{3,4}-|\\s)?\\d{8}$", RegexOptions.Compiled);
        }

        //
        // 摘要:
        //     包含html标签
        //
        // 参数:
        //   source:
        //     字符串
        public static bool IsHasHtml(this string source)
        {
            Regex regex = new Regex("<|>");
            return regex.IsMatch(source);
        }

        //
        // 摘要:
        //     是否匹配正则表达式，匹配返回true，否则false
        //
        // 参数:
        //   source:
        //     字符串
        //
        //   regex:
        //     正则表达式
        public static bool IsMatchRegex(this string source, string regex)
        {
            Regex regex2 = new Regex(regex);
            return regex2.IsMatch(source);
        }

        //
        // 摘要:
        //     判断字符串是否是IP，如果是返回True，不是返回False
        //
        // 参数:
        //   source:
        //     字符串
        public static bool IsIp(this string source)
        {
            Regex regex = new Regex("^(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])\\.(\\d{1,2}|1\\d\\d|2[0-4]\\d|25[0-5])$", RegexOptions.Compiled);
            return regex.Match(source).Success;
        }

        //
        // 摘要:
        //     是否包含中文或全角字符
        //
        // 参数:
        //   checkStr:
        //     字符串
        public static bool IsHasChinese(this string checkStr)
        {
            ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
            byte[] bytes = aSCIIEncoding.GetBytes(checkStr);
            for (int i = 0; i <= bytes.Length - 1; i++)
            {
                if (bytes[i] == 63)
                {
                    return true;
                }
            }

            return false;
        }

        //
        // 摘要:
        //     是否是中文
        //
        // 参数:
        //   checkStr:
        //     字符串
        public static bool IsAllChinese(this string checkStr)
        {
            checkStr = checkStr.Trim();
            if (checkStr == string.Empty)
            {
                return false;
            }

            Regex regex = new Regex("^([\\u4e00-\\u9fa5]*)$", RegexOptions.Compiled);
            return regex.IsMatch(checkStr);
        }

        //
        // 摘要:
        //     是否为正整数
        //
        // 参数:
        //   intStr:
        //     字符串
        public static bool IsInt(this string intStr)
        {
            Regex regex = new Regex("^\\d+$", RegexOptions.Compiled);
            return regex.IsMatch(intStr.Trim());
        }

        //
        // 摘要:
        //     非负整数
        //
        // 参数:
        //   intStr:
        //     字符串
        public static bool IsIntWithZero(this string intStr)
        {
            return Regex.IsMatch(intStr, "^\\\\d+$", RegexOptions.Compiled);
        }

        //
        // 摘要:
        //     是否是数字
        //
        // 参数:
        //   checkStr:
        //     字符串
        public static bool IsNumber(this string checkStr)
        {
            return Regex.IsMatch(checkStr, "^[+-]?[0123456789]*[.]?[0123456789]*$", RegexOptions.Compiled);
        }

        //
        // 摘要:
        //     是否是Decimal类型数据
        //
        // 参数:
        //   checkStr:
        //     字符串
        public static bool IsDecimal(this string checkStr)
        {
            return Regex.IsMatch(checkStr, "^[0-9]+/.?[0-9]{0,2}$", RegexOptions.Compiled);
        }

        //
        // 摘要:
        //     是否是DateTime类型数据
        //
        // 参数:
        //   checkStr:
        //     字符串
        public static bool IsDateTime(this string checkStr)
        {
            return Regex.IsMatch(checkStr, "^[ ]*[012 ]?[0123456789]?[0123456789]{2}[ ]*[-]{1}[ ]*[01]?[0123456789]{1}[ ]*[-]{1}[ ]*[0123]?[0123456789]{1}[ ]*[012]?[0123456789]{1}[ ]*[:]{1}[ ]*[012345]?[0123456789]{1}[ ]*[:]{1}[ ]*[012345]?[0123456789]{1}[ ]*$", RegexOptions.Compiled);
        }

        //
        // 摘要:
        //     判断是否是XML 1.0允许的字符
        //
        // 参数:
        //   character:
        //     字符串
        private static bool IsLegalXmlChar(this int character)
        {
            return character == 9 || character == 10 || character == 13 || (character >= 32 && character <= 55295) || (character >= 57344 && character <= 65533) || (character >= 65536 && character <= 1114111);
        }

        //
        // 摘要:
        //     判断是否是合法的 XML 1.0标准允许的字符串 true：标准 false：包含不标准的字符
        //
        // 参数:
        //   xml:
        //     xml
        public static bool IsLegalXmlChar(this string xml)
        {
            return string.IsNullOrEmpty(xml) || xml.All((char c) => IsLegalXmlChar(c));
        }

        //
        // 摘要:
        //     创建伪随机字符串
        //
        // 参数:
        //   str:
        //
        //   strleg:
        //     长度
        public static string CreateNonce(this string str, long strleg = 15L)
        {
            Random random = new Random();
            StringBuilder stringBuilder = new StringBuilder();
            int num = strs.Length;
            for (int i = 0; i < strleg; i++)
            {
                stringBuilder.Append(strs[random.Next(num - 1)]);
            }

            return stringBuilder.ToString();
        }

        //
        // 摘要:
        //     创建伪随机数字符串
        //
        // 参数:
        //   str:
        //
        //   numleg:
        public static string CreateNumberNonce(this string str, int numleg = 4)
        {
            Random random = new Random();
            StringBuilder stringBuilder = new StringBuilder();
            int num = nums.Length;
            for (int i = 0; i < numleg; i++)
            {
                stringBuilder.Append(nums[random.Next(num - 1)]);
            }

            return stringBuilder.ToString();
        }

        //
        // 摘要:
        //     移除换行
        //
        // 参数:
        //   str:
        public static string RemoveLine(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return str;
            }

            return str.Replace("\r", "").Replace("\n", "");
        }

        //
        // 摘要:
        //     url进行编码
        //
        // 参数:
        //   url:
        public static string ToUrlEncode(this string url)
        {
            if (url.IsNullOrEmpty())
            {
                return url;
            }

            return HttpUtility.UrlEncode(url);
        }

        //
        // 摘要:
        //     url进行解码
        //
        // 参数:
        //   url:
        public static string ToUrlDecode(this string url)
        {
            if (url.IsNullOrEmpty())
            {
                return url;
            }

            return HttpUtility.UrlDecode(url);
        }
    }
}
