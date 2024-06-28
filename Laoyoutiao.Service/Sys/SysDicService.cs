using AutoMapper;
using Laoyoutiao.Caches;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Dto.Sys.Dic;
using Laoyoutiao.Models.Entitys.Sys;
using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Service.Sys
{
    public class SysDicService : BaseTreeService<SysDic>, ISysDicService
    {
        private readonly IMapper _mapper;
        public SysDicService(IMapper mapper, CustomCache cache) : base(mapper, cache)
        {
            _mapper = mapper;
        }
        /// <summary>
        /// 获取字典类型数据
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="req"></param>
        /// <returns></returns>
        //public override async Task<PageInfo> GetTreeAsync<TReq, TRes>(TReq req)
        //{
        //    var dicReq = req as DicReq;
        //    PageInfo pageInfo = new PageInfo();
        //    if (dicReq != null)
        //    {//查询条件

        //        var allDics = await _db.Queryable<SysDic>().Where(a => a.IsDeleted == 0)
        //               .WhereIF(!string.IsNullOrEmpty(dicReq.SearchName), a => a.Name.Contains(dicReq.SearchName) || a.DicCode.Contains(dicReq.SearchName))
        //               .ToTreeAsync(it=>it.Children,it=>it.ParentId,0);              
        //        pageInfo.data = allDics;
        //        pageInfo.total = allDics.Count();
        //    }
        //    return pageInfo;

        //}

        private async Task GetPath(long pid, ArrayList ids)
        {
            ids.Add(pid);
            var model = await _db.Queryable<SysDic>().Where(a => a.Id == pid).FirstAsync();
            if (model != null && model.ParentId > 0)
            {
                await GetPath(model.ParentId, ids);
            }
        }
        //添加功能
        public override async Task<bool> Add<TEdit>(TEdit input)
        {
            var edit = input as DicEdit;
            SysDic info = _mapper.Map<SysDic>(edit);
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
                    info.CreateUserId = _currentUser.loginUser.Id;
                    info.CreateDate = DateTime.Now;
                    info.FullId = path;
                    info.IsDeleted = 0;
                    long id = await _db.Insertable(info).ExecuteReturnBigIdentityAsync();
                    if (pid > 0)
                    {
                        info.FullId = path + ":" + id;//更新path字段

                    }
                    else
                    {
                        info.FullId = id + "";
                    }
                    info.Id = id;
                    //如果没填写编码，就自动生成一个编码
                    if (string.IsNullOrEmpty(info.DicCode))
                    {
                        info.DicCode = SnowFlakeSingle.Instance.NextId().ToString();
                    }
                    await _db.Updateable(info).ExecuteCommandAsync();
                }
                else
                {
                    info.ModifyUserId = _currentUser.loginUser.Id;
                    info.ModifyDate = DateTime.Now;
                    info.FullId = path;
                    await _db.Updateable(info).ExecuteCommandAsync();
                }
                var updateList = await _db.Queryable<SysDic>().Where(a => a.ParentId == info.Id).ToListAsync();
                updateList.ForEach(a => a.FullId = info.FullId + ":" + a.Id);
                await _db.Fastest<SysDic>().BulkUpdateAsync(updateList);
                tran.CommitTran();
                return true;
            };

        }

        /// <summary>
        /// 判断是否有子项目，如果有就不可以删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<bool> DelAsync(long id)
        {
            //判断这条数据是否有子数据
            var listDic = await _db.Queryable<SysDic>().Where(a => a.ParentId == id && a.IsDeleted == 0).ToListAsync();
            var listDicData = await _db.Queryable<SysDicData>().Where(a => a.ParentId == id && a.IsDeleted == 0).ToListAsync();
            if (listDic.Count == 0 && listDicData.Count == 0)
            {
                await base.DelAsync(id);
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
