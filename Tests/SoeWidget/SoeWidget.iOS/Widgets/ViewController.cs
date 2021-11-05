using Foundation;
using System;
using UIKit;

namespace SOE.iOS.Widgets
{
    [Register(nameof(ViewController)),Preserve()]
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        UITextView text;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Imagine this using https://docs.microsoft.com/en-us/xamarin/ios/app-fundamentals/backgrounding/ios-backgrounding-techniques/updating-an-application-in-the-background
            // instead of just on launch

            this.text = new UITextView(new CoreGraphics.CGRect(200, 200, 200, 100));
            this.text.Text = "Before";
            this.View.AddSubview(this.text);

            

            this.text.Text = "After";
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}