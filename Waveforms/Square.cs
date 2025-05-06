using System;
using System.Windows;
using System.Windows.Controls;

namespace DG2072_USB_Control
{
    public partial class MainWindow : System.Windows.Window
    {
        private void Ch1SquareDutyCycleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch1SquareDutyCycleTextBox.Text, out double dutyCycle))
            {
                rigolDG2072.SetSquareDutyCycle(1, dutyCycle);
            }
            else
            {
                LogMessage("Invalid duty cycle value for CH1");
            }
        }

        private void Ch2SquareDutyCycleTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch2SquareDutyCycleTextBox.Text, out double dutyCycle))
            {
                rigolDG2072.SetSquareDutyCycle(2, dutyCycle);
            }
            else
            {
                LogMessage("Invalid duty cycle value for CH2");
            }
        }
    }
}