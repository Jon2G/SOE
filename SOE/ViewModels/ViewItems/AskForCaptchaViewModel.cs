﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using APIModels;
using AsyncAwaitBestPractices;
using Kit;
using Kit.Model;
using Rg.Plugins.Popup.Services;
using SOE.Data;
using SOE.Views.PopUps;
using Xamarin.Forms;

namespace SOE.ViewModels.ViewItems
{
    public class AskForCaptchaViewModel : ModelBase
    {
        private ImageSource _CaptchaImg;
        public ImageSource CaptchaImg
        {
            get => _CaptchaImg;
            set
            {
                _CaptchaImg = value;
                Raise(() => CaptchaImg);
            }
        }
        private string _Captcha;
        public string Captcha
        {
            get => _Captcha;
            set
            {
                _Captcha = value;
                Raise(() => Captcha);
                this.SignInCommand.ChangeCanExecute();
            }
        }
        private ICommand _RefreshCaptchaCommand;
        public ICommand RefreshCaptchaCommand => _RefreshCaptchaCommand ??= new Command(RefreshCaptcha);
        private Command _SignInCommand;
        public Command SignInCommand => _SignInCommand ??= new Command(SignIn, ValidateCanExecute);
        public int AttemptCount { get; set; }
        public readonly Func<AskForCaptcha,Task<bool>> OnSucceedAction;
        private readonly AskForCaptcha AskForCaptcha;

        public AskForCaptchaViewModel(AskForCaptcha AskForCaptcha, Func<AskForCaptcha,Task<bool>> OnSucceedAction)
        {
            this.AskForCaptcha = AskForCaptcha;
            this.OnSucceedAction = OnSucceedAction;
        }
        public async void RefreshCaptcha()
        {
            this.CaptchaImg = await AppData.Instance.SAES.GetCaptcha();
        }
        private async void SignIn()
        {
            this.AttemptCount++;
            if (await AppData.Instance.SAES.LogIn(this.Captcha, this.AttemptCount, false))
            {
                await OnSucceedAction.Invoke(this.AskForCaptcha);
            }
            else
            {
                this.Captcha = string.Empty;
                RefreshCaptcha();
                Acr.UserDialogs.UserDialogs.Instance.Alert("Usuario o contraseña invalidos", "Atención", "Ok");
            }
        }
        private bool ValidateCanExecute()
        {
            return !string.IsNullOrEmpty(AppData.Instance.User.Boleta)
                   && Validations.IsValidBoleta(AppData.Instance.User.Boleta)
                   && !string.IsNullOrEmpty(AppData.Instance.User.Password)
                   && !string.IsNullOrEmpty(Captcha);
        }
    }
}