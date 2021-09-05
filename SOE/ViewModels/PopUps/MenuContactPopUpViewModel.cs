using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using SOE.Services;
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
    public class MenuContactPopUpViewModel
    {
        MenuContactPopUp PopUp;
        SchoolContact Contact;
        private ICommand _EditCommand;
        public ICommand EditCommand => _EditCommand ??= new AsyncCommand(Editar);

        private ICommand _DeleteCommand;
        public ICommand DeleteCommand => _DeleteCommand ??= new AsyncCommand(Eliminar);
        private ICommand _ReportCommand;
        public ICommand ReportCommand => _ReportCommand ??= new Command(Reportar);
        private List<Departament> Departaments { get; }

        public MenuContactPopUpViewModel(List<Departament> Departaments, MenuContactPopUp popUp, SchoolContact contact)
        {
            this.Departaments = Departaments;
            this.PopUp = popUp;
            this.Contact = contact;
        }
        private async Task Editar()
        { // abrir popup alta
            await Task.Yield();
            this.PopUp.Close().SafeFireAndForget();
            AddContactPage pr = new AddContactPage(this.Departaments, this.Contact);
            await pr.ShowDialog();
        }


        private async Task Eliminar()
        {
            await Task.Yield();
            this.PopUp.Close().SafeFireAndForget();
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Eliminado contacto"))
            {
                if (await Contact.Delete())
                {
                    
                    await AcademicDirectory.Instance.Model.Init();
                    Shell.Current.CurrentPage.DisplayAlert(
                     title: "¡Gracias!",
                     message:
                     "Este contacto a sido eliminado exitosamente.",
                      "Ok").SafeFireAndForget();
                }
                else
                {
                    Shell.Current.CurrentPage.DisplayAlert(
                        title: "Opps...",
                        message: "Ocurrio un error al tratar eliminar de  este contacto.\nPor favor intente más tarde o envie un correo a soporte  técnico.",
                        "Ok")
                    .SafeFireAndForget();
                    
                }
            }
        }

        private void Reportar()
        {
            this.PopUp.Close().SafeFireAndForget();
            ReportContact pr = new(Contact);
            pr.ShowDialog().SafeFireAndForget();
        }

    }
}
