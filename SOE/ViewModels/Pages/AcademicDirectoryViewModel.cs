using AsyncAwaitBestPractices;
using Kit;
using Kit.Model;
using Microsoft.AppCenter.Crashes;
using SOE.Data;
using SOE.Models;
using SOE.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{
    public class AcademicDirectoryViewModel : ModelBase
    {
        private bool _IsLoading;

        public bool IsLoading
        {
            get => _IsLoading;
            set
            {
                _IsLoading = value;
                Raise(() => IsLoading);
            }
        }

        private ObservableCollection<ContactsByDeparment> _Contacts;

        public ObservableCollection<ContactsByDeparment> Contacts
        {
            get => _Contacts;
            set
            {
                _Contacts = value;
                Raise(() => Contacts);
            }
        }

        private List<string> Departaments => Contacts?.Select(x => x.Departament).ToList() ?? new List<string>();

        public ICommand OpenLinkCommand { get; set; }
        public ICommand ContactMessageCommand { get; set; }
        public ICommand ContactCallCommand { get; set; }
        public ICommand ContactLinkCommand { get; set; }
        public ICommand AddContactCommand { get; set; }
        private ICommand _MenuCommand;
        public ICommand MenuCommand => _MenuCommand ??= new Command<SchoolContact>(MenuContact);
        private bool _IsOffline;
        public bool IsOffline
        {
            get => _IsOffline;
            set
            {
                _IsOffline = value;
                Raise(() => IsOffline);
            }
        }

        public ICommand RetryCommand { get; }

        public AcademicDirectoryViewModel()
        {
            this.OpenLinkCommand = new Command(this.OpenLink);
            this.ContactMessageCommand = new Command<SchoolContact>(ContactMessage);
            this.ContactCallCommand = new Command<SchoolContact>(ContactCall);
            this.RetryCommand = new AsyncCommand(() => Init(true));
            AddContactCommand = new Command<SchoolContact>(AddContact);
            ContactLinkCommand = new Command<SchoolContact>(ContactLink);
            Init().SafeFireAndForget();
        }
        public async Task Init(bool Online = true)
        {
            IsOffline = false;
            IsLoading = true;
            await Task.Delay(100);
            if (!Online)
            {
                IsOffline = true;
                IsLoading = false;
                return;
            }
            await ContactsByDeparment.GetByDepartment().ContinueWith(t =>
             {
                 this.Contacts = new ObservableCollection<ContactsByDeparment>(t.Result);
             });
            IsLoading = false;
        }
        private void ContactCall(SchoolContact contact)
        {
            try
            {
                PhoneDialer.Open(contact.Phone);
            }
            catch (Exception ex)
            {
                Crashes.GenerateTestCrash(); Log.Logger.Error(ex, "ContactCall");
                Acr.UserDialogs.UserDialogs.Instance.Alert("Oops..", "Esta característica no es soportada por su dispositivo.\nSe ha copiado el número en el portapapeles.", "Ok");
                Clipboard.SetTextAsync(contact.Phone).SafeFireAndForget();

            }
        }

        private void ContactLink(SchoolContact obj) => OpenBrowser(obj.Url).SafeFireAndForget();
        private void OpenLink(object obj) => OpenBrowser(AppData.Instance.User.School.SchoolPage).SafeFireAndForget();

        private void MenuContact(SchoolContact contact)
        {
            MenuContactPopUp pr = new(Departaments, contact);
            pr.ShowDialog().SafeFireAndForget();
        }
        private void AddContact(SchoolContact obj)
        {
            AddContactPage pr = new(this.Departaments);
            pr.ShowDialog().SafeFireAndForget();
        }

        private async void ContactMessage(SchoolContact contact)
        {
            try
            {
                string saludo = DateTime.Now.Saludo();
                await Email.ComposeAsync(new EmailMessage
                {
                    Subject = contact.Name,
                    Body = $"Estimado(a): {contact.Name}\n{saludo}\n#Escribe aquí tu mensaje.#\nAtt.{AppData.Instance.User.Name}",
                    To = new List<string>() { contact.Correo },
                });
            }
            catch (Exception ex)
            {
                Crashes.GenerateTestCrash(); Log.Logger.Error(ex, nameof(ContactMessage));
            }
        }

        private async Task OpenBrowser(string url)
        {
            await Task.Yield();
            try
            {
                Uri uri = new Uri(url);
                await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, nameof(OpenBrowser));
            }
        }
    }
}
