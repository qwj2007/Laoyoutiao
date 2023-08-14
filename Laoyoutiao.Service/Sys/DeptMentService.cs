using AutoMapper;
using Laoyoutiao.Common;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using SqlSugar;
using System.Collections;
using System.Linq.Expressions;

namespace Laoyoutiao.Service.Sys
{
    public class DeptMentService : BaseTreeService<DeptMent>, IDeptMentService
    {
        private readonly IMapper _mapper;
        public DeptMentService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
        }

        public override async Task<PageInfo> GetTreeAsync<TReq, TRes>(TReq req)
        {
            PageInfo pageInfo = await base.GetTreeAsync<TReq, TRes>(req);
            List<DeptRes> list = pageInfo.data as List<DeptRes>;
            foreach (var item in list)
            {
                item.StatusName = item.Status == 1 ? "启用" : "禁用";
                GetStatusName(item);
            }
            return pageInfo;
        }

        private void GetStatusName(DeptRes item)
        {
            foreach (var it in item.Children)
            {
                it.StatusName = it.Status == 1 ? "启用" : "禁用";
                GetStatusName(it);
            }
        }


        //public override async Task<PageInfo> GetPagesAsync<TReq, TRes>(TReq req)
        //{
        //    PageInfo pageInfo = new PageInfo();
        //    DeptReq deptReq = req as DeptReq;
        //    Expression<Func<DeptMent, bool>> expression = a => a.IsDeleted == 0;
        //    string deptName = deptReq.DeptName ?? "";
        //    //var idArray = new ArrayList();
        //    //var idArray = new HashSet<long>();
        //    // object[] inIds = new object[] {2,3,4,5,7,8,11 };
        //    //影响构造树的条件过滤
        //    object[] inIds = _db.Queryable<DeptMent>().Where(a => a.IsDeleted == 0)
        //    .WhereIF(!string.IsNullOrEmpty(deptName), a => a.DeptName.Contains(deptName))
        //    .Select(it => it.Id).ToList().Cast<object>().ToArray();
        //    //查找所有的部门信息
        //    var list = _mapper.Map<List<DeptRes>>(await GetListByWhereAsync(expression));

        //    //查找到所有数据转换成树形结构
        //    var listTree = _db.Queryable<DeptMent>().Where(a => a.IsDeleted == 0).ToTree(it => it.Children, it => it.ParentId, 0, inIds);
        //    var parentList = _mapper.Map<List<DeptRes>>(listTree);


        //    #region
        //    //查找所有Id
        //    //if (!string.IsNullOrEmpty(deptName))
        //    //{
        //    //    var modelList = list.WhereIF(!string.IsNullOrEmpty(deptName), a => a.DeptName.Contains(deptName)).OrderByDescending(a => a.Id).ToList();
        //    //    if (modelList != null && modelList.Count > 0)
        //    //    {
        //    //        foreach (var item in modelList)
        //    //        {
        //    //            //这些父节点的所有子孙节点有要查找的信息
        //    //            //idArray.Add(item.Id);
        //    //            inIds.Prepend(item.Id);
        //    //        }
        //    //    }
        //    //}
        //    //查找所有的父节点信息
        //    //var parentList = _mapper.Map<List<DeptRes>>(list.Where(a => a.ParentId == 0).WhereIF(idArray.Count > 0, a => idArray.Contains(a.Id)));


        //    //foreach (var item in parentList)
        //    //{
        //    //    DeptRes deptRes = await GetChildList(list, item);
        //    //}
        //    ////过滤出包含deptName字段的属性的值
        //    //List<DeptRes> listDeptSearch = new List<DeptRes>();
        //    //foreach (var item in parentList)
        //    //{
        //    //    DeptRes deptRes = new DeptRes();
        //    //    var list1= item.Children;
        //    //    foreach (var it in list1) { 
        //    //    }
        //    //    if(list1.Count==0)
        //    //    listDeptSearch.Add(item);
        //    //}
        //    #endregion

        //    pageInfo.total = list.Count;
        //    pageInfo.data = parentList;
        //    return pageInfo;
        //}
        //private async Task<DeptRes> GetChildList(List<DeptRes> list, DeptRes item, string deptName)
        //{
        //    item.StatusName = item.Status == 1 ? "启用" : "禁用";
        //    //查找这条记录的所有的子记录
        //    var childList = list.Where(a => a.ParentId == item.Id).ToList();
        //    //var childDeptList = childList.Where(a=>a.DeptName.Contains(deptName));
        //    //if (childDeptList.Any()) { 
        //    //}
        //    foreach (var child in childList)
        //    {
        //        await GetChildList(list, child, deptName);
        //    }
        //    item.Children = childList;
        //    return item;
        //}
        //private async Task<DeptRes> GetChildList(List<DeptRes> list, DeptRes item)
        //{
        //    item.StatusName = item.Status == 1 ? "启用" : "禁用";
        //    //查找这条记录的所有的子记录
        //    var childList = list.Where(a => a.ParentId == item.Id).ToList();
        //    foreach (var child in childList)
        //    {
        //        await GetChildList(list, child);
        //    }
        //    item.Children = childList;
        //    return item;
        //}

        public override async Task<bool> Add<TEdit>(TEdit input, long userId)
        {
            DeptEdit deptEdit = input as DeptEdit;
            DeptMent info = _mapper.Map<DeptMent>(input);
            string path = "";
            //查找父组件传值过来
            long pid = info.ParentId;
            if (pid > 0)
            {
                var ids = new ArrayList();
                await GetDeptPath(pid, ids);
                path = string.Join(":", ids.ToArray().Reverse());
            }

            if (info.Id == 0)
            {
                info.CreateUserId = userId;
                info.CreateDate = DateTime.Now;
                info.Path = path;
                //info.UserType = 1;//0=炒鸡管理员，系统内置的
                info.IsDeleted = 0;
                long id = await _db.Insertable(info).ExecuteReturnBigIdentityAsync();
                if (pid > 0)
                {
                    info.Path = path + ":" + id;

                }
                else
                {
                    info.Path = id + "";
                }
                info.Id = id;
                return await _db.Updateable(info).ExecuteCommandAsync() > 0;

            }
            else
            {
                info.ModifyUserId = userId;
                info.ModifyDate = DateTime.Now;
                info.Path = path;
                return await _db.Updateable(info).ExecuteCommandAsync() > 0;
            }

        }

        /// <summary>
        /// 判断有没有子部门
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> IsExitChildList(long Id)
        {
            var list = await _db.Queryable<DeptMent>().Where(a => a.IsDeleted == 0 && a.ParentId == Id).ToListAsync();
            if (list.Count() > 0)
            {
                return true;//存在子部门
            }
            return false;
        }

        private async Task GetDeptPath(long pid, ArrayList ids)
        {
            ids.Add(pid);
            var model = await _db.Queryable<DeptMent>().Where(a => a.Id == pid).FirstAsync();
            if (model != null && model.ParentId > 0)
            {
                await GetDeptPath(model.ParentId, ids);
            }
        }
    }
}
