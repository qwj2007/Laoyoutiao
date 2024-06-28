//using Laoyoutiao.Models.Common;
//using Microsoft.AspNetCore.Http;
//using System.Security.Claims;

//namespace Laoyoutiao.Extends
//{
//    public static class HttpContextExt
//    {
//        /// <summary>
//        /// 获取用户信息
//        /// </summary>
//        /// <param name="httpContext"></param>
//        /// <returns></returns>
//        public static CurrentUserInfo GetUserInfo(this HttpContext httpContext)
//        {
//            var Claims = httpContext.User.Claims;
//            if (Claims.Count() == 0)
//            {
//                return null;
//            }
//            CurrentUserInfo currentUserInfo = new CurrentUserInfo()
//            {
//                //Code = Claims.FirstOrDefault(t => t.Type == "Code")?.Value,
//                //Id = long.Parse(Claims.FirstOrDefault(t => t.Type == ClaimTypes.PrimarySid)?.Value),
//                //Name = httpContext.User.Identity.Name,
//                //DeptId= long.Parse(Claims.FirstOrDefault(t => t.Type == ClaimTypes.PrimarySid)?.Value)
//                // HeadImg = Claims.FirstOrDefault(t => t.Type == "HeadImg")?.Value
//            };





//            return currentUserInfo;
//        }
//        /// <summary>
//        /// Ajax请求
//        /// </summary>
//        /// <param name="request"></param>
//        /// <returns></returns>
//        public static bool IsAjaxRequest(this HttpContext httpContext)
//        {
//            string header = httpContext.Request.Headers["X-Requested-With"];
//            var method = httpContext.Request.Headers["method"];
//            return "XMLHttpRequest".Equals(header) || method.Equals("POST");
//        }
//    }
//}
