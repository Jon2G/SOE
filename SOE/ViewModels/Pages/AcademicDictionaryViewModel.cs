using Kit;
using Kit.Model;
using SOE.Data;
using SOE.Models.Data;
using SOE.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using SOE.Views.Pages;

namespace SOE.ViewModels.Pages
{
  public class AcademicDictionaryViewModel : ModelBase
    {
        public ObservableCollection<SchoolContact> Contacts { get; set; }
        public User User { get; set; }
        public ICommand OpenLinkCommand { get; set; }
        public ICommand CallnumberCommand { get; set; }
        public ICommand ContactMessageCommand { get; set; }
        public ICommand ContactCallCommand { get; set; }
        public ICommand AddContactCommand { get; set; }
        public AcademicDictionaryViewModel()
        {
            this.User = AppData.Instance.User;
            this.OpenLinkCommand = new Command(this.OpenLink);
            this.CallnumberCommand = new Command(this.Callnumber);
            this.ContactMessageCommand = new Command<SchoolContact>(ContactMessage);
            this.ContactCallCommand = new Command<SchoolContact>(ContactCall);
            AddContactCommand = new Command<SchoolContact>(AddContact);
            this.Contacts = this.ContactsList();
        }

        private void AddContact(SchoolContact obj)
        {
            App.Current.MainPage.Navigation.PushAsync(new AddContactPage());
        }

        private void ContactCall(SchoolContact contact)
        {
            PhoneDialer.Open(contact.Phone);
        }

        private async void ContactMessage(SchoolContact contact)
        {
            try
            {
                await Email.ComposeAsync(new EmailMessage
                {
                    Subject = contact.Name,
                    Body = "",
                    To = new List<string>() { contact.Url },
                });
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, nameof(ContactMessage));
            }

        }

        private void Callnumber(object obj)
        {
            PhoneDialer.Open("55 5624 2000");
        }

        private void OpenLink(object obj)
        {
            OpenBrowser("https://www.esimecu.ipn.mx/");
        }
        private async void OpenBrowser(string url)
        {
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
        public ObservableCollection<SchoolContact> ContactsList()
        {
            return new ObservableCollection<SchoolContact>
            {
                new SchoolContact("Gestion escolar","Mario","553215456","ictramitescu@ipn.mx"),
                new SchoolContact("Gestion escolar","Mario","553215456","ictramitescu@ipn.mx"),
                new SchoolContact("Gestion escolar","Mario","553215456","ictramitescu@ipn.mx"),
                new SchoolContact("Gestion escolar","Mario","553215456","ictramitescu@ipn.mx"),
                new SchoolContact("Gestion escolar","Mario","553215456","ictramitescu@ipn.mx"),
                new SchoolContact("Gestion escolar","Mario","553215456","ictramitescu@ipn.mx"),
                new SchoolContact("Gestion escolar","Mario","553215456","ictramitescu@ipn.mx"),
                new SchoolContact("Gestion escolar","M. en C. Jose Luis Bautista Arias","553215456","ictramitescu@ipn.mx")
            };
        }
    }
}
