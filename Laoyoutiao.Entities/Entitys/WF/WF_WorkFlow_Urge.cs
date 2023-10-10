using Laoyoutiao.Models.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Entitys.WF
{
    [SugarTable("wf_workflow_urge")]
    [Tenant("0")]
    public class WF_WorkFlow_Urge: BaseEntity
    {
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "流程实例Id")]
        public string? InstanceId { get; set; }
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "节点Id")]
        public string? NodeId { get; set; }

        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "节点名称")]
        public string? NodeName { get; set; }
        [SugarColumn(IsNullable = true,  ColumnDescription = "发送人")]
        public long Sender { get; set; }

        /// <summary>
        /// 催办人
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 50, ColumnDescription = "催办人")]
        public string UrgeUser { get; set; }

        /// <summary>
        /// 催办类型
        /// <see cref="MsSystem.WF.Model.UrgeType"/>
        /// ex: 0,1,2,3
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 50, ColumnDescription = "催办类型")]
        public string UrgeType { get; set; }

        /// <summary>
        /// 催办信息
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 500, ColumnDescription = "催办信息")]
        public string UrgeContent { get; set; }
    }

    /// <summary>
    /// 催办类型
    /// </summary>
    public enum UrgeType
    {
        /// <summary>
        /// SignalR 内部消息
        /// </summary>
        [Description("内部消息")]
        SignalR = 0,
        [Description("邮件提示")]
        EMail = 1,
        [Description("短信提示")]
        SMS = 2,
        [Description("企业微信")]
        WeChat = 3
    }
}
