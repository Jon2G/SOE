using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Kit.Forms.Extensions;
using Kit.Forms.Services;
using Kit.Model;
using Kit.Services.Interfaces;
using SOE.Data.Images;
using SOE.ViewModels.ViewItems;
using SOE.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SOE.ViewModels.PopUps
{
    public class MenuAvatarPopUpViewModel : ModelBase
    {
        private ICommand _CameraCommand;
        public ICommand CameraCommand => _CameraCommand ??= new AsyncCommand<ICrossWindow>(UsarCamara);
        private ICommand _PhotoCommand;
        public ICommand PhotoCommand => _PhotoCommand ??= new AsyncCommand<ICrossWindow>(Galeria);
        public ICommand DeleteCommand { get; }
        public ICommand CloseCommand { get; }
        private readonly FlyOutViewModel FlyOutViewModel;
        public MenuAvatarPopUpViewModel(FlyOutViewModel FlyOutViewModel)
        {
            this.FlyOutViewModel = FlyOutViewModel;
            this.DeleteCommand = new Command<ICrossWindow>(Delete);
            this.CloseCommand = new Command<ICrossWindow>(p => p.Close().SafeFireAndForget());
        }

        private void Delete(ICrossWindow popup)
        {
            Keeper.DeleteAvatar();
            this.FlyOutViewModel.AvatarSource = null;
            popup.Close().SafeFireAndForget();
        }
        private async Task UsarCamara(ICrossWindow popup)
        {
            await Task.Yield();
            popup.Close().SafeFireAndForget();
            if (await Permisos.GetPermissionStatus<Permissions.Camera>() != PermissionStatus.Granted)
            {
                RequestCameraPage request = new();
                await request.ShowDialog();
            }

            if (!await Permisos.RequestStorage())
            {
                return;
            }
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
                if (this.FlyOutViewModel.AvatarSource is null)
                {
                    this.FlyOutViewModel.AvatarSource = (FileImageSource)FileImageSource.FromFile(result.FullPath);
                }
                else
                {
                    this.FlyOutViewModel.AvatarSource.File = result.FullPath;
                }
                Keeper.SaveAvatar(this.FlyOutViewModel.GetAvatarStream()).SafeFireAndForget();
            }

        }
        private async Task Galeria(ICrossWindow popup)
        {
            await Task.Yield();
            popup.Close().SafeFireAndForget();
            if (await Permisos.GetPermissionStatus<Permissions.StorageRead>() != PermissionStatus.Granted)
            {
                RequestCameraPage request = new();
                await request.ShowDialog();
            }
            if (!await Permisos.RequestStorage())
            {
                return;
            }
            var result = await Xamarin.Essentials.MediaPicker.PickPhotoAsync(new MediaPickerOptions()
            {
                Title = "Selecione una imagen"
            });
            var file = await result.LoadPhotoAsync();
            if (file != null)
            {
                if (this.FlyOutViewModel.AvatarSource is null)
                {
                    this.FlyOutViewModel.AvatarSource = (FileImageSource)FileImageSource.FromFile(file.FullName);
                }
                else
                {
                    this.FlyOutViewModel.AvatarSource.File = result.FullPath;
                }
                Keeper.SaveAvatar(this.FlyOutViewModel.GetAvatarStream()).SafeFireAndForget();
            }
        }

    }
}
