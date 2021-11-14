using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SOE.Droid.Renders;
using SOE.Renders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.PancakeView.Droid;

[assembly: ExportRenderer(typeof(CustomPancakeView), typeof(CustomPancakeViewRenderer))]
namespace SOE.Droid.Renders
{
    [Preserve]
    public class CustomPancakeViewRenderer : PancakeViewRenderer
    {
        public CustomPancakeViewRenderer() : base(((Kit.Droid.ToolsImplementation)Kit.Droid.Tools.Instance).MainActivity)
        {

        }
        public CustomPancakeViewRenderer(Context context) : base(context)
        {
        }

        [Obsolete]
        public CustomPancakeViewRenderer(IntPtr handle, JniHandleOwnership transfer) : base(Xamarin.Forms.Forms.Context)
        {
        }
    }

}