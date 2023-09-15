using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Common
{
    public static class EnumerableExtensions
    {
        //
        // 摘要:
        //     区分去重
        //
        // 参数:
        //   source:
        //     要去重的集合
        //
        //   keySelector:
        //     去重表达
        //
        // 类型参数:
        //   TSource:
        //     实体类型
        //
        //   TKey:
        //     去重返回值类型
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> hash = new HashSet<TKey>();
            return source.Where((TSource p) => hash.Add(keySelector(p)));
        }

        //
        // 摘要:
        //     随机取IEnumerable中的一个对象
        //
        // 参数:
        //   source:
        //     IEnumerable
        //
        // 类型参数:
        //   T:
        //     对象类型
        public static T Random<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            Random random = new Random();
            if (source is ICollection)
            {
                ICollection collection = source as ICollection;
                int count = collection.Count;
                if (count == 0)
                {
                    throw new Exception("IEnumerable没有数据");
                }

                int index = random.Next(count);
                return source.ElementAt(index);
            }

            using IEnumerator<T> enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw new Exception("IEnumerable没有数据");
            }

            int num = 1;
            T current = enumerator.Current;
            while (enumerator.MoveNext())
            {
                num++;
                if (random.Next(num) == 0)
                {
                    current = enumerator.Current;
                }
            }

            return current;
        }

        //
        // 摘要:
        //     随机取IEnumerable中的一个对象
        //
        // 参数:
        //   source:
        //     IEnumerable
        //
        //   random:
        //     随机对象
        //
        // 类型参数:
        //   T:
        //     对象类型
        public static T Random<T>(this IEnumerable<T> source, Random random)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (random == null)
            {
                throw new ArgumentNullException("random");
            }

            if (source is ICollection)
            {
                ICollection collection = source as ICollection;
                int count = collection.Count;
                if (count == 0)
                {
                    throw new Exception("IEnumerable没有数据");
                }

                int index = random.Next(count);
                return source.ElementAt(index);
            }

            using IEnumerator<T> enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw new Exception("IEnumerable没有数据");
            }

            int num = 1;
            T current = enumerator.Current;
            while (enumerator.MoveNext())
            {
                num++;
                if (random.Next(num) == 0)
                {
                    current = enumerator.Current;
                }
            }

            return current;
        }

        //
        // 摘要:
        //     IEnumerable集合中加入分隔符
        //
        // 参数:
        //   values:
        //
        //   separator:
        //     分隔符默认 ,
        //
        // 类型参数:
        //   T:
        public static string Join<T>(this IEnumerable<T> values, string separator = ",")
        {
            if (values == null || !values.Any())
            {
                return string.Empty;
            }

            if (separator.IsNull())
            {
                separator = string.Empty;
            }

            return string.Join(separator, values);
        }

        //
        // 摘要:
        //     判断IEnumerable是否有元素
        //
        // 参数:
        //   values:
        //
        // 类型参数:
        //   T:
        public static bool HasItems<T>(this IEnumerable<T> values)
        {
            return values.IsNotNull() && values.Any();
        }

        //
        // 摘要:
        //     System.Collections.IEnumerable转换成JadeFramework.Core.Domain.Entities.SelectListItem集合类型
        //
        // 参数:
        //   source:
        //     数据
        //
        //   valueSelector:
        //     用于从每个元素中提取值的函数
        //
        //   textSelector:
        //     用于从每个元素中提取文本的函数
        //
        // 类型参数:
        //   TSource:
        //     数据元素类型
        //
        //   TValue:
        //     JadeFramework.Core.Domain.Entities.SelectListItem值类型
        //
        //   TText:
        //     JadeFramework.Core.Domain.Entities.SelectListItem显示文本类型
        //public static List<SelectListItem> ToSelectListItem<TSource, TValue, TText>(this IEnumerable<TSource> source, Func<TSource, TValue> valueSelector, Func<TSource, TText> textSelector)
        //{
        //    if (source == null)
        //    {
        //        throw new ArgumentException("source");
        //    }

        //    if (valueSelector == null)
        //    {
        //        throw new ArgumentException("valueSelector");
        //    }

        //    if (textSelector == null)
        //    {
        //        throw new ArgumentException("textSelector");
        //    }

        //    return source.Select((TSource s) => new SelectListItem
        //    {
        //        Value = valueSelector(s).ToString(),
        //        Text = textSelector(s).ToString()
        //    }).ToList();
        //}
    }
}
