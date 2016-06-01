using System.Windows;
using System.Windows.Controls;

namespace PinzOutlookAddIn
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : UserControl
    {
        public Shell()
        {
            InitializeComponent();
        }

        private void PinzRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (PinzRadioButton.IsChecked == true)
            {
                OutlookMainRegion.Visibility = Visibility.Hidden;
                PinzMainRegion.Visibility = Visibility.Visible;
            }
        }

        private void OutlookRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (OutlookRadioButton.IsChecked == true)
            {
                PinzMainRegion.Visibility = Visibility.Hidden;
                OutlookMainRegion.Visibility = Visibility.Visible;
            }
        }
    }
}
