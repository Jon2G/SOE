using System.Collections.ObjectModel;
using System.Windows.Input;
using Kit.Model;
using Xamarin.Forms;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using DocumentFormat.OpenXml.Packaging;
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

        private ObservableCollection<IconView> _Views;
        public ObservableCollection<IconView> Views => _Views ??= new ObservableCollection<IconView>()
        {
            new SchoolGrades(),
            new MainView(),
            new ScheduleViewMain()
        };


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
           
        }


        private void SelectionChanged(int Index)
        {
            this.SelectedView = this.Views[Index];
        }

        private bool Showed = false;
        public async Task OnAppearing()
        {
            await Task.Yield();
            if (this.Showed)
            {
                return;
            }
            this.Showed = true;
            foreach (IconView view in this.Views)
            {
                view.OnAppearing();
            }
        }
    }
}