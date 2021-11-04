using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Google.Android.Play.Core.Appupdate;
using Com.Google.Android.Play.Core.Install.Model;
using Kit.Droid;
using SOE.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Object = Java.Lang.Object;
using Task = Com.Google.Android.Play.Core.Tasks.Task;

[assembly: Xamarin.Forms.Dependency(typeof(SOE.Droid.Services.AvailableUpdateService))]
namespace SOE.Droid.Services
{
    public class AvailableUpdateService : IAvailableUpdateService
    {
        public void HasUpdate()
        {
            Activity activity= ((Kit.Droid.ToolsImplementation)Tools.Instance).MainActivity;
            IAppUpdateManager AppUpdateManager = AppUpdateManagerFactory.Create(activity);
            // Returns an intent object that you use to check for an update.
            Task appUpdateInfoTask = AppUpdateManager.AppUpdateInfo;
            // Checks that the platform will allow the specified type of update.
            appUpdateInfoTask.AddOnSuccessListener(
                new OnUpdateSuccesListener(AppUpdateManager, activity));
        }
    }
}