using Laoyoutiao.Common;
using Laoyoutiao.Models.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Laoyoutiao.webapi.Filter
{
    /// <summary>
    /// 全局异常处理
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="exception"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            string url = httpContext.Request.Path.Value;
            string param = httpContext.Request.QueryString.Value;
            string method = httpContext.Request.Method;
            Log.Error(exception, "异常信息: {Message},请求方式：{method},请求路径:{url},请求参数：{param}", exception.Message,url,param,method);
        
            var problemDetails = new ProblemDetails
            {               
                Status = StatusCodes.Status500InternalServerError,
                Title = exception.Message,
                Detail=exception.StackTrace,
                Extensions=(IDictionary<string, object?>)exception.Data
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
