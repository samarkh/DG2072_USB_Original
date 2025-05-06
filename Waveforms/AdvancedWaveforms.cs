using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DG2072_USB_Control
{
    public partial class MainWindow : System.Windows.Window
    {
        #region Advanced Waveform Event Handlers

        private void Ch1AdvancedWaveformComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isConnected || Ch1AdvancedWaveformComboBox.SelectedItem == null) return;

            ComboBoxItem selectedItem = Ch1AdvancedWaveformComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string waveform = selectedItem.Content.ToString();
            
            // Show appropriate parameter controls for the selected waveform
            ShowAdvancedWaveformSpecificControls(1, waveform);
            
            // Apply waveform with current parameters
            try
            {
                Dictionary<string, object> parameters = GetAdvancedChannelParameters(1);
                rigolDG2072.ApplyWaveformWithSpecificParams(1, waveform, parameters);
                LogMessage($"Applied {waveform} to CH1");
            }
            catch (Exception ex)
            {
                LogMessage($"Error applying advanced waveform: {ex.Message}");
            }
        }

        private void Ch1AdvApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            ComboBoxItem selectedItem = Ch1AdvancedWaveformComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string waveform = selectedItem.Content.ToString();
            Dictionary<string, object> parameters = GetAdvancedChannelParameters(1);

            try
            {
                rigolDG2072.ApplyWaveformWithSpecificParams(1, waveform, parameters);
                LogMessage($"Applied {waveform} settings to CH1");
            }
            catch (Exception ex)
            {
                LogMessage($"Error applying CH1 advanced settings: {ex.Message}");
            }
        }

        private void Ch2AdvApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            ComboBoxItem selectedItem = Ch2AdvancedWaveformComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string waveform = selectedItem.Content.ToString();
            Dictionary<string, object> parameters = GetAdvancedChannelParameters(2);

            try
            {
                rigolDG2072.ApplyWaveformWithSpecificParams(2, waveform, parameters);
                LogMessage($"Applied {waveform} settings to CH2");
            }
            catch (Exception ex)
            {
                LogMessage($"Error applying CH2 advanced settings: {ex.Message}");
            }
        }



        private void Ch2AdvancedWaveformComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isConnected || Ch2AdvancedWaveformComboBox.SelectedItem == null) return;

            ComboBoxItem selectedItem = Ch2AdvancedWaveformComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string waveform = selectedItem.Content.ToString();
            
            // Show appropriate parameter controls for the selected waveform
            ShowAdvancedWaveformSpecificControls(2, waveform);
            
            // Apply waveform with current parameters
            try
            {
                Dictionary<string, object> parameters = GetAdvancedChannelParameters(2);
                rigolDG2072.ApplyWaveformWithSpecificParams(2, waveform, parameters);
                LogMessage($"Applied {waveform} to CH2");
            }
            catch (Exception ex)
            {
                LogMessage($"Error applying advanced waveform: {ex.Message}");
            }
        }

        #endregion

        #region Advanced Waveform Helpers

        private void ShowAdvancedWaveformSpecificControls(int channel, string waveform)
        {
            // Hide all advanced waveform-specific control groups first
            if (channel == 1)
            {
                Ch1DualToneParamsGroup.Visibility = Visibility.Collapsed;
                Ch1PRBSParamsGroup.Visibility = Visibility.Collapsed;
                Ch1RS232ParamsGroup.Visibility = Visibility.Collapsed;
            }
            else
            {
                Ch2DualToneParamsGroup.Visibility = Visibility.Collapsed;
                Ch2PRBSParamsGroup.Visibility = Visibility.Collapsed;
                Ch2RS232ParamsGroup.Visibility = Visibility.Collapsed;
            }

            // Show the appropriate parameter group based on advanced waveform type
            switch (waveform.ToUpper())
            {
                case "DUAL-TONE":
                    if (channel == 1)
                        Ch1DualToneParamsGroup.Visibility = Visibility.Visible;
                    else
                        Ch2DualToneParamsGroup.Visibility = Visibility.Visible;
                    break;
                case "PRBS":
                    if (channel == 1)
                        Ch1PRBSParamsGroup.Visibility = Visibility.Visible;
                    else
                        Ch2PRBSParamsGroup.Visibility = Visibility.Visible;
                    break;
                case "RS232":
                    if (channel == 1)
                        Ch1RS232ParamsGroup.Visibility = Visibility.Visible;
                    else
                        Ch2RS232ParamsGroup.Visibility = Visibility.Visible;
                    break;
                case "USER":
                    // User waveform might need specific controls in the future
                    break;
            }
        }

        private Dictionary<string, object> GetAdvancedChannelParameters(int channel)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            // Get the selected waveform
            ComboBox waveformComboBox = channel == 1 ? Ch1AdvancedWaveformComboBox : Ch2AdvancedWaveformComboBox;
            string waveform = ((ComboBoxItem)waveformComboBox.SelectedItem).Content.ToString();

            // Add common parameters
            if (channel == 1)
            {
                if (double.TryParse(Ch1AdvAmplitudeTextBox.Text, out double amplitude))
                    parameters["Amplitude"] = amplitude;

                if (double.TryParse(Ch1AdvOffsetTextBox.Text, out double offset))
                    parameters["Offset"] = offset;
            }
            else
            {
                if (double.TryParse(Ch2AdvAmplitudeTextBox.Text, out double amplitude))
                    parameters["Amplitude"] = amplitude;

                if (double.TryParse(Ch2AdvOffsetTextBox.Text, out double offset))
                    parameters["Offset"] = offset;
            }

            // Add waveform-specific parameters based on the selected waveform
            switch (waveform.ToUpper())
            {
                case "DUAL-TONE":
                    if (channel == 1)
                    {
                        if (double.TryParse(Ch1DualToneFreq1TextBox.Text, out double freq1))
                            parameters["Frequency1"] = freq1;

                        if (double.TryParse(Ch1DualToneFreq2TextBox.Text, out double freq2))
                            parameters["Frequency2"] = freq2;

                        if (double.TryParse(Ch1DualToneCenterFreqTextBox.Text, out double centerFreq))
                            parameters["CenterFrequency"] = centerFreq;

                        if (double.TryParse(Ch1DualToneOffsetFreqTextBox.Text, out double offsetFreq))
                            parameters["OffsetFrequency"] = offsetFreq;
                    }
                    else
                    {
                        if (double.TryParse(Ch2DualToneFreq1TextBox.Text, out double freq1))
                            parameters["Frequency1"] = freq1;

                        if (double.TryParse(Ch2DualToneFreq2TextBox.Text, out double freq2))
                            parameters["Frequency2"] = freq2;

                        if (double.TryParse(Ch2DualToneCenterFreqTextBox.Text, out double centerFreq))
                            parameters["CenterFrequency"] = centerFreq;

                        if (double.TryParse(Ch2DualToneOffsetFreqTextBox.Text, out double offsetFreq))
                            parameters["OffsetFrequency"] = offsetFreq;
                    }
                    break;

                case "PRBS":
                    if (channel == 1)
                    {
                        if (double.TryParse(Ch1PRBSBitRateTextBox.Text, out double bitRate))
                            parameters["BitRate"] = bitRate;

                        ComboBoxItem dataTypeItem = Ch1PRBSDataTypeComboBox.SelectedItem as ComboBoxItem;
                        if (dataTypeItem != null)
                            parameters["DataType"] = dataTypeItem.Content.ToString();
                    }
                    else
                    {
                        if (double.TryParse(Ch2PRBSBitRateTextBox.Text, out double bitRate))
                            parameters["BitRate"] = bitRate;

                        ComboBoxItem dataTypeItem = Ch2PRBSDataTypeComboBox.SelectedItem as ComboBoxItem;
                        if (dataTypeItem != null)
                            parameters["DataType"] = dataTypeItem.Content.ToString();
                    }
                    break;

                case "RS232":
                    if (channel == 1)
                    {
                        ComboBoxItem baudRateItem1 = Ch1RS232BaudRateComboBox.SelectedItem as ComboBoxItem;
                        if (baudRateItem1 != null && int.TryParse(baudRateItem1.Content.ToString(), out int baudRate1))
                            parameters["BaudRate"] = baudRate1;

                        ComboBoxItem dataBitsItem1 = Ch1RS232DataBitsComboBox.SelectedItem as ComboBoxItem;
                        if (dataBitsItem1 != null && int.TryParse(dataBitsItem1.Content.ToString(), out int dataBits1))
                            parameters["DataBits"] = dataBits1;

                        ComboBoxItem stopBitsItem1 = Ch1RS232StopBitsComboBox.SelectedItem as ComboBoxItem;
                        if (stopBitsItem1 != null && double.TryParse(stopBitsItem1.Content.ToString(), out double stopBits1))
                            parameters["StopBits"] = stopBits1;

                        ComboBoxItem parityItem1 = Ch1RS232ParityComboBox.SelectedItem as ComboBoxItem;
                        if (parityItem1 != null)
                            parameters["Parity"] = parityItem1.Content.ToString();

                        if (int.TryParse(Ch1RS232DataTextBox.Text, out int data1))
                            parameters["Data"] = data1;
                    }
                    else
                    {
                        ComboBoxItem baudRateItem2 = Ch2RS232BaudRateComboBox.SelectedItem as ComboBoxItem;
                        if (baudRateItem2 != null && int.TryParse(baudRateItem2.Content.ToString(), out int baudRate2))
                            parameters["BaudRate"] = baudRate2;

                        ComboBoxItem dataBitsItem2 = Ch2RS232DataBitsComboBox.SelectedItem as ComboBoxItem;
                        if (dataBitsItem2 != null && int.TryParse(dataBitsItem2.Content.ToString(), out int dataBits2))
                            parameters["DataBits"] = dataBits2;

                        ComboBoxItem stopBitsItem2 = Ch2RS232StopBitsComboBox.SelectedItem as ComboBoxItem;
                        if (stopBitsItem2 != null && double.TryParse(stopBitsItem2.Content.ToString(), out double stopBits2))
                            parameters["StopBits"] = stopBits2;

                        ComboBoxItem parityItem2 = Ch2RS232ParityComboBox.SelectedItem as ComboBoxItem;
                        if (parityItem2 != null)
                            parameters["Parity"] = parityItem2.Content.ToString();

                        if (int.TryParse(Ch2RS232DataTextBox.Text, out int data2))
                            parameters["Data"] = data2;
                    }
                    break;
            }

            return parameters;
        }

        #endregion
    }
}