
using Kit.Model;
using Plugin.CloudFirestore.Attributes;
using SOE.Enums;

namespace SOE.Models.TodoModels
{

    public class DocumentPart : ModelBase
    {
        [Id]
        public string DocumentId { get; set; }
        private string _Content;

        public string Content
        {
            get => _Content;
            set
            {
                _Content = value;
                Raise(() => Content);
            }
        }

        public DocType DocType { get; set; }
        public DocumentPart()
        {

        }
    }
}
