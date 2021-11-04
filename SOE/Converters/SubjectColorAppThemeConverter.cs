﻿using System;
using System.Globalization;
using SOEWeb.Shared;
using Xamarin.Forms;

namespace SOE.Converters
{
    public class SubjectColorAppThemeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Subject subject && subject.Id > 0)
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
            else if (parameter is Color color)
            {
                return color;
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