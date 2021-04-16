using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using SchoolOrganizer;
using Sharpnado.MaterialFrame.iOS;
using UIKit;

namespace OrganizadorEscolar.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate :Kit.iOS.Services.AppDelegate //global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        protected override Xamarin.Forms.Application GetApp => new App();

        protected override void Initialize()
        {
            iOSMaterialFrameRenderer.Init();
            Plugin.InputKit.Platforms.iOS.Config.Init();
        }
    }
}
