namespace Laoyoutiao.Models.Common
{
    public class ApiResult
    {
        public bool IsSuccess { get; set; }
        public object? Result { get; set; }
        public string? Msg { get; set; }
        public string? Code { get; set; }
    }
}
