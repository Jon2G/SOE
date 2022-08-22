using AsyncAwaitBestPractices;
using Kit.Forms;
using Kit.Model;
using SOE.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace SOE.ViewModels.Pages
{
    public class ColorPickerPageViewModel : ModelBase
    {
        private ThemeColor _color;
        public ThemeColor? Color { get => this._color; set => RaiseIfChanged(ref this._color, value); }
        public ObservableCollection<ThemeColor> Colors { get; set; }
        public ICommand SelectColorCommand { get; set; }
        public ColorPickerPageViewModel()
        {
            SelectColorCommand = new AsyncFeedbackCommand<ThemeColor>(SelectColor);
            this.Colors = new ObservableCollection<ThemeColor>(ThemeColor.GetAll());
        }

        private void SelectColor(ThemeColor? color)
        {
            Color = color;
            Shell.Current.Navigation.PopAsync().SafeFireAndForget();
        }
    }
}
