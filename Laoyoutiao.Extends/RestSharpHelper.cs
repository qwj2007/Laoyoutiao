using RestSharp;

namespace Laoyoutiao.Extends
{
    public class RestSharpHelper
    {
       
        public static async Task<(string, string)> PostAsync(string url, object model)
        {
            var client = new RestClient(url);
            var request = new RestRequest(new Uri(url), Method.Post);
            request.AddHeader("ContentType", "application/json");
            request.AddJsonBody(model);

            var res = await client.ExecuteAsync(request);

            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string str = res.Content;

                return (null, str);
            }
            else
            {
                return ("请求失败", null);
            }
        }
        public static async Task<Tuple<string, string>> Getsync(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(new Uri(url),Method.Get);

            var res = await client.ExecuteAsync(request);

            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string str = res.Content;

                return new Tuple<string, string>(null, str);
            }
            else
            {
                // CG.Log.Logger.Default.WriteLog(Log.LogType.Fatal, $"{res}");
                return new Tuple<string, string>("请求失败", null);
            }
        }
        public static Tuple<string, string> Get(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(new Uri(url), Method.Get);
            var res = client.Execute(request);

            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string str = res.Content;

                return new Tuple<string, string>(null, str);
            }
            else
            {
                // CG.Log.Logger.Default.WriteLog(Log.LogType.Fatal, $"{res}");
                return new Tuple<string, string>("请求失败", null);
            }
        }
    }
}
