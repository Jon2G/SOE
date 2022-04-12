using Plugin.CloudFirestore.Attributes;

namespace SOE.FireBase
{
    public interface IFireStoreObject
    {
        [Id]
        public string DocumentId { get; set; }

        public string GetDocumentId();
    }
}
