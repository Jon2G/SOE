using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolOrganizer.Models.TaskFirst;
using SchoolOrganizer.ViewModels.ViewItems;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace SchoolOrganizer.Views.Pages
{
    [DesignTimeVisible(false)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskFirstPage : ContentView
    {
       
        public TaskFirstPage()
        {
            InitializeComponent();
            BindingContext = new TaskFirstViewModel();
        }


    }
}