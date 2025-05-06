using System;
using System.Windows;
using System.Windows.Controls;

namespace DG2072_USB_Control
{
    public partial class MainWindow : System.Windows.Window
    {
        // Method to update pulse parameter displays
        private void UpdatePulseParametersDisplay(int channel)
        {
            if (!isConnected) return;

            try
            {
                if (channel == 1)
                {
                    // Get current values from device
                    double period = rigolDG2072.GetPulsePeriod(1);
                    double width = rigolDG2072.GetPulseWidth(1);
                    double riseTime = rigolDG2072.GetPulseRiseTime(1);
                    double fallTime = rigolDG2072.GetPulseFallTime(1);

                    // Store values in base units
                    _ch1PulsePeriodInSeconds = period;
                    _ch1PulseWidthInSeconds = width;
                    _ch1PulseRiseTimeInSeconds = riseTime;
                    _ch1PulseFallTimeInSeconds = fallTime;

                    // Update displayed values with appropriate units
                    string periodUnit = ((ComboBoxItem)Ch1PulsePeriodUnitComboBox.SelectedItem).Content.ToString();
                    string widthUnit = ((ComboBoxItem)Ch1PulseWidthUnitComboBox.SelectedItem).Content.ToString();
                    string riseTimeUnit = ((ComboBoxItem)Ch1PulseRiseTimeUnitComboBox.SelectedItem).Content.ToString();
                    string fallTimeUnit = ((ComboBoxItem)Ch1PulseFallTimeUnitComboBox.SelectedItem).Content.ToString();

                    Ch1PulsePeriodTextBox.Text = ConvertTimeFromBaseUnits(period, periodUnit).ToString("G6");
                    Ch1PulseWidthTextBox.Text = ConvertTimeFromBaseUnits(width, widthUnit).ToString("G6");
                    Ch1PulseRiseTimeTextBox.Text = ConvertTimeFromBaseUnits(riseTime, riseTimeUnit).ToString("G6");
                    Ch1PulseFallTimeTextBox.Text = ConvertTimeFromBaseUnits(fallTime, fallTimeUnit).ToString("G6");
                }
                else if (channel == 2)
                {
                    // Get current values from device
                    double period = rigolDG2072.GetPulsePeriod(2);
                    double width = rigolDG2072.GetPulseWidth(2);
                    double riseTime = rigolDG2072.GetPulseRiseTime(2);
                    double fallTime = rigolDG2072.GetPulseFallTime(2);

                    // Store values in base units
                    _ch2PulsePeriodInSeconds = period;
                    _ch2PulseWidthInSeconds = width;
                    _ch2PulseRiseTimeInSeconds = riseTime;
                    _ch2PulseFallTimeInSeconds = fallTime;

                    // Update displayed values with appropriate units
                    string periodUnit = ((ComboBoxItem)Ch2PulsePeriodUnitComboBox.SelectedItem).Content.ToString();
                    string widthUnit = ((ComboBoxItem)Ch2PulseWidthUnitComboBox.SelectedItem).Content.ToString();
                    string riseTimeUnit = ((ComboBoxItem)Ch2PulseRiseTimeUnitComboBox.SelectedItem).Content.ToString();
                    string fallTimeUnit = ((ComboBoxItem)Ch2PulseFallTimeUnitComboBox.SelectedItem).Content.ToString();

                    Ch2PulsePeriodTextBox.Text = ConvertTimeFromBaseUnits(period, periodUnit).ToString("G6");
                    Ch2PulseWidthTextBox.Text = ConvertTimeFromBaseUnits(width, widthUnit).ToString("G6");
                    Ch2PulseRiseTimeTextBox.Text = ConvertTimeFromBaseUnits(riseTime, riseTimeUnit).ToString("G6");
                    Ch2PulseFallTimeTextBox.Text = ConvertTimeFromBaseUnits(fallTime, fallTimeUnit).ToString("G6");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error updating pulse parameters display: {ex.Message}");
            }
        }

        #region Pulse Parameter Event Handlers

        private void Ch1PulsePeriodTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch1PulsePeriodTextBox.Text, out double displayedPeriod))
            {
                string unit = ((ComboBoxItem)Ch1PulsePeriodUnitComboBox.SelectedItem).Content.ToString();
                double basePeriod = ConvertTimeToBaseUnits(displayedPeriod, unit);

                _ch1PulsePeriodInSeconds = basePeriod;
                rigolDG2072.SetPulsePeriod(1, basePeriod);
                LogMessage($"Set CH1 pulse period to {displayedPeriod} {unit} ({basePeriod} s)");
            }
            else
            {
                LogMessage("Invalid period value for CH1 pulse");
                string unit = ((ComboBoxItem)Ch1PulsePeriodUnitComboBox.SelectedItem).Content.ToString();
                double displayValue = ConvertTimeFromBaseUnits(_ch1PulsePeriodInSeconds, unit);
                Ch1PulsePeriodTextBox.Text = displayValue.ToString("G6");
            }
        }

        private void Ch1PulseWidthTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch1PulseWidthTextBox.Text, out double displayedWidth))
            {
                string unit = ((ComboBoxItem)Ch1PulseWidthUnitComboBox.SelectedItem).Content.ToString();
                double baseWidth = ConvertTimeToBaseUnits(displayedWidth, unit);

                _ch1PulseWidthInSeconds = baseWidth;
                rigolDG2072.SetPulseWidth(1, baseWidth);
                LogMessage($"Set CH1 pulse width to {displayedWidth} {unit} ({baseWidth} s)");
            }
            else
            {
                LogMessage("Invalid width value for CH1 pulse");
                string unit = ((ComboBoxItem)Ch1PulseWidthUnitComboBox.SelectedItem).Content.ToString();
                double displayValue = ConvertTimeFromBaseUnits(_ch1PulseWidthInSeconds, unit);
                Ch1PulseWidthTextBox.Text = displayValue.ToString("G6");
            }
        }

        private void Ch1PulseRiseTimeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch1PulseRiseTimeTextBox.Text, out double displayedRiseTime))
            {
                string unit = ((ComboBoxItem)Ch1PulseRiseTimeUnitComboBox.SelectedItem).Content.ToString();
                double baseRiseTime = ConvertTimeToBaseUnits(displayedRiseTime, unit);

                // Store the calculated base rise time value
                _ch1PulseRiseTimeInSeconds = baseRiseTime;

                // Set the device pulse rise time
                rigolDG2072.SetPulseRiseTime(1, baseRiseTime);
                LogMessage($"Set CH1 pulse rise time to {displayedRiseTime} {unit} ({baseRiseTime} s)");
            }
            else
            {
                LogMessage("Invalid rise time value for CH1 pulse");
                // Revert to the previous valid value
                string unit = ((ComboBoxItem)Ch1PulseRiseTimeUnitComboBox.SelectedItem).Content.ToString();
                double displayValue = ConvertTimeFromBaseUnits(_ch1PulseRiseTimeInSeconds, unit);
                Ch1PulseRiseTimeTextBox.Text = displayValue.ToString("G6");
            }
        }

        private void Ch1PulseFallTimeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch1PulseFallTimeTextBox.Text, out double displayedFallTime))
            {
                string unit = ((ComboBoxItem)Ch1PulseFallTimeUnitComboBox.SelectedItem).Content.ToString();
                double baseFallTime = ConvertTimeToBaseUnits(displayedFallTime, unit);

                // Store the calculated base fall time value
                _ch1PulseFallTimeInSeconds = baseFallTime;

                // Set the device pulse fall time
                rigolDG2072.SetPulseFallTime(1, baseFallTime);
                LogMessage($"Set CH1 pulse fall time to {displayedFallTime} {unit} ({baseFallTime} s)");
            }
            else
            {
                LogMessage("Invalid fall time value for CH1 pulse");
                // Revert to the previous valid value
                string unit = ((ComboBoxItem)Ch1PulseFallTimeUnitComboBox.SelectedItem).Content.ToString();
                double displayValue = ConvertTimeFromBaseUnits(_ch1PulseFallTimeInSeconds, unit);
                Ch1PulseFallTimeTextBox.Text = displayValue.ToString("G6");
            }
        }

        private void Ch2PulsePeriodTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch2PulsePeriodTextBox.Text, out double displayedPeriod))
            {
                string unit = ((ComboBoxItem)Ch2PulsePeriodUnitComboBox.SelectedItem).Content.ToString();
                double basePeriod = ConvertTimeToBaseUnits(displayedPeriod, unit);

                _ch2PulsePeriodInSeconds = basePeriod;
                rigolDG2072.SetPulsePeriod(2, basePeriod);
                LogMessage($"Set CH2 pulse period to {displayedPeriod} {unit} ({basePeriod} s)");
            }
            else
            {
                LogMessage("Invalid period value for CH2 pulse");
                string unit = ((ComboBoxItem)Ch2PulsePeriodUnitComboBox.SelectedItem).Content.ToString();
                double displayValue = ConvertTimeFromBaseUnits(_ch2PulsePeriodInSeconds, unit);
                Ch2PulsePeriodTextBox.Text = displayValue.ToString("G6");
            }
        }

        private void Ch2PulseWidthTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch2PulseWidthTextBox.Text, out double displayedWidth))
            {
                string unit = ((ComboBoxItem)Ch2PulseWidthUnitComboBox.SelectedItem).Content.ToString();
                double baseWidth = ConvertTimeToBaseUnits(displayedWidth, unit);

                _ch2PulseWidthInSeconds = baseWidth;
                rigolDG2072.SetPulseWidth(2, baseWidth);
                LogMessage($"Set CH2 pulse width to {displayedWidth} {unit} ({baseWidth} s)");
            }
            else
            {
                LogMessage("Invalid width value for CH2 pulse");
                string unit = ((ComboBoxItem)Ch2PulseWidthUnitComboBox.SelectedItem).Content.ToString();
                double displayValue = ConvertTimeFromBaseUnits(_ch2PulseWidthInSeconds, unit);
                Ch2PulseWidthTextBox.Text = displayValue.ToString("G6");
            }
        }

        private void Ch2PulseRiseTimeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch2PulseRiseTimeTextBox.Text, out double displayedRiseTime))
            {
                string unit = ((ComboBoxItem)Ch2PulseRiseTimeUnitComboBox.SelectedItem).Content.ToString();
                double baseRiseTime = ConvertTimeToBaseUnits(displayedRiseTime, unit);

                _ch2PulseRiseTimeInSeconds = baseRiseTime;
                rigolDG2072.SetPulseRiseTime(2, baseRiseTime);
                LogMessage($"Set CH2 pulse rise time to {displayedRiseTime} {unit} ({baseRiseTime} s)");
            }
            else
            {
                LogMessage("Invalid rise time value for CH2 pulse");
                string unit = ((ComboBoxItem)Ch2PulseRiseTimeUnitComboBox.SelectedItem).Content.ToString();
                double displayValue = ConvertTimeFromBaseUnits(_ch2PulseRiseTimeInSeconds, unit);
                Ch2PulseRiseTimeTextBox.Text = displayValue.ToString("G6");
            }
        }

        private void Ch2PulseFallTimeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch2PulseFallTimeTextBox.Text, out double displayedFallTime))
            {
                string unit = ((ComboBoxItem)Ch2PulseFallTimeUnitComboBox.SelectedItem).Content.ToString();
                double baseFallTime = ConvertTimeToBaseUnits(displayedFallTime, unit);

                _ch2PulseFallTimeInSeconds = baseFallTime;
                rigolDG2072.SetPulseFallTime(2, baseFallTime);
                LogMessage($"Set CH2 pulse fall time to {displayedFallTime} {unit} ({baseFallTime} s)");
            }
            else
            {
                LogMessage("Invalid fall time value for CH2 pulse");
                string unit = ((ComboBoxItem)Ch2PulseFallTimeUnitComboBox.SelectedItem).Content.ToString();
                double displayValue = ConvertTimeFromBaseUnits(_ch2PulseFallTimeInSeconds, unit);
                Ch2PulseFallTimeTextBox.Text = displayValue.ToString("G6");
            }
        }

        // Unit conversion handlers - moved to MainWindow.xaml.cs to avoid duplication
        #endregion
    }
}