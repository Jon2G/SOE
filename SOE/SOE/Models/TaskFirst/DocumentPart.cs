using Kit.Model;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using SchoolOrganizer.Enums;
using SchoolOrganizer.Data;

namespace SchoolOrganizer.Models.TaskFirst
{

    public class DocumentPart : ModelBase, IGuid
    {
        private string _Content;
        public Guid Guid { get; set; }

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get;
            set;
        }
        public string Content
        {
            get => _Content;
            set
            {
                _Content = value;
                Raise(() => Content);
            }
        }
        public int IdDocumento { get; set; }
        public DocType DocType { get; set; }
        public void Save(Document origin)
        {
            this.IdDocumento = origin.Id;
            AppData.Instance.LiteConnection.Insert(this);
        }

        internal static IEnumerable<DocumentPart> GetDoc(int idDocument)
        {
            return AppData.Instance.LiteConnection.Table<DocumentPart>().Where(x => x.IdDocumento == idDocument);
        }
    }
}
