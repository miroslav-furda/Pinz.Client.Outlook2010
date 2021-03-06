﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Com.Pinz.Client.Commons.Wpf.Converter
{
    public class ParametrizedBooleanToVisibilityConverter : IValueConverter
    {
        enum Parameters
        {
            Normal, Inverted
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool)value;
            Parameters direction;
            if (parameter == null)
            {
                direction = Parameters.Normal;
            }
            else
            {
                direction = (Parameters)Enum.Parse(typeof(Parameters), (string)parameter);
            }

            if (direction == Parameters.Inverted)
                return !boolValue ? Visibility.Visible : Visibility.Collapsed;

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}