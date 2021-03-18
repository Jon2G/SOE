using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchoolOrganizer.ViewModels.ViewItems;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.Pages
{
    [DesignTimeVisible(false)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskFirstPage : ContentPage
    {
        public int dateTime { get; set; }
        public int dateTime2 { get; set; }
        public string mes { get; set; }
        public TaskFirstPage()
        {
            InitializeComponent();
            dateTime = DateTime.Now.Day;
            dateTime2 = DateTime.Now.Day + 5;
            mes = DateTime.Now.Month.ToString();
        }

        
    }
}