using Kit.Model;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using SchoolOrganizer.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolOrganizer.Models.TaskFirst
{
    public class Document : ModelBase, IGuid
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public void Save()
        {
            AppData.Instance.LiteConnection.Insert(this);
        }

        internal static void Delete(int idDocument)
        {
            AppData.Instance.LiteConnection.Table<DocumentPart>().Where(x => x.IdDocumento == idDocument).Delete();
            AppData.Instance.LiteConnection.Table<Document>().Where(x => x.Id == idDocument).Delete();
        }
    }
}
