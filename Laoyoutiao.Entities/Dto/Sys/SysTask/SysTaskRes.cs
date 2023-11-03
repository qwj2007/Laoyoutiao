using Laoyoutiao.Models.CustomAttribute;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto.Sys.SysTask
{
    [TypeMapper(SourceType = typeof(Laoyoutiao.Models.Entitys.Sys.SysTask))]
    public class SysTaskRes
    {
        public long Id { get; set; }
        /// <summary>
        /// 任务组
        /// </summary>     
        public string Groups { get; set; }

        /// <summary>
        /// 任务名称 类名称
        /// </summary> 
        public string TaskName { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>    
        public string TaskDesc { get; set; }

        /// <summary>
        /// 运行时间
        /// </summary>   
        public string RunTime { get; set; }

        /// <summary>
        /// 运行开始时间
        /// </summary>
        public DateTime? RunStartTime { get; set; }

        /// <summary>
        /// 下次运行时间
        /// </summary>
        public DateTime? NextTime { get; set; }
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 共运行次数
        /// </summary>      
        public long Counts { get; set; }

        /// <summary>
        /// 定时表达式 
        /// https://www.bejson.com/othertools/cron/
        /// https://cron.qqe2.com/
        /// </summary>    

        public string Cron { get; set; }

        public int Status { get; set; }
    }
}
