using Kit.Model;
using Kit.Sql.Attributes;

namespace SOEWeb.Shared
{
    public class UserBase : ModelBase
    {
        [PrimaryKey]
        public int Id { get; set; }
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
    }
}
