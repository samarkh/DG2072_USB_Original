using System;
using System.Windows;
using System.Windows.Controls;

namespace DG2072_USB_Control
{
    public partial class MainWindow : System.Windows.Window
    {
        private void Ch1RampSymmetryTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch1RampSymmetryTextBox.Text, out double symmetry))
            {
                rigolDG2072.SetRampSymmetry(1, symmetry);
            }
            else
            {
                LogMessage("Invalid symmetry value for CH1");
            }
        }

        private void Ch2RampSymmetryTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch2RampSymmetryTextBox.Text, out double symmetry))
            {
                rigolDG2072.SetRampSymmetry(2, symmetry);
            }
            else
            {
                LogMessage("Invalid symmetry value for CH2");
            }
        }
    }
}