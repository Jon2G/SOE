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
using Kit.Services.Web;
using SOE.API;
using SOE.Services;
using SOEWeb.Shared.Interfaces;
using System.Linq;
using Xamarin.CommunityToolkit.ObjectModel;

namespace SOE.ViewModels.Pages
{
    public class AcademicDirectoryViewModel : ModelBase, IOffline
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

        private List<Departament> Departaments => Contacts?.Select(x => x.Departament).ToList() ?? new List<Departament>();

        public ICommand OpenLinkCommand { get; set; }
        public ICommand CallnumberCommand { get; set; }
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
            this.CallnumberCommand = new Command(this.Callnumber);
            this.ContactMessageCommand = new Command<SchoolContact>(ContactMessage);
            this.ContactCallCommand = new Command<SchoolContact>(ContactCall);
            this.RetryCommand = new AsyncCommand(() => Init(true));
            AddContactCommand = new Command<SchoolContact>(AddContact);
            ContactLinkCommand = new Command<SchoolContact>(ContactLink);
            if (AppData.Instance.User.IsOffline)
            {
                SyncUser().SafeFireAndForget();
                return;
            }
            Init().SafeFireAndForget();
        }
        private async Task SyncUser()
        {
            await Task.Yield();
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Actualizando información..."))
            {
                if (!await AppData.Instance.User.Sync(AppData.Instance, new SyncService()))
                {
                    await this.Init(false);
                }
            }
            this.Init().SafeFireAndForget();
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
            if (AppData.Instance.User.School.Id <= 0)
            {
                using (Acr.UserDialogs.UserDialogs.Instance.Loading("Cargando información de la escuela..."))
                {
                    AppData.Instance.LiteConnection.CreateTable<School>();
                    AppData.Instance.User.School.Id = await SchoolService.GetId(AppData.Instance.User);
                    AppData.Instance.User.Save();
                }
            }

            var response
                = await APIService.GetContacts(AppData.Instance.User.School);
            switch (response.ResponseResult)
            {
                case APIResponseResult.OK:
                    this.Contacts = new ObservableCollection<ContactsByDeparment>(response.Extra);
                    break;
                case APIResponseResult.INTERNAL_ERROR:
                    this.IsOffline = true;
                    break;
                default:
                    this.IsOffline = true;
                    Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Alerta", response.Message).SafeFireAndForget();
                    break;
            }
            IsLoading = false;
        }
        private void ContactCall(SchoolContact contact) => PhoneDialer.Open(contact.Phone);
        private void ContactLink(SchoolContact obj) => OpenBrowser(obj.Url);
        private void Callnumber(object obj) => PhoneDialer.Open("55 5624 2000");
        private void OpenLink(object obj) => OpenBrowser(AppData.Instance.User.School.HomePage);

        private async void MenuContact(SchoolContact contact)
        {
            MenuContactPopUp pr = new(Departaments, contact);
            await pr.ShowDialog();
        }
        private async void AddContact(SchoolContact obj)
        {
            AddContactPage pr = new(this.Departaments);
            await pr.ShowDialog();
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