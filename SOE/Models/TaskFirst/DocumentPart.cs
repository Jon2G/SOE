using System;
using System.Collections.Generic;
using Kit.Model;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using SOE.Data;
using SOE.Enums;

namespace SOE.Models.TaskFirst
{

    public class DocumentPart : ModelBase, IGuid
    {
        private string _Content;
        [PrimaryKey, AutoIncrement]
        public Guid Guid { get; set; }

        public string Content
        {
            get => _Content;
            set
            {
                _Content = value;
                Raise(() => Content);
            }
        }
        public Guid IdDocumento { get; set; }
        public DocType DocType { get; set; }
        public void Save(Document origin)
        {
            this.IdDocumento = origin.Guid;
            AppData.Instance.LiteConnection.Insert(this, false);
        }

        internal static IEnumerable<DocumentPart> GetDoc(Guid idDocument)
        {
            return AppData.Instance.LiteConnection.Table<DocumentPart>().Where(x => x.IdDocumento == idDocument);
        }
    }
}
