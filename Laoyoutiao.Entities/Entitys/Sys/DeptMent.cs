using Laoyoutiao.Models.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Entitys.Sys
{
    [SugarTable("sys_dept")]
    [Tenant("0")]
    public class DeptMent: BaseTreeEntity<DeptMent>
    {
    
        [SugarColumn(IsNullable = false, Length = 100)]
        /// <summary>
        /// 部门名称
        /// </summary>
        /// <value></value>
        public string? DeptName { get; set; }

        [SugarColumn(IsNullable = false, Length = 100)]
        /// <summary>
        /// 部门编码
        /// </summary>
        /// <value></value>
        public string DeptCode { get; set; }

        /// <summary>
        /// 上级部门
        /// </summary>

        [SugarColumn(IsNullable = false)]
        // public long ParentId { get; set; }
        public override long ParentId { get => base.ParentId; set => base.ParentId = value; }

        /// <summary>
        /// 部门所属所有部门，包括自己
        /// </summary>
        [SugarColumn(IsNullable = false, Length = 100)]
        public string Path { get; set; }
        /// <summary>
        /// 1,启用，0禁用
        /// </summary>
        [SugarColumn(IsNullable = false)]
        public int Status { get; set; } = 1;

        /// <summary>
        /// 备注
        /// </summary>
        [SugarColumn(IsNullable = true, Length = 200)]
        public string Memo { get; set; }
     
    }
}
