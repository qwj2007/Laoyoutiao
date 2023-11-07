using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Common
{
    public class ServiceProviderInstance
    {
        public static IServiceProvider Instance { get; set; }
        
        public static string wwwrootpath { get; set; }
    }
}
