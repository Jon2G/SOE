using System;
using System.Collections.Generic;
using System.Text;
using Kit.Model;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Academic;
using SchoolOrganizer.Models.Data;

namespace SchoolOrganizer.ViewModels.Pages
{
    public class UserProfileViewModel : ModelBase
    {
        public Credits Credits { get; set; }
        public User User { get; set; }
        public string Semester { get; set; }
        public double Progress { get; set; }

        public UserProfileViewModel()
        {
            this.Semester = "7";
            this.User = AppData.Instance.User;
            this.Credits =
                AppData.Instance.LiteConnection.Table<Credits>().First();

            this.Progress = Credits.Percentage / 100;
        }
    }

}
