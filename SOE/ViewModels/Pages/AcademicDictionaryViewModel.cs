﻿using Kit;
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
using SOE.Views.PopUps;
using System.Threading.Tasks;
using SOEWeb.Shared;
using AsyncAwaitBestPractices;
using SOE.Services;

namespace SOE.ViewModels.Pages
{
  public class AcademicDictionaryViewModel : ModelBase
    {
        public ObservableCollection<ContactsByDeparment> Contacts { get; set; }
        public ICommand OpenLinkCommand { get; set; }
        public ICommand CallnumberCommand { get; set; }
        public ICommand ContactMessageCommand { get; set; }
        public ICommand ContactCallCommand { get; set; }
        public ICommand ContactLinkCommand { get; set; }
        public ICommand AddContactCommand { get; set; }
        private ICommand _ReportCommand;
        public ICommand ReportCommand => _ReportCommand ??= new Command<SchoolContact>(Reportar);

        public AcademicDictionaryViewModel()
        {
            this.OpenLinkCommand = new Command(this.OpenLink);
            this.CallnumberCommand = new Command(this.Callnumber);
            this.ContactMessageCommand = new Command<SchoolContact>(ContactMessage);
            this.ContactCallCommand = new Command<SchoolContact>(ContactCall);
            AddContactCommand = new Command<SchoolContact>(AddContact);
            ContactLinkCommand = new Command<SchoolContact>(ContactLink);
            Init().SafeFireAndForget();
        }
        public async Task Init()
        {
            await Task.Yield();
            this.Contacts = await SchoolContactsService.Get();
        }
        private void ContactCall(SchoolContact contact) => PhoneDialer.Open(contact.Phone);
        private void ContactLink(SchoolContact obj) => OpenBrowser(obj.Url);
        private void Callnumber(object obj) => PhoneDialer.Open("55 5624 2000");
        private void OpenLink(object obj) => OpenBrowser("https://www.esimecu.ipn.mx/");

        private async void Reportar(SchoolContact contact)
        {
            ReportContact pr = new(contact);
            await pr.ShowDialog();
        }
        private async void AddContact(SchoolContact obj)
        {
            AddContactPage pr = new();
            await pr.ShowDialog();
        }

       
        private async void ContactMessage(SchoolContact contact)
        {
            try
            {
                await Email.ComposeAsync(new EmailMessage
                {
                    Subject = contact.Name,
                    Body = "",
                    To = new List<string>() { contact.Correo },
                });
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, nameof(ContactMessage));
            }

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
    }
}
