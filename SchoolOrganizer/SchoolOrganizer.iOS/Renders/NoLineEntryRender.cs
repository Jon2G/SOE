using Foundation;
using OrganizadorEscolar.iOS.Renders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Entry), typeof(NoLineEntryRender))]

namespace OrganizadorEscolar.iOS.Renders
{
    class NoLineEntryRender: EntryRenderer
    {
    protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
    {
        base.OnElementChanged(e);

        if (Control != null)
        {

            Control.BorderStyle = UITextBorderStyle.None;
            Control.Layer.CornerRadius = 10;
            Control.TextColor = UIColor.White;

        }
     }
    }
}