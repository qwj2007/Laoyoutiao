namespace Laoyoutiao.Models.CustomAttribute
{
    /// <summary>
    /// 源类型
    /// </summary>
    //AttributeUsage用与指定声明的特性的使用范围  
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Class, Inherited = true)]
    public class TypeMapperAttribute : Attribute
    {
        /// <summary>
        /// 源类型
        /// </summary>
        public Type? SourceType { get; set; }

    }
    /// <summary>
    /// 目标类型
    /// </summary>
    //AttributeUsage用与指定声明的特性的使用范围  
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class PropertyMapperAttribute : Attribute
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string? SourceName { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public Type? SourceDataType { get; set; }
    }
    /// <summary>
    /// DateTime映射到String
    /// </summary>
    //public class FormatBatchConvert : IValueConverter<DateTime, string>
    //{
    //    public string Convert(DateTime sourceMember, ResolutionContext context)
    //    {
    //        if (sourceMember == null)
    //            return DateTime.Now.ToString("yyyyMMddHHmmssfff");
    //        return sourceMember.ToString("yyyyMMddHHmmssfff");
    //    }
    //}
}
