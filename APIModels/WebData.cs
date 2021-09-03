using Kit.Sql.SqlServer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace SOEWeb.Shared
{
    public static class WebData
    {
        public static string ConnectionString { get; set; }
        public static SqlConnection Connection => new SqlConnection(ConnectionString);


    }
}
