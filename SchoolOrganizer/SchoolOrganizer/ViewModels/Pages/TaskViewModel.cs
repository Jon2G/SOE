
using SchoolOrganizer.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;
using Kit.Sql.Base;
using Kit.Sql.Helpers;
using SchoolOrganizer.Data;
using SchoolOrganizer.Models.Scheduler;
using SchoolOrganizer.Models.TaskFirst;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using Kit.Model;
using SchoolOrganizer.Data.Images;
using SchoolOrganizer.Enums;
using SchoolOrganizer.Models;
using Xamarin.Essentials;
using Kit.Forms.Extensions;

namespace SchoolOrganizer.ViewModels.Pages
{

    public class TaskViewModel : ModelBase
    {

        public Command TaskCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OnDateChangedCommand { get; }
        public ICommand DeleteImageCommand { get; set; }
        public ICommand CameraImageCommand { get; set; }
        public ICommand GaleryImageCommand { get; set; }

        private Subject _selectedSubject;


        public ObservableCollection<Archive<FileImageSource>> Photos { get; }



        private ToDo _Tarea;

        public ToDo Tarea
        {
            get => _Tarea;
            set
            {
                _Tarea = value;
                Raise(() => Tarea);
            }
        }
        private FileImageSource _TaskImage;
        public FileImageSource TaskImage
        {
            get => _TaskImage;
            set
            {
                _TaskImage = value;
                Raise(() => TaskImage);
            }
        }
        public TaskViewModel()
        {
            Tarea = new ToDo();
            TaskCommand = new Command(TaskClicked);
            SaveCommand = new Command(Save);
            CameraImageCommand = new Command(UsarCamara);
            GaleryImageCommand = new Command(Galeria);
            OnDateChangedCommand = new Command(OnDateChanged);
            DeleteImageCommand = new Command<Archive<FileImageSource>>(DeleteImage);
            this.Photos = new ObservableCollection<Archive<FileImageSource>>();
        }

        private void OnDateChanged()
        {
            if (this.Tarea is not null && this.Tarea.Subject != null && this.Tarea.Date != null)
            {
                this.Tarea.Time =
                    TimeSpan.FromTicks(AppData.Instance.LiteConnection.Single<long>
                        ($"SELECT BEGIN FROM ClassTime WHERE IdSubject={this.Tarea.Subject.Id} AND DAY={(int)this.Tarea.Date.DayOfWeek}"));
            }
        }

        private void DeleteImage(Archive<FileImageSource> img)
        {
            this.Photos.Remove(img);
        }

        private async Task Save()
        {
            Document.Delete(this.Tarea.IdDocument);
            if (this.Tarea.Description == null)
            {
                this.Tarea.Description = "";
            }
            Document doc = Document.PaseAndSave(this.Tarea.Description);
            this.Tarea.IdDocument = doc.Id;


            Keeper.Delete(this.Tarea.IdKeeper);
            Keeper keeper = Keeper.New();
            foreach (Archive archive in Photos)
            {
                await keeper.Save(archive);
            }
            this.Tarea.IdKeeper = keeper.Id;
            /////////////



            AppData.Instance.LiteConnection.InsertOrReplace(this.Tarea);
            /////////////
            if (Shell.Current is AppShell app)
            {
                await app.MasterPage.TaskFirstPage.Model.Refresh();
            }

            await Shell.Current.Navigation.PopToRootAsync(true);
            //photos ?
        }
        private async void Save(object obj)
        {
            if (Tarea.Subject == null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("Por favor seleccione una materia");
                await SelectSubject();
                if (Tarea.Subject is not null)
                    Save(obj);
                return;
            }
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Guardando tarea..."))
            {
                await Save();
            }
        }

        private async Task SelectSubject()
        {
            var pr = new SubjectPopUp();
            await pr.ShowDialog();
            this.Tarea.Subject = pr.Modelo.SelectedSubject;
        }
        private async void TaskClicked()
        {
            await SelectSubject();
        }

        private async void Galeria()
        {
            if (!await Permisos.TenemosPermiso(Plugin.Permissions.Abstractions.Permission.Storage))
            {
                await Permisos.PedirPermiso(Plugin.Permissions.Abstractions.Permission.Storage);
            }
            AddPhoto(await MediaPicker.PickPhotoAsync(new MediaPickerOptions()));
        }

        private async void UsarCamara()
        {
            if (!await Permisos.TenemosPermiso(Plugin.Permissions.Abstractions.Permission.Camera))
            {
                await Permisos.PedirPermiso(Plugin.Permissions.Abstractions.Permission.Camera);
            }
            AddPhoto(await MediaPicker.CapturePhotoAsync(new MediaPickerOptions()));
        }

        private void AddPhoto(FileResult result)
        {
            if (result is null)
            {
                return;
            }
            Archive<FileImageSource> archive = new Archive<FileImageSource>(result, FileType.Photo)
            {
                Value = (FileImageSource)ImageSource.FromFile(result.FullPath)
            };
            Photos.Add(archive);
        }



    }
}
