﻿using Plugin.Media;
using Plugin.Media.Abstractions;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using SchoolOrganizer.Views.Pages;
using SchoolOrganizer.Views.PopUps;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace SchoolOrganizer.ViewModels.Pages
{

    public class TaskViewModel : BaseViewModel
    {
        public Command TaskCommand { get; }
       
        public TaskViewModel()
        {
            TaskCommand = new Command(TaskClicked);
        }

        private async void TaskClicked(object obj)
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
