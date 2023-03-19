using Dapper;
using PluginCore;
using TgClientPlugin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgClientPlugin
{
    public class DbContext
    {
        public static string DbFilePath
        {
            get
            {
                string dbFilePath = Path.Combine(PluginPathProvider.PluginsRootPath(), nameof(TgClientPlugin), $"{nameof(TgClientPlugin)}.sqlite");

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

                string sql = "INSERT INTO Message (UName,UId,Content,GroupName,GroupId,CreateTime) Values (@UName,@UId,@Content,@GroupName,@GroupId,@CreateTime);";

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

        public async static Task<IEnumerable<(string UId, long TotalContentLen)>> TopByGroup(string groupId)
        {
            using (IDbConnection con = new SQLiteConnection(ConnStr))
            {
                con.Open();

                string sql = @"SELECT UId, SUM(ContentLen) AS TotalContentLen
                                FROM(SELECT Id, UId, LENGTH(Content) AS ContentLen FROM Message WHERE GroupId = @GroupId)
                                GROUP BY UId
                                ORDER BY TotalContentLen DESC
                                LIMIT 10;";

                return await con.QueryAsync<(string UId, long TotalContentLen)>(sql, new
                {
                    GroupId = groupId
                });
            }
        }

    }
}
