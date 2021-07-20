using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOEWeb.Shared;
using AsyncAwaitBestPractices;
using SOE.Fonts;
using SOE.ViewModels.ViewItems;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubjectClassmatesView 
    {
        public override string Icon => FontelloIcons.AddressBook;
        public SubjectClassmatesViewModel Model { get; set; }
        public SubjectClassmatesView(Subject subject)
        {
            this.Model = new SubjectClassmatesViewModel(subject);
            this.BindingContext = this.Model;
            InitializeComponent();
        }
    }
}