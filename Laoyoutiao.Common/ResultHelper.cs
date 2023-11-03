using Laoyoutiao.Models.Common;

namespace Laoyoutiao.Common
{
    public class ResultHelper
    {
        public static ApiResult Success(object res)
        {
            return new ApiResult() { IsSuccess = true, Result = res, Code = "0" };
        }
        public static ApiResult Error(string message="操作失败")
        {
            return new ApiResult() { IsSuccess = false, Msg = message, Code = "10000" };
        }

    }
}
