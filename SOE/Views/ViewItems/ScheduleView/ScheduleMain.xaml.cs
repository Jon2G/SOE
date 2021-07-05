﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SOE.Data;
using SOE.Models.Scheduler;
using SOE.Views.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems.ScheduleView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScheduleViewMain : ContentView
    {
        public static ScheduleViewMain Instance { get; private set; }
        private bool IsDayViewVisible
        {
            get => DayView.IsVisible;
            set
            {
                DayView.IsTabStop = value;
                DayView.IsEnabled = value;
                DayView.IsVisible = value;
            }
        }

        private ICommand _OnDayTappedCommand;
        public ICommand OnDayTappedCommand => _OnDayTappedCommand ??= new Command<SheduleDay>(OnDayTapped);
        public ScheduleViewMain()
        {
            Instance = this;
            InitializeComponent();
            if (!AppData.Instance.User.HasSubjects)
            {
                this.Content = new NoInscriptionView();
                return;
            }
            this.IsDayViewVisible = false;

        }



        private void OnDayTapped(SheduleDay day)
        {
            if (day is null) { return; }
            DayView.DayModel = day;
            DayView.MainModel = this.Model;
            Fade(true);

            if (Shell.Current?.CurrentPage is MasterPage master)
            {
                master.TabView.IsSwipeEnabled = false;
            }
        }

        internal bool OnBackButtonPressed()
        {
            if (IsDayViewVisible)
            {
                Fade(false);
                if (Shell.Current.CurrentPage is MasterPage master)
                {
                    master.TabView.IsSwipeEnabled = true;
                }
                return true;
            }
            return false;
        }

        private async void Fade(bool IsDayViewVisible)
        {
            if (IsDayViewVisible)
            {
                this.IsDayViewVisible = IsDayViewVisible;
                await Task.Delay(100);
            }
            await DayView.FadeTo(IsDayViewVisible ? 1 : 0, 500u, Easing.Linear);
            this.IsDayViewVisible = IsDayViewVisible;
        }

        private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            Acr.UserDialogs.UserDialogs.Instance.Alert("Hola :)");
        }
    }
}