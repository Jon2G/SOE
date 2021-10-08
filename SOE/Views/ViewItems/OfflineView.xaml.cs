using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SOE.Views.ViewItems
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OfflineView : ContentView
    {
        public static readonly BindableProperty RetryCommandProperty = BindableProperty.Create(
            nameof(RetryCommand), typeof(ICommand), typeof(OfflineView),
            null, BindingMode.OneWay, propertyChanged: (bindable, value, newValue) =>
            {
                if (bindable is OfflineView view) view.RetryCommand = (ICommand)newValue;
            });
        public ICommand RetryCommand
        {
            get => (ICommand)GetValue(RetryCommandProperty);
            set
            {
                SetValue(RetryCommandProperty, value);
                this.OnPropertyChanged();
            }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsVisible))
            {
                this.InputTransparent = !IsVisible;
            }
        }

        public OfflineView()
        {
            InitializeComponent();
        }
    }
}