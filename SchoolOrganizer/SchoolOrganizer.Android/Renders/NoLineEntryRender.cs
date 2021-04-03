using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Text;
using SchoolOrganizer.Droid.Renders;
using SchoolOrganizer.Renders;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(NoLineEntry), typeof(NoLineEntryRender))]
namespace SchoolOrganizer.Droid.Renders
{
    public class NoLineEntryRender : EntryRenderer
    {
        public NoLineEntryRender(Context Context) : base(Context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetColor(global::Android.Graphics.Color.Transparent);
                this.Control.SetBackgroundDrawable(gd);
                this.Control.SetRawInputType(InputTypes.TextFlagNoSuggestions);
                Control.SetHintTextColor(ColorStateList.ValueOf(global::Android.Graphics.Color.Black));
            }
        }
    }
}