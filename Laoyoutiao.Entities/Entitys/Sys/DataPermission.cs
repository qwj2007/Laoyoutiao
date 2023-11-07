using Laoyoutiao.Models.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Entitys.Sys
{
    /// <summary>
    /// 数据权限表
    /// </summary>
    [SugarTable("sys_data_permission")]
    [Tenant("0")]
    public class DataPermission:BaseEntity
    {
        /// <summary>
        /// 数据类型，role--角色,users--用户
        /// </summary>
        [SugarColumn(IsNullable = true,ColumnDescription = "数据类型，role--角色,users--用户")]
        public string DataType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "权限范围，99个人，-1全部，0所在公司，1直属上级部门及下属部门，2本部门及下属部门，3本部门，4自定义")]
        public long DataRange { get; set; }

        /// <summary>
        /// 数据Id
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDescription = "数据Id")]

        public long DataId { get; set; }
        [SugarColumn(IsNullable = true, ColumnDescription = "自定义是所选的部门Id集合")]
        public string Depts { get; set; }
    }
}
