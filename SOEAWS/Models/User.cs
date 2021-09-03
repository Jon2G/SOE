using Kit;
using Kit.Sql.Attributes;
using Kit.Sql.SqlServer;
using SOEWeb.Shared;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SOEAWS.Models
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

        public static string GetNickName(int UserId) =>
            WebData.Connection.Single<string>("SP_GET_NICKNAME", CommandType.StoredProcedure,
                new SqlParameter("USER_ID", UserId));
        internal static int GetId(string user)
        {
            try
            {
                return WebData.Connection.Single<int>("SP_GET_USER_ID", CommandType.StoredProcedure,
                    new SqlParameter("USER", user));
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "User.GetId");
            }

            return -1;
        }
    }
}
