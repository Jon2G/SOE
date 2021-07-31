using System;
using System.Threading.Tasks;
using Kit.Forms.Pages;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using SOE.ViewModels.Pages;
using Xamarin.Forms.Xaml;

namespace SOE.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterPopUp
    {
        public MasterPopUpViewModel Model { get; set; }
        public MasterPopUp()
        {
            this.Model = new MasterPopUpViewModel(this);
            this.BindingContext = Model;
            InitializeComponent();
        }
        public override async Task<BasePopUp> Show()
        {
            ScaleAnimation scaleAnimation = new ScaleAnimation
            {
                PositionIn = MoveAnimationOptions.Bottom,
                PositionOut = MoveAnimationOptions.Bottom,
                DurationIn = 100,
                DurationOut = 100,
                HasBackgroundAnimation = false
            };
            this.Animation = scaleAnimation;
            await PopupNavigation.Instance.PushAsync(this, true);
            return this;
        }
        private void OnClose(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
    }
}