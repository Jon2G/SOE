using System.Collections.ObjectModel;
using System.Windows.Input;
using Kit.Model;
using Xamarin.Forms;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using SOE.Models;
using SOE.Views.Pages;
using SOE.Views.ViewItems;
using SOE.Views.ViewItems.ScheduleView;

namespace SOE.ViewModels.Pages
{
    public class MasterPageViewModel : ModelBase
    {
        private IconView _SelectedView;

        public IconView SelectedView
        {
            get => _SelectedView;
            set
            {
                _SelectedView = value;
                Raise(() => SelectedView);
            }
        }
        public ObservableCollection<IconView> Views { get; }

        private ICommand _SelectionChangedCommand;
        public ICommand SelectionChangedCommand
            => _SelectionChangedCommand ??= new Xamarin.Forms.Command<int>(SelectionChanged);



        public MasterPageViewModel()
        {
            Views = new ObservableCollection<IconView>();
            Load().SafeFireAndForget();
        }

        private void SelectionChanged(int Index)
        {
            this.SelectedView = this.Views[Index];
            ContentPage page = MasterPage.Instance;
            Shell.SetNavBarIsVisible(page,Index!=1);
            lock (page.ToolbarItems)
            {
                page.ToolbarItems.Clear();
                if (SelectedView.ToolbarItem is null)
                {
                    return;
                }
                page.ToolbarItems.Add(SelectedView.ToolbarItem);
            }
        }
        private async Task Load()
        {
            await Task.Yield();
            Views.Add(new SchoolGrades());
            Views.Add(new MainView());
            Views.Add(new ScheduleViewMain());
            //Views.Add(new NotificationView());
        }
    }
}