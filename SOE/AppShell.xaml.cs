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
            this.MasterPage.Appearing += this.MasterPage_Appearing;
        }

        private void MasterPage_Appearing(object sender, EventArgs e)
        {
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
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
