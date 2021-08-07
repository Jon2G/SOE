using Kit.Sql.SqlServer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace SOEWeb.Shared
{
    public static class WebData
    {
        public static  SQLServerConnection Connection;
        private static string  ConnectionString=>Connection.ConnectionString.ToString();

        public static SqlConnection Con()
        {
            return new SqlConnection(ConnectionString);
        }

    }
}
