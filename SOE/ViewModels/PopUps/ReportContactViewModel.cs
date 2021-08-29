using AsyncAwaitBestPractices;
using SOE.API;
using SOE.Models;
using SOE.Models.Data;
using SOE.Views.PopUps;
using SOEWeb.Shared;
using SOEWeb.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.ViewModels.PopUps
{
    public class ReportContactViewModel
    {

        public SchoolContact Contact { get; }
        public ReportContact ReportContact { get; }
        public ReportContactViewModel(SchoolContact Contact, ReportContact ReportContact)
        {
            this.Contact = Contact;
            this.ReportContact = ReportContact;
        }
        //private ICommand _ReportCommand;
        //public ICommand ReportCommand => _ReportCommand ??= new Command<ReportContactReason>(Reportar);
        //private async void Reportar(ReportContactReason report)
        //{
        //    bool result = false;
        //    using (var a = Acr.UserDialogs.UserDialogs.Instance.Loading("Reportando..."))
        //    {
        //        result = await APIService.ReportLink(Contact, report);
        //    }
        //    if (result)
        //    {
        //        Shell.Current.CurrentPage.DisplayAlert(
        //            title: "¡Gracias!",
        //            message:
        //            "Estaremos revisando este enlace a la brevedad.\nGracias por ayudar a esta comunidad a ser un entorno seguro.",
        //             "Ok").SafeFireAndForget();
        //        ReportContact.Close().SafeFireAndForget();
        //    }
        //    else
        //    {
        //        Shell.Current.CurrentPage.DisplayAlert(
        //                title: "Opps...",
        //                message: "Ocurrio un error al reportar este enlace.\nPor favor intente más tarde o envie un correo a soporte  técnico.",
        //                "Ok")
        //            .SafeFireAndForget();
        //        ReportContact.Close().SafeFireAndForget();
        //    }
        //}
    }
}
