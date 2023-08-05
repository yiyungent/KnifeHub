using Dapper;
using PluginCore;
using TgBotStatPlugin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// using DapperExtensions;

namespace TgBotStatPlugin
{
    public class DbContext
    {
        public static string DbFilePath
        {
            get
            {
                string dbFilePath = Path.Combine(PluginPathProvider.PluginsRootPath(), nameof(TgBotStatPlugin), $"{nameof(TgBotStatPlugin)}.sqlite");

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

        public async static Task<long> CountByGroupId(string groupId)
        {
            using (IDbConnection con = new SQLiteConnection(ConnStr))
            {
                con.Open();

                string sql = @"SELECT COUNT(*) FROM Message
                               WHERE GroupId = @GroupId;";

                return await con.QueryFirstAsync<long>(sql, new
                {
                    GroupId = groupId
                });
            }
        }

        public async static Task<IEnumerable<Message>> QueryByGroupId(string groupId, Pager pager)
        {
            using (IDbConnection con = new SQLiteConnection(ConnStr))
            {
                con.Open();

                string sql = @"SELECT * FROM Message 
                               WHERE GroupId = @GroupId
                               ORDER BY Id
                               LIMIT @PageSize OFFSET (@PageIndex * @PageSize);";

                return await con.QueryAsync<Message>(sql, new
                {
                    GroupId = groupId,
                    PageSize = pager.PageSize,
                    // PageIndex 从 0 开始
                    PageIndex = pager.Page - 1
                });
            }
        }
    }

    public class Pager
    {
        public int Page { get; set; }
        public int PageSize { get; set; }

        public int Offset { get; set; }
        public int Next { get; set; }

        public Pager(int page, int pageSize = 10)
        {
            Page = page < 1 ? 1 : page;
            PageSize = pageSize < 1 ? 10 : pageSize;

            Next = pageSize;
            Offset = (Page - 1) * Next;
        }
    }
}
