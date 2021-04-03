﻿using Rg.Plugins.Popup.Services;
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
using System.Threading;
using System.Windows.Input;
using SchoolOrganizer.Data;
using SchoolOrganizer.Fonts;
using SchoolOrganizer.ViewModels;
using SchoolOrganizer.ViewModels.ViewItems;
using SchoolOrganizer.Views.ViewItems;
using SchoolOrganizer.Views.ViewItems.ScheduleView;
using Xamarin.CommunityToolkit.UI.Views;

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

        }
        protected override bool OnBackButtonPressed()
        {
            if (TabView.SelectedIndex < 0)
            {
                TabView.SelectedIndex = 0;
            }
            TabViewItem selectedView = this.TabView.TabItems[TabView.SelectedIndex];
            switch (selectedView.Content)
            {
                case ScheduleViewMain schedule:
                    if (schedule.OnBackButtonPressed())
                    {
                        return true;
                    }
                    break;
            }
            return base.OnBackButtonPressed();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            InitBrowser();
            TabView.SelectedIndex = 1;
        }

        private void TabView_OnSelectionChanged(object? sender, TabSelectionChangedEventArgs e)
        {
            if (TabView.SelectedIndex == 0)
            {
                var item = new ToolbarItem
                {
                    Command = (TabView.TabItems[0].Content as SchoolGrades)?.Model.RefreshCommand,
                    CommandParameter = this,
                    IconImageSource = new FontImageSource()
                    {
                        FontFamily = FontelloIcons.Font,
                        Glyph = FontelloIcons.Home
                    }
                };
                this.ToolbarItems.Add(item);
            }
            else
            {
                this.ToolbarItems.Clear();
            }
        }

        private void InitBrowser()
        {
            if (this.Browser is null)
            {
                this.BrowserHolder.Content = new WebView();
            }
        }
    }
}