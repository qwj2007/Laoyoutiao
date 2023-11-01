using AutoMapper;
using Laoyoutiao.Common;
using Laoyoutiao.IService.OA;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.OA.Leave;
using Laoyoutiao.Models.Entitys.OA;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.Models.Entitys.WF;
using Laoyoutiao.Models.Views;
using Laoyoutiao.WorkFlow.Core;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Service.OA
{
    public class LeaveService : BaseService<OALeave>, ILeaveService
    {
        private readonly IMapper _mapper;
        public LeaveService(IMapper mapper) : base(mapper)
        {
            this._mapper = mapper;
        }
        public override Task<bool> Add<TEdit>(TEdit input, long userId)
        {
            LeaveEdit leaveEdit = input as LeaveEdit;           
            leaveEdit.UserId = userId;
            leaveEdit.Days = Convert.ToDecimal(new TimeSpan(leaveEdit.EndTime.Ticks - leaveEdit.StartTime.Ticks).Days) + 1;
            return base.Add(input, userId);
        }
        public override Task<long> AddOneRerunKeyValue<TEdit>(TEdit input, long userId)
        {
            LeaveEdit leaveEdit = input as LeaveEdit;            
            leaveEdit.UserId = userId;
            leaveEdit.Days = Convert.ToDecimal(new TimeSpan(leaveEdit.EndTime.Ticks - leaveEdit.StartTime.Ticks).Days) + 1;
            return base.AddOneRerunKeyValue(input, userId);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEdit"></typeparam>
        /// <param name="input"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public override Task<OALeave> AddOrUpdateReturnEntity<TEdit>(TEdit input, long userId)
        {
            LeaveEdit leaveEdit = input as LeaveEdit;           
            leaveEdit.UserId = userId;
            leaveEdit.Days = Convert.ToDecimal(new TimeSpan(leaveEdit.EndTime.Ticks - leaveEdit.StartTime.Ticks).Days) + 1;
            return base.AddOrUpdateReturnEntity(input, userId);           
        }

        /// <summary>
        /// 查找数据
        /// </summary>
        /// <typeparam name="TReq"></typeparam>
        /// <typeparam name="TRes"></typeparam>
        /// <param name="req"></param>
        /// <returns></returns>
        public override async Task<PageInfo> GetPagesAsync<TReq, TRes>(TReq req)
        {

            //查找
            LeaveReq leaveReq = req as LeaveReq;
            var list = _db.Queryable<OALeave>()
                .LeftJoin<V_WorkFlow>((oa, win) => oa.Id.ToString() == win.BusinessId && oa.Code == win.BusinessCode)
                .Where(oa => oa.IsDeleted == 0).WhereIF(!string.IsNullOrEmpty(leaveReq.Title), oa => oa.Title.Contains(leaveReq.Title))
                .Select<LeaveRes>((oa, win) => new LeaveRes //转换成dto
                {
                     Id = oa.Id,
                     Code = oa.Code,
                     Title = oa.Title,
                     UserId = oa.UserId,
                     LeaveType = oa.LeaveType,
                     Reason = oa.Reason,
                     Days = oa.Days,
                     StartTime = oa.StartTime,
                     EndTime = oa.EndTime,
                     FlowStatus = oa.FlowStatus,
                     FlowTime = oa.FlowTime,
                     Url = win.MenuUrl,
                     InstanceId = win.InstanceId,
                     FlowId = win.FlowId,
                     FormId = win.FormId,
                     FlowStatusName = "",
                     CreateDate = oa.CreateDate,
                     ActivityType=win.ActivityType

                 });

            var queryList = await list.Skip((leaveReq.PageIndex - 1) * leaveReq.PageSize)
            .Take(leaveReq.PageSize)
            .ToListAsync();
            PageInfo pageInfo = new PageInfo();
            pageInfo.data = queryList;// _mapper.Map<List<LeaveRes>>(queryList);
            pageInfo.total = list.Count();
            foreach (var item in queryList)
            {              
               
                item.FlowStatusName = EnumHelper.EnumToDescription<WorkFlowStatus>(item.FlowStatus);
            }

            return pageInfo;
           
        }

        private static void GetEnumDesctiption(LeaveRes item)
        {
            foreach (WorkFlowStatus st in Enum.GetValues(typeof(WorkFlowStatus)))
            {
                int v = (int)st;
                if (item.FlowStatus == v)
                {
                    FieldInfo field = st.GetType().GetField(st.ToString());
                    object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (objs == null || objs.Length == 0)
                    {
                        item.FlowStatusName = "";
                    }
                    else
                    {
                        var desc = (DescriptionAttribute)objs[0];
                        item.FlowStatusName = desc.Description;
                    }
                }
            }
        }
    }
}
