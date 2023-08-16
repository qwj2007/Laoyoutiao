using AutoMapper;
using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Dto;
using System.Reflection;

namespace Laoyoutiao.webapi.Config
{
    /// <summary>
    /// 批量映射
    /// </summary>
    public class BatchMapperProfile:Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public BatchMapperProfile() {
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
                var mapper = CreateMap(attribute.SourceType, type);

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
                        mapper.ForMember(property.Name, src => src.MapFrom(propertyAttribute.SourceName));
                    }
                    if (propertyAttribute.SourceDataType != null && propertyAttribute.SourceDataType == typeof(DateTime))
                    {
                        //DateTime数据类型 映射 自定义字符串格式
                       // mapper.ForMember(property.Name, src => src.ConvertUsing(new FormatBatchConvert()));
                    }
                });

            });

        }
    }
}
