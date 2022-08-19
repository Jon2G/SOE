using Android.Content;
using Android.Graphics.Drawables;
using Android.Runtime;
using SOE.Droid.Renders;
using SOE.Renders;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(NoLineEntry), typeof(NoLineEntryRender))]
namespace SOE.Droid.Renders
{
    [Preserve]
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
                this.Control.SetBackground(gd);
                this.Control.SetPadding(0, 0, 0, 0);
                //this.Control.SetRawInputType(InputTypes.TextFlagNoSuggestions);
            }
        }
    }
}