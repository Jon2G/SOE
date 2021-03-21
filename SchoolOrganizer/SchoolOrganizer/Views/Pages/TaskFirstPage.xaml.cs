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
    public partial class TaskFirstPage : ContentPage,INotifyPropertyChanged
    {

        private int _dateTime;
        public int dateTime
        {
            get { return _dateTime; }
            set
            {
                _dateTime = value;
                OnPropertyChanged();
            }
        }
        private int _dateTime2;
        public int dateTime2
        {
            get { return _dateTime2; }
            set
            {
                _dateTime2 = value;
                OnPropertyChanged();
            }
        }
        private string _mes;
        public string mes
        {
            get { return _mes; }
            set
            {
                _mes = value;
                OnPropertyChanged();
            }
        }
        public TaskFirstPage()
        {
            InitializeComponent();
            dateTime = DateTime.Now.Hour;
            dateTime2 = DateTime.Now.Minute;
            
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                Device.BeginInvokeOnMainThread(()=> mes=DateTime.Now.ToString("hh:mm:ss tt")
                    );
                return true;
            });
        }

      
    }
}