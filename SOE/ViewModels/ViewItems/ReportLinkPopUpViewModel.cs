﻿using AsyncAwaitBestPractices;
using SOE.API;
using SOE.Views.PopUps;
using SOEWeb.Shared;
using SOEWeb.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.ViewModels.ViewItems
{
    public class ReportLinkPopUpViewModel
    {
        public Link Link { get; }
        public ReportLinkPopUp ReportLinkPop { get; }
        private ICommand _ReportCommand;
        public ICommand ReportCommand => _ReportCommand ??= new Command<ReportReason>(Reportar);

        public ReportLinkPopUpViewModel(Link Link, ReportLinkPopUp reportLinkPop)
        {
            this.Link = Link;
            this.ReportLinkPop = reportLinkPop;
        }
        private async void Reportar(ReportReason report)
        {
            bool result = false;
            using (var a = Acr.UserDialogs.UserDialogs.Instance.Loading("Reportando..."))
            {
                result = await APIService.ReportLink(Link, report);
            }
            if (result)
            {
                Shell.Current.CurrentPage.DisplayAlert(
                    title: "¡Gracias!",
                    message:
                    "Estaremos revisando este enlace a la brevedad.\nGracias por ayudar a esta comunidad a ser un entorno seguro.",
                     "Ok").SafeFireAndForget();
                ReportLinkPop.Close().SafeFireAndForget();
            }
            else
            {
                Shell.Current.CurrentPage.DisplayAlert(
                        title: "Opps...",
                        message: "Ocurrio un error al reportar este enlace.\nPor favor intente más tarde o envie un correo a soporte  técnico.", 
                        "Ok")
                    .SafeFireAndForget();
                ReportLinkPop.Close().SafeFireAndForget();
            }
        }
    }
}
