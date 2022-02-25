using P42.Utils;
using SOE.Models.TodoModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
    public partial class NewTaskPage : ContentPage
    {

        private int _position;

        public int Position
        {
            get { return _position; }
            set
            {
                _position = value;
                OnPropertyChanged();
            }
        }
        public NewTaskPage()
        {
            InitializeComponent();
        }

        public NewTaskPage(ToDo toDo)
        {
            InitializeComponent();
            this.Modelo.Tarea = toDo;
            GetPhotos(toDo);
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            ToDo Task = this.Modelo.Tarea;
            //this.Modelo.Tareas.Add(Task);
            this.Modelo.Tarea = new ToDo();

        }

        private void DatePick_OnDateSelected(object sender, DateChangedEventArgs e)
        {
            this.Modelo?.OnDateChangedCommand?.Execute(sender);
        }

        private void GetPhotos(ToDo toDo)
        {
            toDo.GetPhotos().ContinueWith(t =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.Modelo.Photos.AddRange(t.Result);
                });
            });
        }
    }
}