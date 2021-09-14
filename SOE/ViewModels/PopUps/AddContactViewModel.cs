using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Kit;
using Kit.Forms.Model;
using Kit.Model;
using SOE.Models;
using SOE.Services;
using SOE.ViewModels.Pages;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using SOEWeb.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using SOEWeb.Shared;

namespace SOE.ViewModels.PopUps
{
    public class AddContactViewModel : ValidationsModelbase
    {
        private readonly AddContactPage AddContactPage;
        private Guid Guid { get; }
        private string _Name;
        [Required(AllowEmptyStrings = false,ErrorMessage = "Debe ingresar un nombre")]
        [Display(Name ="Nombre")]
        public string Name
        {
            get => _Name;
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    ValidateProperty(value);
                    Raise(() => Name);
                    this.AddContactCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _Url;

        [Url(ErrorMessage ="Url invalido")]
       
        public string Url
        {
            get => _Url;

            set
            {
                if (_Url != value)
                {

                    _Url = value;
                    ValidateProperty(value);
                    Raise(() => Url);
                    this.AddContactCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private Departament _Departament;

        
        public Departament Departament
        {
            get => _Departament;
            set
            {
                if (_Departament!=value)
                {
                    _Departament = value;
                    ValidateProperty(value);
                    this.Raise(() => this.Departament);
                }
            }
        }

        private string _Phone;

        [Phone(ErrorMessage = "Numero de telefono invalido")]
        public string Phone
        {
            get => _Phone;
            set
            {
                this._Phone = value;
                ValidateProperty(value);
                this.Raise(() => this.Phone);
            }
        }
        
        private string _Correo;

        [EmailAddress(ErrorMessage = "Correo invalido")]
        public string Correo
        {
            get => _Correo;
            set
            {
                this._Correo = value;
                ValidateProperty(value);
                this.Raise(() => this.Correo);
            }
        }

        public AsyncCommand AddContactCommand { get; set; }


        private ICommand _DepartamentTextChangedCommand;
        public ICommand DepartamentTextChangedCommand => _DepartamentTextChangedCommand ??= new Command<string>(DepartamentTextChanged);

        public AddContactViewModel(AddContactPage addContact)
        {
            this.AddContactPage = addContact;
            AddContactCommand = new AsyncCommand(AddContact);


        }
        public AddContactViewModel(AddContactPage addContact, SchoolContact contact) : this(addContact)
        {
            if (contact is not null)
            {
                this.Name = contact.Name;
                this.Phone = contact.Phone;
                this.Url = contact.Url;
                this.Correo = contact.Correo;
                this.Departament = contact.Departament;
                this.Guid = contact.Guid;
            }
        }

        private void DepartamentTextChanged(string Name)
        {
            if (!string.IsNullOrEmpty(Name) && (Departament is null || Departament.Guid == Guid.Empty))
            {
                Departament = new Departament() { Name = Name.Trim() };
            }
        }
        private bool CanAddContact(object arg)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return false;
            }
            if (!string.IsNullOrEmpty(Url))
            {
                if (!UriExtensions.IsValidUrl(Url, out Uri uri))
                    return false;
                Url = uri.AbsoluteUri;
            }
            if (!string.IsNullOrEmpty(Correo) && !SOEWeb.Shared.Validations.IsValidEmail(Correo))
            {
                return false;
            }
            return true;
        }

        private async Task AddContact()
        {
            await Task.Yield();

            
            this.Validate();

            if (this.HasErrors)
            {
                // Error message
                this.ScrollToControlProperty(this.GetFirstInvalidPropertyName);
            }
            else
            {
                // No error
            }

            if (string.IsNullOrEmpty(this.Name))
            {
                Shell.Current.CurrentPage.DisplayAlert("¿Olvido el nombre?",
                    "Debe ingresar un nombre para este link.", "Entiendo").SafeFireAndForget();
                return;
            }
            if (!string.IsNullOrEmpty(this.Url))
            {
                if (UriExtensions.IsValidUrl(this.Url, out Uri uri))
                {
                    this._Url = uri.AbsoluteUri;
                }
                else
                {
                    Shell.Current.CurrentPage.DisplayAlert("El link es invalido",
                        "La dirección url es invalida.", "Entiendo").SafeFireAndForget();
                    return;
                }
            }
            if (!string.IsNullOrEmpty(this.Correo) && !SOEWeb.Shared.Validations.IsValidEmail(this.Correo))
            {
                Shell.Current.CurrentPage.DisplayAlert("El correo es invalido",
                    "La dirección correo es invalida.", "Entiendo").SafeFireAndForget();
                return;
            }
            if (!(Departament?.IsValid() ?? false))
            {
                Shell.Current.CurrentPage.DisplayAlert("El departamento es invalido",
                   "El departamento no puede estar vacio.", "Entiendo").SafeFireAndForget();
                return;
            }
            SchoolContact contact = new SchoolContact(this.Guid, Departament, Name, Phone, Url, Correo);
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
