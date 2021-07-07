using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOE.Fonts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubjectClassmatesView 
    {
        public override string Icon => FontelloIcons.AddressBook;
        public SubjectClassmatesView()
        {
            InitializeComponent();
        }
    }
}