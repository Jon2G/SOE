using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using FFImageLoading.Forms;
using Kit.Forms.Extensions;
using Kit.Model;
using SOE.Data;
using SOE.Data.Images;
using SOE.Enums;
using SOE.Interfaces;
using SOE.Models.TaskFirst;
using SOE.Views.PopUps;
using SOE.Widgets;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{

    public class NewTaskPageViewModel : ModelBase
    {

        public Command TaskCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand OnDateChangedCommand { get; }
        public ICommand DeleteImageCommand { get; set; }
        public ICommand CameraImageCommand { get; set; }
        public ICommand GaleryImageCommand { get; set; }



        public ObservableCollection<PhotoArchive> Photos { get; }



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
        public NewTaskPageViewModel()
        {
            Tarea = new ToDo();
            TaskCommand = new Command(TaskClicked);
            SaveCommand = new Command(Save);
            CameraImageCommand = new Command(UsarCamara);
            GaleryImageCommand = new Command(Galeria);
            OnDateChangedCommand = new Command(OnDateChanged);
            DeleteImageCommand = new Command<PhotoArchive>(DeleteImage);
            this.Photos = new ObservableCollection<PhotoArchive>();
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

        private void DeleteImage(PhotoArchive img)
        {
            this.Photos.Remove(img);
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
            if (Tarea.Title==null)
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("La tarea debe contener titulo para poder ser guardada");
                if (Tarea.Title is not null)
                    Save(obj);
                return;
            }
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Guardando tarea..."))
            {
                await ToDo.Save(this.Tarea, Photos);
                ToDosWidget.UpdateWidget();
                DependencyService.Get<IStartNotificationsService>()?.ReSheduleTask(this.Tarea);

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
            PhotoArchive archive = new PhotoArchive(result.FullPath, FileType.Photo)
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
