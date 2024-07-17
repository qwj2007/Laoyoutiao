namespace ServiceA.Hystrix
{
    /// <summary>
    /// 自定义熔断接口
    /// </summary>
    public interface IHystrix
    {

        Task<string> Fallback();
    }
}
