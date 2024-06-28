using AutoMapper;
using Laoyoutiao.Caches;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Dto.Sys.DataPermission;
using Laoyoutiao.Models.Dto.User;
using Laoyoutiao.Models.Entitys;
using Laoyoutiao.Models.Entitys.Sys;

namespace Laoyoutiao.Service.Sys
{
    public class DataPermissionService : BaseService<DataPermission>, IDataPermissionService
    {
        private readonly IMapper _mapper;
        public DataPermissionService(IMapper mapper, CustomCache cache) : base(mapper, cache)
        {
            _mapper = mapper;
        }
        /// <summary>
        /// 根dataId返回一个对象
        /// </summary>
        /// <param name="DataId"></param>
        /// <param name="DataType"></param>
        /// <returns></returns>

        public DataPermissionRes GetModel(long DataId, string DataType)
        {
            var model =  _db.Queryable<DataPermission>().Where(a => a.DataId == DataId && a.DataType == DataType).First();
            //if (model != null) {
            // return   _mapper.Map<DataPermissionRes>(await model.FirstAsync()); 
            //}
           // if (model != null) {
                return _mapper.Map<DataPermissionRes>(model);
            //}         
            
            //return new DataPermissionRes();

        }
        public override async Task<DataPermission> AddOrUpdateReturnEntity<TEdit>(TEdit input)
        {
            var model = input as DataPermissionEdit;
           var oldmodel=await _db.Queryable<DataPermission>().Where(a => a.DataId == model.DataId && model.DataType == a.DataType && a.IsDeleted == 0).FirstAsync();
            if (oldmodel != null) {
                model.Id = oldmodel.Id;
            }
            if (!string.IsNullOrEmpty(model.Depts)) {
                //排序
                Array.Sort(model.Depts.Split(','));
                model.Depts = string.Join(',', model.Depts.Split(','));               
                
            }           
            return await base.AddOrUpdateReturnEntity(model);
        }
    }
}
