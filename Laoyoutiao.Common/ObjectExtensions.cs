using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Common
{
    public static class ObjectExtensions
    {
        //
        // 摘要:
        //     Json转对象
        //
        // 参数:
        //   json:
        //
        // 类型参数:
        //   T:
        public static T ToObject<T>(this string json) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        //
        // 摘要:
        //     对象转JSON
        //
        // 参数:
        //   obj:
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        //
        // 摘要:
        //     将对象序列化成url参数形式
        //
        // 参数:
        //   obj:
        //     对象
        public static string ToUrlParam(this object obj)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();
            StringBuilder stringBuilder = new StringBuilder();
            PropertyInfo[] array = properties;
            foreach (PropertyInfo propertyInfo in array)
            {
                string name = propertyInfo.Name;
                object value = propertyInfo.GetValue(obj, null);
                if (value != null)
                {
                    stringBuilder.AppendFormat("{0}={1}&", name, value.ToString());
                }
            }

            return stringBuilder.ToString();
        }

        //
        // 摘要:
        //     判断对象是否为空
        //
        // 参数:
        //   obj:
        //     对象
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        //
        // 摘要:
        //     判断对象不是空
        //
        // 参数:
        //   obj:
        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }

        //
        // 摘要:
        //     是否存在T集合中
        //
        // 参数:
        //   value:
        //     要判断的值
        //
        //   list:
        //     集合
        //
        // 类型参数:
        //   T:
        //     集合类型
        public static bool In<T>(this T value, IEnumerable<T> list)
        {
            return list.Contains(value);
        }

        //
        // 摘要:
        //     Returns the result of func if obj is not null.
        //     Request.Url.ReadValue(x => x.Query)
        //
        // 参数:
        //   obj:
        //     The obj.
        //
        //   func:
        //     The func.
        //
        // 类型参数:
        //   T:
        //
        //   TResult:
        //     The type of the result.
        public static TResult ReadValue<T, TResult>(this T obj, Func<T, TResult> func) where T : class
        {
            return obj.ReadValue(func, default(TResult));
        }

        //
        // 摘要:
        //     Returns the result of func if obj is not null. Otherwise, defaultValue is returned.
        //     Request.Url.ReadValue(x => x.Query, "default")
        //
        // 参数:
        //   obj:
        //
        //   func:
        //
        //   defaultValue:
        //
        // 类型参数:
        //   T:
        //
        //   TResult:
        public static TResult ReadValue<T, TResult>(this T obj, Func<T, TResult> func, TResult defaultValue) where T : class
        {
            return (obj != null) ? func(obj) : defaultValue;
        }

        //
        // 摘要:
        //     Executes an action if obj is not null, otherwise does nothing
        //
        // 参数:
        //   obj:
        //
        //   action:
        //
        // 类型参数:
        //   T:
        public static void ExecuteAction<T>(this T obj, Action<T> action) where T : class
        {
            if (obj != null)
            {
                action(obj);
            }
        }

        //
        // 参数:
        //   obj:
        //
        //   func:
        //
        // 类型参数:
        //   T:
        //
        //   TResult:
        public static TResult ReadNullableValue<T, TResult>(this T? obj, Func<T, TResult> func) where T : struct
        {
            return obj.ReadNullableValue(func, default(TResult));
        }

        //
        // 参数:
        //   obj:
        //
        //   func:
        //
        //   defaultValue:
        //
        // 类型参数:
        //   T:
        //
        //   TResult:
        public static TResult ReadNullableValue<T, TResult>(this T? obj, Func<T, TResult> func, TResult defaultValue) where T : struct
        {
            return obj.HasValue ? func(obj.Value) : defaultValue;
        }
    }
}
