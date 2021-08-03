using Kit.Sql.Attributes;
using SOE.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOE.Models.Data
{
    public class InscriptionDate
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        public string Date { get;  set; }

        public InscriptionDate()
        {
            
        }
        public InscriptionDate(string Date)
        {
            this.Date = Date;
        }

        public void Save()
        {
            AppData.Instance.LiteConnection.DeleteAll<InscriptionDate>();
            AppData.Instance.LiteConnection.Insert(this);
        }

        public static InscriptionDate Get() => AppData.Instance.LiteConnection.Table<InscriptionDate>().FirstOrDefault();
    }
}
