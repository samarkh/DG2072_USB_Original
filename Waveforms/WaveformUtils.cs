using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DG2072_USB_Control
{
    public partial class MainWindow : System.Windows.Window
    {
        #region Waveform UI Helpers

        private void ShowWaveformSpecificControls(int channel, string waveform)
        {
            // Store the selected waveform for use with visibility converters
            if (channel == 1)
            {
                Ch1SelectedWaveform = waveform;
            }
            else
            {
                Ch2SelectedWaveform = waveform;
            }
            
            // Hide all waveform-specific control groups first
            if (channel == 1)
            {
                Ch1RampParamsGroup.Visibility = Visibility.Collapsed;
                Ch1SquareParamsGroup.Visibility = Visibility.Collapsed;
                Ch1PulseParamsGroup.Visibility = Visibility.Collapsed;
                Ch1FrequencyPanel.Visibility = Visibility.Visible;
                Ch1PhasePanel.Visibility = Visibility.Visible;
            }
            else
            {
                Ch2RampParamsGroup.Visibility = Visibility.Collapsed;
                Ch2SquareParamsGroup.Visibility = Visibility.Collapsed;
                Ch2PulseParamsGroup.Visibility = Visibility.Collapsed;
                Ch2FrequencyPanel.Visibility = Visibility.Visible;
                Ch2PhasePanel.Visibility = Visibility.Visible;
            }

            // Show the appropriate parameter group based on waveform type
            switch (waveform.ToUpper())
            {
                case "RAMP":
                    if (channel == 1)
                        Ch1RampParamsGroup.Visibility = Visibility.Visible;
                    else
                        Ch2RampParamsGroup.Visibility = Visibility.Visible;
                    break;
                case "SQUARE":
                    if (channel == 1)
                        Ch1SquareParamsGroup.Visibility = Visibility.Visible;
                    else
                        Ch2SquareParamsGroup.Visibility = Visibility.Visible;
                    break;

                case "PULSE":
                    if (channel == 1)
                    {
                        Ch1PulseParamsGroup.Visibility = Visibility.Visible;

                        // Update the pulse parameter display when switching to pulse waveform
                        UpdatePulseParametersDisplay(1);
                    }
                    else
                    {
                        Ch2PulseParamsGroup.Visibility = Visibility.Visible;

                        // Update the pulse parameter display when switching to pulse waveform
                        UpdatePulseParametersDisplay(2);
                    }
                    break;
                case "NOISE":
                case "DC":
                    // These waveforms don't use frequency or phase
                    if (channel == 1)
                    {
                        Ch1FrequencyPanel.Visibility = Visibility.Collapsed;
                        Ch1PhasePanel.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        Ch2FrequencyPanel.Visibility = Visibility.Collapsed;
                        Ch2PhasePanel.Visibility = Visibility.Collapsed;
                    }
                    break;
            }
        }

        // Helper method to get all parameters for a channel
        private Dictionary<string, object> GetChannelParameters(int channel)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            // Get the selected waveform
            ComboBox waveformComboBox = channel == 1 ? Ch1WaveformComboBox : Ch2WaveformComboBox;
            string waveform = ((ComboBoxItem)waveformComboBox.SelectedItem).Content.ToString();

            // Add common parameters
            if (channel == 1)
            {
                if (waveform.ToUpper() != "NOISE" && waveform.ToUpper() != "DC")
                {
                    parameters["Frequency"] = _ch1FrequencyInHz;
                }
                
                parameters["Amplitude"] = _ch1AmplitudeInVolts;
                parameters["Offset"] = _ch1OffsetInVolts;
                
                if (waveform.ToUpper() != "NOISE" && waveform.ToUpper() != "DC")
                {
                    if (double.TryParse(Ch1PhaseTextBox.Text, out double phase))
                    {
                        parameters["Phase"] = phase;
                    }
                }
            }
            else
            {
                if (waveform.ToUpper() != "NOISE" && waveform.ToUpper() != "DC")
                {
                    parameters["Frequency"] = _ch2FrequencyInHz;
                }
                
                parameters["Amplitude"] = _ch2AmplitudeInVolts;
                parameters["Offset"] = _ch2OffsetInVolts;
                
                if (waveform.ToUpper() != "NOISE" && waveform.ToUpper() != "DC")
                {
                    if (double.TryParse(Ch2PhaseTextBox.Text, out double phase))
                    {
                        parameters["Phase"] = phase;
                    }
                }
            }

            // Add waveform-specific parameters
            switch (waveform.ToUpper())
            {
                case "RAMP":
                    if (channel == 1 && double.TryParse(Ch1RampSymmetryTextBox.Text, out double symmetry1))
                        parameters["Symmetry"] = symmetry1;
                    else if (channel == 2 && double.TryParse(Ch2RampSymmetryTextBox.Text, out double symmetry2))
                        parameters["Symmetry"] = symmetry2;
                    break;

                case "SQUARE":
                    if (channel == 1 && double.TryParse(Ch1SquareDutyCycleTextBox.Text, out double dutyCycle1))
                        parameters["DutyCycle"] = dutyCycle1;
                    else if (channel == 2 && double.TryParse(Ch2SquareDutyCycleTextBox.Text, out double dutyCycle2))
                        parameters["DutyCycle"] = dutyCycle2;
                    break;

                case "PULSE":
                    if (channel == 1)
                    {
                        parameters["Period"] = _ch1PulsePeriodInSeconds;
                        parameters["Width"] = _ch1PulseWidthInSeconds;
                        parameters["RiseTime"] = _ch1PulseRiseTimeInSeconds;
                        parameters["FallTime"] = _ch1PulseFallTimeInSeconds;
                    }
                    else
                    {
                        parameters["Period"] = _ch2PulsePeriodInSeconds;
                        parameters["Width"] = _ch2PulseWidthInSeconds;
                        parameters["RiseTime"] = _ch2PulseRiseTimeInSeconds;
                        parameters["FallTime"] = _ch2PulseFallTimeInSeconds;
                    }
                    break;
            }

            return parameters;
        }
        #endregion
    }
}