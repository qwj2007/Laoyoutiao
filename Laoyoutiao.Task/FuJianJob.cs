using Quartz;
using SqlSugar;

namespace Laoyoutiao.Tasks.Core
{
    /// <summary>
    /// 清理3天前多余的附件
    /// </summary>
    public class FuJianJob : IJob
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public FuJianJob(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }
        public async System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {

            var now = DateTime.Now.AddDays(-3);
            //var fujians = await _sqlSugarClient.Queryable<C_Base_File>().Where(t => t.Pid == -1 && t.CreateDate < now).ToListAsync();
            //if (fujians.Count > 0)
            //{

            //    var basepath = AppDomain.CurrentDomain.BaseDirectory;
            //    List<long> fjids = new List<long>();
            //    foreach (var item in fujians)
            //    {
            //        try
            //        {                        
            //            System.IO.File.Delete($"{basepath}/{item.FilePath}");
            //        }
            //        catch 
            //        {
            //            continue;
            //        }
            //    }
            //    _sqlSugarClient.Deleteable<C_Base_File>(fujians).ExecuteCommand();
            //}

        }

       
    }
}
