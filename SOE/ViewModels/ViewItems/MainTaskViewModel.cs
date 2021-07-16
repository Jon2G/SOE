using AsyncAwaitBestPractices;
using Kit.Model;
using SOE.Models;
using SOE.Views.Pages;
using SOE.Views.ViewItems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace SOE.ViewModels.ViewItems
{
    public class MainTaskViewModel : ModelBase
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

        private ICommand _TareasViewCommand;
        public ICommand TareasViewCommand
            => _TareasViewCommand ??= new Xamarin.Forms.Command<TabView>(TareasView);
        private ICommand _RemindersViewCommand;
        public ICommand RemindersViewCommand
            => _RemindersViewCommand ??= new Xamarin.Forms.Command<TabView>(RemindersView);
        private ICommand _AddCommand;
        public ICommand AddCommand
            => _AddCommand ??= new Xamarin.Forms.Command(Add);


        private void Add()
        {
            switch (SelectedView)
            {
                case PendingTasksView:
                    App.Current.MainPage.Navigation.PushAsync(new NewTaskPage(), true).SafeFireAndForget();
                    break;
                case PendingRemindersView:
                    ReminderPage pr = new ReminderPage();
                    pr.ShowDialog().SafeFireAndForget();
                    break;
            }
        }
        private void TareasView(TabView TabView)
        {
            TabView.SelectedIndex = 0;
            this.SelectedView = this.Views[0];
        }

        private void RemindersView(TabView TabView)
        {
            TabView.SelectedIndex = 1;
            this.SelectedView = this.Views[1];
        }
        private void SelectionChanged(int Index)
        {
            this.SelectedView = this.Views[Index];
        }

        public MainTaskViewModel()
        {
            Views = new ObservableCollection<IconView>();
            Load().SafeFireAndForget();
        }

        private async Task Load()
        {
            await Task.Yield();
            SelectedView = new PendingTasksView();
            Views.Add(SelectedView);
            Views.Add(new PendingRemindersView());
        }
    }
}
