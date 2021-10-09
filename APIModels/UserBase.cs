using Kit;
using Kit.Model;
using Kit.Sql.Attributes;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SOEWeb.Shared
{
    public class UserBase : ModelBase
    {
        [PrimaryKey]
        public int Id { get; set; }

        private string _NickName;
        public string NickName
        {
            get => this._NickName;
            set
            {
                _NickName = value;
                this.Raise(() => this.NickName);
            }
        }

        private string _Boleta;

        [Column("Boleta"),MaxLength(10)]
        public string Boleta
        {
            get => this._Boleta;
            set
            {
                if (!Validations.IsValidBoleta(value))
                {
                    return;
                }
                this._Boleta = value;
                this.Raise(() => this.Boleta);

            }
        }
        private string _Password;
        public string Password { get => this._Password; set { this._Password = value; this.Raise(() => this.Password); } }
        [Ignore]
        public School School
        {
            get => this._School;
            set
            {
                this._School = value;
                this.Raise(() => this.School);
            }
        }
        private School _School;


        public static string GetNickName(int UserId,SqlConnection con) =>
            con.Single<string>("SP_GET_NICKNAME", CommandType.StoredProcedure,
                new SqlParameter("USER_ID", UserId));
        internal static int GetId(string user, SqlConnection con)
        {
            try
            {
                return con.Single<int>("SP_GET_USER_ID", CommandType.StoredProcedure,
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
