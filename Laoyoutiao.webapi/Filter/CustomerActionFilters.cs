using Laoyoutiao.Caches;
using Laoyoutiao.Models.Common;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace Laoyoutiao.webapi.Filter
{
    /// <summary>
    /// 
    /// </summary>
   // [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomerActionFilters : Attribute, IAsyncActionFilter
    {
        private readonly CurrentUserCache _customcache;
        public CustomerActionFilters(CurrentUserCache cache) {
            _customcache = cache;
        }
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
#if DEBUG
            LoginUserInfo userInfo= _customcache.GetUserInfo();
            string msg = string.Format(@"执行方法【{0}】;参数【{1}】", actionName, queryString);
            if (userInfo!=null) {
                msg = string.Format(@"当前人员【{0}】);执行方法【{1}】;参数【{2}】", _customcache.GetUserInfo().loginUser.UserName, actionName, queryString);
            }

           // Log.Information(msg);
#endif
            //在...之后
        }
    }
}
