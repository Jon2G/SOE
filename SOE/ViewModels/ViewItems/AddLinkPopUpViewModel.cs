using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Kit;
using Kit.Model;
using SOE.Models;
using SOE.Models.Scheduler;
using SOE.Services;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using SOEWeb.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.ViewModels.ViewItems
{

    public class AddLinkPopUpViewModel : ModelBase
    {

        public ClassSquare ClassSquare { get; }
        public Link Link { get; set; }
        private ICommand _AddLinkCommand;
        public ICommand AddLinkCommand => _AddLinkCommand ??= new AsyncCommand<AddLinkPopUp>(AddLink);

        public AddLinkPopUpViewModel(ClassSquare square)
        {
        
            this.ClassSquare = square;
            this.Link = new Link();

        }

       
        private async Task AddLink(AddLinkPopUp popUp)
        {
            if (string.IsNullOrEmpty(this.Link.Name))
            {
                Shell.Current.CurrentPage.DisplayAlert("¿Olvido el nombre?",
                    "Debe ingresar un nombre para este link.","Entiendo").SafeFireAndForget();
                return;
            }
            if (string.IsNullOrEmpty(this.Link.Url)||!Validations.IsValidUrl(this.Link.Url))
            {
                Shell.Current.CurrentPage.DisplayAlert("El link es invalido",
                    "La dirección url es invalida.", "Entiendo").SafeFireAndForget();
                return;
            }
            if (await this.Link.Upload(this.ClassSquare.Subject))
            {
                Shell.Current.CurrentPage.DisplayAlert("Gracias por compartir",
                    "Este enlace será revisado por los moderadores para garantizar un entorno seguro en la comunidad.",
                    "Entiendo").SafeFireAndForget();
            }
            LinksPage.Instance.Model.Init().SafeFireAndForget();
            popUp.Close().SafeFireAndForget();


        }
    }
}
