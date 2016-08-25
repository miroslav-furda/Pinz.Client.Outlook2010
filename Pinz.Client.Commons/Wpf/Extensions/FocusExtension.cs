using Common.Logging;
using System;
using System.Windows;

namespace Com.Pinz.Client.Commons.Wpf.Extensions
{
    public static class FocusExtension
    {
        private static readonly ILog Log = LogManager.GetLogger("FocusExtension");

        public static bool GetIsFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }


        public static void SetIsFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }


        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(FocusExtension), new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));


        private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uie = (UIElement)d;
            if ((bool)e.NewValue)
            {
                try
                {
                    uie.Focus(); // Don't care about false values.
                }catch(Exception exc)
                {
                    Log.Error(exc);
                }
            }
        }
    }
}
