using Laoyoutiao.Models.CustomAttribute;
using Laoyoutiao.Models.Entitys.WF;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Views
{
    /// <summary>
    /// 流程视图
    /// </summary>
    [SugarTable("V_WorkFlow")]
    [Tenant("0")]
    [IgnoreCreate]//不用来创建表
    public class V_WorkFlow:WF_WorkFlow_Instance
    {
        
      public string? MenuUrl { get; set; }
    }
}
