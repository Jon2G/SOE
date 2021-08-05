using Kit.Sql.Attributes;
using Kit.Sql.SqlServer;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SOEWeb.Models
{
    public class User
    {
        [Column("ID"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Column("GUID"), NotNull, Unique]
        public Guid Guid { get; set; }
        [Column("TYPE")]
        public int Type { get; set; }
        [Column("NAME"), MaxLength(100)]
        public string Name { get; set; }
        [Column("BOLETA"), MaxLength(11)]
        public string Boleta { get; set; }
        [Column("MAIL"), MaxLength(100)]
        public string Mail { get; set; }
        [Column("PASSWORD_PIN"), MaxLength(5), NotNull]
        public string PasswordPin { get; set; }
        public int Strikes { get; set; }
        public bool Banned { get; set; }
        public bool Deleted { get; set; }

        //internal static User GetByDevice(SQLServerConnection Connection, Device device) =>
        //    Connection.Table<User>().FirstOrDefault(x => x.Id == device.UserId);

        //public static User GetByBoleta(SQLServerConnection Connection, string boleta)
        //    => Connection.Table<User>().FirstOrDefault(x => x.Boleta == boleta || x.Mail == boleta);

        internal static int GetId(SQLServerConnection Connection, string user)
        {
            return Connection.Single<int>("SP_GET_USER_ID", CommandType.StoredProcedure,
                new SqlParameter("USER", user));
        }
    }
}
