using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Kit;
using Kit.Forms.Extensions;
using Kit.Model;
using SOE.Data;
using SOE.Views.Pages;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SOE.ViewModels.ViewItems
{
    public class FlyOutViewModel : ModelBase
    {
        public ICommand TareasCommand { get; }
        public ICommand SettingCommand { get; set; }
        public ICommand TapAvatarCommand { get; set; }
        public ICommand UserCommand { get; set; }
        private FileImageSource _AvatarSource;
        public string _UserInitials;

        public string UserInitials
        {
            get => _UserInitials;
            set
            {
                _UserInitials = value;
                OnPropertyChanged();
            }
        }

        public FileImageSource AvatarSource
        {
            get => _AvatarSource;
            set
            {
                _AvatarSource = value;
                Raise(() => AvatarSource);
            }
        }

        public FlyOutViewModel()
        {
            this.SettingCommand = new Command(OpenSettings);
            this.TareasCommand = new Command(Tareas);
            this.TapAvatarCommand = new Command(TapAvatar);
            this.UserCommand = new Command(UserProfile);
            GetAvatar();
        }

        private async void GetAvatar()
        {
            await Task.Yield();
            //this.AvatarSource = User.GetAvatar();
            string name = AppData.Instance.User.Name;
            this.UserInitials = name.ExtractInitialsFromName();
        }

        private void TapAvatar()
        {
            var config = new ActionSheetConfig()
            {
                Cancel = new ActionSheetOption("Cancelar"),
                Title = "Cambiar imagen de perfil",
                Message = "Seleccione una opción",
                Options = new List<ActionSheetOption>()
                {
                    new ActionSheetOption("Usar camara", UsarCamara),
                    new ActionSheetOption("Seleccionar imagén desde la galeria", Galeria),
                },
                UseBottomSheet = true
            };
            Acr.UserDialogs.UserDialogs.Instance.ActionSheet(config);


        }

        private async void Galeria()
        {
            if (!await Permisos.RequestStorage())
            {
                return;
            }
            var result = await Xamarin.Essentials.MediaPicker.PickPhotoAsync(new MediaPickerOptions()
            {
                Title = "Selecione una imagen"
            });
            if (result != null)
            {
                if (this.AvatarSource is null)
                {
                    this.AvatarSource = (FileImageSource)FileImageSource.FromFile(result.FullPath);
                }
                else
                {
                    this.AvatarSource.File = result.FullPath;
                }

                //await User.SaveAvatar(result);
            }
        }

        private async void UsarCamara()
        {
            if ((await Permisos.EnsurePermission<Permissions.Camera>()) != PermissionStatus.Granted)
            {
                return;
            }
            var result = await Xamarin.Essentials.MediaPicker.CapturePhotoAsync(new MediaPickerOptions()
            {
                Title = "Selecione una imagen"
            });
            if (result != null)
            {
                if (this.AvatarSource is null)
                {
                    this.AvatarSource = (FileImageSource)FileImageSource.FromFile(result.FullPath);
                }
                else
                {
                    this.AvatarSource.File = result.FullPath;
                }
                //await User.SaveAvatar(result);
            }

        }



        private void Tareas()
        {
            //Shell.Current.Navigation.PushAsync(new TaskFirstPage(), true);
        }

        private void OpenSettings()
        {
            Shell.Current.FlyoutIsPresented = false;
            Shell.Current.Navigation.PushAsync(new SettingsView(), true);
        }

        private void UserProfile(object obj)
        {
            Shell.Current.FlyoutIsPresented = false;
            Shell.Current.Navigation.PushAsync(new UserProfile(), true);
        }
    }
}
