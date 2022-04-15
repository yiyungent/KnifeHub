using PluginCore;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQStatPlugin
{
    public class DbContext
    {
        public static string DbFilePath
        {
            get
            {
                string dbFilePath = Path.Combine(PluginPathProvider.PluginsRootPath(), nameof(QQStatPlugin), $"{nameof(QQStatPlugin)}.sqlite");

                return dbFilePath;
            }
        }


        public static SqlSugarScope Instance = new SqlSugarScope(new ConnectionConfig()
        {
            ConnectionString = $"Server={DbFilePath}",//连接符字串
            DbType = DbType.Sqlite,//数据库类型
            IsAutoCloseConnection = true //不设成true要手动close
        },
          db =>
          {
              //(A)全局生效配置点
              //调试SQL事件，可以删掉
              db.Aop.OnLogExecuting = (sql, pars) =>
              {
                  Console.WriteLine(sql);//输出sql,查看执行sql
              };
          });


    }
}
