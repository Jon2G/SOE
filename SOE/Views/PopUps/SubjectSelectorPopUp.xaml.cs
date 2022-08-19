using AsyncAwaitBestPractices;
using Kit.Forms.Pages;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using SOE.ViewModels.PopUps;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace SOE.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubjectPopUp
    {
        public SubjectSelectorPopUpViewModel Modelo { get; set; }
        public SubjectPopUp()
        {
            this.Modelo = new SubjectSelectorPopUpViewModel(this);
            InitializeComponent();
            this.BindingContext = this.Modelo;
            App.Current.RequestedThemeChanged += Current_RequestedThemeChanged;
        }

        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();
        }

        public virtual async Task<BasePopUp> Show()
        {
            ScaleAnimation scaleAnimation = new ScaleAnimation
            {
                PositionIn = MoveAnimationOptions.Bottom,
                PositionOut = MoveAnimationOptions.Top
            };
            this.Animation = scaleAnimation;
            await PopupNavigation.Instance.PushAsync(this, true);
            return this;
        }
        private void Current_RequestedThemeChanged(object sender, Xamarin.Forms.AppThemeChangedEventArgs e)
        {
            this.Modelo.Refresh().SafeFireAndForget();
        }
    }
}