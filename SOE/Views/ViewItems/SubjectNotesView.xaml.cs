using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIModels;
using AsyncAwaitBestPractices;
using SOE.Fonts;
using SOE.Services;
using SOE.ViewModels.ViewItems;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubjectNotesView
    {
        public override string Icon => FontelloIcons.Community;
        public SubjectNotesViewModel Model { get; set; }
        public SubjectNotesView(Subject Subject)
        {
            this.Model = new SubjectNotesViewModel(Subject);
            this.BindingContext = this.Model;
            InitializeComponent();
        }
    }
}