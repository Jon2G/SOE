using System;
using System.Windows.Input;
using Kit.Model;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SOE.Data;
using SOE.Models.Data;
using SOE.Views.PopUps;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{
    public class SettingsViewModel : ModelBase
    {
        public Settings Settings { get; set; }
        public ICommand SaveCommand { get; }
        public ICommand ViewChangeCommand { get; set; }
        public ICommand OnFingerPrintToogledCommand { get; set; }

        public SettingsViewModel()
        {
            this.OnFingerPrintToogledCommand = new Command(OnFingerPrintToogled);
            this.SaveCommand = new Command(Save);
            ViewChangeCommand = new Command(ViewOpen);
            Settings = AppData.Instance.User.Settings?? AppData.Instance.User.GetSettings();
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
