﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using Android.Database;
using Android.Provider;
using System.Collections.Generic;
using Xamarin.Forms;
using Android.Content;
using Android.Util;
using Acr.UserDialogs;
using ImageCircle.Forms.Plugin.Droid;
using Plugin.Fingerprint;
using Rg.Plugins.Popup;
namespace OrganizadorEscolar.Droid
{
    [Activity(Label = "OrganizadorEscolar", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            CrossFingerprint.SetCurrentActivityResolver(() => this);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            ImageCircleRenderer.Init();
            UserDialogs.Init(this);
            //OrganizadorEscolar.Widgets.Horario.WidgetHorario.Init(new Widgets.Horario.WidgetHorario(this));
            //OrganizadorEscolar.Widgets.Tareas.WidgetTareas.Init(new Widgets.Tareas.WidgetTareas(this));
            Plugin.InputKit.Platforms.Droid.Config.Init(this, savedInstanceState);
            
            LoadApplication(new SchoolOrganizer.App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
     
        }
}