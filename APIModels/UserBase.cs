using Kit;
using Kit.Model;
using Kit.Services.Web;
using Kit.Sql.Attributes;
using Kit.Sql.Sqlite;
using SOEWeb.Shared.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SOEWeb.Shared
{
    public class UserBase : OfflineSync
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

        [Column("Boleta"), MaxLength(10)]
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
        private string _Semester;
        public string Semester
        {
            get => _Semester;
            set
            {
                _Semester = value;
                Raise(() => Semester);
            }
        }

        private string _Career;
        public string Career
        {
            get => _Career;
            set
            {
                _Career = value;
                Raise(() => Career);
            }
        }

        public string _Email;
        public string Email
        {
            get => _Email;
            set
            {
                _Email = value;
                Raise(() => Email);
            }
        }

        public string _Name;
        public string Name { get => _Name; set { _Name = value; Raise(() => Name); } }

        public static string GetNickName(int UserId, SqlConnection con) =>
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

        public override async Task<bool> Sync(IApplicationData app, ISyncService apiService)
        {
            await Task.Yield();
            Response<UserBase> response = await apiService.Sync(this);
            if (response.ResponseResult == APIResponseResult.OK)
            {
                return true;
            }
            return false;
        }
    }
}
