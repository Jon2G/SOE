﻿using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Kit.Services.Interfaces;
using SOE.Data;
using SOE.Models.Data;
using SOE.Views.Pages;
using SOE.Views.Pages.Login;
using SOE.Views.PopUps;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.ViewModels.ViewItems
{
    public class NoInscriptionViewModel
    {
        private ICommand _RefreshDataCommand;
        public ICommand RefreshDataCommand => _RefreshDataCommand ??= new AsyncCommand(RefreshData);
        public User User { get; set; }

        public NoInscriptionViewModel()
        {
            User = AppData.Instance.User;
        }
        private async Task RefreshData()
        {
            AskForCaptcha ask = null;
            if (AppData.Instance.SAES is null ||
                !await AppData.Instance.SAES.IsLoggedIn())
            {
                ask = new AskForCaptcha(OnLoginComplete);
                ask.Show().SafeFireAndForget();
            }
            else
            {
                await OnLoginComplete(ask);
            }
        }

        private async Task<bool> OnLoginComplete(ICrossWindow AskForCaptcha)
        {
            await Task.Yield();
            if (AskForCaptcha is not null)
            {
                await AskForCaptcha.Close();
            }

            App.Current.MainPage = new RefreshDataPage(false);
            await AppData.Instance.SAES.GetUserData();
            Application.Current.MainPage = new SplashScreen();
            return true;
        }
    }
}
