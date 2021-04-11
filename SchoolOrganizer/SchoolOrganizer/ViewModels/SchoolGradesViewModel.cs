using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Acr.UserDialogs;
using Kit.Extensions;
using Kit.Model;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Academic;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Saes;
using SchoolOrganizer.ViewModels.Pages;
using SchoolOrganizer.Views.PopUps;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels
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
            this.RefreshCommand = new Command(Refresh);
        }

        private void GetGrades()
        {
            this.Grades.Clear();
            this.Grades.AddRange(Subject.ToList().Select(s => new SchoolGrade(s)));
        }

        private async void Refresh()
        {
            SAES saes = AppData.Instance.SAES;
            saes.School = AppData.Instance.User.School;
            await saes.GoTo(saes.School.HomePage);
            if (!await saes.IsLoggedIn())
            {
                LoginViewModel loginViewModel = new LoginViewModel()
                {
                    User = AppData.Instance.User,
                    CaptchaImg = await saes.GetCaptcha()
                };
                AskForCaptcha captcha = new AskForCaptcha(loginViewModel);

                await captcha.ShowDialog();

                if (!await saes.LogIn(loginViewModel, false))
                {
                    UserDialogs.Instance.Alert("Usuario o contraseña invalidos", "Atención", "Ok");
                    return;
                }
                else
                {
                    Refresh();
                }
                return;
            }

            using (UserDialogs.Instance.Loading("Actualizando calificaciones...."))
            {
                await AppData.Instance.SAES.GetSchoolGrades();
            }
            GetGrades();
        }
    }
}
