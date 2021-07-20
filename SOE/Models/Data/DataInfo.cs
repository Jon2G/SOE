using System.Linq;
using SOEWeb.Shared;
using Kit.Model;
using Kit.Sql.Attributes;
using SOE.Data;
using SOE.Models.Scheduler;

namespace SOE.Models.Data
{
    public class DataInfo : ModelBase
    {
        private string _Boleta;
        [PrimaryKey, MaxLength(10)]
        public string Boleta { get => _Boleta; set { _Boleta = value; Raise(() => Boleta); } }

        public static bool HasTimeTable()
        {
            return AppData.Instance.LiteConnection.Table<Subject>().Any();
        } 
    }
}
