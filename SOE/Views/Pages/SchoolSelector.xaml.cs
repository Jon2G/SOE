using AsyncAwaitBestPractices;
using SOE.Views.Pages.Login;
using SOE.Views.PopUps;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SchoolSelector
    {
        public SchoolSelector():this(false)
        {

        }
        public SchoolSelector(bool PrivacyAlertDisplayed)
        {
            InitializeComponent();
            this.Model.PrivacyAlertDisplayed = PrivacyAlertDisplayed;
        }

        //protected override async void OnAppearing()
        //{
        //    base.OnAppearing();
        //    this.InputTransparent = true;
        //    PrivacyPopUp pop = new PrivacyPopUp();
        //    await pop.ShowDialog();
        //    if (!pop.Accept)
        //    {
        //        Acr.UserDialogs.UserDialogs.Instance
        //            .AlertAsync("Debe aceptar las politícas para hacer uso de esta aplicación","Atención","Entiendo")
        //            .SafeFireAndForget();
        //        App.Current.MainPage = new LoginPage();
        //        return;
        //    }

        //    this.InputTransparent = false;
        //}

    }
}