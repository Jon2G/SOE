using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Acr.UserDialogs;
using APIModels;
using Kit;
using Kit.Model;
using SOE.Data;
using SOE.Models.Academic;
using SOE.Models.Scheduler;
using SOE.Saes;
using SOE.Services;
using Xamarin.Forms;

namespace SOE.ViewModels
{
    public class SchoolGradesViewModel : ModelBase
    {
        private ObservableCollection<SchoolGrade> _Grades;
        public ObservableCollection<SchoolGrade> Grades
        {
            get => _Grades;
            set
            {
                _Grades = value;
                Raise(() => Grades);
            }
        }

        public ICommand RefreshCommand { get; }

        public SchoolGradesViewModel()
        {
            this.Grades = new ObservableCollection<SchoolGrade>();
            GetGrades();
            this.RefreshCommand = new Xamarin.Forms.Command(Refresh);
            App.Current.RequestedThemeChanged += Current_RequestedThemeChanged;
        }

        private void Current_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            GetGrades();
            Raise(()=>Grades);
        }

        private void GetGrades()
        {
            this.Grades.Clear();
            this.Grades.AddRange(SubjectService.ToList().Select(s => new SchoolGrade(s)));
        }

        private async void Refresh()
        {
            SAES saes = AppData.Instance.SAES;
            await saes.GoHome();
            if (!await saes.IsLoggedIn())
            {
                //LoginViewModel loginViewModel = new LoginViewModel(saes.School)
                //{
                //    User = AppData.Instance.User
                //};
                //AskForCaptcha captcha = new AskForCaptcha(loginViewModel);

                //await captcha.ShowDialog();

                //if (!await saes.LogIn(loginViewModel, false))
                //{
                //    UserDialogs.Instance.Alert("Usuario o contraseña invalidos", "Atención", "Ok");
                //    return;
                //}
                //else
                //{
                //    Refresh();
                //}
                //return;
            }

            using (UserDialogs.Instance.Loading("Actualizando calificaciones...."))
            {
                await AppData.Instance.SAES.GetGrades();
            }
            GetGrades();
        }
    }
}
