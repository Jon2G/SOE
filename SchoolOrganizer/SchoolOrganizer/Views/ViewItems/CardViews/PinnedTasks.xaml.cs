using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.ViewItems.CardViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PinnedTasks : ContentView
    {
        public PinnedTasks()
        {
            InitializeComponent();
        }
    }
}