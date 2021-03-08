using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Animations;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Enums;
using SchoolOrganizer.Saes;
using SchoolOrganizer.Views.PopUps;

namespace SchoolOrganizer.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPage : IBrowser
    {
        public WebView Browser => BrowserHolder.Content as WebView;
        public MasterPage()
        {
            InitializeComponent();
        }

        public void SetBrowser(IBrowser browser)
        {
            if (browser != null)
            {
                this.BrowserHolder.Content = browser.Browser;
            }
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            var pr = new TaskPage();
            var scaleAnimation = new ScaleAnimation
            {
                PositionIn = MoveAnimationOptions.Right,
                PositionOut = MoveAnimationOptions.Left
            };

            pr.Animation = scaleAnimation;
            await PopupNavigation.Instance.PushAsync(pr);
        }



    }
}