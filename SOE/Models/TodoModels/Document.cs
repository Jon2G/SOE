using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Kit.Model;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using SOE.Data;

namespace SOE.Models.TodoModels
{
    public class Document : ModelBase, IGuid
    {
        [PrimaryKey, AutoIncrement]
        public Guid Guid { get; set; }
        public void Save()
        {
            AppData.Instance.LiteConnection.Insert(this, false);
        }

        internal static void Delete(Guid idDocument)
        {
            AppData.Instance.LiteConnection.Table<DocumentPart>().Where(x => x.IdDocumento == idDocument).Delete();
            AppData.Instance.LiteConnection.Table<Document>().Where(x => x.Guid == idDocument).Delete();
        }


        internal static Document PaseAndSave(string Description)
        {
            Regex regex = new(@"(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var files = new List<string>();
            List<DocumentPart> Contenido = new List<DocumentPart>();
            MatchCollection matches = regex.Matches(Description);
            if (matches.Count==0)
            {
                Contenido.Add(new DocumentPart() { Content = Description, DocType = Enums.DocType.Text });
            }
            else
            {
                foreach (Match match in matches)
                {
                    files.Add(match.ToString());
                }
                //obtiene la primera pocision
                int last_index = 0;
                if (files.Any())
                {
                    int start = Description.IndexOf(files[0]);
                    if (start > 0)
                    {
                        string con = Description.Substring(0, start);
                        Contenido.Add(new DocumentPart() { Content = con, DocType = Enums.DocType.Text });
                    }
                    last_index = start + files[0].Length;
                    Contenido.Add(new DocumentPart() { Content = files[0], DocType = Enums.DocType.Link });
                }
                //Separa texto
                for (int i = 1; i < files.Count; i++)
                {
                    string file = files[i];
                    string contexto = Description;
                    int s_index = Description.IndexOf(file);
                    if (last_index > 0)
                    {
                        contexto = Description.Substring(last_index, s_index - last_index);
                        Contenido.Add(new DocumentPart() { Content = contexto, DocType = Enums.DocType.Text });
                        last_index = s_index + file.Length;
                    }
                    Contenido.Add(new DocumentPart() { Content = file, DocType = Enums.DocType.Link });
                }
            }
           
            //
            Document doc = new Document();
            doc.Save();
            Contenido.ForEach(c => c.Save(doc));
            return doc;
        }
    }
}
