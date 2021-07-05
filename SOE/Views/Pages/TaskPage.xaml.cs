using System;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using SOE.Data;
using SOE.Data.Images;
using SOE.Models.TaskFirst;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
    public partial class TaskPage : ContentPage
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

        public TaskPage()
        {

            InitializeComponent();
        }

        public TaskPage(ToDo toDo)
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
            foreach (Archive archive in AppData.Instance.LiteConnection.Table<Archive>()
                .Where(x => x.IdKeeper == toDo.IdKeeper))
            {
                this.Modelo.Photos.Add(new Archive<CachedImage>(
                    archive,
                    new CachedImage()
                    {
                        Source = FileImageSource.FromFile(archive.Path),
                        DownsampleToViewSize = true
                    }));
            }
        }
    }
}