using Firebase.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using PanCardView.iOS;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace SOE.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : Kit.iOS.Services.AppDelegate
    {
        protected override Xamarin.Forms.Application GetApp => new App();

        protected override void Initialize()
        {
            UINavigationBar.Appearance.Translucent = false;
            UINavigationBar.Appearance.BackgroundColor = Xamarin.Forms.Color.MidnightBlue.ToUIColor();
            UINavigationBar.Appearance.BarTintColor = Xamarin.Forms.Color.White.ToUIColor();

        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            bool result = base.FinishedLaunching(app, options);
            Firebase.Core.App.Configure();
            var instance = Firebase.Installations.Installations.DefaultInstance;
            Analytics.SetAnalyticsCollectionEnabled(true);
            CardsViewRenderer.Preserve();
            return result;
        }
    }
}
