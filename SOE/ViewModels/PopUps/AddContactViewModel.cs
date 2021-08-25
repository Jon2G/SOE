using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.Models;
using SOE.ViewModels.Pages;
using SOE.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.ViewModels.PopUps
{
    public class AddContactViewModel: ModelBase
    {
        public AcademicDictionaryViewModel Model { get; set; }
        AddContactPage AddContactPage;
        private SchoolContact _contact;

        public SchoolContact contact
        {
            get => _contact;
            set
            {
                _contact = value;
                Raise(() => contact);
            }
        }
        public ICommand AddContactCommand { get; set; }
        public AddContactViewModel(AddContactPage addContact)
        {
            this.AddContactPage = addContact;
            AddContactCommand = new Command(AddContact);
            Model = new AcademicDictionaryViewModel();
            contact = new SchoolContact();
        }

        private void AddContact(object obj)
        {
            Model.Contacts.Add(this.contact);
            AddContactPage.Close().SafeFireAndForget();
        }
    }
}
