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
    public partial class MainView
    {
        public override string Title => "Tareas";
        public override string Icon => FontelloIcons.Book;

        public MainView()
        {
            InitializeComponent();
            this.ToolbarItem.Command = this.Model.AddTaskCommand;
        }
    }
}