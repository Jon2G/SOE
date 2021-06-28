using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Kit.Model;
using P42.Utils;
using SchoolOrganizer.Data;
using SchoolOrganizer.Views.Pages;
using Xamarin.Forms;

namespace SchoolOrganizer.Saes
{
    public class SchoolSelectorViewModel : ModelBase
    {
        private readonly SchoolLevel Level;
        public ObservableCollection<School> Schools { get; set; }
        public ICommand SelectSchoolCommand { get; }
        public SchoolSelectorViewModel(SchoolLevel Level)
        {
            this.SelectSchoolCommand = new Command<School>(SelectSchool);
            this.Level = Level;
            this.Schools = new ObservableCollection<School>();
        }

        private void SelectSchool(School School)
        {
            App.Current.MainPage = new LoginPage(School);
        }

        public async void Init()
        {
            var schools = await AppData.Instance.SAES.GetSchools(this.Level);
            this.Schools.AddRange(schools);
        }
    }
}
