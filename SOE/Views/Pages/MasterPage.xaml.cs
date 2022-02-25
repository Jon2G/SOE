using AsyncAwaitBestPractices;
using Kit;
using Kit.Enums;
using Kit.Forms.Services.Interfaces;
using Microsoft.AppCenter.Crashes;
using SOE.Data;
using SOE.FireBase;
using SOE.Notifications;
using SOE.Services;
using SOE.Views.ViewItems.ScheduleView;
using SOE.Widgets;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
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

        protected override void OnFirstAppearing()
        {
            base.OnFirstAppearing();
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
                        try { this.Model.SelectedIndex = 1; }
                        catch (Exception ex)
                        {
                            Crashes.GenerateTestCrash();
                            Log.Logger.Error(ex, "CenterCarrousel");
                        }

                    }

                    Dispatcher.BeginInvokeOnMainThread(CenterCarrousel);
                }

                await this.Model.OnAppearing();
                if (Tools.Instance.RuntimePlatform == RuntimePlatform.Android)
                    DependencyService.Get<ILocalNotificationService>()?.ScheduleAll();
                TimeLineWidget.UpdateWidget();
                ToDosWidget.UpdateWidget();
                UpdateService.AvaibleUpdate();
                if (Device.RuntimePlatform == Device.iOS && Xamarin.Essentials.DeviceInfo.Version.Major >= 14)
                {
                    var appTrackingTransparencyPermission =
                        TinyIoC.TinyIoCContainer.Current.Resolve<IAppTrackingTransparencyPermission>();
                    var status = PermissionStatus.Denied;
                    if (appTrackingTransparencyPermission is not null)
                        status = await appTrackingTransparencyPermission.CheckStatusAsync();
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
            catch (Exception ex)
            {
                Crashes.GenerateTestCrash();
                Log.Logger.Error(ex, "CenterCarrousel");
                this.DisplayAlert("Error", ex.ToString(), "Ok").SafeFireAndForget();
            }
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
                Crashes.GenerateTestCrash(); Log.Logger.Error(ex, "Execute");
                App.Current.MainPage.DisplayAlert("Error", ex.ToString(), "Ok").SafeFireAndForget();
            }
        }
    }
}