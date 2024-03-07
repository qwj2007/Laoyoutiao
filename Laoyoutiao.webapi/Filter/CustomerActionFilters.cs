using Microsoft.AspNetCore.Mvc.Filters;

namespace Laoyoutiao.webapi.Filter
{
    /// <summary>
    /// 
    /// </summary>
   // [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomerActionFilters : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //在方法执行之前
            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];
            var queryString = context.HttpContext.Request.QueryString.ToString();
            // _logger.LogInformation($"CustomActionFilterAttribute.OnActionExecutionAsync====controllerName:{controllerName},actionName:{actionName},queryString{queryString}");
            Console.WriteLine(@"执行方法前开始执行。。。。。。,传递的conroller是{0},执行的方法{1},传递的参数{2}",controllerName,actionName,queryString);
            await next.Invoke();
            Console.WriteLine("执行方法后开始执行。。。。。。");
            //在...之后
        }
    }
}
