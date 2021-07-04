﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using SOE.Data;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using Xamarin.Forms;

namespace SOE.ViewModels.ViewItems
{
    public class NoInscriptionViewModel
    {
        private ICommand _RefreshDataCommand;
        public ICommand RefreshDataCommand => _RefreshDataCommand ??= new Command(RefreshData);

        private async void RefreshData()
        {
            AskForCaptcha ask = null;
            if (AppData.Instance.SAES is null || !await AppData.Instance.SAES.IsLoggedIn())
            {
                ask = new AskForCaptcha(OnLoginComplete);
                ask.Show().SafeFireAndForget();
            }
            else
            {
                await OnLoginComplete(ask);
            }
        }

        private async Task<bool> OnLoginComplete(AskForCaptcha AskForCaptcha)
        {
            await Task.Yield();
            await AppData.Instance.SAES.GetUserData(AppData.Instance.User);
            if (AskForCaptcha is not null)
            {
                await AskForCaptcha.Close();
            }
            Application.Current.MainPage = new SplashScreen();
            return true;
        }


    }
}