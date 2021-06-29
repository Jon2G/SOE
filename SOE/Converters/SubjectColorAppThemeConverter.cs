using System;
using System.ComponentModel;
using System.Globalization;
using APIModels;
using Kit.Model;
using Xamarin.Forms;

namespace SOE.Converters
{
    public class SubjectColorAppThemeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Subject subject)
            {
                if (App.Current.RequestedTheme == OSAppTheme.Dark)
                {
                    return subject.ColorDark;
                }
                else
                {
                    return subject.Color;
                }
            }

            if (App.Current.RequestedTheme == OSAppTheme.Dark)
            {
                return App.Current.Resources["BackgroundSecondaryDarkColor"];
            }
            else
            {
                return App.Current.Resources["BackgroundSecondaryLightColor"];
            }
               
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}