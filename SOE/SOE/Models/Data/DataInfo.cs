using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kit;
using Kit.Model;
using Kit.Sql.Attributes;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Scheduler;

namespace SchoolOrganizer.Models.Data
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
