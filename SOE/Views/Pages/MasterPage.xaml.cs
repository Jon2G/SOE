using System;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using Kit;
using Kit.Forms.Services.Interfaces;
using SOE.API;
using SOE.Data;
using SOE.FireBase;
using SOE.Interfaces;
using SOE.Models.Scheduler;
using SOE.Models.TaskFirst;
using SOE.Services;
using SOE.Services.ActionResponse;
using SOE.Views.ViewItems.ScheduleView;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using DeviceInfo = Xamarin.Forms.Internals.DeviceInfo;
using Log = Kit.Log;

namespace SOE.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPage
    {
        public static MasterPage Instance { get; private set; }
        public MasterPage()
        {
            Instance = this;
            InitializeComponent();
            AppData.Instance.SAES = this.Saes;
        }
        protected override bool OnBackButtonPressed()
        {
            switch (Model.SelectedView)
            {
                case ScheduleViewMain schedule:
                    if (schedule.OnBackButtonPressed())
                    {
                        return true;
                    }
                    break;
            }
            return base.OnBackButtonPressed();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.OnAppearingAsync().SafeFireAndForget();
        }

        private async Task OnAppearingAsync()
        {
            await Task.Yield();
            try
            {
                if (this.Model.SelectedIndex <= 0)
                {
                    void CenterCarrousel()
                    {
                        try { this.Model.SelectedIndex = 1; } catch (Exception ex) { Log.Logger.Error(ex, "CenterCarrousel"); }

                    }
                    Dispatcher.BeginInvokeOnMainThread(CenterCarrousel);
                }
                await this.Model.OnAppearing();
                DependencyService.Get<IStartNotificationsService>()?.StartNotificationsService();
                if (Device.RuntimePlatform == Device.iOS)
                {
                    var appTrackingTransparencyPermission = DependencyService.Get<IAppTrackingTransparencyPermission>();
                    var status = await appTrackingTransparencyPermission.CheckStatusAsync();
                    switch (status)
                    {
                        case PermissionStatus.Denied:
                        case PermissionStatus.Granted:
                            return;
                        case PermissionStatus.Disabled:
                        case PermissionStatus.Unknown:
                            appTrackingTransparencyPermission.RequestAsync((s) => { });
                            break;
                    }
                }
            }
            catch (Exception ex) { Log.Logger.Error(ex, "CenterCarrousel"); this.DisplayAlert("Error", ex.ToString(), "Ok").SafeFireAndForget(); }

        }
        public static void ResponseTo(IActionResponse PendingAction) =>
            App.Current.Dispatcher.BeginInvokeOnMainThread(action: () => Execute(PendingAction).SafeFireAndForget());
        private static async Task Execute(IActionResponse pendingAction)
        {
            await Task.Yield();
            try
            {
                using (Acr.UserDialogs.UserDialogs.Instance.Loading("Espere un momento..."))
                {
                    await pendingAction.Execute();
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Execute");
                App.Current.MainPage.DisplayAlert("Error", ex.ToString(), "Ok").SafeFireAndForget();
            }
        }
    }
}