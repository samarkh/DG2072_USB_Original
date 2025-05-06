using System;
using System.Windows;
using System.Windows.Controls;

namespace DG2072_USB_Control
{
    public partial class MainWindow : System.Windows.Window
    {
        private void Ch1DualToneFreq1TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch1DualToneFreq1TextBox.Text, out double frequency))
            {
                rigolDG2072.SetDualToneFrequency1(1, frequency);
            }
            else
            {
                LogMessage("Invalid frequency 1 value for CH1 dual-tone");
            }
        }

        private void Ch1DualToneFreq2TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch1DualToneFreq2TextBox.Text, out double frequency))
            {
                rigolDG2072.SetDualToneFrequency2(1, frequency);
            }
            else
            {
                LogMessage("Invalid frequency 2 value for CH1 dual-tone");
            }
        }

        private void Ch1DualToneCenterFreqTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch1DualToneCenterFreqTextBox.Text, out double frequency))
            {
                rigolDG2072.SetDualToneCenterFrequency(1, frequency);
            }
            else
            {
                LogMessage("Invalid center frequency value for CH1 dual-tone");
            }
        }

        private void Ch1DualToneOffsetFreqTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch1DualToneOffsetFreqTextBox.Text, out double frequency))
            {
                rigolDG2072.SetDualToneOffsetFrequency(1, frequency);
            }
            else
            {
                LogMessage("Invalid offset frequency value for CH1 dual-tone");
            }
        }

        private void Ch2DualToneFreq1TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch2DualToneFreq1TextBox.Text, out double frequency))
            {
                rigolDG2072.SetDualToneFrequency1(2, frequency);
            }
            else
            {
                LogMessage("Invalid frequency 1 value for CH2 dual-tone");
            }
        }

        private void Ch2DualToneFreq2TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch2DualToneFreq2TextBox.Text, out double frequency))
            {
                rigolDG2072.SetDualToneFrequency2(2, frequency);
            }
            else
            {
                LogMessage("Invalid frequency 2 value for CH2 dual-tone");
            }
        }

        private void Ch2DualToneCenterFreqTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch2DualToneCenterFreqTextBox.Text, out double frequency))
            {
                rigolDG2072.SetDualToneCenterFrequency(2, frequency);
            }
            else
            {
                LogMessage("Invalid center frequency value for CH2 dual-tone");
            }
        }

        private void Ch2DualToneOffsetFreqTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch2DualToneOffsetFreqTextBox.Text, out double frequency))
            {
                rigolDG2072.SetDualToneOffsetFrequency(2, frequency);
            }
            else
            {
                LogMessage("Invalid offset frequency value for CH2 dual-tone");
            }
        }
    }
}