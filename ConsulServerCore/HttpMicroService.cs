using Laoyoutiao.Common;

namespace ConsulServerCore
{
    public class HttpMicroService
    {
        /// <summary>
        /// 发起POST同步请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="contentType">application/xml、application/json、application/text、application/x-www-form-urlencoded</param>
        /// <param name="headers">填充消息头</param>
        /// <returns></returns>
        public static string HttpPost(string microServiceName, string apiPath, string postData = null, string contentType = "application/json", int timeOut = 30, Dictionary<string, string> headers = null)
        {
            //根据微服务名称和API路径获取微服务地址
            string url = ConsulHelper.GetMicroService(microServiceName, apiPath);
            return HttpHelper.HttpPost(url, postData, contentType, timeOut, headers);
        }
        public static T HttpPost<T>(string microServiceName, string apiPath, string postData = null, string contentType = "application/json", int timeOut = 30, Dictionary<string, string> headers = null)
        {
            return HttpPost(microServiceName, apiPath, postData, contentType, timeOut, headers).ToEntity<T>();
        }
        public static async Task<T> HttpPostAsync<T>(string microServiceName, string apiPath, string postData = null, string contentType = "application/json", int timeOut = 30, Dictionary<string, string> headers = null)
        {
            var res = await HttpPostAsync(microServiceName, apiPath, postData, contentType, timeOut, headers);
            return res.ToEntity<T>();
        }

        /// <summary>
        /// 发起POST异步请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="contentType">application/xml、application/json、application/text、application/x-www-form-urlencoded</param>
        /// <param name="headers">填充消息头</param>
        /// <returns></returns>
        public static async Task<string> HttpPostAsync(string microServiceName, string apiPath, string postData = null,
            string contentType = "application/json",
            int timeOut = 30, Dictionary<string, string> headers = null)
        {
            //根据微服务名称和API路径获取微服务地址
            string url = ConsulHelper.GetMicroService(microServiceName, apiPath);
            return await HttpHelper.HttpPostAsync(url, postData, contentType, timeOut, headers);            
        }


        public static string HttpGet(string microServiceName, string apiPath, string contentType = "application/json", Dictionary<string, string> headers = null)
        {
            //根据微服务名称和API路径获取微服务地址
            string url = ConsulHelper.GetMicroService(microServiceName, apiPath);
            return HttpHelper.HttpGet(url, contentType, headers);            
        }

        public static async Task<string> HttpGetAsync(string microServiceName, string apiPath, string contentType = "application/json", Dictionary<string, string> headers = null)
        {
            //根据微服务名称和API路径获取微服务地址
            string url =await ConsulHelper.GetMicroServiceAsync(microServiceName, apiPath);
            return HttpHelper.HttpGet(url, contentType, headers);
           
        }

        /// <summary>
        /// 发起GET同步请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static T HttpGet<T>(string microServiceName, string apiPath, string contentType = "application/json", Dictionary<string, string> headers = null)
        {
            return HttpGet(microServiceName, apiPath, contentType, headers).ToEntity<T>();
        }

        /// <summary>
        /// 发起GET异步请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static async Task<T> HttpGetAsync<T>(string microServiceName, string apiPath, string contentType = "application/json", Dictionary<string, string> headers = null)
        {
            var res = await HttpGetAsync(microServiceName, apiPath, contentType, headers);
            return res.ToEntity<T>();
        }
    }
}
