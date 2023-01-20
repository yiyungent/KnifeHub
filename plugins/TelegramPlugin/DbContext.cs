using Dapper;
using PluginCore;
using TelegramPlugin.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramPlugin
{
    public class DbContext
    {
        public static string DbFilePath
        {
            get
            {
                string dbFilePath = Path.Combine(PluginPathProvider.PluginsRootPath(), nameof(TelegramPlugin), $"{nameof(TelegramPlugin)}.sqlite");

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


        public static int InsertIntoQABox(QABox model)
        {
            using (IDbConnection con = new SQLiteConnection(ConnStr))
            {
                con.Open();

                string sql = "INSERT INTO QABox (Question,Answer,CreateTime,UpdateTime,QQGroup) Values (@Question,@Answer,@CreateTime,@UpdateTime,@QQGroup);";

                return con.Execute(sql, model);
            }
        }

        public static int UpdateQABox(QABox model)
        {
            using (IDbConnection con = new SQLiteConnection(ConnStr))
            {
                con.Open();

                string sql = "UPDATE QABox SET Question=@Question, Answer=@Answer, CreateTime=@CreateTime, UpdateTime=@UpdateTime, QQGroup=@QQGroup WHERE Id=@Id;";

                return con.Execute(sql, model);
            }
        }

        public static int DeleteQABox(QABox model)
        {
            using (IDbConnection con = new SQLiteConnection(ConnStr))
            {
                con.Open();

                string sql = "DELETE FROM QABox WHERE Id=@Id;";

                return con.Execute(sql, model);
            }
        }

        public static List<QABox> QueryAllQABox()
        {
            using (IDbConnection con = new SQLiteConnection(ConnStr))
            {
                con.Open();

                string sql = "SELECT * FROM QABox;";

                return con.Query<QABox>(sql).ToList();
            }
        }

    }
}
