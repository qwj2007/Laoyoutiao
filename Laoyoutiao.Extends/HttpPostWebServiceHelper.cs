using System.Net;
using System.Text;
using System.Web;

namespace Laoyoutiao.Extends
{
    public class HttpPostWebServiceHelper
    {
        /// <summary>
        /// 调用webservice
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="token"></param>
        /// <param name="datatype"></param>
        /// <returns></returns>
        public static string HttpPostWebService(string url, string method, string token, string datatype)
        {
            string result = string.Empty;
            string param = string.Empty;
            byte[] bytes = null;

            Stream writer = null;
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            param = HttpUtility.UrlEncode("Token") + "=" + token + "&" + HttpUtility.UrlEncode("DataType") + "=" + HttpUtility.UrlEncode(datatype);
            bytes = Encoding.UTF8.GetBytes(param);


            request = (HttpWebRequest)WebRequest.Create(url + "/" + method);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;

            try
            {
                writer = request.GetRequestStream();        //获取用于写入请求数据的Stream对象
            }
            catch (Exception ex)
            {
                return "";
            }

            writer.Write(bytes, 0, bytes.Length);       //把参数数据写入请求数据流
            writer.Close();

            try
            {
                response = (HttpWebResponse)request.GetResponse();      //获得响应
            }
            catch (WebException ex)
            {
                return "";
            }

            #region 这种方式读取到的是一个返回的结果字符串
            //Stream stream = response.GetResponseStream();        //获取响应流
            //XmlTextReader Reader = new XmlTextReader(stream);
            //Reader.MoveToContent();
            //result = Reader.ReadInnerXml();
            #endregion

            #region 这种方式读取到的是一个Xml格式的字符串
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            result = reader.ReadToEnd();
            #endregion

            response.Dispose();
            response.Close();

            reader.Close();
            reader.Dispose();

            //Reader.Dispose();
            // Reader.Close();

            //stream.Dispose();
            //stream.Close();
            result = result.Replace("&lt;", "<").Replace("&gt;", ">").Replace("<XML>","").Replace("</XML>","");
            return result;
        }
    }
}