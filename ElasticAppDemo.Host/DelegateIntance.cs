namespace ElasticAppDemo.Host
{
    /// <summary>
    /// 委托实例
    /// </summary>
    public class DelegateIntance
    {
        /// <summary>
        /// 使用Action委托处理整数列表,无返回值
        /// </summary>
        /// <param name="items"></param>
        /// <param name="action"></param>
        public static void PorcessItemss(IList<int> items,Action<int> action) {
        foreach (var item in items)
            {
                action(item);
            }
        }
        /// <summary>
        /// 使用Func委托处理整数列表,有返回值
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="itmes"></param>
        /// <param name="transformer"></param>
        /// <returns></returns>
        public static List<TOutput> TransformItems<TInput, TOutput>(IList<TInput> itmes,Func<TInput,TOutput> transformer) { 
        var result=new List<TOutput>();
            foreach (var item in itmes)
            {
                result.Add(transformer(item));
            }
            return result;
        }
        /// <summary>
        /// 使用Func委托过滤列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IList<T> FileterItems<T>(List<T> items,Func<T,bool> predicate) { 
            var result=new List<T>();
            foreach (var item in items)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }

            }
            return result;
        }

        // 异步处理示例
        public static async Task ProcessItemsAsync(List<int> items, Func<int, Task> asyncAction)
        {
            foreach (var item in items)
            {
                await asyncAction(item); // 异步执行操作
            }
        }

//        // 使用示例
//        await ProcessItemsAsync(numbers, async n =>
//{
//            await Task.Delay(100); // 模拟异步操作
//            Console.WriteLine($"处理完成: {n}");
//        });
    }
}
