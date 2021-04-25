﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Kit.Sql.Helpers;
namespace SchoolOrganizer.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
    {
        protected override Assembly[] IncludedAssemblies => null;
        protected override void Init()
        {
         //  PdfSharp.Xamarin.Forms.UWP.Platform.Init();
        }

        protected override Type MainPageType => typeof(MainPage);
        public App():base()
        {
            this.InitializeComponent();
        }
    }
}
