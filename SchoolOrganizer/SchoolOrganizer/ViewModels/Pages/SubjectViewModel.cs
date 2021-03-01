using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using SchoolOrganizer.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.Pages
{
    class SubjectViewModel: BaseViewModel
    {
        public Command SubjectCommand { get; }
        public SubjectViewModel()
        {
            SubjectCommand = new Command(SubjectClicked);
        }

        private async void SubjectClicked(object obj)
        {
            var pr = new SubjectPopUp();
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
