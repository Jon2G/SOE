using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using FFImageLoading.Forms;
using Kit.Forms.Extensions;
using Kit.Model;
using SOE.Data;
using SOE.Data.Images;
using SOE.Enums;
using SOE.Models.Scheduler;
using SOE.Models.TaskFirst;
using SOE.Views.PopUps;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
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


        public ObservableCollection<Archive<CachedImage>> Photos { get; }



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
            DeleteImageCommand = new Command<Archive<CachedImage>>(DeleteImage);
            this.Photos = new ObservableCollection<Archive<CachedImage>>();
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

        private void DeleteImage(Archive<CachedImage> img)
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
            foreach (Archive<CachedImage> archive in Photos)
            {
                CachedImage image = archive.Value;
                using (FileStream file = new FileStream(archive.Path, FileMode.OpenOrCreate))
                {
                    using (MemoryStream memory = new MemoryStream(await image.GetImageAsPngAsync()))
                    {
                        await memory.CopyToAsync(file);
                    }
                }
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
            var permiso = new Permissions.Photos();
            if (!await Permisos.TenemosPermiso(new Permissions.Photos()))
            {
                RequestCameraPage request = new RequestCameraPage();
                await request.ShowDialog();
                if (await permiso.CheckStatusAsync() != PermissionStatus.Granted)
                {
                    await Task.Delay(500);
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Ha denegado el acceso a su camera, por favor permita el acceso desde ajustes de su dispositivo", "Alerta");
                    return;
                }
                await Task.Delay(500);
            }
            if (!await Permisos.RequestStorage())
            {
                await Task.Delay(500);
                Acr.UserDialogs.UserDialogs.Instance.Alert("Ha denegado el acceso a su camera, por favor permita el acceso desde ajustes de su dispositivo", "Alerta");
                return;
            }
            AddPhoto(await MediaPicker.PickPhotoAsync(new MediaPickerOptions()));
        }

        private async void UsarCamara()
        {
            var permiso = new Permissions.Camera();
            if (!await Permisos.TenemosPermiso(permiso))
            {
                RequestCameraPage request = new RequestCameraPage();
                await request.ShowDialog();
                if (await permiso.CheckStatusAsync() != PermissionStatus.Granted)
                {
                    await Task.Delay(500);
                    Acr.UserDialogs.UserDialogs.Instance.Alert("Ha denegado el acceso a su camera, por favor permita el acceso desde ajustes de su dispositivo", "Alerta");
                    return;
                }
                await Task.Delay(500);
            }

            if ((await Permisos.EnsurePermission<Permissions.Camera>()) != PermissionStatus.Granted)
            {
                return;
            }
            AddPhoto(await MediaPicker.CapturePhotoAsync(new MediaPickerOptions()));
        }

        private void AddPhoto(FileResult result)
        {
            if (result is null)
            {
                return;
            }
            Archive<CachedImage> archive = new Archive<CachedImage>(result, FileType.Photo)
            {
                Value = new CachedImage()
                {
                    DownsampleToViewSize = true,
                    Aspect = Aspect.AspectFit,
                    Source = ImageSource.FromFile(result.FullPath)
                }
            };
            Photos.Add(archive);
        }



    }
}
