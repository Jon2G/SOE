﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace OrganizadorEscolar.Droid.Widgets.Horario
{
    /// <summary>
    /// The main purpose of RemoteViewsService is to return a RemoteViewsFactory object 
    /// which further handles the task of filling the widget with appropriate data.
    /// </summary>
    [Service(Permission = "android.permission.BIND_REMOTEVIEWS", Exported = false)]
    public class ListWidgetService : RemoteViewsService
    {
        public override IRemoteViewsFactory OnGetViewFactory(Intent intent)
        {
            return new ListRemoteViewsFactory(ApplicationContext, intent);
        }
    }
}