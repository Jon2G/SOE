
using Kit.Model;
using Plugin.CloudFirestore.Attributes;
using SOE.Enums;
using System;

namespace SOE.Models.TodoModels
{
    [Xamarin.Forms.Internals.Preserve(AllMembers =true),Serializable]
    public class DocumentPart : ModelBase
    {
        private string? _Content;

        public string? Content
        {
            get => _Content;
            set
            {
                if (_Content != value)
                {
                    _Content = value;
                    Raise(() => Content);
                }
            }
        }

        public DocType DocType { get; set; }
        public DocumentPart()
        {
            Content = string.Empty;
        }
    }
}
