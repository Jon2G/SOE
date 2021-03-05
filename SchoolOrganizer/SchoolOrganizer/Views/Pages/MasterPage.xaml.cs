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
using System.Threading;

namespace SchoolOrganizer.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPage 
    {
        private bool _isSocialOpened;
        private CancellationTokenSource _tokenSource;
        public string Twitter => "https://twitter.com/Andrik_Just4Fun";
        public string LinkedIn => "https://www.linkedin.com/in/andrei-misiukevich-416589aa/";
        public string Instagram => "https://www.instagram.com/andrik_just4fun/";
        public MasterPage()
        {
            InitializeComponent();
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