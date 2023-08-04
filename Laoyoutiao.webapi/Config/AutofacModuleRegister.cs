using Autofac;
using Laoyoutiao.IService;
using Laoyoutiao.Service;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Laoyoutiao.webapi.Config
{
    /// <summary>
    /// 用autofac实现依赖注入
    /// </summary>
    public class AutofacModuleRegister : Autofac.Module
    {
        /// <summary>
        /// 用autofac实现依赖注入
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {

            //Assembly interfasceAss = Assembly.Load("demo.Interface");
            //Assembly serviceAss = Assembly.Load("demo.Service");
            //builder.RegisterAssemblyTypes(interfasceAss, serviceAss).AsImplementedInterfaces();

            //获取所有程序集合
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //批量注册所有仓储 && Service
            builder.RegisterAssemblyTypes(assemblies)//程序集内所有具象类（concrete classes）
                .Where(cc => cc.Name.EndsWith("Repository") |//筛选
                             cc.Name.EndsWith("Service"))
                .PublicOnly()//只要public访问权限的
                .Where(cc => cc.IsClass)//只要class型（主要为了排除值和interface类型）
                .AsImplementedInterfaces();//自动以其实现的所有接口类型暴露（包括IDisposable接口）

            //注册泛型仓储
            builder.RegisterGeneric(typeof(BaseService<>)).As(typeof(IBaseService<>));


            var controllerBaseType = typeof(ControllerBase);
            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
                .PropertiesAutowired();
        }
    }
}
