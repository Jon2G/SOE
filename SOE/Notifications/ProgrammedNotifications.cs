using Kit.Sql.Attributes;
using SOE.Data;
using SOE.Enums;

namespace SOE.Notifications
{
    public class ProgrammedNotifications
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public NotificationType Type { get; set; }
        public string DocumentId { get; set; }

        static ProgrammedNotifications()
        {
            if (!AppData.Instance.LiteConnection.TableExists<ProgrammedNotifications>())
            {
                AppData.Instance.LiteConnection.CreateTable<ProgrammedNotifications>();
            }
        }
        public ProgrammedNotifications()
        {

        }

        public static ProgrammedNotifications? Get(NotificationType type, string documentId)
        {
            return AppData.Instance.LiteConnection.Table<ProgrammedNotifications>()
                 .FirstOrDefault(x => x.Type == type && x.DocumentId == documentId);
        }

        public static ProgrammedNotifications Save(NotificationType type, string documentId)
        {
            var obj = new ProgrammedNotifications() { DocumentId = documentId, Type = type };
            AppData.Instance.LiteConnection.Insert(obj);
            return obj;
        }
    }
}
