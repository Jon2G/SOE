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
using System.Linq;

namespace SOE.ViewModels.Pages
{
    public class MasterPageViewModel : ModelBase
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
                    SelectionChanged(value);
                }
            }
        }

        private IconView _SelectedView;

        public IconView SelectedView
        {
            get => _SelectedView;
            set
            {
                if (_SelectedView != value)
                {
                    _SelectedView = value;
                    SelectedIndex = Views.IndexOf(value);
                    Raise(() => SelectedView);
                }
            }
        }
        public ObservableCollection<IconView> Views { get; }


        private ICommand _TareasViewCommand;
        public ICommand TareasViewCommand
            => _TareasViewCommand ??= new Xamarin.Forms.Command(TareasView);

        private ICommand _GradesViewCommand;
        public ICommand GradesViewCommand
            => _GradesViewCommand ??= new Xamarin.Forms.Command(GradesView);

        private ICommand _ClassTimeViewCommand;
        public ICommand ClassTimeViewCommand
            => _ClassTimeViewCommand ??= new Xamarin.Forms.Command(ClassTimeView);

        private void GradesView()
        {
            SelectedIndex = 0;
        }
        private void ClassTimeView()
        {
            SelectedIndex = 2;
        }
        private void TareasView()
        {
            SelectedIndex = 1;
        }


        public MasterPageViewModel()
        {
            Views = new ObservableCollection<IconView>();
            Load().SafeFireAndForget();
        }


        private void SelectionChanged(int Index)
        {
            this.SelectedView = this.Views[Index];
            ContentPage page = MasterPage.Instance;
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