using System;
using Kit.Forms.Pages;
using SchoolOrganizer.Saes;
using SchoolOrganizer.Views.Pages;
using Xamarin.Forms;

namespace SchoolOrganizer
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell(IBrowser Browser=null)
        {
            InitializeComponent();
            this.MasterPage.SetBrowser(Browser);
        }


    }
}
