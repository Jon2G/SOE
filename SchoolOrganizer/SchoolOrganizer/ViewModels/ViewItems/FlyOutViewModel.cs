using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Forms9Patch;
using Kit;
using Kit.Model;
using SchoolOrganizer.Data;
using SchoolOrganizer.Fonts;
using SchoolOrganizer.Models.Academic;
using SchoolOrganizer.Models.Data;
using SchoolOrganizer.Views.Pages;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using ImageSource = Xamarin.Forms.ImageSource;

namespace SchoolOrganizer.ViewModels.ViewItems
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
            this.AvatarSource = User.GetAvatar();
            string name = AppData.Instance.User.Name;
            this.UserInitials = Kit.Extensions.Helpers.ExtractInitialsFromName(name);
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

                await User.SaveAvatar(result);
            }
        }

        private async void UsarCamara()
        {
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
                await User.SaveAvatar(result);
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
