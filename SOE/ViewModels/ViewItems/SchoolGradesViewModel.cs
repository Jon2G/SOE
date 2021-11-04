using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Kit;
using Kit.Model;
using Kit.Services.Interfaces;
using SOE.API;
using SOE.Data;
using SOE.Models.Academic;
using SOE.Services;
using SOE.Views.PopUps;
using SOEWeb.Shared.Enums;
using System.Collections.Generic;
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

        private float _SemesterAvg;

        public float SemesterAvg
        {
            get => _SemesterAvg;
            set
            {
                _SemesterAvg = value;
                Raise(() => SemesterAvg);
            }
        }

        private ICommand _RefreshCommand;
        public ICommand RefreshCommand => _RefreshCommand ??= new AsyncCommand(Refresh);
        private ICommand _FlyOutCommand;
        public ICommand FlyOutCommand
            => _FlyOutCommand ??= new Command(OpenFlyOut);

        private void OpenFlyOut()
        {
            AppShell.OpenFlyout();
        }
        public SchoolGradesViewModel()
        {
            this.Grades = new ObservableCollection<SchoolGrade>();
            App.Current.RequestedThemeChanged += Current_RequestedThemeChanged;
        }

        private void Current_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            GetGrades();
            Raise(() => Grades);
            Raise(() => SemesterAvg);
        }

        public void GetGrades()
        {
            this.Grades.Clear();
            this.Grades.AddRange(SubjectService.ToList().Select(s => new SchoolGrade(s)));
            SemesterAvg = 0;
            if (Grades.Any())
            {
                List<SOEWeb.Shared.Grade> grades = this.Grades.SelectMany(x => x.Grades.Where(y => y.Partial == GradePartial.Final && y.NumericScore > 0)).ToList();
                if (grades?.Any() ?? false)
                {
                    this.SemesterAvg = (float)grades.Average(x => x.NumericScore);
                }
            }
        }

        private async Task Refresh()
        {
            await Task.Yield();
            bool isLogedIn = false;
            if (AppData.Instance.SAES is not null)
            {
                isLogedIn = await AppData.Instance.SAES.IsLoggedIn();
            }
            if (!isLogedIn)
            {
                AskForCaptcha ask = new AskForCaptcha(RefreshGrades);
                ask.Show().SafeFireAndForget();
            }
            else
            {
                RefreshGrades(null).SafeFireAndForget();
            }

        }

        private async Task<bool> RefreshGrades(ICrossWindow captcha)
        {
            await Task.Yield();
            AppData.Instance.SAES.ShowLoading = false;
            await AppData.Instance.SAES.GoHome();
            using (UserDialogs.Instance.Loading("Actualizando calificaciones...."))
            {
                await AppData.Instance.SAES.GetGrades(await APIService.IsOnline());
            }
            if (captcha is not null)
                await captcha.Close();
            GetGrades();
            return true;
        }
    }
}
