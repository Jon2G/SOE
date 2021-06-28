using Android.Content;
using Java.Lang;
using SchoolOrganizer.Droid.Renders;
using SchoolOrganizer.Renders;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BorderlessDatePicker), typeof(BorderlessDatePickerRenderer))]
namespace SchoolOrganizer.Droid.Renders
{
    public class BorderlessDatePickerRenderer : DatePickerRenderer
    {
        [System.Obsolete]
        public BorderlessDatePickerRenderer()
        {

        }

        public BorderlessDatePickerRenderer(Context context) : base(context)
        {

        }

        public static void Init() { }
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.DatePicker> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                Control.Background = null;

                var layoutParams = new MarginLayoutParams(Control.LayoutParameters);
                layoutParams.SetMargins(0, 0, 0, 0);
                LayoutParameters = layoutParams;
                Control.LayoutParameters = layoutParams;
                Control.SetPadding(0, 0, 0, 0);
                SetPadding(0, 0, 0, 0);
            }
        }
    }
}