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
    public partial class AcademicDirectory : ContentPage
    {
        public static AcademicDirectory Instance { get; private set; }
        public AcademicDirectory()
        {
            InitializeComponent();
            Instance = this;
        }

        protected override bool OnBackButtonPressed() =>
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PopupStack.Any();
    }
}