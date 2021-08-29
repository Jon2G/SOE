using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Kit.Model;
using SOE.Models;
using SOE.Services;
using SOE.ViewModels.Pages;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using SOEWeb.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.ViewModels.PopUps
{
    public class AddContactViewModel: ModelBase
    {
        AddContactPage AddContactPage;

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
                    this.AddContactCommand.RaiseCanExecuteChanged();
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
                    this.AddContactCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private Departament _Departament;
        public Departament Departament
        {
            get => this._Departament;
            set
            {
                this._Departament = value;
                this.Raise(() => this.Departament);
            }
        }
        private string _Phone;
        public string Phone
        {
            get => this._Phone;
            set
            {
                this._Phone = value;
                this.Raise(() => this.Phone);
            }
        }
        private string _Correo;
        public string Correo
        {
            get => this._Correo;
            set
            {
                this._Correo = value;
                this.Raise(() => this.Correo);
            }
        }
        public AsyncCommand AddContactCommand { get; set; }
        public AddContactViewModel(AddContactPage addContact)
        {
            this.AddContactPage = addContact;
            AddContactCommand = new AsyncCommand(AddContact,CanAddContact);
        }

        private bool CanAddContact(object arg)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return false;
            }
            if (!string.IsNullOrEmpty(Url))
            {
                if (!Validations.IsValidUrl(Url, out Uri uri))
                    return false;
                Url = uri.AbsoluteUri;
            }
            if (!string.IsNullOrEmpty(Correo) && !Validations.IsValidEmail(Correo))
            {
                    return false;
            }
            return true;
        }

        private async Task AddContact()
        {
            await Task.Yield();
            if (string.IsNullOrEmpty(this.Name))
            {
                Shell.Current.CurrentPage.DisplayAlert("¿Olvido el nombre?",
                    "Debe ingresar un nombre para este link.", "Entiendo").SafeFireAndForget();
                return;
            }
            if (!string.IsNullOrEmpty(this.Url))
            {
                if (!Validations.IsValidUrl(this.Url, out Uri uri))
                {
                    this._Url = uri.AbsoluteUri;
                }
                Shell.Current.CurrentPage.DisplayAlert("El link es invalido",
                    "La dirección url es invalida.", "Entiendo").SafeFireAndForget();
                return;
            }
            if (!string.IsNullOrEmpty(this.Correo) && !Validations.IsValidEmail(this.Correo))
            {
                Shell.Current.CurrentPage.DisplayAlert("El correo es invalido",
                    "La dirección correo es invalida.", "Entiendo").SafeFireAndForget();
                return;
            }
            if (!(Departament?.IsValid()??false))
            {
                Shell.Current.CurrentPage.DisplayAlert("El departamento es invalido",
                   "El departamento no puede estar vacio.", "Entiendo").SafeFireAndForget();
                return;
            }
            SchoolContact contact = new SchoolContact(Departament,Name,Phone,Url,Correo);
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Compartiendo Contacto"))
            {
                if (await contact.Upload())
                {
                    Shell.Current.CurrentPage.DisplayAlert("Gracias por compartir",
                        "Este contacto será revisado por los moderadores para garantizar un entorno seguro en la comunidad.",
                        "Entiendo").SafeFireAndForget();
                }
                else
                {
                    Shell.Current.CurrentPage.DisplayAlert("Ooops",
                        "Ocurrio un problema al compartir este contacto por favor intente nuevamente ó reporte este problema",
                        "Entiendo").SafeFireAndForget();
                }
            }
            AcademicDirectory.Instance.Model.Init().SafeFireAndForget();
            AddContactPage.Close().SafeFireAndForget();
        }
    }
}
