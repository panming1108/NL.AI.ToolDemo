using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL.AI.ToolDemo.DAL.DBConnect
{
    public class DbBaseManagement
    {
        private IDbHelper _dh;
        private IDbConnection Customize()
        {
            try
            {
                _dh = new SqliteDbHelper();

                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB/NL.AIToolsData.db");
                string connectionstr = string.Format("Data Source={0};Version=3", path); //链接字符串
                SQLiteConnection sqlc = new SQLiteConnection(connectionstr);
                return sqlc;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        protected Task<IEnumerable<T>> SqlQueryList<T>(string sql, object param = null)
        {
            var dc = Customize();
            return _dh.QueryList<T>(dc, sql, param);
        }

        protected Task<int> SqlExcute(string sql, object param = null)
        {
            var dc = Customize();
            return _dh.ExcuteSql(dc, sql, param);
        }

        protected Task<T> ExecuteScalar<T>(string sql, object param = null)
        {
            var dc = Customize();
            return _dh.ExecuteScalar<T>(dc, sql, param);
        }
    }
}
