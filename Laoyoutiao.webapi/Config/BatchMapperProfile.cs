using AutoMapper;
using AutoMapper.Configuration;
using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Dto;
using System.Reflection;

namespace Laoyoutiao.webapi.Config
{
    /// <summary>
    /// 批量映射
    /// </summary>
    public class BatchMapperProfile : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public BatchMapperProfile()
        {
            InitMapper();
        }
        /// <summary>
        /// 动态创建automapper
        /// </summary>
        public void InitMapper()
        {
            //获取所有需要依据特性进行映射的DTO类
            var typeList = Assembly.GetAssembly(typeof(BaseDto)).GetTypes().Where(t => t.GetCustomAttributes(typeof(TypeMapperAttribute)).Any()).ToList();
            typeList.ForEach(type =>
            {
                //获取类指定的特性
                if (type.GetCustomAttributes(typeof(TypeMapperAttribute)).FirstOrDefault() is not TypeMapperAttribute attribute || attribute.SourceType == null)
                    return;
                //类映射
                var mapper = CreateMap(attribute.SourceType, type).ReverseMap();

                //处理类中映射规则不同的属性
                var propertyAttributes = type.GetProperties().Where(p => p.GetCustomAttributes(typeof(PropertyMapperAttribute)).Any()).ToList();
                propertyAttributes.ForEach(property =>
                {
                    //获取属性指定特性
                    var propertyAttribute = (PropertyMapperAttribute)property.GetCustomAttributes(typeof(PropertyMapperAttribute)).FirstOrDefault();
                    if (propertyAttribute == null)
                        return;
                    if (!string.IsNullOrEmpty(propertyAttribute.SourceName))
                    {
                        //属性名称自定义映射
                        mapper.ForMember(property.Name, src => src.MapFrom(propertyAttribute.SourceName)).ReverseMap();
                        //mapper.ForMember(property.Name, src => src.MapFrom(propertyAttribute.SourceName));
                    }
                    if (propertyAttribute.SourceDataType != null && propertyAttribute.SourceDataType == typeof(DateTime))
                    {

                        //DateTime数据类型 映射 自定义字符串格式
                        // 处理类型转换（如 DateTime 转字符串）                       
                        mapper.ForMember(property.Name, opt => opt.ConvertUsing( new CustomTypeConverter(propertyAttribute.SourceDataType, property.PropertyType)
                    ));
                    }
                    
                    
                });

            });

        }
    }
}

/// <summary>
/// 自定义类型转换器
/// </summary>

public class CustomTypeConverter : IValueConverter<object, object>
{
    private readonly Type _sourceType;
    private readonly Type _destinationType;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="sourceType"></param>
    /// <param name="destinationType"></param>
    public CustomTypeConverter(Type sourceType, Type destinationType)
    {
        _sourceType = sourceType;
        _destinationType = destinationType;
    }

   /// <summary>
   /// 转换方法
   /// </summary>
   /// <param name="sourceMember"></param>
   /// <param name="context"></param>
   /// <returns></returns>
    public object Convert(object sourceMember, ResolutionContext context)
    {
        if (sourceMember == null) return null;

        // 示例：DateTime 转格式化字符串
        if (_sourceType == typeof(DateTime) && _destinationType == typeof(string))
        {
            return ((DateTime)sourceMember).ToString("yyyy-MM-dd HH:mm:ss");
        }

        // 其他自定义转换逻辑...
        return System.Convert.ChangeType(sourceMember, _destinationType);
    }
}