using Serilog.Sinks.MSSqlServer;
using Serilog;
using System.Collections.ObjectModel;
using System.Net;

namespace Laoyoutiao.webapi.Config
{

    /// <summary>
    /// Serilog配置
    /// </summary>
    public static class SerilogConfig
    {
        public static void CreateLogger()
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "Logs/Serilog/log.txt");
            var logDB = @"Server=;database=Serilog;uidsa=;pwd=1qaz2wsx~;TrustServerCertificate=true";
            var sinkOpts = new MSSqlServerSinkOptions();
            sinkOpts.TableName = "Logs_Serilog";
            sinkOpts.AutoCreateSqlTable = true;
            sinkOpts.BatchPostingLimit = 1;
            sinkOpts.BatchPeriod = TimeSpan.FromSeconds(1);
            var columnOpts = new ColumnOptions();
            //columnOpts.Store.Remove(StandardColumn.Properties);
            //columnOpts.Store.Add(StandardColumn.LogEvent);
            //columnOpts.LogEvent.DataLength = 2048;
            //columnOpts.PrimaryKey = columnOpts.TimeStamp;
            //columnOpts.TimeStamp.NonClusteredIndex = true;
            columnOpts.Store.Remove(StandardColumn.MessageTemplate);
            columnOpts.Properties.ExcludeAdditionalProperties = true;
            columnOpts.AdditionalColumns = new Collection<SqlColumn>
    {
      new SqlColumn{DataType = System.Data.SqlDbType.NVarChar, DataLength = 32, ColumnName = "IP"}
    };

            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Information()
              .WriteTo.Console(
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ({ThreadId}) {Message}{NewLine}{Exception}")
              .WriteTo.File(filePath,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] ({ThreadId}) {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Hour,
                fileSizeLimitBytes: 1073741824) //1GB
                    .Enrich.FromLogContext()
              .Enrich.WithProperty("IP", GetIpAddress())
              //.WriteTo.MSSqlServer(
              //  connectionString: logDB,
              //  sinkOptions: sinkOpts,
              //  columnOptions: columnOpts
              //  )
              .CreateLogger();
        }

        public static void RefreshLogger()
        {
            if (Log.Logger != null)
            {
                Log.CloseAndFlush();
            }
            CreateLogger();
        }

        private static string GetIpAddress()
        {
            string ipAddress = "127.0.0.1";
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in ips)
            {
                if (ip.AddressFamily.ToString().ToLower().Equals("internetwork"))
                {
                    ipAddress = ip.ToString();
                    return ipAddress;
                }
            }

            return ipAddress;
        }
    }

}
