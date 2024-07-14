using Newtonsoft.Json;
using PollyServerCore.Attributes;
namespace PollyServerCore.Unity
{
    public class RequestApi
    {
        [TimeOut]
        [CustomRetry]
        [CircuitBreaker]
        [FallBack]
        public virtual string InvokeApi(string url, string parameter = null)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Method = HttpMethod.Get;
                httpRequestMessage.RequestUri = new Uri(url);
                var result = httpClient.SendAsync(httpRequestMessage).Result;
                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(JsonConvert.SerializeObject(result.RequestMessage));
                }
                string content = result.Content.ReadAsStringAsync().Result;
                return content;
            }

        }
    }
}
