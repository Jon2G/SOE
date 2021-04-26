using Kit.Model;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Data;
using SchoolOrganizer.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.Pages
{
   
    class LoginPopUpViewModel : ModelBase
    {
        
        User user = User.Get();
        LoginPopUp LoginPop;
        private string _Boleta;
        private string _Password;
        public string Boleta { get => _Boleta; set { _Boleta = value; Raise(() => Boleta); } }
        public string Password { get => _Password; set { _Password = value; Raise(() => Password); } }
        public ICommand IngresarCommand { get; set; }
        public LoginPopUpViewModel()
        {
            IngresarCommand = new Command(Ingresar);
        }

        private  void Ingresar(object obj)
        {
            if(this.Boleta == user.Boleta && this.Password== user.Password)
            {
                AppData.Instance.User = user;
                App.Current.MainPage = new AppShell();
                // await LoginPop.Close();
            }
            else
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Error", "Validacion Fallida");
            }
        }
    }
}
