using SOE.ViewModels.ViewItems;
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
    public partial class ReminderPage 
    {
        public ReminderPageVIewModel Model { get; set; }
        public ReminderPage()
        {
            this.Model = new ReminderPageVIewModel(this);
            this.BindingContext = Model;
            InitializeComponent();
        }
    }
}