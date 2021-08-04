using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using AsyncAwaitBestPractices;
using Kit;
using Kit.Forms.Extensions;
using Kit.Model;
using SOE.Data;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using Xamarin.Essentials;
using Xamarin.Forms;
using Command = Xamarin.Forms.Command;

namespace SOE.ViewModels.ViewItems
{
    public class FlyOutViewModel : ModelBase
    {
        private ICommand _TareasCommand;
        public ICommand TareasCommand => _TareasCommand ??= new Command((x) => Goto(1));
        private ICommand _HorarioCommand;
        public ICommand HorarioCommand => _HorarioCommand ??= new Command((x) => Goto(2));
        public ICommand ComingCommand { get; }
        public ICommand SettingCommand { get; set; }
        public ICommand TapAvatarCommand { get; set; }
        public ICommand UserCommand { get; set; }
        public ICommand PrivacityCommand { get; }

        private ICommand _AboutUsCommand;
        public ICommand AboutUsCommand => _AboutUsCommand ??= new Command(AboutUs);

        private void AboutUs()
        {
            Shell.Current.FlyoutIsPresented = false;
            Shell.Current.Navigation.PushAsync(new AboutUsPage()).SafeFireAndForget();
        } 

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
        private Command _DeveloperCommand;

        public Command DeveloperCommand => _DeveloperCommand ??= new Command(Developer);
        public FlyOutViewModel()
        {
            this.SettingCommand = new Command(OpenSettings);
            this.TapAvatarCommand = new Command(TapAvatar);
            this.UserCommand = new Command(UserProfile);
            this.ComingCommand = new Command(Coming);
            this.PrivacityCommand = new Command(Privacity);
            GetAvatar();
        }

        private void Privacity(object obj)
        {
            Shell.Current.FlyoutIsPresented = false;
            Shell.Current.Navigation.PushAsync(new PrivacityPage()).SafeFireAndForget(); ;
        }

        private async void Developer() => await Application.Current.MainPage.Navigation.PushModalAsync(new DeveloperOptions());

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



        private void Goto(int index)
        {
            App.Current.Dispatcher.BeginInvokeOnMainThread(() => MasterPage.Instance.Model.SelectedIndex = index);
            AppShell.Current.FlyoutIsPresented = false;
        }
        private void Coming()
        {
            ComingSoon.Alert();
        }

        private  void OpenSettings()
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
