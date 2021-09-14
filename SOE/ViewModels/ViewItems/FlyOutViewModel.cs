using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Kit;
using Kit.Forms.Extensions;
using Kit.Forms.Services;
using Kit.Model;
using Kit.Sql.Reflection;
using Plugin.XamarinFormsSaveOpenPDFPackage;
using SOE.Data;
using SOE.Data.Images;
using SOE.Models.Data;
using SOE.Views.Pages;
using SOE.Views.PopUps;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using Command = Xamarin.Forms.Command;

namespace SOE.ViewModels.ViewItems
{
    public class FlyOutViewModel : ModelBase
    {
        private ICommand _TareasCommand;
        public ICommand TareasCommand => _TareasCommand ??= new Command((x) => Goto(1));

        private ICommand _PdfCalendarCommand;
        public ICommand PdfCalendarCommand => this._PdfCalendarCommand ??= new AsyncCommand(PdfCalendar);

        private ICommand _HorarioCommand;
        public ICommand HorarioCommand => _HorarioCommand ??= new Command((x) => Goto(2));
        public ICommand ComingCommand { get; }
        public ICommand SettingCommand { get; set; }
        private ICommand _TapAvatarCommand;
        public ICommand TapAvatarCommand => _TapAvatarCommand ??= new AsyncCommand(TapAvatar);
        public ICommand UserCommand { get; set; }
        public ICommand PrivacityCommand { get; }
        private ICommand _AboutUsCommand;
        public ICommand AboutUsCommand => _AboutUsCommand ??= new Command(AboutUs);
        private ICommand _CameraCommand;
        public ICommand CameraCommand => _CameraCommand ??= new Command(UsarCamara);
        private ICommand _PhotoCommand;
        public ICommand PhotoCommand => _PhotoCommand ??= new Command(Galeria);
        private void AboutUs()
        {
            AppShell.CloseFlyout();
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
            this.UserCommand = new Command(UserProfile);
            this.ComingCommand = new Command(Coming);
            this.PrivacityCommand = new Command(Privacity);
            GetAvatar();
        }

        private async Task PdfCalendar()
        {
            await Task.Yield();
            if (!CrossXamarinFormsSaveOpenPDFPackage.IsSupported)
            {
                Shell.Current.DisplayAlert("Su dispositivo no soporta esta característica, lo sentimos...", "Mensaje informativo", "Ok").SafeFireAndForget();
                return;
            }
            using (ReflectionCaller caller = ReflectionCaller.FromAssembly<App>())
            {
                using (Stream resourceStream = caller.GetResource("CalendarioEscolar.pdf"))
                {
                    using (MemoryStream stream = new())
                    {
                        resourceStream.Position = 0;
                        await resourceStream.CopyToAsync(stream);
                        stream.Position = 0;
                        await CrossXamarinFormsSaveOpenPDFPackage.Current.SaveAndView(
                             $"Calendario_Escolar.pdf", "application/pdf", stream,
                             PDFOpenContext.InApp);
                    }
                }
            }


        }
        private void Privacity(object obj)
        {
            AppShell.CloseFlyout();
            Shell.Current.Navigation.PushAsync(new PrivacityPage()).SafeFireAndForget(); ;
        }
        private async void Developer() => await Application.Current.MainPage.Navigation.PushModalAsync(new DeveloperOptions());
        private async void GetAvatar()
        {
            await Task.Yield();
            this.AvatarSource = await Keeper.GetAvatar();
            string name = AppData.Instance.User.Name;
            this.UserInitials = name.ExtractInitialsFromName();
        }
        private async Task TapAvatar()
        {
            var page = new MenuAvatarPopUp();
            await page.ShowDialog();
        }
        private async void Galeria()
        {
            RequestCameraPage request = new();
            await request.ShowDialog();
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
                if (this.AvatarSource is null)
                {
                    this.AvatarSource = (FileImageSource)FileImageSource.FromFile(file.FullName);
                }
                else
                {
                    this.AvatarSource.File = result.FullPath;
                }
                Keeper.SaveAvatar(GetAvatarStream()).SafeFireAndForget();
            }
        }
        public async Task<Stream> GetAvatarStream()
        {
            await Task.Yield();
            return this.AvatarSource.ImageToStream();
        }
        private async void UsarCamara()
        {
            RequestCameraPage request = new();
            await request.ShowDialog();
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
                if (this.AvatarSource is null)
                {
                    this.AvatarSource = (FileImageSource)FileImageSource.FromFile(result.FullPath);
                }
                else
                {
                    this.AvatarSource.File = result.FullPath;
                }
                Keeper.SaveAvatar(GetAvatarStream()).SafeFireAndForget();
            }

        }
        private void Goto(int index)
        {
            App.Current.Dispatcher.BeginInvokeOnMainThread(() => MasterPage.Instance.Model.SelectedIndex = index);
            AppShell.CloseFlyout();
        }
        private void Coming()
        {
            AppShell.CloseFlyout();
            Shell.Current.Navigation.PushAsync(new AcademicDirectory(), true);
        }
        private void OpenSettings()
        {
            AppShell.CloseFlyout();
            Shell.Current.Navigation.PushAsync(new SettingsView(), true);

        }
        private void UserProfile(object obj)
        {
            AppShell.CloseFlyout();
            Shell.Current.Navigation.PushAsync(new UserProfile(), true);
        }
    }
}
