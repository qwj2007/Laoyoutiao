using AutoMapper;
using Laoyoutiao.Common;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.IService.WF;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.Sys;
using Laoyoutiao.Models.Dto.WF;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.Models.Entitys.WF;
using Laoyoutiao.WorkFlow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Service.WF
{
    public class WorkFlowService : BaseService<WF_WorkFlow>, IWorkFlowService
    {
        private readonly IMapper _mapper;
        public WorkFlowService(IMapper mapper) : base(mapper)
        {
            _mapper = mapper;
        }
        public override Task<bool> Add<TEdit>(TEdit input, long userId)
        {
            WFEdit wfedit = input as WFEdit;

            //WF_WorkFlow info = _mapper.Map<WF_WorkFlow>(wfedit);

            if (wfedit.Id > 0)//编辑
            {
                //wfedit.FlowId = Guid.NewGuid().ToString();
                //wfedit.FlowCode = TimeSpan.FromSeconds(DateTime.Now.Second).ToString();
            }
            else
            { //新增
                wfedit.FlowId = Guid.NewGuid().ToString();
                wfedit.FlowCode = DateTime.Now.Ticks.ToString();
            }

            return base.Add(wfedit, userId);
        }
        public override async Task<PageInfo> GetPagesAsync<TReq, TRes>(TReq req)
        {
            var menus = await _db.Queryable<Menus>().Where(a => a.IsDeleted == 0).ToListAsync();
            var flows = await base.GetPagesAsync<TReq, TRes>(req);
            var flowLists = flows.data as List<WFRes>;
            foreach (var item in flowLists)
            {
                var menu = menus.FirstOrDefault(a => a.Id == item.FormId);
                if (menu != null)
                {
                    item.FormName = menu.Name;
                }
            }
            return flows;
        }
        /// <summary>
        /// 更新表的流程状态
        /// </summary>
        /// <param name="statusChange"></param>
        /// <returns></returns>
        public async Task<bool> ChangeTableStatusAsync(WorkFlowStatusChange statusChange)
        {
            try
            {
                
                string sql = $" UPDATE {statusChange.TableName} SET FlowStatus='{(int)statusChange.Status}',FlowTime='{statusChange.FlowTime}' WHERE {statusChange.KeyName} = '{statusChange.KeyValue}'";

                int res = await _db.Ado.ExecuteCommandAsync(sql);
                return res > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
