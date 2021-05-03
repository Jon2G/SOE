using System;
using System.IO;
using System.Threading.Tasks;
using FFImageLoading.Cache;
using FFImageLoading.Forms;
using FFImageLoading.Work;
using Rg.Plugins.Popup.Pages;
using SchoolOrganizer.Data;
using SchoolOrganizer.Data.Images;
using SchoolOrganizer.Models.TaskFirst;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SchoolOrganizer.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Skip)]
    public partial class TaskPage : ContentPage
    {

        private int _position;
        public int Position { get { return _position; } set { _position = value; OnPropertyChanged(); } }
        public TaskPage()
        {

            InitializeComponent();
            DatePick.MinimumDate = DateTime.Now;
           
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
                this.Modelo.Photos.Add(new Archive<FileImageSource>(
                    archive,
                    (FileImageSource)FileImageSource.FromFile(archive.Path)));
            }
        }
    }
}