using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental
{
    public class DatabaseManager
    {
        private static readonly string connectionString = @"DATA SOURCE=192.168.1.21:1521/XE;" + "USER ID=cardb; PASSWORD=carrental";

        public static DataTable ExecuteQuery(string query)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    connection.Open();
                    DataTable dataTable = new DataTable();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                    return dataTable;
                }
            }
        }
    }
}
