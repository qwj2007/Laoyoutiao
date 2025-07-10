using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Laoyoutiao.webapi.Util
{
    /// <summary>
    /// swagger工具类
    /// </summary>
    public class SwaggerUtil
    {
        /// <summary>
        /// 获取控制器对应的swagger分组值
        /// </summary>
        public static string GetSwaggerGroupName(Type controller)
        {
            var groupname = controller.Name.Replace("Controller", "");
            var apisetting = controller.GetCustomAttribute(typeof(ApiExplorerSettingsAttribute));
            if (apisetting != null)
            {
                groupname = ((ApiExplorerSettingsAttribute)apisetting).GroupName;
            }

            return groupname;
        }

        /// <summary>
        /// 获取所有的控制器
        /// </summary>
        public static List<Type> GetControllers()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            var controlleractionlist = asm.GetTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type))
                .OrderBy(x => x.Name).ToList();
            return controlleractionlist;
        }
    }
}
