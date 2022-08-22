using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Kit.Model;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SOE.Data;
using SOE.Models.Data;
using SOE.Views.PopUps;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
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
            this.OnFingerPrintToogledCommand = new AsyncCommand(OnFingerPrintToogled);
            this.SaveCommand = new AsyncCommand(() =>
            {
                AppData.Instance.User.Settings = Settings;
                return AppData.Instance.User.Save();
            });
            ViewChangeCommand = new Command(ViewOpen);
            Settings = AppData.Instance.User.Settings;
        }

        private async Task OnFingerPrintToogled()
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

        private void ViewOpen(object obj)
        {
            ViewChangePopUp a = new ViewChangePopUp();
            a.ShowDialog().SafeFireAndForget();
        }



    }

}
