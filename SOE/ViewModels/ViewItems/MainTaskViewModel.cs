﻿using AsyncAwaitBestPractices;
using Kit;
using Kit.Model;
using SOE.Enums;
using SOE.Models;
using SOE.Views.Pages;
using SOE.Views.ViewItems;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.ViewModels.ViewItems
{
    public class MainTaskViewModel : ModelBase
    {
        private int _SelectedIndex;

        public int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                if (value >= 0 && this._SelectedIndex != value && (Views?.Any() ?? false))
                {
                    _SelectedIndex = value;
                    SelectedView = Views[value];
                    Raise(() => SelectedIndex);
                    if (SelectedIndex == 1 && MainView.Instance?.Model is not null && MainView.Instance.Model.Title == "Archivadas")
                    {
                        MainView.Instance.Model.Title = "Pendientes";
                        PendingRemindersViewModel.Instance.Load(PendingStatus.Pending).SafeFireAndForget();
                        PendingTasksViewModel.Instance.Refresh(PendingStatus.Pending).SafeFireAndForget();
                    }
                }
                _SelectedIndex = value;
            }
        }

        private IconView _SelectedView;

        public IconView SelectedView
        {
            get => _SelectedView;
            set
            {
                if (_SelectedView != value && (Views?.Any() ?? false))
                {
                    _SelectedView = value;
                    SelectedIndex = Views.IndexOf(value);
                    Raise(() => SelectedView);
                }
            }
        }
        public ObservableCollection<IconView> Views { get; }

        private ICommand _SelectionChangedCommand;
        public ICommand SelectionChangedCommand
            => _SelectionChangedCommand ??= new Xamarin.Forms.Command<int>(SelectionChanged);

        private ICommand _TareasViewCommand;
        public ICommand TareasViewCommand
            => _TareasViewCommand ??= new Xamarin.Forms.Command(TareasView);
        private ICommand _RemindersViewCommand;
        public ICommand RemindersViewCommand
            => _RemindersViewCommand ??= new Xamarin.Forms.Command(this.RemindersView);
        private ICommand _AddCommand;
        public ICommand AddCommand
            => _AddCommand ??= new Xamarin.Forms.Command(Add);


        private ICommand _FlyOutCommand;
        public ICommand FlyOutCommand
            => _FlyOutCommand ??= new Command(AppShell.OpenFlyout);
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
        private void TareasView()
        {
            SelectedIndex = 0;
        }

        private void RemindersView()
        {
            SelectedIndex = 1;
        }
        private void SelectionChanged(int Index)
        {
            this.SelectedView = this.Views[Index];
        }

        public MainTaskViewModel()
        {
            PendingTasksView pt = new ();
            PendingRemindersView pr = new ();
            Views = new ObservableCollection<IconView>()
            {
                new PendingTasksView(),
                new PendingRemindersView()
            };
            Load(pt, pr);
        }

        private void Load(PendingTasksView pt, PendingRemindersView pr)
        {
            pt.Init.ContinueWith((t, o) => pr.Init, null).SafeFireAndForget();
            this.TareasView();
        }

        public void OnAppearing()
        {
            foreach (IconView iconView in Views)
            {
                iconView.OnAppearing();
            }
        }
    }
}
