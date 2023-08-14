using AutoMapper;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Menu;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Entitys.Sys;
using System.Collections;

namespace Laoyoutiao.Service.Sys
{
    public class MenusService : BaseTreeService<Menus>, IMenusService
    {
        private readonly IMapper _mapper;
        public MenusService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
        }
        //public override async Task<PageInfo> GetTreeAsync<TReq, TRes>(TReq req)
        //{
        //    PageInfo pageInfo = new PageInfo();
        //    MenusReq menusReq = req as MenusReq;
        //    //影响构造树的条件过滤
        //    var exp =  _db.Queryable<Menus>().WhereIF(!string.IsNullOrEmpty(menusReq.MenuName),a=>a.MenuName.Contains(menusReq.MenuName));
           
        //    var res = await exp.ToListAsync();
        //    object[] inIds = (await exp.Select(it => it.Id).ToListAsync()).Cast<object>().ToArray();

        //    //查找到所有数据转换成树形结构
        //    var listTree = _db.Queryable<Menus>().Where(a => a.IsDeleted == 0).ToTree(it => it.Children, it => it.ParentId, 0, inIds);
        //    var parentList = _mapper.Map<List<Menus>>(listTree);
        //    pageInfo.total = res.Count;
        //    pageInfo.data = parentList;
        //    return pageInfo;
        //}

        private void GetStatusName(MenusRes item)
        {
            foreach (var it in item.Children)
            {
               // it.StatusName = it.Status == 1 ? "启用" : "禁用";
                GetStatusName(it);
            }
        }


       
        public override async Task<bool> Add<TEdit>(TEdit input, long userId)
        {
           
            Menus info = _mapper.Map<Menus>(input);
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
            var list = await _db.Queryable<Menus>().Where(a => a.IsDeleted == 0 && a.ParentId == Id).ToListAsync();
            if (list.Count() > 0)
            {
                return true;//存在子部门
            }
            return false;
        }

        private async Task GetDeptPath(long pid, ArrayList ids)
        {
            ids.Add(pid);
            var model = await _db.Queryable<Menus>().Where(a => a.Id == pid).FirstAsync();
            if (model != null && model.ParentId > 0)
            {
                await GetDeptPath(model.ParentId, ids);
            }
        }
    }
}
