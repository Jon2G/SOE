using AsyncAwaitBestPractices;
using Kit.Forms.Pages;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using SOE.Models;
using SOE.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReminderPopUp 
    {
        public ReminderPopUpViewModel Model { get; set; }
        public ReminderPopUp(Reminder reminder)
        {
            this.Model = new ReminderPopUpViewModel(this, reminder);
            this.BindingContext = Model;
            InitializeComponent();
        }
        public static void ShowPopUp(Reminder reminder)
        {
            ReminderPopUp popUp = new ReminderPopUp(reminder);
            popUp.Show().SafeFireAndForget();
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
        private void Button_Clicked(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
    }
}