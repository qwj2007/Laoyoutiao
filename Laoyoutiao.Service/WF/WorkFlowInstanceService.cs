using AutoMapper;
using DotNetCore.CAP;
using Laoyoutiao.Caches;
using Laoyoutiao.Common;
using Laoyoutiao.IService.WF;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Dto.OA.Leave;
using Laoyoutiao.Models.Dto.WF;
using Laoyoutiao.Models.Dto.WF.Instance;
using Laoyoutiao.Models.Dto.WF.Urge;
using Laoyoutiao.Models.Entitys.OA;
using Laoyoutiao.Models.Entitys.Sys;
using Laoyoutiao.Models.Entitys.WF;
using Laoyoutiao.Models.Views;
using Laoyoutiao.WorkFlow.Core;
using Org.BouncyCastle.Utilities;
using System.Data;
using System.Security.Policy;
using static System.TimeZoneInfo;

namespace Laoyoutiao.Service.WF
{
    /// <summary>
    /// 工作流实例业务实现类
    /// 主要负责：创建实例、流程流转、审批、通知、催办等与流程实例相关的业务逻辑。
    /// 对外实现 IWorkFlowInstanceService。
    /// </summary>
    public class WorkFlowInstanceService : BaseService<WF_WorkFlow_Instance>, IWorkFlowInstanceService
    {
        private readonly IMapper _mapper;
        private readonly ICapPublisher _capPublisher;

        // Constants
        private const string ALL_USERS_MARKER = "0";
        private const string COMMA_SUFFIX = ",";
        private const int DEFAULT_PAGE_INDEX = 1;
        private const int DEFAULT_PAGE_SIZE = 20;

        /// <summary>
        /// 构造函数，注入 mapper、CAP 发布器和当前用户缓存
        /// </summary>
        public WorkFlowInstanceService(IMapper mapper, ICapPublisher capPublisher, CurrentUserCache cache)
            : base(mapper, cache)
        {
            _mapper = mapper;
            _capPublisher = capPublisher;
        }

        /// <summary>
        /// 分页查询流程实例视图
        /// 支持按 BusinessName 模糊查询，并且非管理员只查本人创建的实例。
        /// </summary>
        public override async Task<PageInfo> GetPagesAsync<TReq, TRes>(TReq req)
        {
            var request = req as WorkFlowInstanceReq ?? new WorkFlowInstanceReq
            {
                PageIndex = DEFAULT_PAGE_INDEX,
                PageSize = DEFAULT_PAGE_SIZE
            };

            var query = _db.Queryable<V_WorkFlow>()
                .WhereIF(!string.IsNullOrEmpty(request.BusinessName),
                    a => a.BusinessName.Contains(request.BusinessName))
                .WhereIF(request.LoginUserId != 1,
                    a => a.CreateUserId == request.LoginUserId);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var result = _mapper.Map<List<WorkFlowInstanceRes>>(items);
            FillFlowStatusNames(result);

            return new PageInfo
            {
                data = result,
                total = totalCount
            };
        }

        /// <summary>
        /// 为工作流实例列表填充 FlowStatusName（枚举描述）
        /// </summary>
        private void FillFlowStatusNames(List<WorkFlowInstanceRes> list)
        {
            if (list == null || !list.Any())
                return;

            foreach (var item in list)
            {
                item.FlowStatusName = EnumHelper.EnumToDescription<WorkFlowStatus>(item.FlowStatus);
            }
        }

        /// <summary>
        /// 根据节点配置获取审批人列表字符串（以逗号分隔，并以逗号结尾）
        /// 支持：指定用户、指定角色、全员（返回 "0"）等类型。
        /// </summary>
        private async Task<string> GetMakerListAsync(
            WorkFlowNode node,
            string userId,
            Dictionary<string, object> optionParams = null)
        {
            if (node.properties.IsNull() || string.IsNullOrWhiteSpace(node.properties.approveType))
            {
                throw new Exception("获取审批人失败，请检查是否配置审批人");
            }

            var approveType = node.properties.approveType.ToUpper();

            return approveType switch
            {
                ApproveType.SPECIAL_USER => FormatMakerList(string.Join(",", node.properties.users)),
                ApproveType.SPECIAL_ROLE => await GetRoleUsersAsync(node.properties.roles),
                ApproveType.ALL_USER => ALL_USERS_MARKER,
                _ => null
            };
        }

        /// <summary>
        /// 格式化审批人列表，确保以逗号结尾
        /// </summary>
        private static string FormatMakerList(string makerList)
        {
            return string.IsNullOrEmpty(makerList) ? makerList : makerList + COMMA_SUFFIX;
        }

        /// <summary>
        /// 根据角色ID列表获取对应用户ID列表
        /// </summary>
        private async Task<string> GetRoleUsersAsync(string roleIds)
        {
            if (string.IsNullOrEmpty(roleIds))
                return string.Empty;

            var roleIdList = roleIds.Split(',').Where(r => !string.IsNullOrEmpty(r)).ToList();

            var userIds = await _db.Queryable<SysUserRole>()
                .Where(a => roleIdList.Contains(a.RoleId.ToString()))
                .Select(a => a.UserId)
                .ToListAsync();

            return FormatMakerList(string.Join(",", userIds));
        }

        #region 我的待办事项
        /// <summary>
        /// 我的待办事项
        /// </summary>
        public async Task<PageInfo> GetUserTodoListAsync(WorkFlowInstanceReq req)
        {
            req ??= new WorkFlowInstanceReq
            {
                PageIndex = DEFAULT_PAGE_INDEX,
                PageSize = DEFAULT_PAGE_SIZE
            };

            var userMarker = req.LoginUserId.ToString() + COMMA_SUFFIX;

            var directTodoQuery = _db.Queryable<V_WorkFlow>()
                .Where(a => a.MakerList.Contains(userMarker));

            var noticeTodoQuery = _db.Queryable<V_WorkFlow>()
                .LeftJoin<WF_WorkFlow_Notice>((vf, wn) => vf.InstanceId == wn.InstanceId)
                .Where((vf, wn) => wn.IsDeleted == 0
                    && wn.IsRead == 0
                    && wn.IsTransition == 1
                    && wn.Status == 1
                    && wn.Maker == req.LoginUserId.ToString());

            var combinedQuery = _db.Union(directTodoQuery, noticeTodoQuery)
                .WhereIF(!string.IsNullOrEmpty(req.UserName),
                    a => a.CreateUserName.Contains(req.UserName))
                .WhereIF(!string.IsNullOrEmpty(req.FlowName),
                    a => a.BusinessName.Contains(req.FlowName))
                .WhereIF(!string.IsNullOrEmpty(req.BusinessName),
                    a => a.BusinessName.Contains(req.BusinessName));

            var totalCount = await combinedQuery.CountAsync();
            var items = await combinedQuery
                .Skip((req.PageIndex - 1) * req.PageSize)
                .Take(req.PageSize)
                .ToListAsync();

            var result = _mapper.Map<List<WorkFlowInstanceRes>>(items);
            FillFlowStatusNames(result);

            return new PageInfo
            {
                data = result,
                total = totalCount
            };
        }
        #endregion

        #region 流程实例创建

        /// <summary>
        /// 创建一个实例
        /// 注意事项：
        /// 1、流程开始节点不可添加任何条件分支（不符合逻辑，故人为规定）,即开始节点之后必须只能有一个任务节点，否则整个逻辑就错误了
        /// </summary>
        public async Task<bool> CreateInstanceAsync(WorkFlowProcessTransition model)
        {
            var userInfo = await GetUserInfoAsync(model.UserId);
            model.UserName = userInfo.UserName;

            var workflow = await GetWorkflowByMenuUrlAsync(model.url);
            var formId = workflow.FormId;

            var existingInstance = await FindExistingInstanceAsync(
                workflow.FlowId,
                formId.ToString(),
                model.Id.ToString(),
                model.StatusChange.TableName);

            var context = CreateWorkFlowContext(workflow);
            var makerList = await GetMakerListAsync(
                context.WorkFlow.Nodes[context.WorkFlow.NextNodeId],
                model.UserId.ToString());

            var result = await _db.Ado.UseTranAsync(async () =>
            {
                var instance = existingInstance ?? CreateNewInstance(workflow, model, userInfo, makerList, context);

                if (existingInstance == null)
                {
                    await _db.Insertable(instance).ExecuteCommandAsync();
                }
                else
                {
                    UpdateInstance(instance, workflow.FlowContent, model, makerList, context);
                    await _db.Updateable(instance).ExecuteCommandAsync();
                }

                await CreateOperationHistoryAsync(instance.InstanceId, userInfo, context, WorkFlowMenu.Submit);
                await CreateTransitionHistoryAsync(instance.InstanceId, userInfo, context, WorkFlowMenu.Submit);
                await PublishFlowStatusChangeAsync(model.StatusChange, WorkFlowStatus.Submit);
            });

            return result.IsSuccess;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        private async Task<SysUser> GetUserInfoAsync(string userId)
        {
            var user = await _db.Queryable<SysUser>()
                .FirstAsync(a => a.Id.ToString() == userId && a.IsDeleted == 0);

            if (user == null)
            {
                throw new ArgumentException("用户不存在", nameof(userId));
            }

            return user;
        }

        /// <summary>
        /// 根据菜单URL获取工作流配置
        /// </summary>
        private async Task<WF_WorkFlow> GetWorkflowByMenuUrlAsync(string menuUrl)
        {
            var menu = await _db.Queryable<Menus>()
                .FirstAsync(a => a.MenuUrl == menuUrl && a.IsDeleted == 0 && a.IsButton == 0);

            if (menu == null)
            {
                throw new ArgumentException("菜单的url未配置", nameof(menuUrl));
            }

            var workflow = await _db.Queryable<WF_WorkFlow>()
                .FirstAsync(a => a.FormId == menu.Id);

            if (workflow == null || string.IsNullOrEmpty(workflow.FlowContent))
            {
                throw new ArgumentException("未配置流程，请联系管理员", nameof(menuUrl));
            }

            return workflow;
        }

        /// <summary>
        /// 查找已存在的流程实例
        /// </summary>
        private async Task<WF_WorkFlow_Instance> FindExistingInstanceAsync(
            string flowId,
            string formId,
            string businessId,
            string businessTable)
        {
            return await _db.Queryable<WF_WorkFlow_Instance>()
                .FirstAsync(a => a.FlowId == flowId
                    && a.FormId == formId
                    && a.IsDeleted == 0
                    && a.BusinessId == businessId
                    && a.BusinessFromTable == businessTable);
        }

        /// <summary>
        /// 创建工作流上下文
        /// </summary>
        private static MsWorkFlowContext CreateWorkFlowContext(WF_WorkFlow workflow)
        {
            return new MsWorkFlowContext(new WorkFlow.Core.WorkFlow
            {
                FlowId = Guid.Parse(workflow.FlowId),
                FlowJson = workflow.FlowContent,
                ActivityNodeId = default
            });
        }

        /// <summary>
        /// 从流程实例创建工作流上下文
        /// </summary>
        private static MsWorkFlowContext CreateWorkFlowContextFromInstance(WF_WorkFlow_Instance instance)
        {
            return new MsWorkFlowContext(new WorkFlow.Core.WorkFlow
            {
                FlowId = Guid.Parse(instance.FlowId),
                FlowJson = instance.FlowContent,
                ActivityNodeId = default
            });
        }

        /// <summary>
        /// 创建新的流程实例
        /// </summary>
        private static WF_WorkFlow_Instance CreateNewInstance(
            WF_WorkFlow workflow,
            WorkFlowProcessTransition model,
            SysUser userInfo,
            string makerList,
            MsWorkFlowContext context)
        {
            return new WF_WorkFlow_Instance
            {
                InstanceId = Guid.NewGuid().ToString(),
                FlowId = workflow.FlowId,
                Code = DateTime.Now.Ticks + string.Empty.CreateNumberNonce(),
                ActivityId = context.WorkFlow.NextNodeId.ToString(),
                ActivityName = context.WorkFlow.NextNode.text.value,
                ActivityType = (int)context.WorkFlow.NextNodeType,
                PreviousId = context.WorkFlow.ActivityNodeId.ToString(),
                MakerList = makerList,
                CreateUserId = long.Parse(model.UserId),
                CreateUserName = model.UserName,
                FlowContent = workflow.FlowContent,
                IsFinish = context.WorkFlow.NextNodeType.ToIsFinish(),
                FlowStatus = (int)WorkFlowStatus.Running,
                Status = 1,
                FormId = workflow.FormId.ToString(),
                BusinessFromTable = model.StatusChange.TableName,
                BusinessName = model.BusinessName,
                BusinessId = model.Id.ToString(),
                BusinessCode = model.Code,
                ComValue = model.ComValue.ToString()
            };
        }

        /// <summary>
        /// 更新现有流程实例
        /// </summary>
        private static void UpdateInstance(
            WF_WorkFlow_Instance instance,
             string flowContent,
             WorkFlowProcessTransition model,
            string makerList,
            MsWorkFlowContext context)
        {
            instance.ActivityId = context.WorkFlow.NextNodeId.ToString();
            instance.ActivityName = context.WorkFlow.NextNode.text.value;
            instance.ActivityType = (int)context.WorkFlow.NextNodeType;
            instance.PreviousId = context.WorkFlow.ActivityNodeId.ToString();
            instance.MakerList = makerList;
            instance.FlowContent = flowContent;
            instance.IsFinish = context.WorkFlow.NextNodeType.ToIsFinish();
            instance.FlowStatus = instance.IsFinish == 1
                ? (int)WorkFlowStatus.IsFinish
                : (int)WorkFlowStatus.Running;
            instance.ModifyDate = DateTime.Now;
            instance.BusinessFromTable = model.StatusChange.TableName;
            instance.BusinessName = model.BusinessName;
            instance.BusinessId = model.Id.ToString();
            instance.BusinessCode = model.Code;
            instance.ComValue = model.ComValue.ToString();
        }

        /// <summary>
        /// 创建操作历史记录
        /// </summary>
        private async Task CreateOperationHistoryAsync(
            string instanceId,
            SysUser userInfo,
            MsWorkFlowContext context,
            WorkFlowMenu transitionType)
        {
            var operationHistory = new WF_WorkFlow_Operation_History
            {
                OperationId = Guid.NewGuid().ToString(),
                InstanceId = instanceId,
                CreateUserId = userInfo.Id,
                CreateUserName = userInfo.UserName,
                Content = GetOperationContent(transitionType),
                NodeName = context.WorkFlow.ActivityNode.text.value,
                NodeId = context.WorkFlow.ActivityNodeId.ToString(),
                TransitionType = (int)transitionType
            };


            await _db.Insertable(operationHistory).ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取操作内容描述
        /// </summary>
        private static string GetOperationContent(WorkFlowMenu menuType)
        {
            return menuType switch
            {
                WorkFlowMenu.Submit => "提交申请",
                WorkFlowMenu.ReSubmit => "流程重新提交",
                WorkFlowMenu.Agree => "同意",
                WorkFlowMenu.Deprecate => "不同意",
                WorkFlowMenu.Back => "退回",
                WorkFlowMenu.View => "流程已阅",
                WorkFlowMenu.Assign => "委托",
                _ => "操作"
            };
        }

        /// <summary>
        /// 创建流转历史记录
        /// </summary>
        private async Task CreateTransitionHistoryAsync(
            string instanceId,
            SysUser userInfo,
            MsWorkFlowContext context,
            WorkFlowMenu workFlowMenu,
            WorkFlowTransitionStateType state = WorkFlowTransitionStateType.Normal)
        {
            var transitionHistory = new WF_WorkFlow_Transition_History
            {
                transitionId = Guid.NewGuid().ToString(),
                InstanceId = instanceId,
                FromNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                FromNodeType = (int)context.WorkFlow.ActivityNodeType,
                FromNodeName = context.WorkFlow.ActivityNode.text.value,
                ToNodeId = context.WorkFlow.NextNodeId.ToString(),
                ToNodeType = (int)context.WorkFlow.NextNodeType,
                ToNodeName = context.WorkFlow.NextNode.text.value,
                CreateUserId = userInfo.Id,
                CreateUserName = userInfo.UserName,
                TransitionState = (int)state,
                IsFinish = context.WorkFlow.NextNodeType.ToIsFinish(),
                TransitionType= (int)workFlowMenu,

            };
            ////如果下一个节点是结束节点，就想操作历史记录中插入一条记录
            //if (transitionHistory.IsFinish == 1)
            //{
            //    var operationHistory = new WF_WorkFlow_Operation_History
            //    {
            //        OperationId = Guid.NewGuid().ToString(),
            //        InstanceId = instanceId,
            //        CreateUserId = userInfo.Id,
            //        CreateUserName = userInfo.UserName,
            //        Content = "结束",
            //        NodeName = context.WorkFlow.NextNode.text.value,
            //        NodeId = context.WorkFlow.NextNodeId.ToString(),
            //        TransitionType = (int)WorkFlowMenu.Agree
            //    };
            //    await _db.Insertable(operationHistory).ExecuteCommandAsync();
            //}

            await _db.Insertable(transitionHistory).ExecuteCommandAsync();
        }

        /// <summary>
        /// CAP发布订阅 - 流程状态变更
        /// </summary>
        private async Task PublishFlowStatusChangeAsync(
            WorkFlowStatusChange statusChange,
            WorkFlowStatus flowStatus)
        {
            if (statusChange == null)
                return;

            statusChange.Status = flowStatus;
            statusChange.FlowTime = DateTime.Now;

            await _capPublisher.PublishAsync(statusChange.TargetName, statusChange);
        }

        #endregion


        /// <summary>
        /// 获取执行过的节点
        /// </summary>
        private async Task<Dictionary<string,string>> GetExcuteNodesIds(string instanceId, string currentNodeId, int? isFinish = 1)
        {
            IEnumerable<WF_WorkFlow_Transition_History> validOperations = await getValidTransitionHistory(instanceId);
            Dictionary<string, string> nodes = new Dictionary<string, string>();
            foreach (var operation in validOperations)
            {
                if (!nodes.ContainsKey(operation.FromNodeId) && !string.IsNullOrEmpty(operation.FromNodeId))
                {
                    nodes.Add(operation.FromNodeId, operation.ToNodeId);
                }
                //如果流程没有完成，遇到回退节点是停止
                if (operation.IsFinish == 0 && operation.TransitionType == (int)WorkFlowMenu.Back)
                {
                    break;
                }
            }

            return nodes;
        }

        private async Task<IEnumerable<WF_WorkFlow_Transition_History>> getValidTransitionHistory(string instanceId)
        {
            var operationHistories = await _db.Queryable<WF_WorkFlow_Transition_History>()
            //var operationHistories = await _db.Queryable<WF_WorkFlow_Operation_History>()
                .Where(a => a.InstanceId == instanceId)
                .OrderBy(a => a.CreateDate)
                .ToListAsync();
            // 先定义需要的类型
            var types = new[]
            {
    (int)WorkFlowMenu.Agree,
    (int)WorkFlowMenu.Submit,
    (int)WorkFlowMenu.ReSubmit,
    (int)WorkFlowMenu.Deprecate,
    (int)WorkFlowMenu.Back

};
            // 用 Union 拼接
            var validOperations = types
                .Select(t => operationHistories.Where(m => m.TransitionType == t /*&& m.NodeId != currentNodeId*/))
                .Aggregate((a, b) => a.Union(b));
            return validOperations;
        }


        /// <summary>
        /// 获取执行过的节点
        /// </summary>
        public async Task<List<WorkFlowNode>> GetExcuteNodes(string instanceId, string currentNodeId, int? isFinish = 1)
        {
            IEnumerable<WF_WorkFlow_Transition_History> validOperations = await getValidTransitionHistory(instanceId);
            Dictionary<string,string> dic = await GetExcuteNodesIds(instanceId, currentNodeId, isFinish);

           
            var nodes = new List<WorkFlowNode>();
            foreach (var item in dic) {
                var SourceNode= validOperations.Where(a => a.FromNodeId == item.Key).FirstOrDefault();
                var endNode= validOperations.Where(a => a.ToNodeId == item.Value).FirstOrDefault();
                if (SourceNode != null)
                {
                    nodes.Add(new WorkFlowNode
                    {
                        Id = Guid.Parse(item.Key),
                        text = { value = SourceNode.FromNodeName },
                        statu = SourceNode.TransitionType.ToString()
                    });
                }
                if (endNode != null) {
                    nodes.Add(new WorkFlowNode {
                        Id = Guid.Parse(item.Value),
                        text = { value = endNode.FromNodeName },
                        statu = endNode.TransitionType.ToString()
                    });
                }
                
            }
            //foreach (var operation in validOperations)
            //{
               

            //    var fromNode = new WorkFlowNode
            //    {
            //        Id = Guid.Parse(operation.FromNodeId),
            //        text = { value = operation.FromNodeName },
            //        statu = operation.TransitionType.ToString()
            //    };
            //    nodes.Add(fromNode);
            //    //流程结束了
            //    if (operation.IsFinish == 1) {
            //        var endNode = new WorkFlowNode
            //        {
            //            Id = Guid.Parse(operation.ToNodeId),
            //            text = { value = operation.ToNodeName },
            //            statu = operation.TransitionType.ToString()
            //        };
            //        nodes.Add(endNode);
            //    }
              

            //    //如果流程没有完成，遇到回退节点是停止
            //    if (isFinish == 0)
            //    {
            //        // 遇到退回节点时停止
            //        if (operation.TransitionType == (int)WorkFlowMenu.Back)
            //            break;
            //    }

            //}

            return nodes;
        }

        /// <summary>
        /// 获取执行的连线
        /// </summary>
        public async Task<List<WorkFlowEdge>> GetExcuteEdges(string instanceId, string currentNodeId)
        {

            var flowInstance = await _db.Queryable<WF_WorkFlow_Instance>()
                .FirstAsync(a => a.InstanceId == instanceId && a.IsDeleted == 0);
            var executedNodes = await GetExcuteNodesIds(instanceId, currentNodeId, flowInstance.IsFinish);
            if (!executedNodes.Any())
                return new List<WorkFlowEdge>();
            if (flowInstance == null)
                return new List<WorkFlowEdge>();

            var context = CreateWorkFlowContextFromInstance(flowInstance);
            var edges = new List<WorkFlowEdge>();

            foreach (var node in executedNodes)
            {                
                edges.AddRange(context.GetLinesByIds(node.Key, node.Value));               
            }

            // 去重：只保留出现两次的连线（双向）
            //edges = edges.GroupBy(e => e.id)
            //   .Where(g => g.Count() == 2)
            //   .Select(g => g.First())
            //   .ToList();
            return edges;
        }

        /// <summary>
        /// 获取工作流进程信息
        /// </summary>
        public async Task<WorkFlowProcess> GetProcessAsync(WorkFlowProcess process)
        {
            var workflow = await _db.Queryable<WF_WorkFlow>()
                .FirstAsync(a => a.FlowId == process.FlowId.ToString());

            if (workflow == null)
            {
                throw new ArgumentException("工作流不存在", nameof(process.FlowId));
            }

            var model = new WorkFlowProcess
            {
                InstanceId = process.InstanceId,
                FlowId = workflow.FlowId,
                FlowName = workflow.FlowName,
                FormId = workflow.FormId.ToString(),
                FormType = WorkFlowFormType.System,
                FormContent = string.Empty,
                FormUrl = string.Empty,
                FormData = null
            };

            // 流程刚开始
            if (process.InstanceId == default(Guid).ToString())
            {
                SetInitialProcessMenus(model);
                model.FlowData = new WorkFlowProcessFlowData
                {
                    IsFinish = null,
                    Status = (int)WorkFlowStatus.UnSubmit
                };
                return model;
            }

            // 获取实例信息
            var instance = await _db.Queryable<WF_WorkFlow_Instance>()
                .FirstAsync(a => a.InstanceId == process.InstanceId);

            if (instance == null)
            {
                throw new ArgumentException("流程实例不存在", nameof(process.InstanceId));
            }

            model.FlowData = new WorkFlowProcessFlowData
            {
                IsFinish = instance.IsFinish,
                Status = instance.Status
            };

            // 流程已结束
            if (instance.IsFinish == (int)WorkFlowInstanceStatus.Finish)
            {
                await SetFinishedProcessMenusAsync(model, instance, process.UserId);
                return model;
            }

            // 流程运行中
            await SetRunningProcessMenusAsync(model, instance, process);

            return model;
        }

        /// <summary>
        /// 设置初始流程菜单
        /// </summary>
        private static void SetInitialProcessMenus(WorkFlowProcess model)
        {
            model.Menus = new List<int>
            {
                (int)WorkFlowMenu.Submit,
                (int)WorkFlowMenu.FlowImage,
                (int)WorkFlowMenu.Save,
                (int)WorkFlowMenu.Return
            };
        }

        /// <summary>
        /// 设置已完成流程的菜单
        /// </summary>
        private async Task SetFinishedProcessMenusAsync(
            WorkFlowProcess model,
            WF_WorkFlow_Instance instance,
            string userId)
        {
            var menus = new List<int>();

            // 流程发起人显示打印按钮
            if (userId == instance.CreateUserId.ToString())
            {
                menus.Add((int)WorkFlowMenu.Print);
            }

            // 检查是否有未读通知
            var hasUnreadNotice = await HasUnreadNoticeAsync(instance.InstanceId, userId);
            if (hasUnreadNotice)
            {
                menus.Add((int)WorkFlowMenu.View);
            }

            menus.AddRange(new[]
            {
                (int)WorkFlowMenu.Approval,
                (int)WorkFlowMenu.FlowImage,
                (int)WorkFlowMenu.Return
            });

            model.Menus = menus;
        }

        /// <summary>
        /// 设置运行中流程的菜单
        /// </summary>
        private async Task SetRunningProcessMenusAsync(
            WorkFlowProcess model,
            WF_WorkFlow_Instance instance,
            WorkFlowProcess process)
        {
            var workflow = await _db.Queryable<WF_WorkFlow>()
                .FirstAsync(a => a.FlowId == instance.FlowId);

            var context = new MsWorkFlowContext(new WorkFlow.Core.WorkFlow
            {
                FlowId = Guid.Parse(workflow.FlowId),
                FlowJson = instance.FlowContent,
                ActivityNodeId = Guid.Parse(instance.ActivityId)
            });

            model.FlowData.CurrentNode = context.WorkFlow.ActivityNode;

            // 退回到开始节点
            if (context.WorkFlow.ActivityNode.Type == WorkFlowNode.START)
            {
                await SetStartNodeMenusAsync(model, instance, process.UserId);
                return;
            }

            // 正常节点处理
            await SetNormalNodeMenusAsync(model, instance, process, context);
        }

        /// <summary>
        /// 设置开始节点菜单
        /// </summary>
        private async Task SetStartNodeMenusAsync(
            WorkFlowProcess model,
            WF_WorkFlow_Instance instance,
            string userId)
        {
            if (userId == instance.CreateUserId.ToString())
            {
                model.Menus = new List<int>
                {
                    (int)WorkFlowMenu.ReSubmit,
                    (int)WorkFlowMenu.Approval,
                    (int)WorkFlowMenu.FlowImage,
                    (int)WorkFlowMenu.Save,
                    (int)WorkFlowMenu.Return
                };
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

        /// <summary>
        /// 设置普通节点菜单
        /// </summary>
        private async Task SetNormalNodeMenusAsync(
            WorkFlowProcess model,
            WF_WorkFlow_Instance instance,
            WorkFlowProcess process,
            MsWorkFlowContext context)
        {
            if (string.IsNullOrEmpty(instance.MakerList))
            {
                model.Menus = new List<int>
                {
                    (int)WorkFlowMenu.Approval,
                    (int)WorkFlowMenu.FlowImage,
                    (int)WorkFlowMenu.Return
                };
                return;
            }

            var menus = new List<int>();

            // 检查当前用户是否为执行人
            if (IsCurrentUserExecutor(instance.MakerList, process.UserId))
            {
                menus.AddRange(new[]
                {
                    (int)WorkFlowMenu.Agree,
                    (int)WorkFlowMenu.Deprecate,
                    (int)WorkFlowMenu.Back
                });

                // 获取执行过的节点
                model.ExecutedNode = await GetExcuteNodes(process.InstanceId, instance.ActivityId, instance.IsFinish);
            }

            // 检查是否有未读通知
            var hasUnreadNotice = await HasUnreadNoticeAsync(instance.InstanceId, process.UserId);
            if (hasUnreadNotice)
            {
                menus.Add((int)WorkFlowMenu.View);
            }

            // 检查是否可以委托
            if (await CanAssignAsync(process, instance.MakerList))
            {
                menus.Add((int)WorkFlowMenu.Assign);
            }

            // 检查是否可以撤回
            if (await CanWithdrawAsync(instance, process.UserId, context))
            {
                menus.Add((int)WorkFlowMenu.Withdraw);
            }

            menus.AddRange(new[]
            {
                (int)WorkFlowMenu.Approval,
                (int)WorkFlowMenu.FlowImage,
                (int)WorkFlowMenu.Return
            });

            model.Menus = menus;
        }

        /// <summary>
        /// 检查当前用户是否为执行人
        /// </summary>
        private static bool IsCurrentUserExecutor(string makerList, string userId)
        {
            if (makerList.Trim() == ALL_USERS_MARKER)
                return true;

            var userIds = makerList.Split(',')
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => Convert.ToInt64(x))
                .ToList();

            return userIds.Contains(userId.ToInt64());
        }

        /// <summary>
        /// 检查是否有未读通知
        /// </summary>
        private async Task<bool> HasUnreadNoticeAsync(string instanceId, string userId)
        {
            var notices = await _db.Queryable<WF_WorkFlow_Notice>()
                .Where(a => a.Maker == userId
                    && a.InstanceId == instanceId
                    && a.IsTransition == 1
                    && a.IsRead == 0
                    && a.Status == 1)
                .AnyAsync();

            return notices;
        }

        /// <summary>
        /// 判断当前用户是否能显示委托操作按钮
        /// </summary>
        private async Task<bool> CanAssignAsync(WorkFlowProcess process, string makerList)
        {
            var userMarker = process.UserId + ",";
            if (!makerList.Contains(userMarker))
                return false;

            // 检查是否已经委托过
            var hasAssigned = await _db.Queryable<WFWorkFlowAssign>()
                .Where(m => m.InstanceId == process.InstanceId
                    && m.FlowId == process.FlowId
                    && m.UserId.ToString() == process.UserId)
                .AnyAsync();

            return !hasAssigned;
        }

        /// <summary>
        /// 检查是否可以撤回
        /// </summary>
        private async Task<bool> CanWithdrawAsync(
            WF_WorkFlow_Instance instance,
            string userId,
            MsWorkFlowContext context)
        {
            var previousNodes = context.GetLinesForFrom(instance.ActivityId);

            if (previousNodes.Count != 1)
                return false;

            var nodeType = context.GetNodeType(previousNodes[0].sourceNodeId);

            return nodeType == WorkFlowInstanceNodeType.Start
                && userId == instance.CreateUserId.ToString().Trim();
        }

        /// <summary>
        /// 系统定制流程获取
        /// </summary>
        public async Task<WorkFlowProcess> GetProcessForSystemAsync(SystemFlowDto model)
        {
            var workflow = await _db.Queryable<WF_WorkFlow>()
                .Where(a => a.FormId == model.FormId)
                .FirstAsync();

            if (workflow == null)
            {
                throw new ArgumentException("工作流不存在", nameof(model.FormId));
            }

            var process = new WorkFlowProcess
            {
                UserId = model.UserId,
                FlowId = workflow.FlowId,
                FlowName = workflow.FlowName,
                FormId = workflow.FormId.ToString()
            };

            // 获取实例ID
            if (!string.IsNullOrEmpty(model.PageId))
            {
                var instance = await _db.Queryable<WF_WorkFlow_Instance>()
                    .Where(a => a.FlowId == process.FlowId && a.FormId == process.FormId)
                    .FirstAsync();

                process.InstanceId = instance?.InstanceId ?? default(Guid).ToString();
            }
            else
            {
                process.InstanceId = default(Guid).ToString();
            }

            return await GetProcessAsync(process);
        }

        /// <summary>
        /// 流程过程流转处理
        /// </summary>
        public async Task<bool> ProcessTransitionFlowAsync(WorkFlowProcessTransition model)
        {
            return model.MenuType switch
            {
                WorkFlowMenu.ReSubmit => await ProcessTransitionReSubmitAsync(model),
                WorkFlowMenu.Agree => await WorkFlowAgreeAsync(model),
                WorkFlowMenu.Deprecate => await WorkFlowDeprecateAsync(model),
                WorkFlowMenu.Back => await WorkFlowBackAsync(model),
                WorkFlowMenu.Withdraw => await WorkFlowWithdrawAsync(model),
                WorkFlowMenu.View => await ProcessTransitionViewAsync(model),
                WorkFlowMenu.Assign => await ProcessTransitionAssignAsync(model),
                _ => false
            };
        }

        /// <summary>
        /// 计算票数
        /// </summary>
        private async Task<WorkFlowInstanceStatus> CalcVotesAsync(
            string instanceId,
            string nodeId,
            WorkFlowNode node,
            ChatParallelCalcType calcType)
        {
            var operations = await _db.Queryable<WF_WorkFlow_Operation_History>()
                .Where(a => a.InstanceId == instanceId && a.NodeId == nodeId)
                .ToListAsync();

            if (!operations.Any())
                return WorkFlowInstanceStatus.Running;

            bool passed = calcType switch
            {
                ChatParallelCalcType.MoreThenHalf =>
                    operations.Count(m => m.TransitionType == (int)WorkFlowMenu.Agree) > (operations.Count / 2),
                _ => operations.All(m => m.TransitionType == (int)WorkFlowMenu.Agree)
            };

            return node.NodeType() == WorkFlowInstanceNodeType.End
                ? WorkFlowInstanceStatus.Finish
                : WorkFlowInstanceStatus.Running;
        }

        #region 会签操作
        /// <summary>
        /// 会签节点逻辑
        /// </summary>
        private async Task HandleChatNodeLogicAsync(
            MsWorkFlowContext context,
            WF_WorkFlow_Instance instance,
            WorkFlowProcessTransition model,
            WorkFlowInstanceStatus status,WorkFlowMenu wfm
            )
        {
            if (context.WorkFlow.ActivityNode.properties.ChatData.ChatType == ChatType.Parallel)
            {
                await HandleParallelChatAsync(context, instance, model, status, wfm);
            }
            else
            {
                await HandleSerialChatAsync(context, instance, model, status,  wfm);
            }
        }

        /// <summary>
        /// 并行会签处理
        /// </summary>
        private async Task HandleParallelChatAsync(
            MsWorkFlowContext context,
            WF_WorkFlow_Instance instance,
            WorkFlowProcessTransition model,
            WorkFlowInstanceStatus status,WorkFlowMenu wfm)
        {
            var makerUsers = instance.MakerList
                .Split(',')
                .Where(m => !string.IsNullOrEmpty(m))
                .ToList();

            // 记录当前节点的流转历史
            await CreateSelfTransitionHistoryAsync(instance, model, context, wfm);

            if (makerUsers.Count == 1) // 最后一人
            {
                await CompleteParallelChatAsync(context, instance, model,wfm);
            }
            else
            {
                // 移除当前用户，更新执行人列表
                makerUsers.Remove(model.UserId);
                instance.MakerList = string.Join(",", makerUsers) + COMMA_SUFFIX;
                await _db.Updateable(instance).ExecuteCommandAsync();
            }
        }

        /// <summary>
        /// 创建自身节点流转历史
        /// </summary>
        private async Task CreateSelfTransitionHistoryAsync(
            WF_WorkFlow_Instance instance,
            WorkFlowProcessTransition model,
            MsWorkFlowContext context,WorkFlowMenu wfm)
        {
            var transitionHistory = new WF_WorkFlow_Transition_History
            {
                transitionId = Guid.NewGuid().ToString(),
                InstanceId = instance.InstanceId,
                CreateUserId = long.Parse(model.UserId),
                CreateUserName = model.UserName,
                TransitionState = (int)WorkFlowTransitionStateType.Normal,
                IsFinish = (int)WorkFlowInstanceStatus.Running,
                FromNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                FromNodeName = context.WorkFlow.ActivityNode.text.value,
                FromNodeType = (int)context.WorkFlow.ActivityNodeType,
                ToNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                ToNodeName = context.WorkFlow.ActivityNode.text.value,
                ToNodeType = (int)context.WorkFlow.ActivityNodeType,
                TransitionType=(int)wfm
            };

            await _db.Insertable(transitionHistory).ExecuteCommandAsync();
        }

        /// <summary>
        /// 完成并行会签
        /// </summary>
        private async Task CompleteParallelChatAsync(
            MsWorkFlowContext context,
            WF_WorkFlow_Instance instance,
            WorkFlowProcessTransition model,
            WorkFlowMenu wfm)
        {
            var edge = context.WorkFlow.Edges[Guid.Parse(instance.ActivityId)][0];
            var nextNode = context.WorkFlow.Nodes[edge.targetNodeId];

            // 记录跳转到下一节点的流转历史
            await CreateTransitionToNextNodeAsync(instance, model, context, nextNode, wfm);

            // 更新实例
            instance.PreviousId = instance.ActivityId;
            instance.ActivityId = nextNode.Id.ToString();
            instance.ActivityName = nextNode.text.value;
            instance.ActivityType = (int)nextNode.NodeType();
            instance.MakerList = nextNode.NodeType() == WorkFlowInstanceNodeType.End
                ? string.Empty
                : await GetMakerListAsync(nextNode, model.UserId, model.OptionParams);

            var result = await CalcVotesAsync(
                instance.InstanceId,
                instance.PreviousId,
                nextNode,
                context.WorkFlow.ActivityNode.properties.ChatData.ParallelCalcType);

            instance.IsFinish = (int)result;
            await _db.Updateable(instance).ExecuteCommandAsync();
        }

        /// <summary>
        /// 串行会签处理
        /// </summary>
        private async Task HandleSerialChatAsync(
            MsWorkFlowContext context,
            WF_WorkFlow_Instance instance,
            WorkFlowProcessTransition model,
            WorkFlowInstanceStatus status,
            WorkFlowMenu wfm)
        {
            var users = context.WorkFlow.ActivityNode.properties.users.Split(',');
            var currentIndex = Array.IndexOf(users, model.UserId);
            var isLastUser = currentIndex == users.Length - 1;

            // 记录当前节点的流转历史
            await CreateSelfTransitionHistoryAsync(instance, model, context,wfm);

            if (isLastUser)
            {
                await CompleteSerialChatAsync(context, instance, model,wfm);
            }
        }

        /// <summary>
        /// 完成串行会签
        /// </summary>
        private async Task CompleteSerialChatAsync(
            MsWorkFlowContext context,
            WF_WorkFlow_Instance instance,
            WorkFlowProcessTransition model,WorkFlowMenu wfm)
        {
            var edge = context.WorkFlow.Edges[Guid.Parse(instance.ActivityId)][0];
            var nextNode = context.WorkFlow.Nodes[edge.sourceNodeId];

            // 记录跳转到下一节点的流转历史
            await CreateTransitionToNextNodeAsync(instance, model, context, nextNode, wfm);

            // 更新实例
            instance.PreviousId = instance.ActivityId;
            instance.ActivityId = nextNode.Id.ToString();
            instance.ActivityName = nextNode.text.value;
            instance.ActivityType = (int)nextNode.NodeType();
            instance.MakerList = nextNode.NodeType() == WorkFlowInstanceNodeType.End
                ? string.Empty
                : await GetMakerListAsync(nextNode, model.UserId, model.OptionParams);

            var result = await CalcVotesAsync(
                instance.InstanceId,
                instance.PreviousId,
                nextNode,
                context.WorkFlow.ActivityNode.properties.ChatData.ParallelCalcType);

            instance.IsFinish = (int)result;
            await _db.Updateable(instance).ExecuteCommandAsync();
        }
        /// <summary>
        /// 下个节点是会签逻辑
        /// </summary>
        private async Task HandleNextChatNodeAsync(
            MsWorkFlowContext context,
            WF_WorkFlow_Instance instance,
            WorkFlowInstanceStatus status)
        {
            instance.IsFinish = (int)status;

            if (context.WorkFlow.NextNode.properties.ChatData.ChatType == ChatType.Parallel)
            {
                // 并行会签
                instance.MakerList = context.WorkFlow.NextNode.properties.users;
            }
            else
            {
                // 串行会签
                instance.MakerList = context.WorkFlow.NextNode.properties.users.Split(',')[0];
            }

            instance.PreviousId = instance.ActivityId;
            instance.ActivityId = context.WorkFlow.NextNodeId.ToString();
            instance.ActivityName = context.WorkFlow.NextNode.text.value;
            instance.ActivityType = (int)context.WorkFlow.NextNodeType;

            await _db.Updateable(instance).ExecuteCommandAsync();
        }
        #endregion

        /// <summary>
        /// 创建跳转到下一节点的流转历史
        /// </summary>
        private async Task CreateTransitionToNextNodeAsync(
            WF_WorkFlow_Instance instance,
            WorkFlowProcessTransition model,
            MsWorkFlowContext context,
            WorkFlowNode nextNode,
            WorkFlowMenu wfm
            
            )
        {
            var transitionHistory = new WF_WorkFlow_Transition_History
            {
                transitionId = Guid.NewGuid().ToString(),
                InstanceId = instance.InstanceId,
                FromNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                FromNodeName = context.WorkFlow.ActivityNode.text.value,
                FromNodeType = (int)context.WorkFlow.ActivityNodeType,
                ToNodeId = nextNode.Id.ToString(),
                ToNodeType = (int)nextNode.NodeType(),
                ToNodeName = nextNode.text.value,
                TransitionState = (int)WorkFlowTransitionStateType.Normal,
                IsFinish = nextNode.NodeType().ToIsFinish(),
                CreateUserId = long.Parse(model.UserId),
                CreateUserName = model.UserName,
                TransitionType=(int)wfm
            };

            await _db.Insertable(transitionHistory).ExecuteCommandAsync();
        }



        #region 同意操作
        /// <summary>
        /// 同意操作
        /// </summary>
        public async Task<bool> WorkFlowAgreeAsync(WorkFlowProcessTransition model)
        {
            var instance = await ValidateAndGetInstanceAsync(model.InstanceId.ToString());
            var userInfo = await GetUserInfoAsync(model.UserId);
            model.UserName = userInfo.UserName;

            var context = new MsWorkFlowContext(new WorkFlow.Core.WorkFlow
            {
                FlowId = model.FlowId,
                FlowJson = instance.FlowContent,
                ActivityNodeId = Guid.Parse(instance.ActivityId),
                PreviousId = Guid.Parse(instance.PreviousId)
            });

            var publishStatus = WorkFlowStatus.Running;

            var result = await _db.Ado.UseTranAsync(async () =>
            {
                if (context.WorkFlow.ActivityNode.NodeType() == WorkFlowInstanceNodeType.Normal)
                {
                    if (context.IsMultipleNextNode())
                    {
                        publishStatus = await HandleMultiBranchAgreeAsync(context, instance, model, WorkFlowMenu.Agree);
                    }
                    else
                    {
                        publishStatus = await HandleSingleBranchAgreeAsync(context, instance, model, WorkFlowMenu.Agree);
                    }
                }
                else
                {
                    throw new Exception("当前只支持正常节点功能");
                }

                await CreateOperationHistoryAsync(
                    instance.InstanceId,
                    userInfo,
                    context,
                    WorkFlowMenu.Agree);

                model.ProcessContent = model.ProcessContent ?? "同意";

                await PublishFlowStatusChangeAsync(model.StatusChange, publishStatus);
            });

            return result.IsSuccess;
        }

        /// <summary>
        /// 验证并获取流程实例
        /// </summary>
        private async Task<WF_WorkFlow_Instance> ValidateAndGetInstanceAsync(string instanceId)
        {
            var instance = await _db.Queryable<WF_WorkFlow_Instance>()
                .FirstAsync(a => a.InstanceId == instanceId && a.IsDeleted == 0);

            if (instance == null)
            {
                throw new ArgumentException("流程实例不存在", nameof(instanceId));
            }

            if (instance.IsFinish == (int)WorkFlowInstanceStatus.Finish)
            {
                throw new Exception("此流程已结束,不可操作");
            }

            return instance;
        }

        /// <summary>
        /// 处理多分支同意
        /// </summary>
        private async Task<WorkFlowStatus> HandleMultiBranchAgreeAsync(
            MsWorkFlowContext context,
            WF_WorkFlow_Instance instance,
            WorkFlowProcessTransition model, WorkFlowMenu wfm)
        {
            var nextLines = context.GetLinesForTo(context.WorkFlow.ActivityNodeId);
            var finalNodeId = await GetFinalNodeId(nextLines, Convert.ToDouble(model.ComValue));
            var nextNode = context.WorkFlow.Nodes[finalNodeId.Value];

            UpdateInstanceForTransition(instance, nextNode, model);

            var publishStatus = WorkFlowStatus.Running;

            if (nextNode.NodeType() == WorkFlowInstanceNodeType.End)
            {
                instance.FlowStatus = (int)WorkFlowStatus.IsFinish;
                publishStatus = WorkFlowStatus.IsFinish;
                //将结束节点插入到WF_WorkFlow_Operation_History中
            }
            else
            {
                instance.FlowStatus = (int)WorkFlowStatus.Running;
            }

            await _db.Updateable(instance).ExecuteCommandAsync();

            // 添加流转记录
            await CreateTransitionToNodeAsync(instance, model, context, nextNode, wfm);

            // 添加通知
            var viewNodes = context.GetNextNodes(null, WorkFlowInstanceNodeType.ViewNode);
            await AddFlowNoticeAsync(viewNodes, instance.CreateUserId.ToString(), model);

            return publishStatus;
        }

        /// <summary>
        /// 处理单分支同意
        /// </summary>
        private async Task<WorkFlowStatus> HandleSingleBranchAgreeAsync(
            MsWorkFlowContext context,
            WF_WorkFlow_Instance instance,
            WorkFlowProcessTransition model,
            WorkFlowMenu wfm
            )
        {
            if (context.WorkFlow.NextNode.NodeType() == WorkFlowInstanceNodeType.ChatNode)
            {
                throw new Exception("当前不支持会签功能");
            }

            UpdateInstanceForTransition(instance, context.WorkFlow.NextNode, model);

            var publishStatus = WorkFlowStatus.Running;

            if (context.WorkFlow.NextNodeType == WorkFlowInstanceNodeType.End)
            {
                instance.FlowStatus = (int)WorkFlowStatus.IsFinish;
                publishStatus = WorkFlowStatus.IsFinish;
            }
            else
            {
                instance.FlowStatus = (int)WorkFlowStatus.Running;
            }

            await _db.Updateable(instance).ExecuteCommandAsync();

            // 添加流转记录
            await CreateTransitionToNextNodeAsync(instance, model, context, wfm);

            // 添加通知
            var viewNodes = context.GetNextNodes(null, WorkFlowInstanceNodeType.ViewNode);
            await AddFlowNoticeAsync(viewNodes, instance.CreateUserId.ToString(), model);

            return publishStatus;
        }

        /// <summary>
        /// 更新实例以进行流转
        /// </summary>
        private void UpdateInstanceForTransition(
            WF_WorkFlow_Instance instance,
            WorkFlowNode nextNode,
            WorkFlowProcessTransition model)
        {
            instance.PreviousId = instance.ActivityId;
            instance.ActivityId = nextNode.Id.ToString();
            instance.ActivityName = nextNode.text.value;
            instance.ActivityType = (int)nextNode.NodeType();
            instance.ModifyDate = DateTime.Now;
            instance.MakerList = nextNode.NodeType() == WorkFlowInstanceNodeType.End
                ? string.Empty
                : GetMakerListAsync(nextNode, model.UserId, model.OptionParams).Result;
            instance.IsFinish = nextNode.NodeType().ToIsFinish();
        }

        /// <summary>
        /// 创建流转到指定节点的记录
        /// </summary>
        private async Task CreateTransitionToNodeAsync(
            WF_WorkFlow_Instance instance,
            WorkFlowProcessTransition model,
            MsWorkFlowContext context,
            WorkFlowNode targetNode, WorkFlowMenu wfm)
        {
            var transitionHistory = new WF_WorkFlow_Transition_History
            {
                transitionId = Guid.NewGuid().ToString(),
                InstanceId = instance.InstanceId,
                FromNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                FromNodeName = context.WorkFlow.ActivityNode.text.value,
                FromNodeType = (int)context.WorkFlow.ActivityNodeType,
                ToNodeId = targetNode.Id.ToString(),
                ToNodeType = (int)targetNode.NodeType(),
                ToNodeName = targetNode.text.value,
                TransitionState = (int)WorkFlowTransitionStateType.Normal,
                IsFinish = targetNode.NodeType().ToIsFinish(),
                CreateUserId = long.Parse(model.UserId),
                CreateUserName = model.UserName,
                TransitionType=(int)wfm
            };
            await _db.Insertable(transitionHistory).ExecuteCommandAsync();
        }

        /// <summary>
        /// 创建流转到下一节点的记录
        /// </summary>
        private async Task CreateTransitionToNextNodeAsync(
            WF_WorkFlow_Instance instance,
            WorkFlowProcessTransition model,
            MsWorkFlowContext context, WorkFlowMenu wfm)
        {
            var transitionHistory = new WF_WorkFlow_Transition_History
            {
                transitionId = Guid.NewGuid().ToString(),
                InstanceId = instance.InstanceId,
                FromNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                FromNodeName = context.WorkFlow.ActivityNode.text.value,
                FromNodeType = (int)context.WorkFlow.ActivityNodeType,
                ToNodeId = context.WorkFlow.NextNodeId.ToString(),
                ToNodeType = (int)context.WorkFlow.NextNodeType,
                ToNodeName = context.WorkFlow.NextNode.text.value,
                TransitionState = (int)WorkFlowTransitionStateType.Normal,
                IsFinish = context.WorkFlow.NextNodeType.ToIsFinish(),
                CreateUserId = long.Parse(model.UserId),
                CreateUserName = model.UserName,
                TransitionType = (int)wfm
            };            
            await _db.Insertable(transitionHistory).ExecuteCommandAsync();
        }

        #endregion

        #region 通知
        /// <summary>
        /// 添加流程通知
        /// </summary>
        private async Task AddFlowNoticeAsync(
            List<WorkFlowNode> viewNodes,
            string createUserId,
            WorkFlowProcessTransition model)
        {
            if (!viewNodes.Any())
                return;

            var noticeDict = new Dictionary<string, WorkFlowNode>();

            foreach (var node in viewNodes)
            {
                var makerList = await GetMakerListAsync(node, model.UserId, model.OptionParams);

                if (string.IsNullOrEmpty(makerList) || makerList == ALL_USERS_MARKER)
                    continue;

                var makers = makerList.Split(',').Where(m => !string.IsNullOrEmpty(m));

                foreach (var userId in makers)
                {
                    if (!noticeDict.ContainsKey(userId))
                    {
                        noticeDict[userId] = node;
                    }
                }
            }

            if (!noticeDict.Any())
                return;

            var notices = noticeDict.Select(m => new WF_WorkFlow_Notice
            {
                IsRead = 0,
                Maker = m.Key,
                NodeId = m.Value.Id.ToString(),
                NodeName = m.Value.text.value,
                Status = 1,
                IsTransition = 1,
                InstanceId = model.InstanceId.ToString()
            }).ToList();

            await _db.Insertable(notices).ExecuteCommandAsync();
        }
        #endregion

        #region 不同意
        /// <summary>
        /// 不同意，现在逻辑是不同意流程直接结束
        /// </summary>
        public async Task<bool> WorkFlowDeprecateAsync(WorkFlowProcessTransition model)
        {
            var instance = await ValidateAndGetInstanceAsync(model.InstanceId.ToString());
            var userInfo = await GetUserInfoAsync(model.UserId);
            model.UserName = userInfo.UserName;

            var context = new MsWorkFlowContext(new WorkFlow.Core.WorkFlow
            {
                FlowId = model.FlowId,
                FlowJson = instance.FlowContent,
                ActivityNodeId = Guid.Parse(instance.ActivityId),
                PreviousId = Guid.Parse(instance.PreviousId),
                NextNodeType = WorkFlowInstanceNodeType.End
            });

            var result = await _db.Ado.UseTranAsync(async () =>
            {
                // 会签节点特殊处理
                if (context.WorkFlow.ActivityNode.NodeType() == WorkFlowInstanceNodeType.ChatNode)
                {
                    await HandleChatNodeLogicAsync(context, instance, model, WorkFlowInstanceStatus.Running,WorkFlowMenu.Deprecate);
                }
                else
                {
                    // 流程直接结束
                    instance.MakerList = string.Empty;
                    instance.IsFinish = 1;
                    instance.FlowStatus = (int)WorkFlowStatus.IsFinish;
                    instance.PreviousId = instance.ActivityId;
                    instance.ActivityId = context.WorkFlow.NextNodeId.ToString();
                    instance.ModifyDate = DateTime.Now;

                    await _db.Updateable(instance).ExecuteCommandAsync();

                    // 流转记录
                    await CreateTransitionHistoryAsync(
                        instance.InstanceId,
                        userInfo,
                        context,
                         WorkFlowMenu.Deprecate,
                        WorkFlowTransitionStateType.Reject);
                }

                // 操作历史
                model.ProcessContent = model.ProcessContent ?? "不同意";
                await CreateOperationHistoryAsync(
                    instance.InstanceId,
                    userInfo,
                    context,
                    WorkFlowMenu.Deprecate);

                await PublishFlowStatusChangeAsync(model.StatusChange, WorkFlowStatus.Deprecate);
            });

            return result.IsSuccess;
        }
        #endregion

        #region 流程退回
        /// <summary>
        /// 流程退回
        /// </summary>
        public async Task<bool> WorkFlowBackAsync(WorkFlowProcessTransition model)
        {
            var instance = await ValidateAndGetInstanceAsync(model.InstanceId.ToString());
            var userInfo = await GetUserInfoAsync(model.UserId);
            model.UserName = userInfo.UserName;

            var context = new MsWorkFlowContext(new WorkFlow.Core.WorkFlow
            {
                FlowId = model.FlowId,
                FlowJson = instance.FlowContent,
                ActivityNodeId = Guid.Parse(instance.ActivityId),
                PreviousId = Guid.Parse(instance.PreviousId)
            }, WorkFlowMenu.Back);

            var result = await _db.Ado.UseTranAsync(async () =>
            {
                if (context.WorkFlow.ActivityNodeType != WorkFlowInstanceNodeType.Normal
                    && context.WorkFlow.ActivityNodeType != WorkFlowInstanceNodeType.Start)
                {
                    return; // 会签节点不可退回
                }

                var rejectNodeId = context.RejectNode(model.NodeRejectType.Value, model.RejectNodeId);
                var rejectNode = context.WorkFlow.Nodes[rejectNodeId];
                

                UpdateInstanceForBack(instance, rejectNode, model);
                await _db.Updateable(instance).ExecuteCommandAsync();

                // 流转记录
                await CreateBackTransitionHistoryAsync(instance, model, context, rejectNodeId, rejectNode);

                // 操作记录
                model.ProcessContent = model.ProcessContent ?? $"退回至{instance.ActivityName}";
                await CreateOperationHistoryAsync(
                    instance.InstanceId,
                    userInfo,
                    context,
                    WorkFlowMenu.Back);

                await PublishFlowStatusChangeAsync(model.StatusChange, WorkFlowStatus.Back);
            });

            return result.IsSuccess;
        }

        /// <summary>
        /// 更新实例以进行退回
        /// </summary>
        private void UpdateInstanceForBack(
            WF_WorkFlow_Instance instance,
            WorkFlowNode rejectNode,
            WorkFlowProcessTransition model)
        {  
            //查找上一个正常流程审批节点，不包含退回的流程
            var wthModel = _db.Queryable<WF_WorkFlow_Transition_History>().Where(a => a.InstanceId == instance.InstanceId
            && a.TransitionType != 7 && a.ToNodeId == instance.ActivityId.ToString() && a.IsDeleted == 0).First();
            if (wthModel != null)
            {
                instance.PreviousId = wthModel.FromNodeId;
            }
            else
            { instance.PreviousId = instance.PreviousId;
            }
            
            instance.ActivityId = rejectNode.Id.ToString();
            instance.ActivityName = rejectNode.text.value;
            instance.ActivityType = (int)rejectNode.NodeType();
            instance.ModifyDate = DateTime.Now;
          
           

            if (rejectNode.NodeType() == WorkFlowInstanceNodeType.Start)
            {
                instance.MakerList = instance.CreateUserId + COMMA_SUFFIX;               
            }
            else
            {
                instance.MakerList = rejectNode.NodeType() == WorkFlowInstanceNodeType.End
                    ? string.Empty
                    : GetMakerListAsync(rejectNode, instance.CreateUserId.ToString(), model.OptionParams).Result;
            }

            instance.IsFinish = rejectNode.NodeType().ToIsFinish();
            instance.FlowStatus = (int)WorkFlowStatus.Back;
        }

        /// <summary>
        /// 创建退回流转历史
        /// </summary>
        private async Task CreateBackTransitionHistoryAsync(
            WF_WorkFlow_Instance instance,
            WorkFlowProcessTransition model,
            MsWorkFlowContext context,
            Guid rejectNodeId,
            WorkFlowNode rejectNode)
        {
            var transitionHistory = new WF_WorkFlow_Transition_History
            {
                transitionId = Guid.NewGuid().ToString(),
                InstanceId = instance.InstanceId,
                CreateUserId = long.Parse(model.UserId),
                CreateUserName = model.UserName,
                IsFinish = (int)WorkFlowInstanceStatus.Running,
                TransitionState = (int)WorkFlowTransitionStateType.Reject,
                FromNodeId = context.WorkFlow.ActivityNodeId.ToString(),
                FromNodeType = (int)context.WorkFlow.ActivityNodeType,
                FromNodeName = context.WorkFlow.ActivityNode.text.value,
                ToNodeId = rejectNodeId.ToString(),
                ToNodeType = (int)rejectNode.NodeType(),
                ToNodeName = rejectNode.text.value,
                TransitionType=(int) WorkFlowMenu.Back
            };

            await _db.Insertable(transitionHistory).ExecuteCommandAsync();
        }
        #endregion

        #region 流程撤回
        /// <summary>
        /// 流程撤回
        /// 刚开始提交，下一个节点未审批情况，流程发起人可以终止
        /// </summary>
        public async Task<bool> WorkFlowWithdrawAsync(WorkFlowProcessTransition model)
        {
            var result = await _db.Ado.UseTranAsync(async () =>
            {
                var instance = await _db.Queryable<WF_WorkFlow_Instance>()
                    .FirstAsync(a => a.InstanceId == model.InstanceId.ToString());

                if (instance == null)
                    return;

                // 删除相关记录
                await DeleteRelatedRecordsAsync(model.InstanceId.ToString());

                // 删除流程实例
                await _db.Deleteable(instance).ExecuteCommandAsync();

                // 改变表单状态
                await PublishFlowStatusChangeAsync(model.StatusChange, WorkFlowStatus.Withdraw);
            });

            return result.IsSuccess;
        }

        /// <summary>
        /// 删除流程相关记录
        /// </summary>
        private async Task DeleteRelatedRecordsAsync(string instanceId)
        {
            // 删除操作记录
            var operations = await _db.Queryable<WF_WorkFlow_Operation_History>()
                .Where(m => m.InstanceId == instanceId)
                .ToListAsync();
            await _db.Deleteable(operations).ExecuteCommandAsync();

            // 删除流转记录
            var transitions = await _db.Queryable<WF_WorkFlow_Transition_History>()
                .Where(m => m.InstanceId == instanceId)
                .ToListAsync();
            await _db.Deleteable(transitions).ExecuteCommandAsync();

            // 删除委托记录
            var assigns = await _db.Queryable<WFWorkFlowAssign>()
                .Where(m => m.InstanceId == instanceId)
                .ToListAsync();
            await _db.Deleteable(assigns).ExecuteCommandAsync();
        }
        #endregion

        #region 已阅操作
        /// <summary>
        /// 已阅操作
        /// </summary>
        protected async Task<bool> ProcessTransitionViewAsync(WorkFlowProcessTransition model)
        {
            var result = await _db.Ado.UseTranAsync(async () =>
            {
                var notices = await _db.Queryable<WF_WorkFlow_Notice>()
                    .Where(m => m.Maker == model.UserId
                        && m.InstanceId == model.InstanceId.ToString()
                        && m.IsTransition == 1
                        && m.IsRead == 0
                        && m.Status == 1)
                    .ToListAsync();

                foreach (var notice in notices)
                {
                    notice.IsRead = 1;

                    // 添加操作记录
                    var operationHistory = new WF_WorkFlow_Operation_History
                    {
                        OperationId = Guid.NewGuid().ToString(),
                        InstanceId = model.InstanceId.ToString(),
                        CreateUserId = long.Parse(model.UserId),
                        CreateUserName = model.UserName,
                        Content = "流程已阅",
                        NodeId = notice.NodeId,
                        NodeName = notice.NodeName,
                        TransitionType = (int)WorkFlowMenu.View
                    };

                    await _db.Insertable(operationHistory).ExecuteCommandAsync();
                }

                if (notices.Any())
                {
                    await _db.Updateable(notices).ExecuteCommandAsync();
                }
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
        protected async Task<bool> ProcessTransitionAssignAsync(WorkFlowProcessTransition model)
        {
            var result = await _db.Ado.UseTranAsync(async () =>
            {
                var instance = await _db.Queryable<WF_WorkFlow_Instance>()
                    .FirstAsync(a => a.InstanceId == model.InstanceId.ToString());

                if (instance == null)
                    throw new ArgumentException("流程实例不存在", nameof(model.InstanceId));

                // 修改流程实例makerlist，替换成委托人
                var oldMarker = model.UserId + COMMA_SUFFIX;
                var newMarker = model.Assign.AssignUserId + COMMA_SUFFIX;
                instance.MakerList = instance.MakerList.Replace(oldMarker, newMarker);
                await _db.Updateable(instance).ExecuteCommandAsync();

                var context = new MsWorkFlowContext(new WorkFlow.Core.WorkFlow
                {
                    FlowId = model.FlowId,
                    FlowJson = instance.FlowContent,
                    ActivityNodeId = Guid.Parse(instance.ActivityId),
                    PreviousId = Guid.Parse(instance.PreviousId)
                });

                // 添加委托记录
                var assign = new WFWorkFlowAssign
                {
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
                await _db.Insertable(assign).ExecuteCommandAsync();

                // 添加操作记录
                var content = $"用户【{assign.UserName}】将流程委托给【{assign.AssignUserName}】";
                if (!string.IsNullOrEmpty(model.Assign.AssignContent))
                {
                    content += "<br/>请求委托意见：" + model.Assign.AssignContent;
                }

                var operationHistory = new WF_WorkFlow_Operation_History
                {
                    OperationId = Guid.NewGuid().ToString(),
                    InstanceId = instance.InstanceId,
                    CreateUserId = long.Parse(model.UserId),
                    CreateUserName = model.UserName,
                    Content = content,
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
        public async Task<IEnumerable<WF_WorkFlow_Operation_History>> GetFlowApprovalAsync(string instanceId)
        {
            var histories = await _db.Queryable<WF_WorkFlow_Operation_History>()
                .Where(a => a.InstanceId == instanceId && a.IsDeleted == 0)
                .ToListAsync();

            var instance = await _db.Queryable<WF_WorkFlow_Instance>()
                .FirstAsync(a => a.InstanceId == instanceId && a.IsDeleted == 0);

            if (instance?.IsFinish == 1)
            {
                var endRecord = new WF_WorkFlow_Operation_History
                {
                    NodeName = "结束",
                    TransitionType = null,
                    CreateUserName = string.Empty,
                    Content = "系统自动结束",
                    CreateDate = DateTime.Now
                };

                return histories.Append(endRecord);
            }

            return histories;
        }
        #endregion

        #region 获取流程图信息
        /// <summary>
        /// 获取流程图信息
        /// </summary>
        public async Task<WorkFlowInstanceRes> GetFlowImageAsync(string? url, string? instanceId)
        {
            // 未提交情况
            if (IsInvalidInstanceId(instanceId))
            {
                return await GetUnsubmittedFlowImageAsync(url);
            }

            // 已提交情况
            var instance = await _db.Queryable<WF_WorkFlow_Instance>()
                .FirstAsync(a => a.InstanceId == instanceId);

            if (instance == null)
            {
                throw new ArgumentException("流程实例不存在", nameof(instanceId));
            }

            return new WorkFlowInstanceRes
            {
                FlowId = instance.FlowId,
                InstanceId = instance.InstanceId,
                ActivityId = instance.ActivityId,
                FlowContent = instance.FlowContent
            };
        }

        /// <summary>
        /// 检查实例ID是否无效
        /// </summary>
        private static bool IsInvalidInstanceId(string? instanceId)
        {
            return string.IsNullOrEmpty(instanceId)
                || instanceId == "null"
                || instanceId == "undefined"
                || instanceId == default(Guid).ToString();
        }

        /// <summary>
        /// 获取未提交流程的流程图信息
        /// </summary>
        private async Task<WorkFlowInstanceRes> GetUnsubmittedFlowImageAsync(string url)
        {
            var menu = await _db.Queryable<Menus>()
                .FirstAsync(a => a.MenuUrl == url && a.IsDeleted == 0 && a.IsButton == 0);

            if (menu == null)
            {
                throw new ArgumentException("菜单的url未配置", nameof(url));
            }

            var workflow = await _db.Queryable<WF_WorkFlow>()
                .FirstAsync(a => a.FormId == menu.Id);

            if (workflow == null || string.IsNullOrEmpty(workflow.FlowContent))
            {
                throw new ArgumentException("未配置流程，请联系管理员", nameof(url));
            }

            return new WorkFlowInstanceRes
            {
                FlowId = workflow.FlowId,
                FlowContent = workflow.FlowContent,
                InstanceId = default(Guid).ToString(),
                ActivityId = default(Guid).ToString()
            };
        }
        #endregion

        #region 流程催办
        /// <summary>
        /// 流程催办
        /// </summary>
        public async Task<bool> UrgeAsync(UrgeEdit urge)
        {
            var instance = await _db.Queryable<WF_WorkFlow_Instance>()
                .Where(a => a.InstanceId == urge.InstanceId)
                .FirstAsync();

            if (instance == null)
            {
                throw new ArgumentException("流程实例不存在", nameof(urge.InstanceId));
            }

            var urgeRecord = new WF_WorkFlow_Urge
            {
                CreateUserId = urge.Sender,
                Sender = urge.Sender,
                InstanceId = urge.InstanceId,
                NodeId = instance.ActivityId,
                NodeName = instance.ActivityName,
                UrgeUser = instance.MakerList,
                UrgeContent = urge.UrgeContent,
                UrgeType = urge.UrgeType
            };

            var result = await _db.Insertable(urgeRecord).ExecuteCommandAsync();
            return result > 0;
        }
        #endregion

        #region 重新提交流程
        /// <summary>
        /// 重新提交流程
        /// 实例只有一次
        /// </summary>
        protected async Task<bool> ProcessTransitionReSubmitAsync(WorkFlowProcessTransition model)
        {
            var workflow = await _db.Queryable<WF_WorkFlow>()
                .FirstAsync(a => a.FlowId == model.FlowId.ToString() && a.IsDeleted == 0);

            var context = new MsWorkFlowContext(new WorkFlow.Core.WorkFlow
            {
                FlowId = Guid.Parse(workflow.FlowId),
                FlowJson = workflow.FlowContent,
                ActivityNodeId = default
            });

            var result = await _db.Ado.UseTranAsync(async () =>
            {
                var instance = await _db.Queryable<WF_WorkFlow_Instance>()
                    .Where(a => a.InstanceId == model.InstanceId.ToString())
                    .FirstAsync();

                if (instance == null)
                    throw new ArgumentException("流程实例不存在", nameof(model.InstanceId));

                // 更新实例
                instance.ActivityId = context.WorkFlow.NextNodeId.ToString();
                instance.ActivityName = context.WorkFlow.NextNode.text.value;
                instance.ActivityType = (int)context.WorkFlow.NextNodeType;
                instance.PreviousId = context.WorkFlow.ActivityNodeId.ToString();
                instance.MakerList = await GetMakerListAsync(
                    context.WorkFlow.Nodes[context.WorkFlow.NextNodeId],
                    model.UserId,
                    model.OptionParams);
                instance.IsFinish = context.WorkFlow.NextNodeType.ToIsFinish();
                instance.Status = (int)WorkFlowStatus.Running;

                await _db.Updateable(instance).ExecuteCommandAsync();

                // 创建操作记录
                await CreateOperationHistoryAsync(
                    instance.InstanceId,
                    await GetUserInfoAsync(model.UserId),
                    context,
                    WorkFlowMenu.ReSubmit);

                // 创建流转记录
                await CreateTransitionHistoryAsync(
                    instance.InstanceId,                  
                    await GetUserInfoAsync(model.UserId),                     
                    context,
                    WorkFlowMenu.ReSubmit
                    );

                // 改变表单状态
                await PublishFlowStatusChangeAsync(model.StatusChange, WorkFlowStatus.Running);
            });

            return result.IsSuccess;
        }
        #endregion

        /// <summary>
        /// 根据条件获取最终流转节点ID
        /// </summary>
        private async Task<Guid?> GetFinalNodeId(List<WorkFlowEdge> nextLines, double? compareValue = 0)
        {
            // 空集合校验
            if (nextLines == null || !nextLines.Any())
                return null;

            // 多分支必须传入比较值
            if (nextLines.Count > 1 && compareValue == null)
                throw new ArgumentException(nameof(compareValue), "多分支流程必须指定比较值！");

            // 单分支直接返回目标节点
            if (nextLines.Count == 1)
                return nextLines.First().targetNodeId;

            // 拆分：带条件的线 / 默认线
            var conditionLines = nextLines
                .Where(line => line.properties?.conditions != null)
                .ToList();

            var defaultLines = nextLines
                .Where(line => line.properties?.conditions == null)
                .ToList();

            // 遍历匹配条件线
            foreach (var line in conditionLines)
            {
                var conditions = line.properties.conditions
                    .OrderBy(c => c.conditionalValue)
                    .ToList();

                if (IsAllConditionsMatched(conditions, compareValue.Value))
                {
                    return line.targetNodeId;
                }
            }

            // 无匹配条件 → 返回默认分支
            if (!defaultLines.Any())
                throw new InvalidOperationException("流程未配置默认分支，无法继续流转！");

            return defaultLines.First().targetNodeId;
        }

        /// <summary>
        /// 校验所有条件是否全部满足（AND逻辑）
        /// </summary>
        private static bool IsAllConditionsMatched(List<Conditions> conditions, double compareValue)
        {
            foreach (var condition in conditions)
            {
                bool isMatch = condition.conditional switch
                {
                    "=" => compareValue == condition.conditionalValue,
                    ">" => compareValue > condition.conditionalValue,
                    ">=" => compareValue >= condition.conditionalValue,
                    "<" => compareValue < condition.conditionalValue,
                    "<=" => compareValue <= condition.conditionalValue,
                    _ => throw new NotSupportedException($"不支持的条件运算符：{condition.conditional}")
                };

                if (!isMatch)
                    return false;
            }

            return true;
        }

        public async Task<WF_WorkFlow_Instance> GetWorkFlowInstanceByInstanceId(string instanceId)
        {
            var model = await _db.Queryable<WF_WorkFlow_Instance>().Where(a => a.InstanceId == instanceId && a.IsDeleted == 0).FirstAsync();
            return model ?? null;
        }
    }
}
