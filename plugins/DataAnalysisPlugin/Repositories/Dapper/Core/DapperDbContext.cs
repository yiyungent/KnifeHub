using PluginCore;
using System.Data;
using System.Data.SQLite;

namespace DataAnalysisPlugin.Repositories.Dapper.Core
{
    public class DapperDbContext
    {
        public string DbFilePath
        {
            get
            {
                var dbFilePath = Path.Combine(PluginPathProvider.PluginsRootPath(), nameof(DataAnalysisPlugin), $"{nameof(DataAnalysisPlugin)}.sqlite");

                return dbFilePath;
            }
        }

        public string ConnStr
        {
            get
            {
                return $"Data Source={DbFilePath};Cache Size=0"; // 连接符字串
            }
        }

        public IDbConnection CreateConnection()
            => new SQLiteConnection(ConnStr);
    }
}
