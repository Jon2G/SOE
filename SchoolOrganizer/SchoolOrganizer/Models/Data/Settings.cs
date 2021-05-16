using System;
using System.Collections.Generic;
using System.Text;
using Kit.Forms.Extensions;
using Kit.Model;
using Kit.Sql.Attributes;
using Kit.Sql.Interfaces;

using SchoolOrganizer.Data;
using SchoolOrganizer.Notifications;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SchoolOrganizer.Models.Data
{
    public class Settings : ModelBase,IGuid
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Unique, MaxLength(10)]
        public string Boleta { get; set; }
        private bool _IsNotificationsActive;
        public bool IsNotificationsActive
        {
            get => _IsNotificationsActive;
            set
            {
                _IsNotificationsActive = value;
                Raise(() => IsNotificationsActive);
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
