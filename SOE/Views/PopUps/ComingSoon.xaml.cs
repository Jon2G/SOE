using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Kit.Forms.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ComingSoon 
    {
        public ComingSoon()
        {
            InitializeComponent();
        }

        public static void Show()
        {
            ComingSoon coming = new ComingSoon();
            ((BasePopUp) coming).Show().SafeFireAndForget();
        }
    }
}