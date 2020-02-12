using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL.AI.ToolDemo.DAL.DBConnect
{
    public class SqliteDbHelper : IDbHelper
    {
        public Task<int> ExcuteSql(IDbConnection iDbConnection, string sql, object param = null)
        {
            var result = ConnectionStateSwitch(() => iDbConnection.Execute(sql, param), iDbConnection);
            return result;
        }

        public Task<IEnumerable<T>> QueryList<T>(IDbConnection iDbConnection, string sql, object param = null)
        {
            var result = ConnectionStateSwitch(() => iDbConnection.Query<T>(sql, param), iDbConnection);
            return result;
        }

        public Task<T> ExecuteScalar<T>(IDbConnection iDbConnection, string sql, object param = null)
        {
            var result = ConnectionStateSwitch(() => iDbConnection.ExecuteScalar<T>(sql, param), iDbConnection);
            return result;
        }

        public Task<T> ConnectionStateSwitch<T>(Func<T> fun, IDbConnection iDbConnection)
        {
            if (iDbConnection.State == ConnectionState.Closed || iDbConnection.State == ConnectionState.Broken)
            {
                iDbConnection.Close();
                iDbConnection.Open();
            }

            var result = Task.Factory.StartNew(() =>
            {
                try
                {
                    return fun();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    iDbConnection.Close();
                    iDbConnection.Dispose();
                }


            });




            return result;
        }
    }
}
