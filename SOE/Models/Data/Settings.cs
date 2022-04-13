
using Kit.Model;
using Kit.Sql.Attributes;

namespace SOE.Models.Data
{
    [Preserve]
    public class Settings : ModelBase
    {

        private bool _ShowTimelineBar;

        public bool ShowTimelineBar
        {
            get => _ShowTimelineBar;
            set
            {
                _ShowTimelineBar = value;
                Raise(() => ShowTimelineBar);
            }
        }
        private bool _IsFingerPrintActive;

        public bool IsFingerPrintActive
        {
            get => _IsFingerPrintActive;
            set
            {
                _IsFingerPrintActive = value;
                Raise(() => IsFingerPrintActive);
            }
        }

        private bool _ExpandCards;

        public bool ExpandCards
        {
            get => _ExpandCards;
            set
            {
                _ExpandCards = value;
                Raise(() => ExpandCards);
            }
        }


        public Settings()
        {
            this.ShowTimelineBar = true;
            this.ExpandCards = true;
        }
        //public static async Task<Settings> Get()
        //{
        //    var snap = await FireBaseConnection.UserDocument.Collection<Settings>().Document("Settings")
        //             .GetAsync();
        //    return snap.ToObject<Settings>()??new Settings();
        //}

        //internal async Task Save()
        //{
        //    await FireBaseConnection.UserDocument.Collection<Settings>().Document("Settings")
        //            .SetAsync(this);
        //    Notifications();
        //}
        public void Notifications()
        {
            //if (IsNotificationsActive)
            //{
            //}
        }
    }
}
