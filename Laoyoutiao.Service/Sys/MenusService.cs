using AutoMapper;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Menu;
using Laoyoutiao.Models.Dto.Sys;

using Laoyoutiao.Models.Entitys.Sys;
using System.Collections;
using System.Linq;

namespace Laoyoutiao.Service.Sys
{
    public class MenusService : BaseTreeService<Menus>, IMenusService
    {
        private readonly IMapper _mapper;
        public MenusService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
        }

        public override async Task<PageInfo> GetTreeAsync<TReq, TRes>(TReq req)
        {
            PageInfo pageInfo = await base.GetTreeAsync<TReq, TRes>(req);
            if (pageInfo.data != null)
            {
                List<MenusRes> list = pageInfo.data as List<MenusRes>;
                foreach (var item in list)
                {

                    item.IsButton = item.IsButton == "1" ? "是" : "否";
                    item.IsShow = item.IsShow == "1" ? "是" : "否";
                    GetStatusName(item);
                }
            }
            return pageInfo;
        }

        private void GetStatusName(MenusRes item)
        {
            foreach (var it in item.Children)
            {
                it.IsButton = it.IsButton == "1" ? "是" : "否";
                it.IsShow = it.IsShow == "1" ? "是" : "否";
                // it.StatusName = it.Status == 1 ? "启用" : "禁用";
                GetStatusName(it);
            }
        }



        public override async Task<bool> Add<TEdit>(TEdit input, long userId)
        {
            var edit = input as MenusEdit;
            Menus info = _mapper.Map<Menus>(edit);
            string path = "";
            //查找父组件传值过来
            long pid = info.ParentId;
            if (pid > 0)
            {
                var ids = new ArrayList();
                await GetPath(pid, ids);//找到一共有几个上级节点
                path = string.Join(":", ids.ToArray().Reverse());
            }

            using (var tran = _db.AsTenant().UseTran())
            {
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
                        info.Path = path + ":" + id;//更新path字段

                    }
                    else
                    {
                        info.Path = id + "";
                    }
                    info.Id = id;
                    await _db.Updateable(info).ExecuteCommandAsync();

                }
                else
                {
                    info.ModifyUserId = userId;
                    info.ModifyDate = DateTime.Now;
                    info.Path = path;
                    await _db.Updateable(info).ExecuteCommandAsync();
                }

                //按钮，先删除在添加
                await _db.Deleteable<Menus>().Where(a => a.ParentId == info.Id && a.IsButton == 1).ExecuteCommandAsync();
                info.Children.ForEach((a) =>
                {
                    a.ParentId = info.Id;
                    a.MenuUrl = " ";
                    a.ComponentUrl = " ";
                    a.CreateDate = DateTime.Now;
                    a.Path = "0";
                    a.CreateUserId = info.CreateUserId??info.ModifyUserId;
                });
                //再添加
                await _db.Insertable<Menus>(info.Children).ExecuteCommandAsync();
                //await _db.Insertable<>
                var updateList = await _db.Queryable<Menus>().Where(a => a.ParentId == info.Id).ToListAsync();
                updateList.ForEach(a => a.Path = info.Path + ":" + a.Id);
                await _db.Fastest<Menus>().BulkUpdateAsync(updateList);
                tran.CommitTran();
                return true;
            };
            
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

        private async Task GetPath(long pid, ArrayList ids)
        {
            ids.Add(pid);
            var model = await _db.Queryable<Menus>().Where(a => a.Id == pid).FirstAsync();
            if (model != null && model.ParentId > 0)
            {
                await GetPath(model.ParentId, ids);
            }
        }

        public async Task<List<MenusRes>> GetChildButtons(long parentId)
        {
            if (parentId > 0)
            {
                var btnList = await _db.Queryable<Menus>().Where(a => a.IsButton == 1 && a.IsDeleted == 0 && a.ParentId == parentId).ToListAsync();
                if (btnList != null && btnList.Count > 0)
                {

                    return _mapper.Map<List<MenusRes>>(btnList);
                }
            }
            else
            {
                //加载系统默认的按钮信息
                var sysbtnList = await _db.Queryable<SysButton>().Where(a => a.IsDeleted == 0).ToListAsync();
                if (sysbtnList != null && sysbtnList.Count > 0)
                {
                    List<MenusRes> btnsList = new List<MenusRes>();
                    // return _mapper.Map<List<MenusRes>>(sysbtnList);
                    // List<MenuButton> btnsList = new List<MenuButton>();
                    foreach (var (item, btn) in from item in sysbtnList
                                                let btn = new MenusRes()
                                                select (item, btn))
                    {
                        btn.ButtonClass = "";
                        btn.IsButton = "1";
                        btn.Name = item.BtnName;
                        btn.Code = item.BtnCode;
                        btn.BtnType = item.BtnType.ToString();
                        btn.Icon = item.Icon;
                        btn.Id = item.Id;
                        btn.IsShow = "0";
                        btn.ComponentUrl = "0";
                        btn.MenuUrl = "0";
                        btn.Memo = item.Memo;
                        btnsList.Add(btn);
                    }
                    return btnsList;
                }

            }
            return null;
        }
    }
}
