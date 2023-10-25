using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.WF;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.WF
{
    [TypeMapper(SourceType = typeof(WF_WorkFlow_Instance))]
    public class WorkFlowInstanceRes
    {
        public long? Id { get; set; }
        public string? InstanceId { get; set; }

        public string? FlowId { get; set; }

        public string? FormId { get; set; }

        public string? FlowContent { get; set; }

        public string? Code { get; set; }

        public string? ActivityId { get; set; }

        public int ActivityType { get; set; }

        public string? ActivityName { get; set; }

        public string? PreviousId { get; set; }

        public string? MakerList { get; set; }

        public int? IsFinish { get; set; } = 0;


        public int Status { get; set; }
        public int FlowStatus { get; set; }

        /// <summary>
        /// 业务来源Id
        /// </summary>

        public string? BusinessId { get; set; }
        /// <summary>
        /// 业务名称
        /// </summary>

        public string? BusinessName { get; set; }

        public string? BusinessFromTable { get; set; }

        public string? BusinessCode { get; set; }
        /// <summary>
        /// 申请人姓名
        /// </summary>
        public string? CreateUserName { get; set; }
        /// <summary>
        /// 菜单地址
        /// </summary>
        public string? MenuUrl { get; set; }

        public string? FlowStatusName { get; set; }

        public DateTime? CreateDate { get; set; }
    }
}
