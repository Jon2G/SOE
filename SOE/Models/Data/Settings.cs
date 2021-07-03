using System;
using Kit.Model;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;
using SOE.Data;

namespace SOE.Models.Data
{
    [Preserve]
    public class Settings : ModelBase,IGuid
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

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
        public Settings()
        {
            this.ShowTimelineBar = true;
        }
        public Guid Guid { get; set; }
        internal void Save()
        {
            AppData.Instance.LiteConnection.InsertOrReplace(this);
            Notifications();
        }
        public void Notifications()
        {
            //if (IsNotificationsActive)
            //{
            //}
        }
    }
}
