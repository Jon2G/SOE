using SOE.Models;
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

            InitializeComponent();
        }

        public ReminderPage(Reminder reminder)
        {
            this.Model = new ReminderPageVIewModel(this) { PReminder = reminder };
            this.BindingContext = Model;
            InitializeComponent();
        }
    }
}