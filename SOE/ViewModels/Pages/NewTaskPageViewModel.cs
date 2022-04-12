using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Kit;
using Kit.Forms.Extensions;
using Kit.Forms.Services;
using Kit.Model;
using SOE.Data.Archives;
using SOE.Enums;
using SOE.Models;
using SOE.Models.TodoModels;
using SOE.Notifications;
using SOE.Views.PopUps;
using SOE.Widgets;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
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
            OnDateChangedCommand = new AsyncCommand(OnDateChanged);
            DeleteImageCommand = new Command<PhotoArchive>(DeleteImage);
            this.Photos = new ObservableCollection<PhotoArchive>();
        }

        private async Task OnDateChanged()
        {
            await Task.Yield();
            if (this.Tarea is not null && this.Tarea.Subject != null && this.Tarea.Date != null)
            {
                ClassTime? classTime = await this.Tarea.Subject.GetClassTime(this.Tarea.Date.DayOfWeek);
                if (classTime is not null)
                    this.Tarea.Time = classTime.Begin;
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
            if (string.IsNullOrEmpty(Tarea.Title))
            {
                Acr.UserDialogs.UserDialogs.Instance.Alert("La tarea debe contener titulo para poder ser guardada");
                if (string.IsNullOrEmpty(Tarea.Title))
                    Save(obj);
                return;
            }
            using (Acr.UserDialogs.UserDialogs.Instance.Loading("Guardando tarea..."))
            {
                await this.Tarea.Save(Photos);
                ToDosWidget.UpdateWidget();
                DependencyService.Get<ILocalNotificationService>()?.ReSheduleTask(this.Tarea);

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
            if (PhotosLimit())
            {
                return;
            }
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
            AddPhoto(await MediaPicker.PickPhotoAsync()).SafeFireAndForget();
        }

        private bool PhotosLimit()
        {
            if ((this.Photos?.Count ?? 0) >= 4)
            {
                Shell.Current.CurrentPage.DisplayAlert("Alerta", "Solo se permiten 4 fotos por tarea", "Ok").SafeFireAndForget();
                return true;
            }
            return false;
        }

        private async void UsarCamara()
        {
            if (PhotosLimit())
            {
                return;
            }
            var permiso = new Permissions.Camera();
            if (!await Permisos.TenemosPermiso(permiso))
            {
                RequestCameraPage request = new();
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
            AddPhoto(await MediaPicker.CapturePhotoAsync()).SafeFireAndForget();
        }

        private async Task AddPhoto(FileResult result, bool retry = true)
        {
            await Task.Yield();
            try
            {
                if (result is null)
                {
                    return;
                }

                FileInfo file = await result.LoadPhotoAsync();
                if (file is not null)
                {
                    PhotoArchive archive = new(file.FullName, FileType.Photo);
                    Photos.Add(archive);
                }
            }
            catch (System.IO.IOException ioException)
            {
                if (retry && await Permisos.RequestStorage())
                {
                    await AddPhoto(result, retry: false);
                    return;
                }
                Log.Logger.Error(ioException, "IOException- AddPhoto");
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "AddPhoto");
            }

        }



    }
}
