using System;
using System.Windows;
using System.Windows.Controls;

namespace DG2072_USB_Control
{
    public partial class MainWindow : System.Windows.Window
    {
        private void Ch1RS232BaudRateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isConnected) return;

            ComboBoxItem selectedItem = Ch1RS232BaudRateComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null && int.TryParse(selectedItem.Content.ToString(), out int baudRate))
            {
                rigolDG2072.SetRS232BaudRate(1, baudRate);
            }
        }

        private void Ch1RS232DataBitsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isConnected) return;

            ComboBoxItem selectedItem = Ch1RS232DataBitsComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null && int.TryParse(selectedItem.Content.ToString(), out int dataBits))
            {
                rigolDG2072.SetRS232DataBits(1, dataBits);
            }
        }

        private void Ch1RS232StopBitsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isConnected) return;

            ComboBoxItem selectedItem = Ch1RS232StopBitsComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null && double.TryParse(selectedItem.Content.ToString(), out double stopBits))
            {
                rigolDG2072.SetRS232StopBits(1, stopBits);
            }
        }

        private void Ch1RS232ParityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isConnected) return;

            ComboBoxItem selectedItem = Ch1RS232ParityComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string parity = selectedItem.Content.ToString();
                rigolDG2072.SetRS232CheckBit(1, parity);
            }
        }

        private void Ch1RS232DataTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (int.TryParse(Ch1RS232DataTextBox.Text, out int data) && data >= 0 && data <= 255)
            {
                rigolDG2072.SetRS232Data(1, data);
            }
            else
            {
                LogMessage("Invalid data value for CH1 RS232 (must be 0-255)");
            }
        }

        private void Ch2RS232BaudRateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isConnected) return;

            ComboBoxItem selectedItem = Ch2RS232BaudRateComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null && int.TryParse(selectedItem.Content.ToString(), out int baudRate))
            {
                rigolDG2072.SetRS232BaudRate(2, baudRate);
            }
        }

        private void Ch2RS232DataBitsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isConnected) return;

            ComboBoxItem selectedItem = Ch2RS232DataBitsComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null && int.TryParse(selectedItem.Content.ToString(), out int dataBits))
            {
                rigolDG2072.SetRS232DataBits(2, dataBits);
            }
        }

        private void Ch2RS232StopBitsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isConnected) return;

            ComboBoxItem selectedItem = Ch2RS232StopBitsComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null && double.TryParse(selectedItem.Content.ToString(), out double stopBits))
            {
                rigolDG2072.SetRS232StopBits(2, stopBits);
            }
        }

        private void Ch2RS232ParityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isConnected) return;

            ComboBoxItem selectedItem = Ch2RS232ParityComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string parity = selectedItem.Content.ToString();
                rigolDG2072.SetRS232CheckBit(2, parity);
            }
        }

        private void Ch2RS232DataTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (int.TryParse(Ch2RS232DataTextBox.Text, out int data) && data >= 0 && data <= 255)
            {
                rigolDG2072.SetRS232Data(2, data);
            }
            else
            {
                LogMessage("Invalid data value for CH2 RS232 (must be 0-255)");
            }
        }
    }
}