using System;
using System.Threading.Tasks;

using Kit.Forms.Extensions;
using SOE.Data;
using SOE.Data.Images;
using SOE.Models.TodoModels;
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
            var Task = this.Modelo.Tarea;
            //this.Modelo.Tareas.Add(Task);
            this.Modelo.Tarea = new ToDo();

        }

        private void DatePick_OnDateSelected(object sender, DateChangedEventArgs e)
        {
            this.Modelo?.OnDateChangedCommand?.Execute(sender);
        }

        private async void GetPhotos(ToDo toDo)
        {
            await Task.Yield();
            foreach (Archive file in AppData.Instance.LiteConnection.Table<Archive>()
                .Where(x => x.IdKeeper == toDo.IdKeeper))
            {
                PhotoArchive archive = new(file.Path, Enums.FileType.Photo,false);
                this.Modelo.Photos.Add(archive);
            }
        }
    }
}