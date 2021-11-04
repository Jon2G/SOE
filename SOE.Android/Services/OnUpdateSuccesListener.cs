using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Google.Android.Play.Core.Appupdate;
using Com.Google.Android.Play.Core.Install.Model;
using Java.Lang;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOE.Droid.Services
{
    public class OnUpdateSuccesListener : Java.Lang.Object, Com.Google.Android.Play.Core.Tasks.IOnSuccessListener
    {
        private readonly IAppUpdateManager AppUpdateManager;
        private readonly Activity Activity;
        public OnUpdateSuccesListener(IAppUpdateManager AppUpdateManager,Activity Activity)
        {
            this.AppUpdateManager = AppUpdateManager;
            this.Activity = Activity;
        }

        public void OnSuccess(Object p0)
        {
            if (p0 is AppUpdateInfo appUpdateInfo)
            {
                if (appUpdateInfo.UpdateAvailability() == UpdateAvailability.UpdateAvailable)
                    // This example applies an immediate update. To apply a flexible update
                    // instead, pass in AppUpdateType.FLEXIBLE)
                {
                    if (appUpdateInfo.IsUpdateTypeAllowed(AppUpdateType.Immediate))
                    {
                        AppUpdateManager.StartUpdateFlowForResult(
                            // Pass the intent that is returned by 'getAppUpdateInfo()'.
                            appUpdateInfo,
                            // Or 'AppUpdateType.FLEXIBLE' for flexible updates.
                            appUpdateInfo.IsUpdateTypeAllowed(AppUpdateType.Immediate)?AppUpdateType.Immediate :AppUpdateType.Flexible,
                            // The current activity making the update request.
                            Activity,
                            // Include a request code to later monitor this update request.
                            1);
                        return;
                    }
                }
            }
        }
    }
}
