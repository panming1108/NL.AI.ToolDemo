using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NL.AI.ToolDemo.DAL.DBConnect
{
    public interface IDbHelper
    {
        Task<IEnumerable<T>> QueryList<T>(IDbConnection iDbConnection, string sql, object param = null);
        Task<int> ExcuteSql(IDbConnection iDbConnection, string sql, object param = null);
        Task<T> ExecuteScalar<T>(IDbConnection iDbConnection, string sql, object param = null);
    }
}
