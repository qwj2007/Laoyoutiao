﻿using Laoyoutiao.Models.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Entitys.WF
{
    /// <summary>
    /// 流程实例表
    /// </summary>
    [SugarTable("wf_workflow_instance")]
    [Tenant("0")]
    public class WF_WorkFlow_Instance : BaseEntity
    {
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "流程实例Id")]
        public string? InstanceId { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "流程Id")]
        public string? FlowId { get; set; }

        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "表单Id")]
        public string? FormId { get; set; }
        [SugarColumn(IsNullable = true, Length = 4000, ColumnDescription = "流程JSON内容")]
        public string? FlowContent { get; set; }

        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "流程实例编号")]
        public string? Code { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "当前活动节点")]
        public string? ActivityId { get; set; }

        [SugarColumn(IsNullable = true, ColumnDescription = "当前节点类型")]
        public int ActivityType { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "当前活动节点名称")]
        public string? ActivityName { get; set; }
        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "上一节点")]
        public string? PreviousId { get; set; }
        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "审批人集合")]
        public string? MakerList { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "流程是否结束")]
        public int? IsFinish { get; set; } = 0;

        [SugarColumn(IsNullable = true, ColumnDescription = "状态")]
        public int Status { get; set; }

        /// <summary>
        /// 业务来源Id
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "业务Id",Length =50)]
        public string? BusinessId { get; set; }
        /// <summary>
        /// 业务名称
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "业务名称",Length =50)]
        public string? BusinessName { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "业务来源表",Length =20)]
        public string? BusinessFromTable { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "业务来源Code",Length =50)]
        public string? BusinessCode { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "创建人姓名", Length = 50)]
        public string? CreateUserName { get; set; }

        [SugarColumn(IsNullable = true, ColumnDescription = "流程比较值", Length = 50)]
        public string? ComValue { get; set; }
        

    }
}
