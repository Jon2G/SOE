using SOE.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DualColorView
    {
        public static readonly BindableProperty ThemeColorProperty = BindableProperty.Create(
            propertyName: nameof(ThemeColor),
            returnType: typeof(ThemeColor),
            declaringType: typeof(DualColorView), defaultValue: null, BindingMode.TwoWay,
            propertyChanged: (e, o, n) =>
            {
                if (e is DualColorView arrow) arrow.ThemeColor = n as ThemeColor;
            });

        public ThemeColor ThemeColor
        {
            get => (ThemeColor)GetValue(ThemeColorProperty);
            set
            {
                SetValue(ThemeColorProperty, value);
                OnPropertyChanged();
            }
        }
        public DualColorView()
        {
            InitializeComponent();
        }
    }
}