using Laoyoutiao.Common;
using Laoyoutiao.Models.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Laoyoutiao.webapi.Filter
{
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
            //_logger.LogError(
            //  exception, "Exception occurred: {Message}", exception.Message);
          //var result=  ResultHelper.Error(exception.Message);
            //return new ApiResult() { IsSuccess = false, Msg = message, Code = "10000" };
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
