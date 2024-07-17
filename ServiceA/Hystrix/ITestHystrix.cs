namespace ServiceA.Hystrix
{
    public interface ITestHystrix
    {
        Task<string> getDemo(Users users);
    }
}
