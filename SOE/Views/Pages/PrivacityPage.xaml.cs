using AsyncAwaitBestPractices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrivacityPage
    {
        public PrivacityPage()
        {
            InitializeComponent();
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            if (Shell.Current is Shell shell)
            {
                shell.Navigation.PopAsync().SafeFireAndForget();
            }
            else if (App.Current.MainPage is not null)
            {
                App.Current.MainPage.Navigation.PopModalAsync().SafeFireAndForget();
            }
        }
    }
}