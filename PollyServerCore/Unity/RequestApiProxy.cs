using Castle.DynamicProxy;

namespace PollyServerCore.Unity
{
    /// <summary>
    /// 代理类
    /// </summary>
    public class RequestApiProxy
    {
        public static string InvokeApi(string url, string parameter = null) {
            ProxyGenerator proxyGenerator = new ProxyGenerator();//创建一个动态代理
            CustomInterceptor customInterceptor = new CustomInterceptor();//指定切入者逻辑
            RequestApi requestApi  = proxyGenerator.CreateClassProxy<RequestApi>(customInterceptor);//类代理
            return requestApi.InvokeApi(url, parameter);
        }
    }
}
