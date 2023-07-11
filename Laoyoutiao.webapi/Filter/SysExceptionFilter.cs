using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Laoyoutiao.webapi.Filter
{
    /// <summary>
    /// 全局异常处理
    /// </summary>
    public class SysExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            string? name = context.ActionDescriptor.DisplayName;
            string method = context.HttpContext.Request.Method.ToLower();
            string? pathV = context.HttpContext.Request.Path.Value;
            if (context.ExceptionHandled == false)
            {
                string msg = context.Exception.Message;
                context.Result = new ContentResult
                {
                    Content = msg,
                    StatusCode = StatusCodes.Status200OK,
                    ContentType = "text/html;charset=utf-8"
                };
            }
            context.ExceptionHandled = true; //异常已处理了
        }
        public override Task OnExceptionAsync(ExceptionContext context)
        {
            //string? name = context.ActionDescriptor.DisplayName;
            //string method = context.HttpContext.Request.Method.ToLower();
            //string? pathV = context.HttpContext.Request.Path.Value;
            //if (context.ExceptionHandled == false)
            //{
            //    string msg = context.Exception.Message;
            //    context.Result = new ContentResult
            //    {
            //        Content = msg,
            //        StatusCode = StatusCodes.Status200OK,
            //        ContentType = "text/html;charset=utf-8"
            //    };
            //}
            //context.ExceptionHandled = true; //异常已处理了
            OnException(context);
            return Task.CompletedTask;
        }
    }
}
