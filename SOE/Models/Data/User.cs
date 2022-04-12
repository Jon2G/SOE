using FirestoreLINQ;

using Kit;
using Kit.Model;
using Kit.Sql.Attributes;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using SOE.API;
using SOE.Models.Academic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SOE.Models.Data
{
    [FireStoreCollection("Users")]
    public class User : ModelBase
    {
        [Id]
        public string DocumentId { get; set; }

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
        [Ignored]
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
        [Ignored]
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
        [Ignored]
        public string _Name;

        public string Name { get => _Name; set { _Name = value; Raise(() => Name); } }

        public static string GetNickName(int UserId, SqlConnection con) =>
            con.Single<string>("SP_GET_NICKNAME", CommandType.StoredProcedure,
                new SqlParameter("USER_ID", UserId));


        private Settings _Settings;

        public Settings Settings
        {
            get => _Settings;
            set
            {
                _Settings = value;
                Raise(() => Settings);
            }
        }

        private Credits _Credits;

        public Credits Credits
        {
            get => _Credits;
            set
            {
                this._Credits = value;
                Raise(() => Credits);
            }
        }

        private string _InscriptionDate;

        public string InscriptionDate
        {
            get => _InscriptionDate;
            set
            {
                _InscriptionDate = value;
                Raise(() => InscriptionDate);
            }
        }
        public async Task<School> GetSchool()
        {
            School = await Models.School.Get();
            return School;
        }

        internal static async Task<User> Get()
        {
            IDocumentReference docRef = FireBaseConnection.UserDocument;
            IDocumentSnapshot snapshot = await docRef.GetAsync();
            var user = snapshot.ToObject<User>();
            if (user is not null)
            {
                user.Settings ??= new();
                user.Credits ??= new();
                await user.GetSchool();
            }

            return user ?? new User() { Settings = new Settings() };
        }

        public bool HasSubjects { get; set; }
        public User() { }

        public Task<User> Save()
        {
            this.DocumentId = FireBaseConnection.UserDocument.Id;
            return FireBaseConnection.UserDocument.SetAsync(this)
                .ContinueWith(t => this);
        }
    }
}
