using Dapper;
using PluginCore;
using QQStatPlugin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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

        public static string ConnStr
        {
            get
            {
                return $"Data Source={DbFilePath};Cache Size=0"; // 连接符字串
            }
        }


        public static int InsertIntoMessage(Message model)
        {
            using (IDbConnection con = new SQLiteConnection(ConnStr))
            {
                con.Open();

                string sql = "INSERT INTO Message (QQName,QQUin,Content,GroupName,GroupUin,CreateTime) Values (@QQName,@QQUin,@Content,@GroupName,@GroupUin,@CreateTime);";

                return con.Execute(sql, model);
            }
        }

        public static List<Message> QueryAllMessage()
        {
            using (IDbConnection con = new SQLiteConnection(ConnStr))
            {
                con.Open();

                string sql = "SELECT * FROM Message;";

                return con.Query<Message>(sql).ToList();
            }
        }

        public async static Task<long> Count()
        {
            using (IDbConnection con = new SQLiteConnection(ConnStr))
            {
                con.Open();

                string sql = "SELECT COUNT(*) FROM Message;";

                return await con.QueryFirstAsync<long>(sql);
            }
        }

        public async static Task<IEnumerable<(string QQUin, long TotalContentLen)>> TopByGroup(string groupUin)
        {
            using (IDbConnection con = new SQLiteConnection(ConnStr))
            {
                con.Open();

                string sql = @"SELECT QQUin, SUM(ContentLen) AS TotalContentLen
                                FROM(SELECT Id, QQUin, LENGTH(Content) AS ContentLen FROM Message WHERE GroupUin = @GroupUin)
                                GROUP BY QQUin
                                ORDER BY TotalContentLen DESC
                                LIMIT 10;";

                return await con.QueryAsync<(string QQUin, long TotalContentLen)>(sql, new
                {
                    GroupUin = groupUin
                });
            }
        }

    }
}
