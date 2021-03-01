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

namespace SchoolOrganizer.Views.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPage 
    {

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