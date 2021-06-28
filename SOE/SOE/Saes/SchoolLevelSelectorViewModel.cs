using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Kit.Model;
using SchoolOrganizer.Data;
using Xamarin.Forms;

namespace SchoolOrganizer.Saes
{
    public class SchoolLevelSelectorViewModel : ModelBase
    {
        public ICommand HighSchoolCommand { get; set; }
        public ICommand UniversityCommand { get; set; }

        public SchoolLevelSelectorViewModel()
        {
            this.HighSchoolCommand = new Command(HighSchool);
            this.UniversityCommand = new Command(University);
        }
        private void HighSchool()
        {
            SelectSchool(SchoolLevel.HighSchool);
        }

        private void University()
        {
            SelectSchool(SchoolLevel.University);
        }

        private void SelectSchool(SchoolLevel level)
        {
           App.Current.MainPage=new SchoolSelector(level);
        }
    }
}
