using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Kit.Model;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Academic;
using SchoolOrganizer.Saes;
using SchoolOrganizer.ViewModels.Pages;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels
{
    public class SchoolGradesViewModel : ModelBase
    {
        private List<Grade> _Grades;

        public List<Grade> Grades
        {
            get => _Grades;
            set
            {
                _Grades = value;
                Raise(() => Grades);
            }
        }
        public ICommand RefreshCommand { get; }
        public ICommand HasBeenRefreshedCommand { get; }

        public SchoolGradesViewModel()
        {
            HasBeenRefreshed();
            this.RefreshCommand = new Command(Refresh);
            this.HasBeenRefreshedCommand = new Command(HasBeenRefreshed);
        }

        private void HasBeenRefreshed()
        {
            this.Grades = AppData.Instance.LiteConnection.Table<Grade>().ToList();
        }

        private void Refresh()
        {
            //AppData.Instance.SAES ??= new Saes._Saes(browser.Browser);
            //AppData.Instance.SAES.OnFinished = this.HasBeenRefreshedCommand;
            //if (!AppData.Instance.SAES.IsLogedIn)
            //{
            //    AppData.Instance.SAES.LoginViewModel.User = AppData.Instance.User;
            //    AppData.Instance.SAES.LoginRequested(AppData.Instance.SAES.LoginViewModel);
            //}
            //AppData.Instance.SAES.RefreshSchoolGrades();
        }
    }
}
