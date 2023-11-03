using Laoyoutiao.Models.Common;
using SqlSugar;

namespace Laoyoutiao.Models.Entitys.Sys
{
    [SugarTable("sys_task")]
    [Tenant("0")]
    public class SysTask:BaseEntity
    {
        /// <summary>
        /// 任务组
        /// </summary>

        [SugarColumn(ColumnDataType = "nvarchar", Length = 255, IsNullable = true, ColumnDescription = "任务组")]
        public string Groups { get; set; } = "datajob";
        /// <summary>
        /// 任务名称 类名称
        /// </summary>

        [SugarColumn(ColumnDataType = "nvarchar", Length = 255, IsNullable = true, ColumnDescription = "任务名称")]
        public string TaskName { get; set; }
        /// <summary>
        /// 任务描述
        /// </summary>
        
        [SugarColumn(ColumnDataType = "nvarchar", Length = 255, IsNullable = true, ColumnDescription = "任务描述")]
        public string TaskDesc { get; set; }

        /// <summary>
        /// 运行时间
        /// </summary>
        
        [SugarColumn(ColumnDataType = "nvarchar", Length = 50, IsNullable = true, ColumnDescription = "运行时间")]
        public string RunTime { get; set; }
        /// <summary>
        /// 运行开始时间
        /// </summary>
        
        [SugarColumn(IsNullable = true, ColumnDescription = "运行开始时间")]
        public DateTime? RunStartTime { get; set; }
        /// <summary>
        /// 下次运行时间
        /// </summary>
       
        [SugarColumn(IsNullable = true, ColumnDescription = "下次运行时间")]
        public DateTime? NextTime { get; set; }
        /// <summary>
        /// 共运行次数
        /// </summary>

        [SugarColumn(ColumnDataType = "bigint", ColumnDescription = "共运行次数")]
        public long Counts { get; set; }
        /// <summary>
        /// 定时表达式 
        /// https://www.bejson.com/othertools/cron/
        /// https://cron.qqe2.com/
        /// </summary>
       
        [SugarColumn(ColumnDataType = "nvarchar", Length = 255, IsNullable = true, ColumnDescription = "Cron 表达式")]
        public string? Cron { get; set; }
    }
}
