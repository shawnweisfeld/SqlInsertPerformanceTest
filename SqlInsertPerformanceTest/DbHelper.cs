using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlInsertPerformanceTest
{
    public class DbHelper
    {
        public static int ExecuteNonQuery<T>(string query, string connectionString, IEnumerable<T> objs, Func<T, IDbDataParameter[]> getParms)
        {
            var result = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                foreach (var obj in objs)
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandTimeout = int.MaxValue;
                        command.Transaction = transaction;
                        command.Parameters.AddRange(getParms(obj));
                        result += command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();

                connection.Close();
                return result;
            }
        }

        public static IDbDataParameter CreateParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }

    }
}
