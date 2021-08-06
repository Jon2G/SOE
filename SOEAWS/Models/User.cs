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


        internal static int GetId(string user)
        {
            try
            {
                using (SqlConnection con = WebData.Con())
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SP_GET_USER_ID", con)
                    {
                        CommandType = CommandType.StoredProcedure
                    })
                    {
                        cmd.Parameters.Add(new SqlParameter("USER", user));
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return Convert.ToInt32(reader[0]);
                            }
                        }
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
            }

            return -1;
        }
    }
}
