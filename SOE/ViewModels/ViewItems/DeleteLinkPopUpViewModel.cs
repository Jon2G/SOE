using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using SOE.API;
using SOE.Data;
using SOE.Models.Data;
using SOE.Views.Pages;
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
    public class DeleteLinkPopUpViewModel
    {
        public Link Link { get; }
        public DeleteLinkPopUp DeleteLinkPop { get; }
        private ICommand _DeleteCommand;
        public ICommand DeleteCommand => _DeleteCommand ??= new Command(Delete);
        private ICommand _CancelCommand;
        public ICommand CancelCommand => _CancelCommand ??= new AsyncCommand(DeleteLinkPop.Close);
        public DeleteLinkPopUpViewModel(Link Link, DeleteLinkPopUp deleteLinkPop)
        {
            this.Link = Link;
            this.DeleteLinkPop = deleteLinkPop;
        }

        private async void Delete(object obj)
        {
            bool result = false;
            using (var a = Acr.UserDialogs.UserDialogs.Instance.Loading("Eliminando..."))
            {
                result = await APIService.DeleteLink(Link, AppData.Instance.User.Id);
            }
            if (result)
            {
                Shell.Current.CurrentPage.DisplayAlert(title: "¡Completado!", message: "El enlace fue eliminado.\nDejará de estar disponible en unos instantes", "Ok")
                    .SafeFireAndForget();
                DeleteLinkPop.Close().SafeFireAndForget();
                LinksPage.Instance.Model.Init().SafeFireAndForget();
            }
            else
            {
                Shell.Current.CurrentPage.DisplayAlert(title: "Opps...", message: "Ocurrio un error al eliminar este enlace.\nPor favor intente más tarde o envie un correo a soporte  técnico.", "Ok")
                    .SafeFireAndForget();
                DeleteLinkPop.Close().SafeFireAndForget();
            }
        }


    }
}
