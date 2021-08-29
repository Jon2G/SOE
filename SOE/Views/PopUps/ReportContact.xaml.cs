using AsyncAwaitBestPractices;
using Kit.Forms.Pages;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using SOE.Models;
using SOE.ViewModels.PopUps;
using SOE.ViewModels.ViewItems;
using SOEWeb.Shared;
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
    public partial class ReportContact
    {
        public ReportContactViewModel Model { get; }
        public ReportContact(SchoolContact contact)
        {
            Model = new ReportContactViewModel(contact, this);
            this.BindingContext = this.Model;
            InitializeComponent();
        }
        public static void ShowPopUp(SchoolContact contact)
        {
            ReportContact popUp = new ReportContact(contact);
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
        private void OnClose(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
    }
}