﻿using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SchoolOrganizer.Views.Pages;
using Xamarin.Forms;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using APIModels;
using APIModels.Enums;
using AsyncAwaitBestPractices;
using Kit.Model;
using Newtonsoft.Json;
using SchoolOrganizer.API;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Data;
using SchoolOrganizer.Saes;
using SkiaSharp.Views.Forms;
using SchoolOrganizer.Views.ViewItems;

namespace SchoolOrganizer.ViewModels.Pages
{
    public class LoginViewModel : ModelBase
    {

        private User _User;
        public User User
        {
            get => _User;
            set
            {
                _User = value;
                this.LoginCommand?.ChangeCanExecute();

            }
        }

        public School School { get; private set; }
        public int AttemptCount { get; set; }

        public Command<LoginViewModel> LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand FingerCommand { get; }
        public ICommand OnLoginSuccess { get; }


        public LoginViewModel(School School)
        {
            this.School = School;
            this.User = new User();
            this.User.PropertyChanged += User_PropertyChanged;
            LoginCommand = new Command<LoginViewModel>(LoginRequested, LoginCanExecute);
            FingerCommand = new Command(FingerClicked);
            OnLoginSuccess = new Command(LoginSuccess);
            RegisterCommand = new Command(Register);


        }

        private  void Register()
        {
            this.User.Password = string.Empty;
            App.Current.MainPage.Navigation.PushModalAsync(new UserSignUpPage(this.School, this.User)).SafeFireAndForget();
        }
        private async void LoginRequested(LoginViewModel obj)
        {
            Response response = Response.Error;
            APIService apiService = new APIService();
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Iniciando sesión..."))
            {
                response = await apiService.Login(obj.User.Boleta, obj.User.Password, School.Name);
            }
            switch (response.ResponseResult)
            {
                case APIResponseResult.SHOULD_ENROLL:
                    this.User.Password = string.Empty;
                    App.Current.MainPage.Navigation.PushModalAsync(new UserSignUpPage(this.School,this.User)).SafeFireAndForget();
                    break;
                case APIResponseResult.KO:
                    this.User.Password = string.Empty;
                    App.Current.MainPage.DisplayAlert("Mensaje informativo", response.Message, "Ok").SafeFireAndForget();
                    break;
                case APIResponseResult.OK:
                    this.OnLoginSuccess.Execute(AppData.Instance.SAES);
                    break;
                case APIResponseResult.DEVICE_NOT_TRUSTED:
                    //WHO ARE YOU?!
                    break;
                case APIResponseResult.INTERNAL_ERROR:
                default:
                    App.Current.MainPage.DisplayAlert("Mensaje informativo","Algo ha salido mal,esto no ha sido tu culpa.\nPor favor intenta nuevamente","Ok").SafeFireAndForget();
                    break;
            }
        }

        private void User_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.LoginCommand?.ChangeCanExecute();
        }

        private void LoginSuccess()
        {
            AppShell shell = new AppShell();
            if (AppData.Instance.LiteConnection.Table<User>().Any(x => x.Boleta == this.User.Boleta))
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert($"Hemos notado que es tu primer inicio de sesión, te damos la bienvenida.", $"!Bienvenido,{AppData.Instance.User.Name}¡", "Vamos allá");
            }
            AppData.Instance.LiteConnection.InsertOrReplace(this.User);
            Application.Current.MainPage = shell;
        }

        private bool LoginCanExecute(object arg)
        {
            return !string.IsNullOrEmpty(User.Boleta) && (Validations.IsValidEmail(User.Boleta) || Validations.IsValidBoleta(User.Boleta)) && !string.IsNullOrEmpty(User.Password);
        }


        private async void FingerClicked(object obj)
        {
            bool isFingerprintAvailable = await CrossFingerprint.Current.IsAvailableAsync(true);
            if (!isFingerprintAvailable)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert($"Error",
                    "La autenticacion biometrica no esta disponible  o no esta configurada.", "OK");
                return;
            }

            AuthenticationRequestConfiguration conf =
                new AuthenticationRequestConfiguration("Authentication",
                "Authenticate access to your personal data");
            conf.AllowAlternativeAuthentication = true;
            var authResult = await CrossFingerprint.Current.AuthenticateAsync(conf);
            if (authResult.Authenticated)
            {
                //Success  
                //App.Current.MainPage = new AppShell();
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert($"Error", "Autenticacion fallida", "OK");
            }
        }


    }
}
