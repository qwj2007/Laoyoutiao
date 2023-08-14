using Laoyoutiao.Common;
using Laoyoutiao.Models.Common;
using Laoyoutiao.Models.Entitys;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using SqlSugar.IOC;
using System.Reflection;

namespace Laoyoutiao.webapi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ToolController : ControllerBase
    {
        private ISqlSugarClient? _db = null;
        public ToolController()
        {

        }
        /// <summary>
        /// 自动创建表
        /// </summary>
        /// <param name="ConnectionConfigs">连接数据库配置的名称</param>
        [HttpGet]
        public void InitDataBase(string ConnectionConfigs = "ConnectionConfigs")
        {
            var files = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Laoyoutiao.Models.dll");
            List<IocConfig> connectionConfigs = AppSettings.App<IocConfig>(new string[] { ConnectionConfigs });
            Type[] typeall = Assembly.LoadFrom(files[0]).GetTypes();
            foreach (var item in connectionConfigs)
            {
                _db = DbScoped.SugarScope.GetConnection(item.ConfigId ?? "0");
                //string flag = "ok";
                //如果不存在则创建数据库
                _db.DbMaintenance.CreateDatabase();

                if (files.Length > 0)
                {
                    Type[] types = typeall.Where(a=> a.IsDefined(typeof(TenantAttribute))&&a.IsDefined(typeof(SugarTable))&& a.GetCustomAttribute<TenantAttribute>().configId.ToString() == item.ConfigId).ToArray();
                    //更新数据库字段，如果有修改就更新
                    //Type[] types = Assembly.LoadFrom(files[0]).GetTypes().Where(it => it.BaseType == typeof(BaseEntity) && it.GetCustomAttribute<TenantAttribute>().configId.ToString() == item.ConfigId).ToArray();
                    _db.CodeFirst.SetStringDefaultLength(200).InitTables(types);
                 
                    //Type[] typetree = typeall.Where(a => a.BaseType == typeof(BaseTreeEntity<>) && a.IsDefined(typeof(TenantAttribute)) 
                    //&& a.GetCustomAttribute<TenantAttribute>().configId.ToString() == item.ConfigId).ToArray();
                    //_db.CodeFirst.SetStringDefaultLength(200).InitTables(typetree);
                }
            }
        }

        private void CreateDB(ISqlSugarClient client, string configID)
        {

            client.DbMaintenance.CreateDatabase();//没有数据库的时候创建数据库
            //var tableLists = client.DbMaintenance.GetTableInfoList();
            var files = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Laoyoutiao.Models.dll");
            if (files.Length > 0)
            {

                //Type[] types = Assembly.LoadFrom(files[0]).GetTypes().Where(it => it.BaseType == typeof(BaseEntity)).ToArray();
                //更新数据库字段，如果有修改就更新
                Type[] types = Assembly.LoadFrom(files[0]).GetTypes().Where(it => it.BaseType == typeof(BaseKey) && it.GetCustomAttribute<TenantAttribute>().configId.ToString() == configID).ToArray();
                client.CodeFirst.SetStringDefaultLength(200).InitTables(types);

                //foreach (var entityType in types)
                //{
                //    //创建数据表
                //    string tableName = entityType.GetCustomAttribute<SugarTable>().TableName;//根据特性获取表名称
                //    var configid = entityType.GetCustomAttribute<TenantAttribute>()?.configId;//根据特性获取租户id
                //    configid = configid == null ? "0" : configid.ToString();
                //    if (!tableLists.Any(p => p.Name == tableName))
                //    {
                //        //创建数据表包括字段更新
                //        client.CodeFirst.SetStringDefaultLength(200).InitTables(entityType);
                //    }
                //}
            }
        }
    }
}
