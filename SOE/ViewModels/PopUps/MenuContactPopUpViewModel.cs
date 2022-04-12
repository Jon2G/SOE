using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using SOE.Models;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using System.Collections.Generic;
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
        private List<string> Departaments { get; }

        public MenuContactPopUpViewModel(List<string> Departaments, MenuContactPopUp popUp, SchoolContact contact)
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
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Eliminado contacto"))
            {
                await Contact.Delete();
                await AcademicDirectory.Instance.Model.Init();
                Shell.Current.CurrentPage.DisplayAlert(
                 title: "¡Gracias!",
                 message:
                 "Este contacto a sido eliminado exitosamente.",
                  "Ok").SafeFireAndForget();

                this.PopUp.Close().SafeFireAndForget();
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
