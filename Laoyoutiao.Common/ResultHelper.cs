using Laoyoutiao.Models.Common;

namespace Laoyoutiao.Common
{
    public class ResultHelper
    {
        public static ApiResult Success(object res)
        {
            return new ApiResult() { IsSuccess = true, Result = res, Code = "1" };
        }
        public static ApiResult Error(string message)
        {
            return new ApiResult() { IsSuccess = false, Msg = message };
        }

    }
}
