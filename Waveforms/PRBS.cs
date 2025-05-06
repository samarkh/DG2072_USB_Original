using System;
using System.Windows;
using System.Windows.Controls;

namespace DG2072_USB_Control
{
    public partial class MainWindow : System.Windows.Window
    {
        private void Ch1PRBSBitRateTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch1PRBSBitRateTextBox.Text, out double bitRate))
            {
                rigolDG2072.SetPRBSBitRate(1, bitRate);
            }
            else
            {
                LogMessage("Invalid bit rate value for CH1 PRBS");
            }
        }

        private void Ch1PRBSDataTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isConnected) return;

            ComboBoxItem selectedItem = Ch1PRBSDataTypeComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string dataType = selectedItem.Content.ToString();
                rigolDG2072.SetPRBSDataType(1, dataType);
            }
        }

        private void Ch2PRBSBitRateTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch2PRBSBitRateTextBox.Text, out double bitRate))
            {
                rigolDG2072.SetPRBSBitRate(2, bitRate);
            }
            else
            {
                LogMessage("Invalid bit rate value for CH2 PRBS");
            }
        }

        private void Ch2PRBSDataTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isConnected) return;

            ComboBoxItem selectedItem = Ch2PRBSDataTypeComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string dataType = selectedItem.Content.ToString();
                rigolDG2072.SetPRBSDataType(2, dataType);
            }
        }
    }
}