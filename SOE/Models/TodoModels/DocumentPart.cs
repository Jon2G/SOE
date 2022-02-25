using Google.Cloud.Firestore;
using Kit.Model;
using SOE.Enums;

namespace SOE.Models.TodoModels
{
    [FirestoreData]
    public class DocumentPart : ModelBase
    {
        [FirestoreDocumentId]
        public string DocumentId { get; set; }
        private string _Content;
        [FirestoreProperty]
        public string Content
        {
            get => _Content;
            set
            {
                _Content = value;
                Raise(() => Content);
            }
        }
        [FirestoreProperty]
        public DocType DocType { get; set; }
        public DocumentPart()
        {

        }
    }
}
