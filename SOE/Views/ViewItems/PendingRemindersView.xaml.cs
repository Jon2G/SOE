using SOE.ViewModels.Pages;
using SOE.ViewModels.ViewItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PendingRemindersView 
    {
        public static PendingRemindersView Instance { get; private set; }
        public override string Title => "RECORDATORIOS";
        public PendingRemindersView()
        {
            Instance = this;
            InitializeComponent();
        }

        public Task Init => this.Model.Load();

    }
}