using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using SchoolOrganizer.Views.PopUps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Kit.Model;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SchoolOrganizer.Data;
using Xamarin.Forms;
using SchoolOrganizer.Models.Data;
using Kit;
using Acr.UserDialogs;
using Kit.Forms.Extensions;
using Xamarin.Essentials;

namespace SchoolOrganizer.ViewModels.Pages
{
    public class SettingsViewModel : ModelBase
    {
        public Settings Settings { get; set; }
        public ICommand SaveCommand { get; }
        public ICommand ViewChangeCommand { get; set; }
        public ICommand ShareDatabaseCommand { get; set; }


        public ICommand OnFingerPrintToogledCommand { get; set; }

        public SettingsViewModel()
        {
            this.OnFingerPrintToogledCommand = new Command(OnFingerPrintToogled);
            this.SaveCommand = new Command(Save);
            ViewChangeCommand = new Command(ViewOpen);
            this.ShareDatabaseCommand = new Command(ShareDatabase);
            Settings = AppData.Instance.User.GetSettings();
        }

        private async void ShareDatabase()
        {
            using (var loading = UserDialogs.Instance.Loading("Cargando..."))
            {
                Log.Logger.Debug("Se solicito la base de datos lite");
                if (!await AbrirArchivo(AppData.Instance.LiteConnection.DatabasePath, "Base de datos local"))
                {
                    await UserDialogs.Instance.AlertAsync("No se pudo abrir el archivo", "Mensaje informativo");
                }
            }
        }
        private static async Task<bool> AbrirArchivo(string ruta, string titulo)
        {
            if (await Permisos.PedirPermiso(Plugin.Permissions.Abstractions.Permission.Storage, "Permita el acceso al almacentamiento"))
            {
                FileInfo file = new FileInfo(ruta);
                if (!file.Exists)
                {
                    await UserDialogs.Instance.AlertAsync("No se encontro el archivo", "Mensaje informativo");
                    return false;
                }
                try
                {
                    ////
                    string temporal = file.FullName;
                    if (file.Directory.FullName != FileSystem.CacheDirectory)
                    {
                        temporal = $"{file.Name.Replace(file.Extension, String.Empty)}_{Guid.NewGuid():N}{file.Extension}";
                        temporal = Path.Combine(FileSystem.CacheDirectory, temporal);
                        File.Copy(file.FullName, temporal);
                    }
                    await Share.RequestAsync(new ShareFileRequest
                    {
                        Title = titulo,
                        File = new ShareFile(temporal)
                    });
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "Al abrir el archivo");
                    await UserDialogs.Instance.AlertAsync($"No se pudo abrir el archivo\n{ex?.Message}", "Mensaje informativo");
                    return false;
                }
            }
            return false;
        }
        private async void OnFingerPrintToogled()
        {
            if (this.Settings.IsFingerPrintActive)
            {
                FingerprintAvailability Availability =
                    await CrossFingerprint.Current.GetAvailabilityAsync(true);
                switch (Availability)
                {
                    case FingerprintAvailability.Available:
                        //:) ta bueno siguele
                        break;
                    case FingerprintAvailability.Denied:
                    case FingerprintAvailability.NoPermission:
                        Acr.UserDialogs.UserDialogs.Instance.Alert("Su dispostivo no cuenta con el permiso para utilizar la autenticación por huella", "Atención", "Ok");
                        this.Settings.IsFingerPrintActive = false;
                        break;
                    case FingerprintAvailability.NoFallback:
                    case FingerprintAvailability.NoFingerprint:
                        Acr.UserDialogs.UserDialogs.Instance.Alert("Debe definir al menos una huella en los ajustes de su dispostivio antes de poder usar esta característica.", "Atención", "Ok");
                        this.Settings.IsFingerPrintActive = false;
                        break;

                    case FingerprintAvailability.NoImplementation:
                    case FingerprintAvailability.NoApi:
                    case FingerprintAvailability.NoSensor:
                    case FingerprintAvailability.Unknown:
                        Acr.UserDialogs.UserDialogs.Instance.Alert("Su dispostivo no cuenta con lector de huella", "Atención", "Ok");
                        this.Settings.IsFingerPrintActive = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
        }

        private async void ViewOpen(object obj)
        {
            var a = new ViewChangePopUp();
            await a.ShowDialog();
        }

        private void Save()
        {
            Settings.Save();
        }

    }

}
