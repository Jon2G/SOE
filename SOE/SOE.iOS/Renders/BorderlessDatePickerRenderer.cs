using Foundation;
using OrganizadorEscolar.iOS.Renders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using SchoolOrganizer.Renders;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BorderlessDatePicker), typeof(BorderlessDatePickerRenderer))]
namespace OrganizadorEscolar.iOS.Renders
{
    class BorderlessDatePickerRenderer : DatePickerRenderer
    {
        public static void Init() { }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            Control.Layer.BorderWidth = 0;
            Control.BorderStyle = UITextBorderStyle.None;
        }
    }
}