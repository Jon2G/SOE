using FirestoreLINQ;

using Kit;
using Kit.Model;
using Kit.Sql.Attributes;
using NameGenerator;
using NameGenerator.Generators;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using SOE.API;
using SOE.Enums;
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
                _NickName = value?.Trim();
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

        public string ApplicationPassword { get; set; }
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
        public bool HasSubjects { get; set; }
        public UserMode Mode { get; set; }
        public User() { }

        public async Task<School> GetSchool()
        {
            School = await Models.School.Get();
            return School;
        }
        private static async Task<User> Get(IDocumentReference docRef)
        {
            IDocumentSnapshot snapshot = await docRef.GetAsync();
            User? user = snapshot.ToObject<User>();
            if (user is not null)
            {
                user.Settings ??= new();
                user.Credits ??= new();
                await user.GetSchool();
            }
            return user ?? new User() { Settings = new Settings() };
        }
        internal static Task<User> Get() => Get(FireBaseConnection.UserDocument);


        public Task<User> Save()
        {
            this.DocumentId = FireBaseConnection.UserDocument.Id;
            return FireBaseConnection.UserDocument.SetAsync(this)
                .ContinueWith(t => this);
        }

        public async static Task<bool> Exists(string boleta)
        {
            User user = await
                Get(FireBaseConnection.GetUserDocument(FireBaseConnection.GetUserPath(boleta)));
            return user.Boleta == boleta;
        }
        public static async Task<bool> LogIn(string boleta, string password)
        {
            User user = await
            Get(FireBaseConnection.GetUserDocument(FireBaseConnection.GetUserPath(boleta)));
            return (user.ApplicationPassword == password);
        }

        internal static string GetRandomNickName()
        {
            string nickName = string.Empty;
            while (!Models.Data.Validations.IsValidNickName(nickName))
            {
                GamerTagGenerator tagGenerator = new()
                {
                    SpaceCharacter = "_",
                    Sex = GeneratorBase.SexTypes.All,
                    GeneratorFlags = GeneratorBase.NameTypes.None
                };
                nickName = tagGenerator.Generate();
            }
            return nickName;
        }
    }
}
