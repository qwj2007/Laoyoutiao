using System.ComponentModel;
using System.Reflection;

namespace Laoyoutiao.Common
{
    public class EnumResult
    {
        public string Label { get; set; }
        public int Value { get; set; }
    }
    public static class EnumHelper
    {
        /// <summary>
        /// 枚举转Dictionary
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static IList<EnumResult> EnumToDictionary<TEnum>()
        {
            var enumType = typeof(TEnum);

            return (from object obj in Enum.GetValues(enumType)
                    select new EnumResult
                    {
                        Label = GetEnumDescription(obj),
                        Value = (int)obj
                    }).ToList();
        }

        private static string GetEnumDescription<TEnum>(this TEnum eunmObj)
        {
            //获取枚举对象的枚举类型  
            var type = eunmObj.GetType();
            //通过反射获取该枚举类型的所有属性  
            var fieldInfos = type.GetFields();

            foreach (var field in fieldInfos)
            {
                //不是参数obj,就直接跳过  
                if (field.Name != eunmObj.ToString())
                {
                    continue;
                }
                //取出参数obj的自定义属性  
                if (!field.IsDefined(typeof(DescriptionAttribute), true)) continue;
                var descriptionAttribute = field.GetCustomAttributes(typeof(DescriptionAttribute),
                    true)[0] as DescriptionAttribute;
                if (descriptionAttribute != null)
                    return descriptionAttribute.Description;
            }
            return eunmObj.ToString();
        }

        /// <summary>
        /// 根据desctiption 的值获取枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        public static T GetEnumByDescription<T>(string description) where T : Enum
        {
            System.Reflection.FieldInfo[] fields = typeof(T).GetFields();
            foreach (FieldInfo field in fields)
            {
                object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);//获取描述信息
                if (objs.Length > 0 && (objs[0] as DescriptionAttribute).Description == description)
                {
                    return (T)field.GetValue(null);
                }
            }
            return default(T);
        }

        public static string GetEnumDescription(Enum enumValue)
        {

            string value = enumValue.ToString();
            FieldInfo field = enumValue.GetType().GetField(value);
            object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (objs == null || objs.Length == 0)
            {
                return value;
            }
            var desc = (DescriptionAttribute)objs[0];
            return desc.Description;

        }




        /// <summary>
        /// 根据枚举值获取description的值
        /// 根据枚举值获取description的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myEnum"></param>
        /// <returns></returns>
        public static string EnumToDescription<T>(this T myEnum)
        {
            Type type = typeof(T);
            FieldInfo info = type.GetField(myEnum.ToString());
            var desc = info.GetCustomAttributes(typeof(DescriptionAttribute), true)[0] as DescriptionAttribute;
            if (desc != null)
            {
                return desc.Description;
            }
            else
            {
                return type.ToString();
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myEnum"></param>
        /// <returns></returns>
        public static string EnumToDescription<T>(int enumValue) where T : Enum
        {
            Type type = typeof(T);
            System.Reflection.FieldInfo[] fields = typeof(T).GetFields();
            //FieldInfo info = type.GetField(myEnum.ToString());
            foreach (FieldInfo info in fields)
            {
                string fieldName = info.Name;
                if (fieldName != "value__")
                {
                    int filedValue = (int)info.GetValue(fieldName);//查找这个数据是不是存在
                    if (enumValue == filedValue)
                    {
                        var desc = info.GetCustomAttributes(typeof(DescriptionAttribute), true)[0] as DescriptionAttribute;
                        if (desc != null)
                        {
                            return desc.Description;
                        }
                    }
                }
            }
            return "";

        }
    }
}
