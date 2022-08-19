using LiteDB;
using SOE.Data;
using SOE.Enums;

namespace SOE.Notifications
{
    public class ProgrammedNotifications
    {
        [BsonId(true)]
        public int Id { get; set; }
        public NotificationType Type { get; set; }
        public string DocumentId { get; set; }

        static ProgrammedNotifications()
        {

        }
        public ProgrammedNotifications()
        {

        }

        public static ProgrammedNotifications? Get(NotificationType type, string documentId)
        {
            return AppData.Instance.LiteDatabase.GetCollection<ProgrammedNotifications>()
                .FindOne(x => x.Type == type && x.DocumentId == documentId);
        }

        public static ProgrammedNotifications Save(NotificationType type, string documentId)
        {
            ProgrammedNotifications? obj = new ProgrammedNotifications() { DocumentId = documentId, Type = type };
            AppData.Instance.LiteDatabase.GetCollection<ProgrammedNotifications>().Upsert(obj);
            return obj;
        }
    }
}
