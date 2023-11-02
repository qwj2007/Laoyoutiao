using Quartz;
using SqlSugar;
using Microsoft.Extensions.Configuration;
using Serilog;
using Laoyoutiao.Common;
using SqlSugar.IOC;
namespace Laoyoutiao.Tasks.Core
{
    /// <summary>
    /// 定时备份数据库
    /// </summary>
    public class DBJob : IJob
    {
        private ISqlSugarClient _sqlSugarClient = null;
        private readonly IConfiguration _configuration;

        public DBJob(IConfiguration configuration)
        {
            
            _configuration = configuration;
        }
        public Task Execute(IJobExecutionContext context)
        {

            var StartBak = _configuration.GetSection("StartBak")?.Value;
            if (StartBak.ToLower() != "true")
            {
                return System.Threading.Tasks.Task.FromResult("");
            }
            Log.Information("开始数据库备份操作");
            string ConnectionConfigs = "ConnectionConfigs";
            List<IocConfig> connectionConfigs = AppSettings.App<IocConfig>(new string[] { ConnectionConfigs });
            foreach (var conn in connectionConfigs)
            {
                _sqlSugarClient = DbScoped.SugarScope.GetConnection(conn.ConfigId ?? "0");                
                var ConnectionString = conn.ConnectionString;
                var temps = ConnectionString.Split(';');
                string dbname = null;
                string[] dbnamekeys = new string[2] { "database", "initial catalog" };
                foreach (var item in temps)
                {
                    var items = item.Split('=');
                    if (dbnamekeys.Contains(items[0].ToLower()))
                    {
                        dbname = items[1].ToLower();
                        break;
                    }
                }
                var paths = @$"{AppDomain.CurrentDomain.BaseDirectory}".Split('\\');

                var path = string.Join('/', paths.Take(paths.Length - 1));
                try
                {
                    var custompath = _configuration.GetConnectionString("BakPath");
                    if (!string.IsNullOrWhiteSpace(custompath))
                    {
                        path = custompath;
                    }
                    _sqlSugarClient.DbMaintenance.BackupDataBase(dbname,
                   $"{path}/DBBack/{DateTime.Now.ToString("yyyy-MM-dd")}");

                    var files = System.IO.Directory.GetFiles($"{path}/DBBack");
                    foreach (var file in files)
                    {
                        if (DateTime.TryParse(System.IO.Path.GetFileNameWithoutExtension(file), out DateTime nfilename))
                        {
                            if ((nfilename - DateTime.Now).TotalDays > 30)
                            {
                                try
                                {
                                    System.IO.File.Delete(file);
                                }
                                catch
                                {

                                    continue;
                                }
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"数据备份失败:{ex}");
                }
            }
            Log.Information("数据库备份成功");//*/
            return System.Threading.Tasks.Task.FromResult("");

        }


    }
}
