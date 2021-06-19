﻿using System;
using System.Security;
using System.Threading.Tasks;
using Kit;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Data;
using SchoolOrganizer.ViewModels.Pages;
using SchoolOrganizer.Views.PopUps;
using SchoolOrganizer.Views.ViewItems;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashScreen : ContentPage
    {

        public SplashScreen()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Task.Run(() => { while (Logo.IsLoading) { } });
            AppData.Init();
            User user = User.Get();
            Settings settings = new Settings();
            if (user != null)
            {
                settings = user.GetSettings();
            }

            if (user != null)
            {
                if (settings.IsFingerPrintActive)
                {
                    if (!await CrossFingerprint.Current.IsAvailableAsync(true))
                    {
                        Acr.UserDialogs.UserDialogs.Instance.Alert("La autenticación biométrica no esta disponible  o no esta configurada.", "Atención", "OK");
                        GotoManualLogin(user, settings);
                        return;
                    }
                    var authResult = await Device.InvokeOnMainThreadAsync(() =>
                    CrossFingerprint.Current.AuthenticateAsync(
                        new AuthenticationRequestConfiguration("Bloqueo de aplicación",
                        "Inicio de sesíon por huella")
                        {
                            AllowAlternativeAuthentication = true
                        }, new System.Threading.CancellationToken(false)));

                    if (authResult.Status == FingerprintAuthenticationResultStatus.Failed &&
                        authResult.ErrorMessage == "Authentication canceled")
                    {
                        Log.Logger.Error("Tu telegono no jala con pin amiko Im so sorry");
                        GotoManualLogin(user, settings);
                    }
                    if (authResult.Authenticated)
                    {
                        GotoApp(user, settings);
                    }
                    else
                    {
                        GotoManualLogin(user, settings);
                    }
                }
                else
                {
                    GotoApp(user, settings);
                }
            }
            else
            {
                App.Current.MainPage = new LoginPage();
                
            }
        }

        private async void GotoManualLogin(User user, Settings settings)
        {
            var a = new LoginPopUp();
               await a.LockModal()
                .ShowDialog();
            GotoApp(user, settings);
        }

        private void GotoApp(User user, Settings settings)
        {
            AppData.Instance.User = user;
            //if (settings.IsTutorialActive)
            //{
            //    App.Current.MainPage = new AppShell();
            //}else
            //{
            //    App.Current.MainPage = new TutorialCarousel();
            //}
            App.Current.MainPage = new AppShell();
            settings.Notifications();
        }
    }
}