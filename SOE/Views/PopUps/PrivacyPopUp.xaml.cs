using AsyncAwaitBestPractices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrivacyPopUp
    {
        public bool Accept => this.RadioButton.IsChecked;
        public PrivacyPopUp()
        {
            this.LockModal();
            InitializeComponent();
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            this.Close().SafeFireAndForget();
        }
    }
}