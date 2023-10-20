using AutoMapper;
using Laoyoutiao.Common;
using Laoyoutiao.IService.OA;
using Laoyoutiao.IService.Sys;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.OA.Leave;
using Laoyoutiao.Models.Entitys.OA;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.WorkFlow.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Service.OA
{
    public class LeaveService : BaseService<OALeave>, ILeaveService
    {
        public LeaveService(IMapper mapper) : base(mapper)
        {
        }
        public override Task<bool> Add<TEdit>(TEdit input, long userId)
        {

            LeaveEdit leaveEdit = input as LeaveEdit;
            if (leaveEdit.Id == 0) {
                leaveEdit.LeaveCode = DateTime.Now.Ticks.ToString();
            }
            leaveEdit.UserId = userId;
            leaveEdit.Days =Convert.ToDecimal( new TimeSpan(leaveEdit.EndTime.Ticks-leaveEdit.StartTime.Ticks).Days)+1;
            return base.Add(input, userId);
        }
        public override async  Task<PageInfo> GetPagesAsync<TReq, TRes>(TReq req)
        {
            var pageinfo=await base.GetPagesAsync<TReq, TRes>(req);
            var datas = pageinfo.data as List<LeaveRes> ;
            
            foreach (var item in datas)
            {
                item.FlowStatusName = EnumHelper.EnumToDescription<WorkFlowStatus>(item.FlowStatus);
               // GetEnumDesctiption(item);
                //item.FlowStatusName = EnumHelper.GetEnumDescription<WorkFlowStatus>(item.FlowStatus);
            }
            return pageinfo;
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
