﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace Laoyoutiao.webapi.Filter
{
    /// <summary>
    /// 全局异常处理
    /// </summary>
    public class SysExceptionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// OnException
        /// </summary>
        /// <param name="context"></param>
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
        /// <summary>
        /// OnExceptionAsync
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override System.Threading.Tasks.Task OnExceptionAsync(ExceptionContext context)
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
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}
