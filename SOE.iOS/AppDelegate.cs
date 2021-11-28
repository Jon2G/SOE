using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Kit.Forms.Services.Interfaces;
using PanCardView.iOS;
using SOE.iOS.Widgets;
using SOE.Widgets;
using UIKit;
using UserNotifications;
using Xamarin.Forms.Platform.iOS;

[assembly: Preserve(typeof(Firebase.Core.App))]
[assembly: Preserve(typeof(SOE.App))]
[assembly: Preserve(typeof(Kit.iOS.Tools))]
namespace SOE.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : Kit.iOS.Services.AppDelegate<App>
    {
        protected override void Initialize()
        {
            UINavigationBar.Appearance.Translucent = false;
            UINavigationBar.Appearance.BackgroundColor = Xamarin.Forms.Color.MidnightBlue.ToUIColor();
            UINavigationBar.Appearance.BarTintColor = Xamarin.Forms.Color.White.ToUIColor();
            base.Initialize();
        }

        public override void BeforeLoadApplication(UIApplication app, NSDictionary options)
        {
            base.BeforeLoadApplication(app, options);
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // Request Permissions
                UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound | UNAuthorizationOptions.TimeSensitive | UNAuthorizationOptions.CriticalAlert, (granted, error) =>
                {
                    // Do something if needed
                });
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                UIUserNotificationSettings notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null
                );
                app.RegisterUserNotificationSettings(notificationSettings);
            }

            //Get current notification settings.
            UNUserNotificationCenter.Current.GetNotificationSettings((settings) => {
                var alertsAllowed = (settings.AlertSetting == UNNotificationSetting.Enabled);
            });
            UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();
            //----------------------
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            bool result = base.FinishedLaunching(app, options);
            Firebase.Core.App.Configure();
            CardsViewRenderer.Preserve();
            // check for a notification
            if (options != null)
            {
                // check for a local notification
                if (options.ContainsKey(UIApplication.LaunchOptionsLocalNotificationKey))
                {
                    var localNotification = options[UIApplication.LaunchOptionsLocalNotificationKey] as UILocalNotification;
                    if (localNotification != null)
                    {
                        UIAlertController okayAlertController = UIAlertController.Create(localNotification.AlertAction, localNotification.AlertBody, UIAlertControllerStyle.Alert);
                        okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                        Window.RootViewController.PresentViewController(okayAlertController, true, null);

                        // reset our badge
                        UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
                    }
                }
            }
            return result;
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            base.ReceivedLocalNotification(application, notification);
            // show an alert
            //UIAlertController okayAlertController = UIAlertController.Create(notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.Alert);
            //okayAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

            //UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(okayAlertController, true, null);

            // reset our badge
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }

        public override void UpdateWidget(string AppWidgetProviderClassName)
        {
            DataGenerator generator=null;
            switch (AppWidgetProviderClassName)
            {
                case TimeLineWidget.AppWidgetProviderFullClass:
                 generator=new TimeLineDataGenerator();

                    break;
                case ToDosWidget.AppWidgetProviderFullClass:
                    generator = new TodoDataGenerator();
                    break;
            }
            generator?.GenerateAndRefresh();
        }
    }
}
