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
    public partial class MasterPage
    {
        public MasterPage()
        {
            InitializeComponent();
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
            this.SchoolGrades.OnAppearing();
            if (TabView.SelectedIndex <= 0)
                TabView.SelectedIndex = 1;
        }

        private void TabView_OnSelectionChanged(object? sender, TabSelectionChangedEventArgs e)
        {
            switch (TabView.SelectedIndex)
            {
                case 0:
                    this.ToolbarItems.Clear();
                    this.ToolbarItems.Add(new ToolbarItem
                    {
                        Command = SchoolGrades.Model.RefreshCommand,
                        CommandParameter = this,
                        IconImageSource = new FontImageSource()
                        {
                            FontFamily = FontelloIcons.Font,
                            Glyph = FontelloIcons.Refresh
                        }
                    });
                    break;
                case 2:
                    this.ToolbarItems.Clear();
                    this.ToolbarItems.Add(new ToolbarItem
                    {
                        Command = ScheduleViewMain.Model.ExportToPdfCommand,
                        CommandParameter = ScheduleViewMain,
                        IconImageSource = new FontImageSource()
                        {
                            FontFamily = FontelloIcons.Font,
                            Glyph = FontelloIcons.PDF
                        }
                    });
                    break;
                default:
                    this.ToolbarItems.Clear();
                    break;
            }
        }

        private void InitBrowser()
        {
            if (AppData.Instance.SAES is null)
            {
                AppData.Instance.SAES = new SAES();

            }
            this.BrowserHolder.Content = AppData.Instance.SAES;
        }
    }
}