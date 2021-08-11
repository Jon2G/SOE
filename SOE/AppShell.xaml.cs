using Kit;
using System;
using Xamarin.Forms;

namespace SOE
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        internal static void CloseFlyout()
        {
            Tools.Instance.SynchronizeInvoke.BeginInvokeOnMainThread(() =>
            {
                Shell.Current.FlyoutIsPresented = false;
            });
        }

        internal static void OpenFlyout()
        {
            Tools.Instance.SynchronizeInvoke.BeginInvokeOnMainThread(() =>
            {
                Shell.Current.FlyoutIsPresented = true;
            });
        }
    }
}
