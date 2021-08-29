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
        private string _Name;
        public string Name
        {
            get => _Name;
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    Raise(() => Name);
                    this.AddLinkCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private string _Url;
        public string Url
        {
            get => _Url;
            set
            {
                if (_Url != value)
                {
                    _Url = value;
                    Raise(() => Url);
                    this.AddLinkCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ClassSquare ClassSquare { get; }
        private AsyncCommand<AddLinkPopUp> _AddLinkCommand;

        public AsyncCommand<AddLinkPopUp> AddLinkCommand
        {
            get
            {
                if (this._AddLinkCommand is null)
                {
                    _AddLinkCommand = new AsyncCommand<AddLinkPopUp>(AddLink,CanAddLink);
                }

                return this._AddLinkCommand;
            }
        }



        public AddLinkPopUpViewModel(ClassSquare square)
        {
            this.ClassSquare = square;
        }

        private bool CanAddLink(object arg)
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Url))
            {
                return false;
            }

            return true;
        }
        private async Task AddLink(AddLinkPopUp popUp)
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                Shell.Current.CurrentPage.DisplayAlert("¿Olvido el nombre?",
                    "Debe ingresar un nombre para este link.", "Entiendo").SafeFireAndForget();
                return;
            }
            if (string.IsNullOrEmpty(this.Url) || !Validations.IsValidUrl(this.Url,out Uri uri))
            {
                Shell.Current.CurrentPage.DisplayAlert("El link es invalido",
                    "La dirección url es invalida.", "Entiendo").SafeFireAndForget();
                return;
            }

            Link link = new Link(Name, uri.AbsoluteUri);
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Compartiendo enlace"))
            {
                if (await link.Upload(this.ClassSquare.Subject))
                {
                    Shell.Current.CurrentPage.DisplayAlert("Gracias por compartir",
                        "Este enlace será revisado por los moderadores para garantizar un entorno seguro en la comunidad.",
                        "Entiendo").SafeFireAndForget();
                }
                else
                {
                    Shell.Current.CurrentPage.DisplayAlert("Ooops",
                        "Ocurrio un problema al compartir este enlace por favor intente nuevamente ó reporte este problema",
                        "Entiendo").SafeFireAndForget();
                }
            }
            LinksPage.Instance.Model.Init().SafeFireAndForget();
            popUp.Close().SafeFireAndForget();


        }
    }
}
