using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.CustomAttribute
{
    /// <summary>
    ///忽略创建表
    /// </summary>
    //AttributeUsage用与指定声明的特性的使用范围  
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Class, Inherited = true)]
    public class IgnoreCreateAttribute : Attribute
    {

    }
}
