using Acr.UserDialogs;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Kit;
using Kit.Model;
using Kit.Services.Interfaces;
using SOE.Data;
using SOE.Enums;
using SOE.Models;
using SOE.Models.Academic;
using SOE.Views.PopUps;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
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
            GetGrades().SafeFireAndForget();
        }

        public async Task GetGrades()
        {
            await Task.Yield();
            List<SchoolGrade> schoolGrades = new List<SchoolGrade>();
            List<Subject> subjects = await Subject.GetAll();
            foreach (Subject subject in subjects)
            {
                SchoolGrade grade = await SchoolGrade.FromSubject(subject);
                schoolGrades.Add(grade);
            }
            SemesterAvg = 0;
            if (schoolGrades.Any())
            {
                List<Grade> grades = schoolGrades.SelectMany(x => x.Grades.Where(y => y.Partial == GradePartial.Final && y.NumericScore > 0)).ToList();
                if (grades?.Any() ?? false)
                {
                    this.SemesterAvg = (float)grades.Average(x => x.NumericScore);
                }
            }
            this.Grades.Clear();
            this.Grades.AddRange(schoolGrades);
            Raise(() => Grades);
            Raise(() => SemesterAvg);
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
                await AppData.Instance.SAES.GetGrades();
            }
            if (captcha is not null)
                await captcha.Close();
            await GetGrades();
            return true;
        }
    }
}
