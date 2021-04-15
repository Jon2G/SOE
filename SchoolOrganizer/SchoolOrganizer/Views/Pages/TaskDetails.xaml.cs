using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolOrganizer.Models.TaskFirst;
using SchoolOrganizer.ViewModels.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskDetails : ContentPage
    {
        public TaskDetailsViewModel Model { get; set; }
        public TaskDetails(ToDo todo)
        {
            this.Model = new TaskDetailsViewModel(todo);
            BindingContext = this.Model;
            InitializeComponent();

        }
    }
}