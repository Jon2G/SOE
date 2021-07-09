﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using APIModels;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Kit;
using Kit.Model;
using Rg.Plugins.Popup.Services;
using SOE.Data;
using SOE.Models.Academic;
using SOE.Models.Scheduler;
using SOE.Saes;
using SOE.Services;
using SOE.Views.PopUps;
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

        private ICommand _RefreshCommand;
        public ICommand RefreshCommand => _RefreshCommand??= new AsyncCommand(Refresh);

        public SchoolGradesViewModel()
        {
            this.Grades = new ObservableCollection<SchoolGrade>();
            App.Current.RequestedThemeChanged += Current_RequestedThemeChanged;
        }

        private void Current_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            GetGrades();
            Raise(() => Grades);
        }

        public void GetGrades()
        {
            this.Grades.Clear();
            this.Grades.AddRange(SubjectService.ToList().Select(s => new SchoolGrade(s)));
        }

        private async Task Refresh()
        {
            if (AppData.Instance.SAES is null || await AppData.Instance.SAES.IsLoggedIn())
            {
                AskForCaptcha ask = new AskForCaptcha(RefreshGrades);
                ask.Show().SafeFireAndForget();
            }
        }

        private async Task<bool> RefreshGrades(AskForCaptcha captcha)
        {
            AppData.Instance.SAES.ShowLoading = false;
            await AppData.Instance.SAES.GoHome();
            using (UserDialogs.Instance.Loading("Actualizando calificaciones...."))
            {
                await AppData.Instance.SAES.GetGrades();
            }
            await captcha.Close();
            GetGrades();
            return true;
        }
    }
}