using AutoMapper;
using Laoyoutiao.Caches;
using Laoyoutiao.Common;
using Laoyoutiao.IService.OA;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.OA.Leave;
using Laoyoutiao.Models.Entitys.OA;
using Laoyoutiao.Models.Views;
using Laoyoutiao.WorkFlow.Core;
using System.ComponentModel;
using System.Reflection;

namespace Laoyoutiao.Service.OA
{
    public class LeaveService : BaseService<OALeave>, ILeaveService
    {
        private readonly IMapper _mapper;
        public LeaveService(IMapper mapper, CurrentUserCache cache) : base(mapper, cache)
        {
            this._mapper = mapper;
        }
        public override Task<bool> Add<TEdit>(TEdit input)
        {
            LeaveEdit leaveEdit = input as LeaveEdit;
            leaveEdit.UserId = _currentUser.loginUser.Id;
            leaveEdit.Days = Convert.ToDecimal(new TimeSpan(leaveEdit.EndTime.Ticks - leaveEdit.StartTime.Ticks).Days) + 1;
            return base.Add(input);
        }
        public override Task<long> AddOneRerunKeyValue<TEdit>(TEdit input)
        {
            LeaveEdit leaveEdit = input as LeaveEdit;
           leaveEdit.UserId = _currentUser.loginUser.Id;
            leaveEdit.Days = Convert.ToDecimal(new TimeSpan(leaveEdit.EndTime.Ticks - leaveEdit.StartTime.Ticks).Days) + 1;
            return base.AddOneRerunKeyValue(input);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEdit"></typeparam>
        /// <param name="input"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public override Task<OALeave> AddOrUpdateReturnEntity<TEdit>(TEdit input)
        {
            LeaveEdit leaveEdit = input as LeaveEdit;
           leaveEdit.UserId = _currentUser.loginUser.Id;
            leaveEdit.Days = Convert.ToDecimal(new TimeSpan(leaveEdit.EndTime.Ticks - leaveEdit.StartTime.Ticks).Days) + 1;
            return base.AddOrUpdateReturnEntity(input);
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
                    ActivityType = win.ActivityType,
                    DeptId = oa.DeptId

                });


            #region 数据权限
            var userInfo = _currentUser;
            if (userInfo != null)
            {
                if (userInfo.isAdmin || userInfo.isPower)
                {//若果是管理员或没有限制，不做任何处理
                }
                else
                {
                    //如果只能查看自己的数据
                    if (userInfo.isOnlySelf)
                    {
                        list = list.Where(oa => oa.UserId == userInfo.loginUser.Id);
                    }
                    if (userInfo.deptDataIds.Count > 0)
                    {
                        string deptIds = string.Join(",", userInfo.deptDataIds.ToArray());
                        list = list.Where(oa => oa.DeptId.Contains(deptIds) || deptIds.Contains(oa.DeptId));
                    }
                }
            }
            #endregion
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
