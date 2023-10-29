using AutoMapper;
using DotNetCore.CAP;
using Laoyoutiao.Common;
using Laoyoutiao.IService.WF;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.WF;
using Laoyoutiao.Models.Dto.WF.Instance;
using Laoyoutiao.Models.Dto.WF.Urge;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.Models.Entitys.WF;
using Laoyoutiao.Models.Views;
using Laoyoutiao.WorkFlow.Core;
using System.Data;
using System.Security.Policy;

namespace Laoyoutiao.Service.WF
{
    public class WorkFlowInstanceService : BaseService<WF_WorkFlow_Instance>, IWorkFlowInstanceService
    {
        private readonly IMapper _mapper;
        private readonly ICapPublisher capPublisher;
        public WorkFlowInstanceService(IMapper mapper, ICapPublisher capPublisher) : base(mapper)
        {
            this._mapper = mapper;
            this.capPublisher = capPublisher;
        }



        /// <summary>
        /// 获取流程审批人
        /// </summary>
        /// <param name="node"></param>
        /// <param name="userid">当前人</param>
        /// <param name="optionParams"></param>
        /// <returns></returns>
        private async Task<string> GetMakerListAsync(WorkFlowNode node, string userid, Dictionary<string, object> optionParams = null)
        {
            if (node.properties.IsNull())
            {
                return "";
            }

            switch (node.properties.approveType.ToUpper())
            {
                case ApproveType.SPECIAL_USER:
                    {
                        string res = string.Join(",", node.properties.users);
                        return res.IsNullOrEmpty() ? res : res + ",";
                    }
                case ApproveType.SPECIAL_ROLE:
                    {
                        //根据角色判断是哪些执行人
                        var userids = _db.Queryable<SysUserRole>().Where(a => node.properties.roles.Contains(a.RoleId.ToString())).ToList();
                        //var userids = await configService.GetUserIdsByRoleIdsAsync(node.properties.roles.Select(x => Convert.ToInt64(x)).ToList());
                        string res = string.Join(",", userids);
                        return res.IsNullOrEmpty() ? res : res + ",";
                    }
                //case FlowNodeSetInfo.SQL:
                //    {
                //        string idsql = node.SetInfo.Nodedesignatedata.SQL;
                //        var array = idsql.Split('_');
                //        if (array.Length >= 2)
                //        {
                //            string sysname = array[0].ToLower();//sys oa  wf weixin
                //            var resids = await configService.GetFlowNodeInfo(sysname, new FlowViewModel
                //            {
                //                sql = idsql,
                //                param = optionParams,
                //                UserId = userid
                //            });
                //            if (!resids.HasItems())
                //            {
                //                return null;
                //            }
                //            string res = string.Join(",", resids);
                //            return res.IsNullOrEmpty() ? res : res + ",";
                //        }
                //        else
                //        {
                //            throw new Exception("无法判断要访问哪个系统！");
                //        }
                //    }
                case ApproveType.ALL_USER:
                    return "0";
                default:
                    return null;
            }
        }

        #region 我的待办事项
        /// <summary>
        /// 我的待办事项
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public async Task<PageInfo> GetUserTodoListAsync(WorkFlowInstanceReq req)
        {
            PageInfo pageInfo = new PageInfo();
            var query1 = _db.Queryable<V_WorkFlow>().Where(a => a.MakerList.Contains(req.LoginUserId.ToString() + ','));
            var query2 = _db.Queryable<V_WorkFlow>()
                .LeftJoin<WF_WorkFlow_Notice>((vf, wn) => vf.InstanceId == wn.InstanceId)
                .Where((vf, wn) => wn.IsDeleted == 0 && wn.IsRead == 0 && wn.IsTransition == 1
                && wn.Status == 1 && wn.Maker == req.LoginUserId.ToString());

            var allQuery = _db.Union(query1, query2)
                .WhereIF(!string.IsNullOrEmpty(req.UserName), a => a.CreateUserName.Contains(req.UserName))
                .WhereIF(!string.IsNullOrEmpty(req.FlowName), a => a.BusinessName.Contains(req.FlowName))
                .WhereIF(!string.IsNullOrEmpty(req.BusinessName), a => a.BusinessName.Contains(req.BusinessName));
            var res = await allQuery.Skip((req.PageIndex - 1) * req.PageSize)
               .Take(req.PageSize)
               .ToListAsync();
            pageInfo.data = _mapper.Map<List<WorkFlowInstanceRes>>(res);
            foreach (var item in pageInfo.data as List<WorkFlowInstanceRes>)
            {
                item.FlowStatusName = EnumHelper.EnumToDescription<WorkFlowStatus>(item.FlowStatus);
            }
            pageInfo.total = allQuery.Count();
            return pageInfo;

        }
        #endregion

        #region 用户流程操作历史记录
        /// <summary>
        /// 获取用户流程操作历史记录
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        //public async Task<Page<WorkFlowOperationHistoryDto>> GetUserOperationHistoryAsync(WorkFlowOperationHistorySearchDto searchDto)
        //{
        //    return await databaseFixture.Db.WorkflowInstance.GetUserOperationHistoryAsync(searchDto);
        //}
        #endregion

        #region 我发起的流程
        /// <summary>
        /// 获取用户发起的流程
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        //public async Task<Page<UserWorkFlowDto>> GetUserWorkFlowPageAsync(int pageIndex, int pageSize, string userId)
        //{
        //    return await databaseFixture.Db.WorkflowInstance.GetUserWorkFlowPageAsync(pageIndex, pageSize, userId);
        //}
        #endregion

        #region 我的审批历史
        /// <summary>
        /// 获取我的审批历史记录
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        //public async Task<Page<UserWorkFlowDto>> GetMyApprovalHistoryAsync(int pageIndex, int pageSize, string userId)
        //{
        //    return await databaseFixture.Db.WorkflowInstance.GetMyApprovalHistoryAsync(pageIndex, pageSize, userId);
        //}
        #endregion

        #region 流程过程流转处理

        #region
        /// <summary>
        /// 添加或修改自定义表单数据
        /// </summary>
        /// <param name="addProcess"></param>
        /// <returns></returns>
        //public async Task<bool> AddOrUpdateCustomFlowFormAsync(WorkFlowProcess addProcess)
        //{
        //    using (var tran = databaseFixture.Db.BeginTransaction())
        //    {
        //        try
        //        {
        //            var dbflow = await databaseFixture.Db.Workflow.FindByIdAsync(addProcess.FlowId);
        //            if (addProcess.InstanceId == default(Guid))
        //            {
        //                WfWorkflowInstance workflowInstance = new WfWorkflowInstance
        //                {
        //                    InstanceId = Guid.NewGuid(),
        //                    FlowId = dbflow.FlowId,
        //                    Code = DateTime.Now.ToTimeStamp() + string.Empty.CreateNumberNonce(),
        //                    CreateUserId = addProcess.UserId,
        //                    CreateUserName = addProcess.UserName,
        //                    FlowContent = dbflow.FlowContent,
        //                    IsFinish = null,
        //                    Status = (int)WorkFlowStatus.UnSubmit,
        //                    UpdateTime = DateTime.Now.ToTimeStamp()
        //                };
        //                await databaseFixture.Db.WorkflowInstance.InsertAsync(workflowInstance, tran);
        //                addProcess.InstanceId = workflowInstance.InstanceId;

        //                //表单关联记录创建
        //                var dbform = await databaseFixture.Db.WorkflowForm.FindByIdAsync(addProcess.FormId);
        //                WfWorkflowInstanceForm instanceForm = new WfWorkflowInstanceForm
        //                {
        //                    Id = Guid.NewGuid(),
        //                    CreateUserId = addProcess.UserId,
        //                    FormContent = dbform.Content,
        //                    FormData = addProcess.FormData,
        //                    InstanceId = addProcess.InstanceId,
        //                    FormId = dbform.FormId,
        //                    FormType = dbform.FormType,
        //                    FormUrl = null,
        //                    CreateTime = DateTime.Now.ToTimeStamp(),
        //                };
        //                await databaseFixture.Db.WorkflowInstanceForm.InsertAsync(instanceForm, tran);
        //            }
        //            else
        //            {
        //                //实例不再创建
        //                //表单关联记录修改
        //                var dbinstanceForm = await databaseFixture.Db.WorkflowInstanceForm.FindAsync(m => m.InstanceId == addProcess.InstanceId);
        //                dbinstanceForm.FormData = addProcess.FormData;
        //                await databaseFixture.Db.WorkflowInstanceForm.UpdateAsync(dbinstanceForm, tran);
        //            }

        //            tran.Commit();
        //            return WorkFlowResult.Success("提交成功", data: addProcess.InstanceId);
        //        }
        //        catch (Exception ex)
        //        {
        //            tran.Rollback();
        //            return WorkFlowResult.Error("提交失败");
        //        }
        //    }
        //}



        #endregion


        /// <summary>
        /// 创建一个实例
        /// /// 注意事项：
        /// 1、流程开始节点不可添加任何条件分支（不符合逻辑，故人为规定）,即开始节点之后必须只能有一个任务节点，否则整个逻辑就错误了
        /// </summary>
        /// <param name="model">传递过来的参数</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> CreateInstanceAsync(WorkFlowProcessTransition model)
        {
            //string instanceId = default(Guid).ToString();
            //先删除再添加
            var result = await _db.Ado.UseTranAsync(async () =>
            {
                var userInfo = await _db.Queryable<SysUser>().FirstAsync(a => a.Id.ToString() == model.UserId && a.IsDeleted == 0);
                model.UserName = userInfo.UserName;
                //根据url查找到flowId
                var menu = await _db.Queryable<Menus>().FirstAsync(a => a.MenuUrl == model.url && a.IsDeleted == 0 && a.IsButton == 0);
                if (menu == null)
                {
                    throw new ArgumentException("菜单的URL", "菜单的url为配置");
                }
                long fromId = menu.Id;
                //根据FormId查找到workflow
                WF_WorkFlow workflow = await _db.Queryable<WF_WorkFlow>().FirstAsync(a => a.FormId == fromId);
                if (workflow == null || workflow.FlowContent.Length == 0)
                {
                    throw new ArgumentException("未配置流程", "未配置流程，请联系管理员");
                }

                //WorkFlowStatusChange StatusChange = new WorkFlowStatusChange
                //{
                //    KeyName = "Id",
                //    KeyValue = model.Id.ToString(),
                //    TableName = model.,
                //    // TargetName = SourceTable + "_ChangeStatus"
                //};
                //查找是否已经有流程实例
                var flowInstance = await _db.Queryable<WF_WorkFlow_Instance>().FirstAsync(a => a.FlowId == workflow.FlowId && a.FormId == fromId.ToString()
                && a.IsDeleted == 0 & a.BusinessId == model.Id.ToString() & a.BusinessFromTable == model.StatusChange.TableName);

                MsWorkFlowContext context = new MsWorkFlowContext(new WorkFlow.Core.WorkFlow
                {
                    FlowId = Guid.Parse(workflow.FlowId),
                    FlowJson = workflow.FlowContent,
                    ActivityNodeId = default(Guid)
                });

                #region 创建/修改实例               
                //创建
                if (flowInstance == null)
                {
                    flowInstance = new WF_WorkFlow_Instance
                    {
                        InstanceId = Guid.NewGuid().ToString(),
                        FlowId = workflow.FlowId.ToString(),
                        Code = DateTime.Now.Ticks + string.Empty.CreateNumberNonce(),
                        ActivityId = context.WorkFlow.NextNodeId.ToString(),
                        ActivityName = context.WorkFlow.NextNode.text.value,
                        ActivityType = (int)context.WorkFlow.NextNodeType,
                        PreviousId = context.WorkFlow.ActivityNodeId.ToString(),
                        MakerList = await this.GetMakerListAsync(context.WorkFlow.Nodes[context.WorkFlow.NextNodeId], model.UserId.ToString()),
                        CreateUserId = long.Parse(model.UserId),
                        CreateUserName = model.UserName,
                        FlowContent = workflow.FlowContent,
                        IsFinish = context.WorkFlow.NextNodeType.ToIsFinish(),
                        FlowStatus = (int)WorkFlowStatus.Running,
                        Status = 1,// (int)WorkFlowStatus.Running,
                        FormId = workflow.FormId.ToString(),
                        BusinessFromTable = model.StatusChange.TableName,
                        BusinessName = model.BusinessName,
                        BusinessId = model.Id.ToString(),// keyValue.ToString(),
                        BusinessCode = model.Code,
                        ComValue=model.ComValue.ToString()
                        

                    };
                    await _db.Insertable(flowInstance).ExecuteCommandAsync();
                }
                else
                {

                    //修改   
                    flowInstance.ActivityId = context.WorkFlow.NextNodeId.ToString();
                    flowInstance.ActivityName = context.WorkFlow.NextNode.text.value;
                    flowInstance.ActivityType = (int)context.WorkFlow.NextNodeType;
                    flowInstance.PreviousId = context.WorkFlow.ActivityNodeId.ToString();
                    flowInstance.MakerList = await this.GetMakerListAsync(context.WorkFlow.Nodes[context.WorkFlow.NextNodeId], model.UserId.ToString());
                    flowInstance.FlowContent = workflow.FlowContent;
                    flowInstance.IsFinish = context.WorkFlow.NextNodeType.ToIsFinish();
                    flowInstance.FlowStatus = (int)WorkFlowStatus.Running;
                    flowInstance.ModifyDate = DateTime.Now;
                    flowInstance.BusinessFromTable = model.StatusChange.TableName;
                    flowInstance.BusinessName = model.BusinessName;
                    flowInstance.BusinessId = model.Id.ToString();
                    flowInstance.BusinessCode = model.Code;
                    flowInstance.ComValue = model.ComValue.ToString();
                    await _db.Updateable(flowInstance).ExecuteCommandAsync();
                }
                #endregion

                #region 创建流程实例表单关联记录
                //var dbform = await databaseFixture.Db.WorkflowForm.FindByIdAsync(dbflow.FormId);
                //if ((WorkFlowFormType)dbform.FormType == WorkFlowFormType.System)
                //{
                //    WfWorkflowInstanceForm instanceForm = new WfWorkflowInstanceForm
                //    {
                //        Id = Guid.NewGuid(),
                //        CreateUserId = model.UserId,
                //        FormContent = model.StatusChange.KeyValue,//保存对应表单主键
                //        FormData = model.StatusChange.KeyValue,
                //        InstanceId = workflowInstance.InstanceId,
                //        FormId = dbform.FormId,
                //        FormType = dbform.FormType,
                //        FormUrl = dbform.FormUrl
                //    };
                //    await databaseFixture.Db.WorkflowInstanceForm.InsertAsync(instanceForm, tran);
                //}
                //else
                //{
                //    //强制修改为null
                //    model.StatusChange = null;
                //}
                #endregion

                #region 创建流程操作记录
                WF_WorkFlow_Operation_History operationHistory = new WF_WorkFlow_Operation_History
                {
                    OperationId = Guid.NewGuid().ToString(),
                    InstanceId = flowInstance.InstanceId,
                    CreateUserId = long.Parse(model.UserId),
                    CreateUserName = userInfo.UserName,
                    Content = "提交流程",
                    NodeName = context.WorkFlow.ActivityNode.text.value,
                    NodeId = context.WorkFlow.ActivityNodeId.ToString(),
                    TransitionType = (int)WorkFlowMenu.Submit
                };
                await _db.Insertable(operationHistory).ExecuteCommandAsync();
                #endregion

                #region 创建流程流转记录

                WF_WorkFlow_Transition_History transitionHistory = new WF_WorkFlow_Transition_History
                {
                    transitionId = Guid.NewGuid().ToString(),
                    InstanceId = flowInstance.InstanceId,
                    FromNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                    FromNodeType = (int)context.WorkFlow.ActivityNodeType,
                    FromNodeName = context.WorkFlow.ActivityNode.text.value,
                    ToNodeId = context.WorkFlow.NextNodeId.ToString(),
                    ToNodeType = (int)context.WorkFlow.NextNodeType,
                    ToNodeName = context.WorkFlow.NextNode.text.value,
                    CreateUserId = long.Parse(model.UserId),
                    CreateUserName = userInfo.UserName,
                    TransitionState = (int)WorkFlowTransitionStateType.Normal,
                    IsFinish = context.WorkFlow.NextNodeType.ToIsFinish(),
                };
                await _db.Insertable(transitionHistory).ExecuteCommandAsync();
                #endregion                
                //改变表单状态,哪个表单订阅了这个就触发改变状态的值。
                await FlowStatusChangePublisher(model.StatusChange, WorkFlowStatus.Submit);
            });
            return result.IsSuccess;
        }
        /// <summary>
        /// CAP发布订阅
        /// </summary>
        /// <param name="statusChange"></param>
        /// <param name="flowStatus">要改变成的状态</param>
        /// <returns></returns>
        private async Task FlowStatusChangePublisher(WorkFlowStatusChange statusChange, WorkFlowStatus flowStatus)
        {
            if (statusChange != null)
            {
                statusChange.Status = flowStatus;
                statusChange.FlowTime = DateTime.Now;
                await capPublisher.PublishAsync(statusChange.TargetName, statusChange);
            }
        }
        /// <summary>
        /// 获取执行过的节点
        /// </summary>
        /// <param name="instanceid"></param>
        /// <param name="currentNodeId"></param>
        /// <returns></returns>
        private async Task<List<WorkFlowNode>> GetExcuteNodes(string instanceid, string currentNodeId)
        {
            var operationHis = await _db.Queryable<WF_WorkFlow_Operation_History>().Where(a => a.InstanceId == instanceid.ToString()).ToListAsync();
            var list = operationHis.Where(m => m.NodeId != currentNodeId && (m.TransitionType == (int)WorkFlowMenu.Agree || m.TransitionType == (int)WorkFlowMenu.Submit))
                .OrderBy(m => m.CreateDate);
            List<WorkFlowNode> nodes = new List<WorkFlowNode>();
            foreach (var item in list)
            {
                if (item.TransitionType == (int)WorkFlowMenu.Back)//当循环到Back节点时候，后面节点不在循环
                {
                    break;
                }
                else
                {
                    if (!nodes.Any(m => m.Id.ToString() == item.NodeId))
                    {
                        var flow = new WorkFlowNode();
                        flow.Id = Guid.Parse(item.NodeId);
                        flow.text.value = item.NodeName;
                        nodes.Add(flow);
                    }
                }
            }
            return nodes;
        }

        /// <summary>
        /// 获取工作流进程信息
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public async Task<WorkFlowProcess> GetProcessAsync(WorkFlowProcess process)
        {
            WorkFlowProcess model = new WorkFlowProcess
            {
                InstanceId = process.InstanceId
            };
            var dbflow = await _db.Queryable<WF_WorkFlow>().FirstAsync(a => a.FlowId == process.FlowId.ToString());

            model.FlowId = dbflow.FlowId;
            model.FlowName = dbflow.FlowName;
            model.FormId = dbflow.FormId.ToString();
            if (process.InstanceId == default(Guid).ToString())//流程刚开始
            {
                // var dbform = await databaseFixture.Db.WorkflowForm.FindByIdAsync(dbflow.FormId);
                model.FormType = WorkFlowFormType.System;// (WorkFlowFormType)dbform.FormType;
                model.FormContent = "";// dbform.Content;
                model.FormUrl = "";// dbform.FormUrl;
                model.FormData = null;
                model.Menus = new List<int>
                {
                    (int)WorkFlowMenu.Submit,
                    (int)WorkFlowMenu.FlowImage,
                    (int)WorkFlowMenu.Save,
                    (int)WorkFlowMenu.Return,
                };
                model.FlowData = new WorkFlowProcessFlowData
                {
                    IsFinish = null,
                    Status = (int)WorkFlowStatus.UnSubmit
                };
            }
            else
            {
                var instanceform = await _db.Queryable<WF_WorkFlow_Instance>().FirstAsync(a => a.InstanceId == process.InstanceId);
                //var instanceform = await databaseFixture.Db.WorkflowInstanceForm.FindAsync(m => m.InstanceId == process.InstanceId);
                model.FormType = WorkFlowFormType.System;// (WorkFlowFormType)instanceform;
                model.FormContent = ""; //instanceform.FormContent;
                model.FormUrl = "";// instanceform.FormUrl;
                model.FormData = "";// instanceform.FormData;

                var flowinstance = await _db.Queryable<WF_WorkFlow_Instance>().FirstAsync(a => a.InstanceId == process.InstanceId); ;
                model.FlowData = new WorkFlowProcessFlowData
                {
                    IsFinish = flowinstance.IsFinish,
                    Status = flowinstance.Status
                };
                if (flowinstance.IsFinish == null && model.FormType == WorkFlowFormType.Custom)//表示自定义表单刚保存情况
                {
                    model.Menus = new List<int>
                    {
                        (int)WorkFlowMenu.Submit,//提交
                        (int)WorkFlowMenu.FlowImage,//工作流程图
                        (int)WorkFlowMenu.Save,//保存按钮
                        (int)WorkFlowMenu.Return//返回                       
                    };
                    return model;
                }
                if (flowinstance.IsFinish == (int)WorkFlowInstanceStatus.Finish) //流程结束情况
                {
                    model.Menus = new List<int>();
                    //流程打印按钮显示判断
                    if (process.UserId == flowinstance.CreateUserId.ToString())//流程通过并且是当前人查看才显示打印按钮
                    {
                        model.Menus.Add((int)WorkFlowMenu.Print);
                    }
                    //已阅按钮显示判断
                    var dbnotices = await _db.Queryable<WF_WorkFlow_Notice>().Where(a => a.Maker == process.UserId && a.InstanceId == process.InstanceId && a.IsTransition == 1 && a.IsRead == 0 && a.Status == 1).ToListAsync();
                    if (dbnotices.Any())
                    {
                        model.Menus.Add((int)WorkFlowMenu.View);
                    }
                    model.Menus.Add((int)WorkFlowMenu.Approval);
                    model.Menus.Add((int)WorkFlowMenu.FlowImage);
                    model.Menus.Add((int)WorkFlowMenu.Return);
                    return model;
                }

                //============= 流程运行中情况判断 =============//

                //根据当前人获取可操作的按钮
                //获取下一步的执行人
                MsWorkFlowContext context = new MsWorkFlowContext(new WorkFlow.Core.WorkFlow
                {
                    FlowId = Guid.Parse(dbflow.FlowId),
                    FlowJson = flowinstance.FlowContent,
                    ActivityNodeId = Guid.Parse(flowinstance.ActivityId)
                });
                model.FlowData.CurrentNode = context.WorkFlow.ActivityNode;

                if (context.WorkFlow.ActivityNode.Type == WorkFlowNode.START)//节点退回到开始节点情况
                {
                    var dbinstance = await _db.Queryable<WF_WorkFlow_Instance>().Where(a => a.InstanceId == process.InstanceId).FirstAsync();
                    //var dbinstance = await databaseFixture.Db.WorkflowInstance.FindByIdAsync(process.InstanceId);
                    if (dbinstance.CreateUserId.ToString() == process.UserId)
                    {
                        model.Menus = new List<int>
                        {
                            (int)WorkFlowMenu.ReSubmit,
                            (int)WorkFlowMenu.Approval,
                            (int)WorkFlowMenu.FlowImage,
                            (int)WorkFlowMenu.Save,
                            (int)WorkFlowMenu.Return,
                        };
                    }
                    else
                    {
                        model.Menus = new List<int>
                        {
                            (int)WorkFlowMenu.Approval,
                            (int)WorkFlowMenu.FlowImage,
                            (int)WorkFlowMenu.Return,
                        };
                    }
                    return model;
                }
                else
                {
                    if (!string.IsNullOrEmpty(flowinstance.MakerList))
                    {
                        if (flowinstance.MakerList.Trim() == "0")//全部人员（实际上不允许存在，因为没有实际意义，但是还是实现了）
                        {
                            model.Menus = new List<int>
                            {
                                (int)WorkFlowMenu.Agree,
                                (int)WorkFlowMenu.Deprecate,
                                (int)WorkFlowMenu.Back,
                            };
                        }
                        else
                        {
                            List<long> userIds = flowinstance.MakerList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToInt64(x)).ToList();
                            if (userIds.Contains(process.UserId.ToInt64()))
                            {
                                model.Menus = new List<int>
                                {
                                    (int)WorkFlowMenu.Agree,
                                    (int)WorkFlowMenu.Deprecate,
                                    (int)WorkFlowMenu.Back,
                                };
                                //获取执行过的节点
                                model.ExecutedNode = await GetExcuteNodes(process.InstanceId, flowinstance.ActivityId);
                            }
                        }
                        if (model.Menus == null)
                        {
                            model.Menus = new List<int>();
                        }
                        //已阅按钮显示判断
                        var dbnotices = await _db.Queryable<WF_WorkFlow_Notice>().Where(
                            a => a.Maker == process.UserId && a.InstanceId == model.InstanceId &&
                        a.IsTransition == 1 && a.IsRead == 0 && a.Status == 1).ToListAsync();

                        //var dbnotices = await databaseFixture.Db.WfWorkflowNotice.FindAllAsync(
                        //    m => m.Maker == model.UserId && m.InstanceId == model.InstanceId 
                        //&& m.IsTransition == 1 && m.IsRead == 0 && m.Status == 1);
                        if (dbnotices.Any())
                        {
                            model.Menus.Add((int)WorkFlowMenu.View);
                        }

                        //委托按钮显示判断
                        if (await CanAssign(process, flowinstance.MakerList))
                        {
                            model.Menus.Add((int)WorkFlowMenu.Assign);
                        }

                        //终止按钮显示判断
                        var prenode = context.GetLinesForFrom(flowinstance.ActivityId);
                        if (prenode.Count == 1)
                        {
                            var nodeType = context.GetNodeType(prenode[0].sourceNodeId);
                            if (nodeType == WorkFlowInstanceNodeType.Start && process.UserId == flowinstance.CreateUserId.ToString().Trim())
                            {
                                model.Menus.Add((int)WorkFlowMenu.Withdraw);//撤回
                            }
                        }
                        model.Menus.Add((int)WorkFlowMenu.Approval);
                        model.Menus.Add((int)WorkFlowMenu.FlowImage);
                        model.Menus.Add((int)WorkFlowMenu.Return);
                    }
                    else
                    {
                        model.Menus = new List<int>
                        {
                            (int)WorkFlowMenu.Approval,
                            (int)WorkFlowMenu.FlowImage,
                            (int)WorkFlowMenu.Return
                        };
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// 判断当前用户是否能显示委托操作按钮
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        private async Task<bool> CanAssign(WorkFlowProcess process, string makerlist)
        {
            string str = process.UserId + ",";
            if (!makerlist.Contains(str))//当前人非执行人员
            {
                return false;
            }
            /// 将自己审批某个流程的权限赋予其他人，让其他用户代审批流程;
            /// 规则：A委托给B，A不能再审批且不能多次委托，B可再次委托给C，同理A
            var dbassigns = await _db.Queryable<WFWorkFlowAssign>().Where(m => m.InstanceId == process.InstanceId && m.FlowId == process.FlowId).ToListAsync();
            //var dbassigns = await databaseFixture.Db.WfWorkflowAssign.FindAllAsync(m => m.InstanceId == process.InstanceId && m.FlowId == process.FlowId);
            if (dbassigns.Any(m => m.UserId.ToString() == process.UserId))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 系统定制流程获取
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<WorkFlowProcess> GetProcessForSystemAsync(SystemFlowDto model)
        {
            WorkFlowProcess process = new WorkFlowProcess
            {
                UserId = model.UserId
            };

            //var dbflowform = await databaseFixture.Db.WorkflowForm.FindAsync(m => m.FormUrl == model.FormUrl);
            var flow = await _db.Queryable<WF_WorkFlow>().Where(a => a.FormId == model.FormId).FirstAsync();
            //var dbflow = await databaseFixture.Db.Workflow.FindAsync(m => m.FormId == dbflowform.FormId);
            process.FlowId = flow.FlowId;
            process.FlowName = flow.FlowName;
            process.FormId = flow.FormId.ToString();
            if (model.PageId.IsNullOrEmpty())
            {
                process.InstanceId = default(Guid).ToString();
            }
            else
            {
                var instanceform = await _db.Queryable<WF_WorkFlow_Instance>().Where(a => a.FlowId == process.FlowId && a.FormId == process.FormId).FirstAsync();
                //var instanceform = await databaseFixture.Db.WorkflowInstanceForm.FindAsync(m => m.FormId == dbflowform.FormId && m.FormContent == model.PageId);
                process.InstanceId = instanceform != null ? instanceform.InstanceId : default(Guid).ToString();
            }
            return await GetProcessAsync(process);
        }

        /// <summary>
        /// 流程过程流转处理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> ProcessTransitionFlowAsync(WorkFlowProcessTransition model)
        {
            bool result = true;
            switch (model.MenuType)//流程按钮类型
            {
                case WorkFlowMenu.ReSubmit://重新提交
                    result = await ProcessTransitionReSubmitAsync(model);
                    break;
                case WorkFlowMenu.Agree://同意
                    result = await WorkFlowAgreeAsync(model);
                    break;
                case WorkFlowMenu.Deprecate://不同意
                    result = await WorkFlowDeprecateAsync(model);
                    break;
                case WorkFlowMenu.Back://返回
                    result = await WorkFlowBackAsync(model);
                    break;
                case WorkFlowMenu.Withdraw://撤回
                    result = await WorkFlowWithdrawAsync(model);
                    break;
                case WorkFlowMenu.View://已阅
                    result = await ProcessTransitionViewAsync(model);
                    break;
                case WorkFlowMenu.Stop://终止
                    break;
                case WorkFlowMenu.Cancel://弃权
                    break;
                case WorkFlowMenu.Throgh://直送
                    break;
                case WorkFlowMenu.Assign://委托
                    result = await ProcessTransitionAssignAsync(model);
                    break;
                case WorkFlowMenu.CC://抄送
                    break;
                case WorkFlowMenu.Suspend://挂起
                    break;
                case WorkFlowMenu.Resume://回复
                    break;
                case WorkFlowMenu.Save://保存
                case WorkFlowMenu.Submit://提交
                case WorkFlowMenu.Return://返回
                case WorkFlowMenu.Approval://审批意见
                case WorkFlowMenu.FlowImage://流程图
                default:
                    result = false;
                    //result = WorkFlowResult.Error("未找到匹配按钮！");
                    break;
            }
            return result;
        }

        /// <summary>
        /// 计算票数
        /// </summary>
        /// <param name="InstanceId"></param>
        /// <param name="nodeId"></param>
        /// <param name="node"></param>
        /// <param name="chatParallelCalcType"></param>
        /// <returns></returns>
        private async Task<WorkFlowInstanceStatus> CalcVotes(string InstanceId, string nodeId, WorkFlowNode node, ChatParallelCalcType chatParallelCalcType)
        {
            var dboperhis = await _db.Queryable<WF_WorkFlow_Operation_History>().Where(a => a.InstanceId == InstanceId && a.NodeId == nodeId).ToListAsync();
            bool result;
            switch (chatParallelCalcType)
            {
                case ChatParallelCalcType.MoreThenHalf://超过一半
                    result = dboperhis.Count(m => m.TransitionType == (int)WorkFlowMenu.Agree) > (dboperhis.Count() / 2);
                    break;
                case ChatParallelCalcType.OneHundredPercent://百分之百
                default:
                    result = dboperhis.Count(m => m.TransitionType == (int)WorkFlowMenu.Agree) == dboperhis.Count();
                    break;
            }
            if (node.NodeType() == WorkFlowInstanceNodeType.End)
            {
                return WorkFlowInstanceStatus.Finish;
            }
            else
            {
                return WorkFlowInstanceStatus.Running;
            }
        }

        /// <summary>
        /// 会签节点逻辑
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="context"></param>
        /// <param name="dbflowinstance"></param>
        /// <param name="model"></param>
        /// <param name="flowInstanceStatus"></param>
        /// <returns></returns>
        private async Task ChatLogic(MsWorkFlowContext context,
            WF_WorkFlow_Instance dbflowinstance, WorkFlowProcessTransition model,
            WorkFlowInstanceStatus flowInstanceStatus)
        {
            //并行逻辑
            if (context.WorkFlow.ActivityNode.properties.ChatData.ChatType == ChatType.Parallel)
            {
                #region 并行
                var makerUsers = dbflowinstance.MakerList.Split(',').Where(m => !string.IsNullOrEmpty(m)).ToList();
                var transitionHistory = new WF_WorkFlow_Transition_History
                {
                    transitionId = Guid.NewGuid().ToString(),
                    InstanceId = dbflowinstance.InstanceId,
                    CreateUserId = long.Parse(model.UserId),
                    CreateUserName = model.UserName,
                    TransitionState = (int)WorkFlowTransitionStateType.Normal,
                    IsFinish = (int)flowInstanceStatus,
                    FromNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                    FromNodeName = context.WorkFlow.ActivityNode.text.value,
                    FromNodeType = (int)context.WorkFlow.ActivityNodeType,
                    ToNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                    ToNodeName = context.WorkFlow.ActivityNode.text.value,
                    ToNodeType = (int)context.WorkFlow.ActivityNodeType,
                };
                await _db.Insertable(transitionHistory).ExecuteCommandAsync();

                if (makerUsers.Count == 1)//当前人是最后一人
                {
                    var line = context.WorkFlow.Edges[Guid.Parse(dbflowinstance.ActivityId)][0];
                    var nextNode = context.WorkFlow.Nodes[line.targetNodeId];

                    var transitionHistoryEnd = new WF_WorkFlow_Transition_History
                    {
                        transitionId = Guid.NewGuid().ToString(),
                        InstanceId = dbflowinstance.InstanceId,
                        FromNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                        FromNodeName = context.WorkFlow.ActivityNode.text.value,
                        FromNodeType = (int)context.WorkFlow.ActivityNodeType,
                        ToNodeId = nextNode.Id.ToString(),
                        ToNodeType = (int)nextNode.NodeType(),
                        ToNodeName = nextNode.text.value,
                        TransitionState = (int)WorkFlowTransitionStateType.Normal,
                        IsFinish = nextNode.NodeType().ToIsFinish(),
                        CreateUserId = long.Parse(model.UserId),
                        CreateUserName = model.UserName
                    };
                    await _db.Insertable(transitionHistoryEnd).ExecuteCommandAsync();
                    //await databaseFixture.Db.WorkflowTransitionHistory.InsertAsync(transitionHistoryEnd, tran);

                    //修改流程实例
                    dbflowinstance.PreviousId = dbflowinstance.ActivityId;
                    dbflowinstance.ActivityId = nextNode.Id.ToString();
                    dbflowinstance.ActivityName = nextNode.text.value;
                    dbflowinstance.ActivityType = (int)nextNode.NodeType();
                    dbflowinstance.MakerList = nextNode.NodeType() == WorkFlowInstanceNodeType.End
                        ? ""
                        : await this.GetMakerListAsync(nextNode, model.UserId, model.OptionParams);
                    //计算票数
                    var result = await CalcVotes(dbflowinstance.InstanceId, dbflowinstance.PreviousId, nextNode, context.WorkFlow.ActivityNode.properties.ChatData.ParallelCalcType);
                    dbflowinstance.IsFinish = (int)result;
                    await _db.Updateable<WF_WorkFlow_Instance>(dbflowinstance).ExecuteCommandAsync();
                }
                else
                {
                    makerUsers.Remove(model.UserId);
                    dbflowinstance.MakerList = string.Join(",", makerUsers) + ",";
                    await _db.Updateable<WF_WorkFlow_Instance>(dbflowinstance).ExecuteCommandAsync();

                }
                #endregion 
            }
            //串行逻辑
            else
            {
                #region 串行逻辑              
                var users = context.WorkFlow.ActivityNode.properties.users.Split(',');
                int index = 0;
                for (int i = 0; i < users.Length; i++)
                {
                    if (users[i] == model.UserId)
                    {
                        index = i + 1;
                        break;
                    }
                }
                string nextUserId = users.Length == index ? "" : users[index];
                var transitionHistory = new WF_WorkFlow_Transition_History
                {
                    transitionId = Guid.NewGuid().ToString(),
                    InstanceId = dbflowinstance.InstanceId,
                    CreateUserId = long.Parse(model.UserId),
                    CreateUserName = model.UserName,
                    TransitionState = (int)WorkFlowTransitionStateType.Normal,
                    IsFinish = (int)flowInstanceStatus,
                    FromNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                    FromNodeName = context.WorkFlow.ActivityNode.text.value,
                    FromNodeType = (int)context.WorkFlow.ActivityNodeType,
                    ToNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                    ToNodeName = context.WorkFlow.ActivityNode.text.value,
                    ToNodeType = (int)context.WorkFlow.ActivityNodeType,
                };
                await _db.Insertable(transitionHistory).ExecuteCommandAsync();

                if (users.Length == index)//最后一个人时候
                {
                    var line = context.WorkFlow.Edges[Guid.Parse(dbflowinstance.ActivityId)][0];
                    var nextNode = context.WorkFlow.Nodes[line.sourceNodeId];
                    var transitionHistoryEnd = new WF_WorkFlow_Transition_History
                    {
                        transitionId = Guid.NewGuid().ToString(),
                        InstanceId = dbflowinstance.InstanceId,
                        FromNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                        FromNodeName = context.WorkFlow.ActivityNode.text.value,
                        FromNodeType = (int)context.WorkFlow.ActivityNodeType,
                        ToNodeId = nextNode.Id.ToString(),
                        ToNodeType = (int)nextNode.NodeType(),
                        ToNodeName = nextNode.text.value,
                        TransitionState = (int)WorkFlowTransitionStateType.Normal,
                        IsFinish = nextNode.NodeType().ToIsFinish(),
                        CreateUserId = long.Parse(model.UserId),
                        CreateUserName = model.UserName
                    };
                    await _db.Insertable(transitionHistoryEnd).ExecuteCommandAsync();
                    //await databaseFixture.Db.WorkflowTransitionHistory.InsertAsync(transitionHistoryEnd, tran);
                    //修改流程实例
                    dbflowinstance.PreviousId = dbflowinstance.ActivityId;
                    dbflowinstance.ActivityId = nextNode.Id.ToString();
                    dbflowinstance.ActivityName = nextNode.text.value;
                    dbflowinstance.ActivityType = (int)nextNode.NodeType();
                    dbflowinstance.MakerList = nextNode.NodeType() == WorkFlowInstanceNodeType.End
                        ? ""
                        : await this.GetMakerListAsync(nextNode, model.UserId, model.OptionParams);
                    //计算票数
                    var result = await CalcVotes(dbflowinstance.InstanceId, dbflowinstance.PreviousId, nextNode, context.WorkFlow.ActivityNode.properties.ChatData.ParallelCalcType);
                    dbflowinstance.IsFinish = (int)result;
                    await _db.Updateable<WF_WorkFlow_Instance>(dbflowinstance).ExecuteCommandAsync();
                }
                #endregion
            }
        }

        /// <summary>
        /// 下个节点是会签逻辑
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="context"></param>
        /// <param name="dbflowinstance"></param>
        /// <param name="flowInstanceStatus"></param>
        /// <returns></returns>
        private async Task NextChatLogic(MsWorkFlowContext context, WF_WorkFlow_Instance dbflowinstance, WorkFlowInstanceStatus flowInstanceStatus)
        {
            dbflowinstance.IsFinish = (int)flowInstanceStatus;
            if (context.WorkFlow.NextNode.properties.ChatData.ChatType == ChatType.Parallel)
            {
                //并行会签
                dbflowinstance.MakerList = context.WorkFlow.NextNode.properties.users;
            }
            else
            {
                //串行会签
                dbflowinstance.MakerList = context.WorkFlow.NextNode.properties.users.Split(',')[0];
            }
            dbflowinstance.PreviousId = dbflowinstance.ActivityId;
            dbflowinstance.ActivityId = context.WorkFlow.NextNodeId.ToString();
            dbflowinstance.ActivityName = context.WorkFlow.NextNode.text.value;
            dbflowinstance.ActivityType = (int)context.WorkFlow.NextNodeType;
            await _db.Updateable(dbflowinstance).ExecuteCommandAsync();
        }

        /// <summary>
        /// 重新提交流程
        /// 实例只有一次
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected async Task<bool> ProcessTransitionReSubmitAsync(WorkFlowProcessTransition model)
        {
            var result = await _db.Ado.UseTranAsync(async () =>
            {
                var dbflow = await _db.Queryable<WF_WorkFlow>().FirstAsync(a => a.FlowId == model.FlowId.ToString() && a.IsDeleted == 0);
                MsWorkFlowContext context = new MsWorkFlowContext(new WorkFlow.Core.WorkFlow
                {
                    FlowId = Guid.Parse(dbflow.FlowId),
                    FlowJson = dbflow.FlowContent,
                    ActivityNodeId = default(Guid)
                });
                #region 改变之前实例
                var dbinstance = await _db.Queryable<WF_WorkFlow_Instance>().Where(a => a.InstanceId == model.InstanceId.ToString()).FirstAsync();
                dbinstance.ActivityId = context.WorkFlow.NextNodeId.ToString();
                dbinstance.ActivityName = context.WorkFlow.NextNode.text.value;
                dbinstance.ActivityType = (int)context.WorkFlow.NextNodeType;
                dbinstance.PreviousId = context.WorkFlow.ActivityNodeId.ToString();
                dbinstance.MakerList = await this.GetMakerListAsync(context.WorkFlow.Nodes[context.WorkFlow.NextNodeId], model.UserId, model.OptionParams);
                dbinstance.IsFinish = context.WorkFlow.NextNodeType.ToIsFinish();
                dbinstance.Status = (int)WorkFlowStatus.Running;
                await _db.Updateable(dbinstance).ExecuteCommandAsync();
                #endregion

                #region 创建流程操作记录

                var operationHistory = new WF_WorkFlow_Operation_History
                {
                    OperationId = Guid.NewGuid().ToString(),
                    InstanceId = dbinstance.InstanceId,
                    CreateUserId = long.Parse(model.UserId),
                    CreateUserName = model.UserName,
                    Content = "流程重新提交",
                    NodeName = context.WorkFlow.ActivityNode.text.value,
                    NodeId = context.WorkFlow.ActivityNodeId.ToString(),
                    TransitionType = (int)WorkFlowMenu.Submit
                };
                await _db.Insertable<WF_WorkFlow_Operation_History>(operationHistory).ExecuteCommandAsync();
                #endregion

                #region 创建流程流转记录

                var transitionHistory = new WF_WorkFlow_Transition_History
                {
                    transitionId = Guid.NewGuid().ToString(),
                    InstanceId = dbinstance.InstanceId,
                    FromNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                    FromNodeType = (int)context.WorkFlow.ActivityNodeType,
                    FromNodeName = context.WorkFlow.ActivityNode.text.value,
                    ToNodeId = context.WorkFlow.NextNodeId.ToString(),
                    ToNodeType = (int)context.WorkFlow.NextNodeType,
                    ToNodeName = context.WorkFlow.NextNode.text.value,
                    CreateUserId = long.Parse(model.UserId),
                    CreateUserName = model.UserName,
                    TransitionState = (int)WorkFlowTransitionStateType.Normal,
                    IsFinish = context.WorkFlow.NextNodeType.ToIsFinish(),
                };
                await _db.Insertable(transitionHistory).ExecuteCommandAsync();

                #endregion

                //改变表单状态
                await FlowStatusChangePublisher(model.StatusChange, WorkFlowStatus.Running);
            });
            return result.IsSuccess;
        }


        /// <summary>
        /// 获取确定最终要执行的唯一节点
        /// </summary>
        /// <param name="nextLines"></param>
        /// <param name="model"></param>
        /// <param name="createUserId">流程发起人</param>
        /// <returns></returns>
        private async Task<Guid?> GetFinalNodeId(List<WorkFlowEdge> nextLines, double? conValue = null)
        {
            if (nextLines.Count > 1 && conValue == null)
            {
                throw new Exception("流程设计比较值未设定！");
            }
            //如果只有一条线，就返回这条线的目标NodeId
            if (nextLines.Count == 1)
            {
                return nextLines[0].targetNodeId;
            }
            Guid? finalid = null;


            foreach (var line in nextLines)
            {
                bool isFind = false;
                //判断符号
                string conditional = line.properties.conditional;//线上设置的条件判断
                double cv = line.properties.conditionalValue;//线路上设置的条件值
                switch (conditional)
                {
                    case "=":
                        isFind = (conValue == cv);
                        break;
                    case ">": isFind = (conValue > cv); break;
                    case ">=": isFind = (conValue >= cv); break;
                    case "<": isFind = (conValue < cv); break;
                    case "<=": isFind = (conValue <= cv); break;
                }
                if (isFind)
                {
                    finalid = line.targetNodeId;
                    break;
                }
            }
            return finalid;

        }

        #region 同意操作
        /// <summary>
        /// 同意操作
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> WorkFlowAgreeAsync(WorkFlowProcessTransition model)
        {
            var result = await _db.Ado.UseTranAsync(async () =>
            {
                WorkFlowStatus publishFlowStatus = WorkFlowStatus.Running;
                var dbflowinstance = await _db.Queryable<WF_WorkFlow_Instance>().FirstAsync(a => a.InstanceId == model.InstanceId.ToString());
                if (dbflowinstance.IsFinish == (int)WorkFlowInstanceStatus.Finish)
                {
                    return;
                }
                var userInfo = await _db.Queryable<SysUser>().FirstAsync(a => a.Id.ToString() == model.UserId && a.IsDeleted == 0);
                model.UserName = userInfo.UserName;
                MsWorkFlowContext context = new MsWorkFlowContext(new WorkFlow.Core.WorkFlow
                {
                    FlowId = model.FlowId,
                    FlowJson = dbflowinstance.FlowContent,
                    ActivityNodeId = Guid.Parse(dbflowinstance.ActivityId),
                    PreviousId = Guid.Parse(dbflowinstance.PreviousId)
                });
                //正常节点
                if (context.WorkFlow.ActivityNode.NodeType() == WorkFlowInstanceNodeType.Normal)
                {
                    if (context.IsMultipleNextNode())
                    {
                        var nextLines = context.GetLinesForTo(context.WorkFlow.ActivityNodeId);
                        /*
                         * 多条连线条件必须都存在情况判断
                         * 注：一个节点下多个连线，则连线必须有SQL判断
                         * **/

                        bool isOk = nextLines.Any(m => m.properties == null || string.IsNullOrEmpty(m.properties.conditional) || m.properties.conditionalValue == null);
                        if (isOk)
                        {
                            throw new Exception("流程线路条件判断设置出错，请检查！");
                        }
                        else
                        {
                            //获取确定最终要执行的唯一节点
                            Guid? finalid = await GetFinalNodeId(nextLines, Convert.ToDouble(model.ComValue));
                            WorkFlowNode reallynode = context.WorkFlow.Nodes[finalid.Value];
                            dbflowinstance.IsFinish = reallynode.NodeType().ToIsFinish();
                            if (reallynode.NodeType() == WorkFlowInstanceNodeType.End)
                            {
                                dbflowinstance.Status = (int)WorkFlowStatus.IsFinish;
                            }
                            else
                            {
                                dbflowinstance.Status = (int)WorkFlowStatus.Running;
                            }
                            dbflowinstance.ActivityId = reallynode.Id.ToString();
                            dbflowinstance.ActivityName = reallynode.text.value;
                            dbflowinstance.ActivityType = (int)reallynode.NodeType();
                            dbflowinstance.ModifyDate = DateTime.Now;
                            dbflowinstance.MakerList = reallynode.NodeType() == WorkFlowInstanceNodeType.End ? "" : await this.GetMakerListAsync(reallynode, model.UserId, model.OptionParams);
                            //await databaseFixture.Db.WorkflowInstance.UpdateAsync(dbflowinstance, tran);
                            await _db.Updateable(dbflowinstance).ExecuteCommandAsync();

                            //流程结束情况
                            if ((int)WorkFlowInstanceStatus.Finish == dbflowinstance.IsFinish)
                            {
                                publishFlowStatus = WorkFlowStatus.IsFinish;
                            }

                            #region 添加流转记录

                            WF_WorkFlow_Transition_History transitionHistory = new WF_WorkFlow_Transition_History
                            {
                                transitionId = Guid.NewGuid().ToString(),
                                InstanceId = dbflowinstance.InstanceId,
                                FromNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                                FromNodeName = context.WorkFlow.ActivityNode.text.value,
                                FromNodeType = (int)context.WorkFlow.ActivityNodeType,
                                ToNodeId = reallynode.Id.ToString(),
                                ToNodeType = (int)reallynode.NodeType(),
                                ToNodeName = reallynode.text.value,
                                TransitionState = (int)WorkFlowTransitionStateType.Normal,
                                IsFinish = reallynode.NodeType().ToIsFinish(),
                                CreateUserId = long.Parse(model.UserId),
                                CreateUserName = model.UserName
                            };
                            await _db.Insertable<WF_WorkFlow_Transition_History>(transitionHistory).ExecuteCommandAsync();
                            // await databaseFixture.Db.WorkflowTransitionHistory.InsertAsync(transitionHistory, tran);
                            #endregion

                            #region 通知节点信息添加

                            var viewNodes = context.GetNextNodes(null, WorkFlowInstanceNodeType.ViewNode);
                            await AddFlowNotice(viewNodes, dbflowinstance.CreateUserId.ToString(), model);

                            #endregion
                        }
                    }
                    else
                    {
                        //下个节点不是会签节点
                        if (context.WorkFlow.NextNode.NodeType() != WorkFlowInstanceNodeType.ChatNode)
                        {
                            //修改流程实例
                            dbflowinstance.PreviousId = dbflowinstance.ActivityId;
                            dbflowinstance.ActivityId = context.WorkFlow.NextNodeId.ToString();
                            dbflowinstance.ActivityName = context.WorkFlow.NextNode.text.value;
                            dbflowinstance.ActivityType = (int)context.WorkFlow.NextNodeType;
                            dbflowinstance.ModifyDate = DateTime.Now;
                            dbflowinstance.MakerList = context.WorkFlow.NextNodeType == WorkFlowInstanceNodeType.End ? "" :
                            await this.GetMakerListAsync(context.WorkFlow.NextNode, model.UserId, model.OptionParams);

                            dbflowinstance.IsFinish = context.WorkFlow.NextNodeType.ToIsFinish();

                            if (context.WorkFlow.NextNodeType == WorkFlowInstanceNodeType.End)
                            {
                                dbflowinstance.Status = (int)WorkFlowStatus.IsFinish;
                            }
                            else
                            {
                                dbflowinstance.Status = (int)WorkFlowStatus.Running;
                            }
                            await _db.Updateable(dbflowinstance).ExecuteCommandAsync();

                            //流程结束情况
                            if ((int)WorkFlowInstanceStatus.Finish == dbflowinstance.IsFinish)
                            {
                                publishFlowStatus = WorkFlowStatus.IsFinish;
                            }

                            #region 通知节点信息添加

                            var viewNodes = context.GetNextNodes(null, WorkFlowInstanceNodeType.ViewNode);
                            await AddFlowNotice(viewNodes, dbflowinstance.CreateUserId.ToString(), model);

                            #endregion
                        }
                        else
                        {
                            throw new Exception("当前不支持会签功能");
                            //await NextChatLogic(tran, context, dbflowinstance, WorkFlowInstanceStatus.Running);
                        }

                        #region 添加流转记录

                        WF_WorkFlow_Transition_History transitionHistory = new WF_WorkFlow_Transition_History
                        {
                            transitionId = Guid.NewGuid().ToString(),
                            InstanceId = dbflowinstance.InstanceId,
                            FromNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                            FromNodeName = context.WorkFlow.ActivityNode.text.value,
                            FromNodeType = (int)context.WorkFlow.ActivityNodeType,
                            ToNodeId = context.WorkFlow.NextNodeId.ToString(),
                            ToNodeType = (int)context.WorkFlow.NextNodeType,
                            ToNodeName = context.WorkFlow.NextNode.text.value,
                            TransitionState = (int)WorkFlowTransitionStateType.Normal,
                            IsFinish = context.WorkFlow.NextNodeType.ToIsFinish(),
                            CreateUserId = long.Parse(model.UserId),
                            CreateUserName = model.UserName
                        };
                        await _db.Insertable<WF_WorkFlow_Transition_History>(transitionHistory).ExecuteCommandAsync();                        

                        #endregion
                    }
                }
                else
                {
                    throw new Exception("当前只支持正常节点功能");
                    //await ChatLogic(tran, context, dbflowinstance, model, WorkFlowInstanceStatus.Running);
                }

                #region 添加操作记录

                WF_WorkFlow_Operation_History operationHistory = new WF_WorkFlow_Operation_History
                {
                    OperationId = Guid.NewGuid().ToString(),
                    InstanceId = dbflowinstance.InstanceId,
                    CreateUserId = long.Parse(model.UserId),
                    CreateUserName = model.UserName,
                    Content = model.ProcessContent,
                    NodeId = context.WorkFlow.ActivityNodeId.ToString(),
                    NodeName = context.WorkFlow.ActivityNode.text.value,
                    TransitionType = (int)WorkFlowMenu.Agree
                };
                //await databaseFixture.Db.WorkflowOperationHistory.InsertAsync(operationHistory, tran);
                await _db.Insertable<WF_WorkFlow_Operation_History>(operationHistory).ExecuteCommandAsync();
                #endregion

                await FlowStatusChangePublisher(model.StatusChange, publishFlowStatus);
            }
            );
            return result.IsSuccess;

        }
        #endregion

        #region 通知
        /// <summary>
        /// 通知
        /// </summary>
        /// <param name="viewNodes"></param>
        /// <param name="createuserid"></param>
        /// <param name="model"></param>
        /// <param name="tran"></param>
        /// <returns></returns>
        private async Task AddFlowNotice(List<WorkFlowNode> viewNodes, string createuserid, WorkFlowProcessTransition model)
        {
            if (viewNodes.Any())
            {
                Dictionary<string, WorkFlowNode> dic = new Dictionary<string, WorkFlowNode>();
                foreach (var item in viewNodes)
                {
                    string makerStr = "";
                    //if (item.SetInfo.NodeDesignate == FlowNodeSetInfo.CREATEUSER)
                    //{
                    //    makerStr = createuserid + ",";
                    //}
                    //else
                    //{
                    makerStr = await this.GetMakerListAsync(item, model.UserId, model.OptionParams);
                    //}
                    if (makerStr.IsNotNullOrEmpty() && makerStr != "0")
                    {
                        string[] makerlist = makerStr.Split(',');
                        foreach (var viewuserid in makerlist.Where(m => !string.IsNullOrEmpty(m)))
                        {
                            if (!dic.ContainsKey(viewuserid))
                            {
                                dic.Add(viewuserid, item);
                            }
                        }
                    }
                }

                List<WF_WorkFlow_Notice> notices = dic.Select(m => new WF_WorkFlow_Notice
                {
                    //Id = Guid.NewGuid().ToString(),
                    IsRead = 0,
                    Maker = m.Key,
                    NodeId = m.Value.Id.ToString(),
                    NodeName = m.Value.text.value,
                    Status = 1,
                    IsTransition = 1,
                    InstanceId = model.InstanceId.ToString()
                }).ToList();
                if (notices.Any())
                {
                    await _db.Insertable<WF_WorkFlow_Notice>(notices).ExecuteCommandAsync();
                }
            }
        }
        #endregion

        #region 不同意
        /// <summary>
        /// 不同意
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> WorkFlowDeprecateAsync(WorkFlowProcessTransition model)
        {
            var result = await _db.Ado.UseTranAsync(async () =>
            {
                WorkFlowStatus publishFlowStatus = WorkFlowStatus.Running;
                var dbflowinstance = await _db.Queryable<WF_WorkFlow_Instance>().FirstAsync(a => a.InstanceId == model.InstanceId.ToString());
                if (dbflowinstance.IsFinish == (int)WorkFlowInstanceStatus.Finish)
                {
                    return;
                }
                var userInfo = await _db.Queryable<SysUser>().FirstAsync(a => a.Id.ToString() == model.UserId && a.IsDeleted == 0);
                model.UserName = userInfo.UserName;
                MsWorkFlowContext context = new MsWorkFlowContext(new WorkFlow.Core.WorkFlow
                {
                    FlowId = model.FlowId,
                    FlowJson = dbflowinstance.FlowContent,
                    ActivityNodeId = Guid.Parse(dbflowinstance.ActivityId),
                    PreviousId = Guid.Parse(dbflowinstance.PreviousId)
                });
                //会签
                if (context.WorkFlow.ActivityNode.NodeType() == WorkFlowInstanceNodeType.ChatNode)
                {
                    await ChatLogic(context, dbflowinstance, model, WorkFlowInstanceStatus.Running);
                }
                else
                {
                    //流程不同意节点判断
                    dbflowinstance.MakerList = "";
                    dbflowinstance.IsFinish = 0;
                    dbflowinstance.Status = (int)WorkFlowStatus.Deprecate;
                    dbflowinstance.PreviousId = dbflowinstance.ActivityId;
                    dbflowinstance.ActivityId = context.WorkFlow.NextNodeId.ToString();
                    dbflowinstance.ModifyDate = DateTime.Now;
                    await _db.Updateable(dbflowinstance).ExecuteCommandAsync();

                    #region 流转记录

                    WF_WorkFlow_Transition_History transitionHistory = new WF_WorkFlow_Transition_History
                    {
                        transitionId = Guid.NewGuid().ToString(),
                        InstanceId = dbflowinstance.InstanceId,
                        TransitionState = (int)WorkFlowTransitionStateType.Reject,
                        IsFinish = (int)WorkFlowInstanceStatus.Running,
                        CreateUserId = long.Parse(model.UserId),
                        CreateUserName = model.UserName,
                        FromNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                        FromNodeName = context.WorkFlow.ActivityNode.text.value,
                        FromNodeType = (int)context.WorkFlow.ActivityNodeType,
                        ToNodeId = context.WorkFlow.NextNodeId.ToString(),
                        ToNodeType = (int)context.WorkFlow.NextNodeType,
                        ToNodeName = context.WorkFlow.NextNode.text.value
                    };
                    await _db.Insertable(transitionHistory).ExecuteCommandAsync();
                    #endregion
                }

                #region 操作历史

                WF_WorkFlow_Operation_History operationHistory = new WF_WorkFlow_Operation_History
                {
                    OperationId = Guid.NewGuid().ToString(),
                    InstanceId = dbflowinstance.InstanceId,
                    CreateUserId = long.Parse(model.UserId),
                    CreateUserName = model.UserName,
                    Content = model.ProcessContent,
                    NodeName = context.WorkFlow.ActivityNode.text.value,
                    NodeId = context.WorkFlow.ActivityNodeId.ToString(),
                    TransitionType = (int)WorkFlowMenu.Deprecate
                };
                await _db.Insertable(operationHistory).ExecuteCommandAsync();
                #endregion

                await FlowStatusChangePublisher(model.StatusChange, WorkFlowStatus.Deprecate);

            });
            return result.IsSuccess;
        }
        #endregion

        #region 流程退回
        /// <summary>
        /// 流程退回
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> WorkFlowBackAsync(WorkFlowProcessTransition model)
        {
            var result = await _db.Ado.UseTranAsync(async () =>
            {
                //WorkFlowStatus publishFlowStatus = WorkFlowStatus.Running;
                var dbflowinstance = await _db.Queryable<WF_WorkFlow_Instance>().FirstAsync(a => a.InstanceId == model.InstanceId.ToString());
                if (dbflowinstance.IsFinish == (int)WorkFlowInstanceStatus.Finish)
                {
                    return;
                }
                MsWorkFlowContext context = new MsWorkFlowContext(new WorkFlow.Core.WorkFlow
                {
                    FlowId = model.FlowId,
                    FlowJson = dbflowinstance.FlowContent,
                    ActivityNodeId = Guid.Parse(dbflowinstance.ActivityId),
                    PreviousId = Guid.Parse(dbflowinstance.PreviousId)
                });
                var userInfo = await _db.Queryable<SysUser>().FirstAsync(a => a.Id.ToString() == model.UserId && a.IsDeleted == 0);
                model.UserName = userInfo.UserName;
                if (context.WorkFlow.ActivityNodeType == WorkFlowInstanceNodeType.Normal || context.WorkFlow.ActivityNodeType == WorkFlowInstanceNodeType.Start)
                {
                    var rejectNodeId = context.RejectNode(model.NodeRejectType.Value, model.RejectNodeId);
                    var rejectNode = context.WorkFlow.Nodes[rejectNodeId];
                    dbflowinstance.PreviousId = dbflowinstance.ActivityId;
                    dbflowinstance.ActivityId = rejectNodeId.ToString();
                    dbflowinstance.ActivityName = rejectNode.text.value;
                    dbflowinstance.ActivityType = (int)rejectNode.NodeType();
                    dbflowinstance.ModifyDate = DateTime.Now;
                    if (rejectNode.NodeType() == WorkFlowInstanceNodeType.Start)//开始节点时候
                    {
                        dbflowinstance.MakerList = dbflowinstance.CreateUserId + ",";
                    }
                    else
                    {
                        dbflowinstance.MakerList = rejectNode.NodeType() == WorkFlowInstanceNodeType.End
                            ? ""
                            : await this.GetMakerListAsync(rejectNode, dbflowinstance.CreateUserId.ToString(), model.OptionParams);
                    }
                    dbflowinstance.IsFinish = rejectNode.NodeType().ToIsFinish();
                    dbflowinstance.Status = (int)WorkFlowStatus.Back;
                    await _db.Updateable(dbflowinstance).ExecuteCommandAsync();


                    #region 流转记录

                    var transitionHistory = new WF_WorkFlow_Transition_History
                    {
                        transitionId = Guid.NewGuid().ToString(),
                        InstanceId = dbflowinstance.InstanceId,
                        CreateUserId = long.Parse(model.UserId),
                        CreateUserName = model.UserName,
                        IsFinish = (int)WorkFlowInstanceStatus.Running,
                        TransitionState = (int)WorkFlowTransitionStateType.Reject,
                        FromNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                        FromNodeType = (int)context.WorkFlow.ActivityNodeType,
                        FromNodeName = context.WorkFlow.ActivityNode.text.value,
                        ToNodeId = rejectNodeId.ToString(),
                        ToNodeType = (int)rejectNode.NodeType(),
                        ToNodeName = rejectNode.text.value
                    };
                    await _db.Insertable(transitionHistory).ExecuteCommandAsync();
                    #endregion

                    #region 操作记录

                    var operationHistory = new WF_WorkFlow_Operation_History
                    {
                        OperationId = Guid.NewGuid().ToString(),
                        InstanceId = dbflowinstance.InstanceId,
                        CreateUserId = long.Parse(model.UserId),
                        CreateUserName = model.UserName,
                        Content = model.ProcessContent,
                        NodeName = context.WorkFlow.ActivityNode.text.value,
                        TransitionType = (int)WorkFlowMenu.Back,
                        NodeId = context.WorkFlow.ActivityNodeId.ToString()
                    };
                    await _db.Insertable(operationHistory).ExecuteCommandAsync();

                    #endregion
                }
                else
                {
                    return;// WorkFlowResult.Error("当前节点为会签节点，不可退回！");
                }
                await FlowStatusChangePublisher(model.StatusChange, WorkFlowStatus.Back);

            });
            return result.IsSuccess;
        }
        #endregion

        #region 流程撤回
        /// <summary>
        /// 流程撤回
        /// 刚开始提交，下一个节点未审批情况，流程发起人可以终止
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> WorkFlowWithdrawAsync(WorkFlowProcessTransition model)
        {
            var result = await _db.Ado.UseTranAsync(async () =>
            {
                //WorkFlowStatus publishFlowStatus = WorkFlowStatus.Running;
                var dbflowinstance = await _db.Queryable<WF_WorkFlow_Instance>().FirstAsync(a => a.InstanceId == model.InstanceId.ToString());
                //删除流程操作记录
                var dboperationHistory = await _db.Queryable<WF_WorkFlow_Operation_History>().Where(m => m.InstanceId == model.InstanceId.ToString()).ToListAsync();
                await _db.Deleteable(dboperationHistory).ExecuteCommandAsync();
                //删除流程流转记录
                var dbtransitionHistory = await _db.Queryable<WF_WorkFlow_Operation_History>().Where(m => m.InstanceId == model.InstanceId.ToString()).ToListAsync();
                await _db.Deleteable(dbtransitionHistory).ExecuteCommandAsync();

                //删除委托表信息
                var dbassigns = await _db.Queryable<WFWorkFlowAssign>().Where(m => m.InstanceId == model.InstanceId.ToString()).ToListAsync();
                await _db.Deleteable(dbassigns).ExecuteCommandAsync();

                //var dbinstanceForm = await databaseFixture.Db.WorkflowInstanceForm.FindAsync(m => m.InstanceId == dbflowinstance.InstanceId);
                //if ((WorkFlowFormType)dbinstanceForm.FormType == WorkFlowFormType.System)//定制表单
                //{
                //    //删除流程实例表单关联记录
                //    await databaseFixture.Db.WorkflowInstanceForm.DeleteAsync(dbinstanceForm, tran);
                //删除流程实例
                await _db.Deleteable(dbflowinstance).ExecuteCommandAsync();
                //await databaseFixture.Db.WorkflowInstance.DeleteAsync(dbflowinstance, tran);

                //改变表单状态
                await FlowStatusChangePublisher(model.StatusChange, WorkFlowStatus.Withdraw);
                //}
                //else
                //{
                //    //自定义表单流程实例修改
                //    dbflowinstance.IsFinish = null;
                //    dbflowinstance.Status = (int)WorkFlowStatus.UnSubmit;
                //    dbflowinstance.MakerList = null;
                //    dbflowinstance.ModifyDate = DateTime.Now;
                //  await  _db.Updateable(dbflowinstance).ExecuteCommandAsync();

                //}
                //if (dbflowinstance.IsFinish == (int)WorkFlowInstanceStatus.Finish)
                //{
                //    return;
                //}
                //MsWorkFlowContext context = new MsWorkFlowContext(new WorkFlow.Core.WorkFlow
                //{
                //    FlowId = model.FlowId,
                //    FlowJson = dbflowinstance.FlowContent,
                //    ActivityNodeId = Guid.Parse(dbflowinstance.ActivityId),
                //    PreviousId = Guid.Parse(dbflowinstance.PreviousId)
                //});
            });

            return result.IsSuccess;
        }

        #endregion

        #region 已阅操作
        /// <summary>
        /// 已阅操作
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected async Task<bool> ProcessTransitionViewAsync(WorkFlowProcessTransition model)
        {
            var result = await _db.Ado.UseTranAsync(async () =>
            {
                var dbnotices = await _db.Queryable<WF_WorkFlow_Notice>().Where(m => m.Maker == model.UserId && m.InstanceId == model.InstanceId.ToString()
                && m.IsTransition == 1 && m.IsRead == 0 && m.Status == 1).ToListAsync();

                dbnotices.ForEach(async item =>
                {
                    item.IsRead = 1;

                    #region 添加操作记录

                    WF_WorkFlow_Operation_History operationHistory = new WF_WorkFlow_Operation_History
                    {
                        OperationId = Guid.NewGuid().ToString(),
                        InstanceId = model.InstanceId.ToString(),
                        CreateUserId = long.Parse(model.UserId),
                        CreateUserName = model.UserName,
                        Content = "流程已阅",
                        NodeId = item.NodeId,
                        NodeName = item.NodeName,
                        TransitionType = (int)WorkFlowMenu.View
                    };
                    await _db.Insertable(operationHistory).ExecuteCommandAsync();
                    #endregion
                });
                await _db.Updateable(dbnotices.ToArray()).ExecuteCommandAsync();
            });

            return result.IsSuccess;

        }
        #endregion

        #region 流程委托
        /// <summary>
        /// 流程委托操作
        /// 将自己审批某个流程的权限赋予其他人，让其他用户代审批流程;
        /// 规则：A委托给B，A不能再审批且不能多次委托，B可再次委托给C，同理A
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected async Task<bool> ProcessTransitionAssignAsync(WorkFlowProcessTransition model)
        {
            var result = await _db.Ado.UseTranAsync(async () =>
            {
                //1、修改流程实例makerlist，替换成委托人
                var flowinstance = await _db.Queryable<WF_WorkFlow_Instance>().FirstAsync(a => a.InstanceId == model.InstanceId.ToString());
                string oldreplaceStr = model.UserId + ",";
                string newreplaceStr = model.Assign.AssignUserId + ",";
                var newmakerList = flowinstance.MakerList.Replace(oldreplaceStr, newreplaceStr);
                flowinstance.MakerList = newmakerList;
                await _db.Updateable<WF_WorkFlow_Instance>(flowinstance).ExecuteCommandAsync();
                MsWorkFlowContext context = new MsWorkFlowContext(new WorkFlow.Core.WorkFlow
                {
                    FlowId = model.FlowId,
                    FlowJson = flowinstance.FlowContent,
                    ActivityNodeId = Guid.Parse(flowinstance.ActivityId),
                    PreviousId = Guid.Parse(flowinstance.PreviousId)
                });
                //2.添加委托记录

                WFWorkFlowAssign workflowAssign = new WFWorkFlowAssign
                {
                    // Id = Guid.NewGuid(),
                    UserId = long.Parse(model.UserId),
                    UserName = model.UserName,
                    FlowId = model.FlowId.ToString(),
                    NodeId = context.WorkFlow.ActivityNodeId.ToString(),
                    NodeName = context.WorkFlow.ActivityNode.text.value,
                    InstanceId = model.InstanceId.ToString(),
                    CreateUserId = long.Parse(model.UserId),
                    AssignUserId = long.Parse(model.Assign.AssignUserId),
                    AssignUserName = model.Assign.AssignUserName,
                    Content = model.Assign.AssignContent
                };
                await _db.Insertable(workflowAssign).ExecuteCommandAsync();
                //3、添加操作记录（不添加流转，因为实际情况流程并没有运行到下一个节点）
                string operConent = $"用户【{workflowAssign.UserName}】将流程委托给【{workflowAssign.AssignUserName}】";
                if (operConent.IsNotNullOrEmpty())
                {
                    operConent += "<br/>请求委托意见：" + model.Assign.AssignContent;
                }

                WF_WorkFlow_Operation_History operationHistory = new WF_WorkFlow_Operation_History
                {
                    OperationId = Guid.NewGuid().ToString(),
                    InstanceId = flowinstance.InstanceId,
                    CreateUserId = long.Parse(model.UserId),
                    CreateUserName = model.UserName,
                    Content = operConent,
                    NodeId = context.WorkFlow.ActivityNodeId.ToString(),
                    NodeName = context.WorkFlow.ActivityNode.text.value,
                    TransitionType = (int)WorkFlowMenu.Assign
                };
                await _db.Insertable(operationHistory).ExecuteCommandAsync();

            });
            return result.IsSuccess;

        }
        #endregion

        #region 获取审批意见
        /// <summary>
        /// 获取审批意见
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IEnumerable<WF_WorkFlow_Operation_History>> GetFlowApprovalAsync(WorkFlowProcessTransition model)
        {
            var dbhistory = await _db.Queryable<WF_WorkFlow_Operation_History>()
                .Where(a => a.InstanceId == model.InstanceId.ToString()
                      && a.IsDeleted == 0).ToListAsync();
            var dbinstance = await _db.Queryable<WF_WorkFlow_Instance>().FirstAsync(a => a.InstanceId == model.InstanceId.ToString()
            && a.IsDeleted == 0);

            if (dbinstance.IsFinish == 1)//已经完成
            {
                List<WF_WorkFlow_Operation_History> list = new List<WF_WorkFlow_Operation_History>
                {
                    new WF_WorkFlow_Operation_History
                    {
                        NodeName = "结束",
                        TransitionType = null,
                        CreateUserName = "",
                        Content = "系统自动结束",
                        CreateDate=DateTime.Now
                        //CreateTime = dbhistory.OrderByDescending(m => m.CreateTime).Select(m => m.CreateTime).First() + 1
                    }
                };
                IEnumerable<WF_WorkFlow_Operation_History> result = dbhistory.Union(list);
                return result;

            }
            else
            {
                return dbhistory;
            }
        }
        #endregion

        #region 获取流程图信息
        /// <summary>
        /// 获取流程图信息
        /// </summary>
        /// <param name="instanceId">实例ID</param>
        /// <returns></returns>
        public async Task<WorkFlowImageDto> GetFlowImageAsync(string flowid, string? instanceId)
        {
            if (instanceId == null || instanceId == default(Guid).ToString())//未提交
            {
                var dbflow = await _db.Queryable<WF_WorkFlow>().FirstAsync(a => a.FlowId == flowid && a.IsDeleted == 0);
                return new WorkFlowImageDto
                {
                    FlowId = dbflow.FlowId,
                    FlowContent = dbflow.FlowContent,
                    InstanceId = default(Guid).ToString(),
                    CurrentNodeId = default(Guid).ToString()
                };
            }
            else
            {
                //提交
                var instance = await _db.Queryable<WF_WorkFlow_Instance>().FirstAsync(a => a.InstanceId == instanceId);
                return new WorkFlowImageDto
                {
                    FlowId = instance.FlowId,
                    InstanceId = instance.InstanceId,
                    CurrentNodeId = instance.ActivityId,
                    FlowContent = instance.FlowContent,
                };
            }

        }
        #endregion

        #endregion

        /// <summary>
        /// 流程催办
        /// </summary>
        /// <param name="urge"></param>
        /// <returns></returns>
        public async Task<bool> UrgeAsync(UrgeEdit urge)
        {

            WF_WorkFlow_Urge workflowUrge = new WF_WorkFlow_Urge
            {
                CreateUserId = urge.Sender,
                Sender = urge.Sender,
                InstanceId = urge.InstanceId,
                UrgeContent = urge.UrgeContent,
                UrgeType = urge.UrgeType
            };
            var instance = await _db.Queryable<WF_WorkFlow_Instance>().Where(a => a.InstanceId == urge.InstanceId).FirstAsync();
            workflowUrge.NodeId = instance.ActivityId;
            workflowUrge.NodeName = instance.ActivityName;
            workflowUrge.UrgeUser = instance.MakerList;
            int res = await _db.Insertable(workflowUrge).ExecuteCommandAsync();
            return res > 0;
        }


    }
}
