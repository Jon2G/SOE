using FirestoreLINQ;
using Google.Cloud.Firestore;
using Kit;
using Kit.Model;
using Kit.Sql.Attributes;
using SOE.API;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SOE.Models.Data
{
    [FireStoreCollection("Users"), FirestoreData]
    public class User : ModelBase
    {
        [FirestoreDocumentId]
        public string DocumentId { get; set; }

        private string _NickName;
        [FirestoreProperty]
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

        [Column("Boleta"), MaxLength(10), FirestoreProperty]
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
        [FirestoreProperty]
        public string Password { get => this._Password; set { this._Password = value; this.Raise(() => this.Password); } }
        [FirestoreProperty]
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
        [FirestoreProperty]
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
        [FirestoreProperty]
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
        [FirestoreProperty]
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
        [FirestoreProperty]
        public string Name { get => _Name; set { _Name = value; Raise(() => Name); } }

        public static string GetNickName(int UserId, SqlConnection con) =>
            con.Single<string>("SP_GET_NICKNAME", CommandType.StoredProcedure,
                new SqlParameter("USER_ID", UserId));
        internal static Guid GetId(string user, SqlConnection con)
        {
            throw new NotImplementedException();
            //try
            //{
            //    return con.Single<Guid>("SP_GET_USER_ID", CommandType.StoredProcedure,
            //        new SqlParameter("USER", user));
            //}
            //catch (Exception ex)
            //{
            //    Log.Logger.Error(ex, "User.GetId");
            //}

            //return Guid.Empty;
        }


        private Settings _Settings;
        [FirestoreProperty]
        public Settings Settings
        {
            get => _Settings;
            set
            {
                _Settings = value;
                Raise(() => Settings);
            }
        }

        internal static async Task<User> Get()
        {
            DocumentReference docRef = FireBaseConnection.Instance.UserDocument;
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            var user = snapshot.ConvertTo<User>();
            user.Settings ??= new();
            return user;
        }
        [FirestoreProperty]
        public bool HasSubjects { get; set; }
        public User() { }

        public async Task<User> Save()
        {
            await Task.Yield();
            var wr = await FireBaseConnection.Instance.UserDocument.SetAsync(this);
            this.DocumentId = FireBaseConnection.Instance.UserDocument.Id;
            return this;
        }

    }
}
