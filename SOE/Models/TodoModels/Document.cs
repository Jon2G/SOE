using FirestoreLINQ;
using Google.Cloud.Firestore;
using Kit.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SOE.Models.TodoModels
{
    [FirestoreData, FireStoreCollection("Document")]
    public class Document : ModelBase
    {
        [FirestoreDocumentId]
        public string DocumentId { get; set; }
        [FirestoreProperty]
        public List<DocumentPart> DocumentParts { get; set; }

        public Document()
        {
            DocumentParts = new List<DocumentPart>();
        }
        public Document Parse(string description)
        {
            Regex regex = new(@"(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            List<string> files = new List<string>();

            MatchCollection matches = regex.Matches(description);
            if (matches.Count == 0)
            {
                DocumentParts.Add(new DocumentPart() { Content = description, DocType = Enums.DocType.Text });
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
                    int start = description.IndexOf(files[0]);
                    if (start > 0)
                    {
                        string con = description.Substring(0, start);
                        DocumentParts.Add(new DocumentPart() { Content = con, DocType = Enums.DocType.Text });
                    }
                    last_index = start + files[0].Length;
                    DocumentParts.Add(new DocumentPart() { Content = files[0], DocType = Enums.DocType.Link });
                }
                //Separa texto
                for (int i = 1; i < files.Count; i++)
                {
                    string file = files[i];
                    string contexto = description;
                    int s_index = description.IndexOf(file);
                    if (last_index > 0)
                    {
                        contexto = description.Substring(last_index, s_index - last_index);
                        DocumentParts.Add(new DocumentPart() { Content = contexto, DocType = Enums.DocType.Text });
                        last_index = s_index + file.Length;
                    }
                    DocumentParts.Add(new DocumentPart() { Content = file, DocType = Enums.DocType.Link });
                }
            }
            return this;
        }
    }
}
