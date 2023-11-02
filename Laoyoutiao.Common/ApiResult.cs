namespace Laoyoutiao.Models.Common
{
    public class ApiResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
        public object? Result { get; set; }
        public string? Msg { get; set; }
        public string? Code { get; set; }
    }
}
