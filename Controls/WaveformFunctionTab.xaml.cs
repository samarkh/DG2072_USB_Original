using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DG2072_USB_Control
{
    /// <summary>
    /// Interaction logic for WaveformFunctionTab.xaml
    /// </summary>
    public partial class WaveformFunctionTab : UserControl
    {
        private RigolDG2072 rigolDG2072;
        private bool ch1IsPercentageMode = true;
        private bool ch2IsPercentageMode = true;
        private double _ch1FundamentalFrequencyInHz = 1000.0;
        private double _ch2FundamentalFrequencyInHz = 1000.0;

        public WaveformFunctionTab()
        {
            InitializeComponent();
            Ch1FundamentalFrequencyUnitComboBox.SelectedIndex = 2; // Hz by default
            Ch2FundamentalFrequencyUnitComboBox.SelectedIndex = 2; // Hz by default
            Ch1FundamentalAmplitudeUnitComboBox.SelectedIndex = 0; // V by default
            Ch2FundamentalAmplitudeUnitComboBox.SelectedIndex = 0; // V by default

            // Initialize frequency unit ComboBoxes  // Line 31
            Ch1FundamentalFrequencyUnitComboBox.SelectedIndex = 2; // Hz by default
            Ch2FundamentalFrequencyUnitComboBox.SelectedIndex = 2; // Hz by default

            // Add this line to attach to the Loaded event
            this.Loaded += (s, e) =>
            {
                if (rigolDG2072 != null && rigolDG2072.IsConnected)
                {
                    UpdateHarmonicUI();
                }
            };
        }

        public void Ch1HarmonicsToggle_Click(object sender, RoutedEventArgs e)
        {
            if (rigolDG2072 == null) return;

            bool isEnabled = Ch1HarmonicsToggle.IsChecked == true;
            rigolDG2072.SetHarmonicState(1, isEnabled);
            Ch1HarmonicsToggle.Content = isEnabled ? "ON" : "OFF";

            // Get current parameters
            double frequency = rigolDG2072.GetFrequency(1);
            double amplitude = rigolDG2072.GetAmplitude(1);
            double offset = rigolDG2072.GetOffset(1);
            double phase = rigolDG2072.GetPhase(1);

            Dictionary<string, object> parameters = new Dictionary<string, object>
    {
        { "Frequency", frequency },
        { "Amplitude", amplitude },
        { "Offset", offset },
        { "Phase", phase }
    };

            if (isEnabled)
            {
                // Switch to harmonic waveform
                rigolDG2072.ApplyWaveformWithSpecificParams(1, "Harmonic", parameters);
            }
            else
            {
                // Switch back to sine waveform when harmonics are disabled
                rigolDG2072.ApplyWaveformWithSpecificParams(1, "Sine", parameters);
            }

            // Update harmonic displays
            UpdateHarmonicUI();
        }

        public void Ch2HarmonicsToggle_Click(object sender, RoutedEventArgs e)
        {
            if (rigolDG2072 == null) return;

            bool isEnabled = Ch2HarmonicsToggle.IsChecked == true;
            rigolDG2072.SetHarmonicState(2, isEnabled);
            Ch2HarmonicsToggle.Content = isEnabled ? "ON" : "OFF";

            // Get current parameters
            double frequency = rigolDG2072.GetFrequency(2);
            double amplitude = rigolDG2072.GetAmplitude(2);
            double offset = rigolDG2072.GetOffset(2);
            double phase = rigolDG2072.GetPhase(2);

            Dictionary<string, object> parameters = new Dictionary<string, object>
    {
        { "Frequency", frequency },
        { "Amplitude", amplitude },
        { "Offset", offset },
        { "Phase", phase }
    };

            if (isEnabled)
            {
                // Switch to harmonic waveform
                rigolDG2072.ApplyWaveformWithSpecificParams(2, "Harmonic", parameters);
            }
            else
            {
                // Switch back to sine waveform when harmonics are disabled
                rigolDG2072.ApplyWaveformWithSpecificParams(2, "Sine", parameters);
            }

            // Update harmonic displays
            UpdateHarmonicUI();
        }

        // Property to get access to the RigolDG2072 instance from MainWindow
        public RigolDG2072 InstrumentController
        {
            get { return rigolDG2072; }
            set { rigolDG2072 = value; }
        }



   #region Channel 1 Harmonic Controls
        private void Ch1AmplitudeModeChanged(object sender, RoutedEventArgs e)
        {
            if (rigolDG2072 == null) return;

            ch1IsPercentageMode = Ch1AmplitudePercentageMode.IsChecked == true;

            // Update the header text
            Ch1AmplitudeHeader.Text = ch1IsPercentageMode ? "Amplitude (%)" : "Amplitude (V)";

            // Update the displayed values based on the new mode
            UpdateHarmonicAmplitudeDisplays(1);
        }

        private void Ch2AmplitudeModeChanged(object sender, RoutedEventArgs e)
        {
            if (rigolDG2072 == null) return;
            ch2IsPercentageMode = Ch2AmplitudePercentageMode.IsChecked == true;
            // Update the header text
            Ch2AmplitudeHeader.Text = ch2IsPercentageMode ? "Amplitude (%)" : "Amplitude (V)";
            // Update the displayed values based on the new mode
            UpdateHarmonicAmplitudeDisplays(2);
        }

        private void UpdateHarmonicAmplitudeDisplays(int channel)
        {
            if (rigolDG2072 == null) return;

            try
            {
                if (channel == 1)
                {
                    // Get the fundamental amplitude for percentage calculations
                    double fundamentalAmplitude = rigolDG2072.GetAmplitude(1);

                    // Update fundamental display
                    Ch1FundamentalAmplitudeTextBox.Text = ch1IsPercentageMode ? "100.0" : fundamentalAmplitude.ToString("F3");

                    // Update harmonics displays
                    for (int i = 2; i <= 8; i++)
                    {
                        TextBox amplitudeTextBox = FindName($"Ch1Harmonic{i}AmplitudeTextBox") as TextBox;
                        if (amplitudeTextBox != null)
                        {
                            double harmonicAmplitude = rigolDG2072.GetHarmonicAmplitude(1, i);

                            if (ch1IsPercentageMode && fundamentalAmplitude > 0)
                            {
                                // Convert from absolute to percentage
                                double percentage = (harmonicAmplitude / fundamentalAmplitude) * 100;
                                amplitudeTextBox.Text = percentage.ToString("F1");
                            }
                            else
                            {
                                // Display absolute amplitude
                                amplitudeTextBox.Text = harmonicAmplitude.ToString("F3");
                            }
                        }
                    }
                }
                else if (channel == 2)
                {
                    // Get the fundamental amplitude for percentage calculations
                    double fundamentalAmplitude = rigolDG2072.GetAmplitude(2);

                    // Update fundamental display
                    Ch2FundamentalAmplitudeTextBox.Text = ch2IsPercentageMode ? "100.0" : fundamentalAmplitude.ToString("F3");

                    // Update harmonics displays
                    for (int i = 2; i <= 8; i++)
                    {
                        TextBox amplitudeTextBox = FindName($"Ch2Harmonic{i}AmplitudeTextBox") as TextBox;
                        if (amplitudeTextBox != null)
                        {
                            double harmonicAmplitude = rigolDG2072.GetHarmonicAmplitude(2, i);

                            if (ch2IsPercentageMode && fundamentalAmplitude > 0)
                            {
                                // Convert from absolute to percentage
                                double percentage = (harmonicAmplitude / fundamentalAmplitude) * 100;
                                amplitudeTextBox.Text = percentage.ToString("F1");
                            }
                            else
                            {
                                // Display absolute amplitude
                                amplitudeTextBox.Text = harmonicAmplitude.ToString("F3");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating harmonic displays: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Ch1HarmonicCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (rigolDG2072 == null) return;

            try
            {
                // Create array of enabled harmonics
                bool[] enabledHarmonics = new bool[7];
                enabledHarmonics[0] = Ch1Harmonic2CheckBox.IsChecked == true;
                enabledHarmonics[1] = Ch1Harmonic3CheckBox.IsChecked == true;
                enabledHarmonics[2] = Ch1Harmonic4CheckBox.IsChecked == true;
                enabledHarmonics[3] = Ch1Harmonic5CheckBox.IsChecked == true;
                enabledHarmonics[4] = Ch1Harmonic6CheckBox.IsChecked == true;
                enabledHarmonics[5] = Ch1Harmonic7CheckBox.IsChecked == true;
                enabledHarmonics[6] = Ch1Harmonic8CheckBox.IsChecked == true;

                // Find the highest enabled harmonic
                int highestHarmonic = 2; // Default to 2 if none are enabled
                for (int i = 6; i >= 0; i--)
                {
                    if (enabledHarmonics[i])
                    {
                        highestHarmonic = i + 2; // +2 because index 0 corresponds to harmonic 2
                        break;
                    }
                }

                // Set the harmonic order to the highest enabled harmonic
                rigolDG2072.SetHarmonicOrder(1, highestHarmonic);

                // Set to USER mode and update enabled harmonics
                rigolDG2072.SetHarmonicType(1, "USER");
                rigolDG2072.SetUserDefinedHarmonics(1, enabledHarmonics);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating harmonic settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Ch2HarmonicCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (rigolDG2072 == null) return;

            try
            {
                // Create array of enabled harmonics
                bool[] enabledHarmonics = new bool[7];
                enabledHarmonics[0] = Ch2Harmonic2CheckBox.IsChecked == true;
                enabledHarmonics[1] = Ch2Harmonic3CheckBox.IsChecked == true;
                enabledHarmonics[2] = Ch2Harmonic4CheckBox.IsChecked == true;
                enabledHarmonics[3] = Ch2Harmonic5CheckBox.IsChecked == true;
                enabledHarmonics[4] = Ch2Harmonic6CheckBox.IsChecked == true;
                enabledHarmonics[5] = Ch2Harmonic7CheckBox.IsChecked == true;
                enabledHarmonics[6] = Ch2Harmonic8CheckBox.IsChecked == true;

                // Find the highest enabled harmonic
                int highestHarmonic = 2; // Default to 2 if none are enabled
                for (int i = 6; i >= 0; i--)
                {
                    if (enabledHarmonics[i])
                    {
                        highestHarmonic = i + 2; // +2 because index 0 corresponds to harmonic 2
                        break;
                    }
                }

                // Set the harmonic order to the highest enabled harmonic
                rigolDG2072.SetHarmonicOrder(2, highestHarmonic);

                // Set to USER mode and update enabled harmonics
                rigolDG2072.SetHarmonicType(2, "USER");
                rigolDG2072.SetUserDefinedHarmonics(2, enabledHarmonics);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating harmonic settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Ch1HarmonicAmplitudeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (rigolDG2072 == null) return;

            TextBox textBox = sender as TextBox;
            if (textBox != null && int.TryParse(textBox.Tag.ToString(), out int harmonicNumber))
            {
                try
                {
                    if (double.TryParse(textBox.Text, out double value))
                    {
                        if (harmonicNumber == 1)
                        {
                            // For fundamental
                            if (ch1IsPercentageMode)
                            {
                                // In percentage mode, fundamental is always 100%
                                // Just update the fundamental amplitude on the device
                                double amplitude = rigolDG2072.GetAmplitude(1);
                                rigolDG2072.SetAmplitude(1, amplitude);
                            }
                            else
                            {
                                // Set the fundamental amplitude directly
                                rigolDG2072.SetAmplitude(1, value);
                            }

                            // Update all harmonic display values since they depend on the fundamental
                            UpdateHarmonicAmplitudeDisplays(1);
                        }
                        else
                        {
                            // For harmonics
                            if (ch1IsPercentageMode)
                            {
                                // Convert percentage to absolute value
                                double fundamentalAmplitude = rigolDG2072.GetAmplitude(1);
                                value = (value / 100) * fundamentalAmplitude;
                            }

                            // Set the harmonic amplitude
                            rigolDG2072.SetHarmonicAmplitude(1, harmonicNumber, value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error setting harmonic amplitude: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Ch2HarmonicAmplitudeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (rigolDG2072 == null) return;

            TextBox textBox = sender as TextBox;
            if (textBox != null && int.TryParse(textBox.Tag.ToString(), out int harmonicNumber))
            {
                try
                {
                    if (double.TryParse(textBox.Text, out double value))
                    {
                        if (harmonicNumber == 1)
                        {
                            // For fundamental
                            if (ch2IsPercentageMode)
                            {
                                // In percentage mode, fundamental is always 100%
                                // Just update the fundamental amplitude on the device
                                double amplitude = rigolDG2072.GetAmplitude(2);
                                rigolDG2072.SetAmplitude(2, amplitude);
                            }
                            else
                            {
                                // Set the fundamental amplitude directly
                                rigolDG2072.SetAmplitude(2, value);
                            }

                            // Update all harmonic display values since they depend on the fundamental
                            UpdateHarmonicAmplitudeDisplays(2);
                        }
                        else
                        {
                            // For harmonics
                            if (ch2IsPercentageMode)
                            {
                                // Convert percentage to absolute value
                                double fundamentalAmplitude = rigolDG2072.GetAmplitude(2);
                                value = (value / 100) * fundamentalAmplitude;
                            }

                            // Set the harmonic amplitude
                            rigolDG2072.SetHarmonicAmplitude(2, harmonicNumber, value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error setting harmonic amplitude: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Ch1HarmonicPhaseTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (rigolDG2072 == null) return;

            TextBox textBox = sender as TextBox;
            if (textBox != null && int.TryParse(textBox.Tag.ToString(), out int harmonicNumber))
            {
                try
                {
                    if (double.TryParse(textBox.Text, out double phase))
                    {
                        if (harmonicNumber == 1)
                        {
                            // Set the fundamental phase
                            rigolDG2072.SetPhase(1, phase);
                        }
                        else
                        {
                            // Set the harmonic phase
                            rigolDG2072.SetHarmonicPhase(1, harmonicNumber, phase);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error setting harmonic phase: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Ch2HarmonicPhaseTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (rigolDG2072 == null) return;

            TextBox textBox = sender as TextBox;
            if (textBox != null && int.TryParse(textBox.Tag.ToString(), out int harmonicNumber))
            {
                try
                {
                    if (double.TryParse(textBox.Text, out double phase))
                    {
                        if (harmonicNumber == 1)
                        {
                            // Set the fundamental phase
                            rigolDG2072.SetPhase(2, phase);
                        }
                        else
                        {
                            // Set the harmonic phase
                            rigolDG2072.SetHarmonicPhase(2, harmonicNumber, phase);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error setting harmonic phase: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Ch1ApplyHarmonicsButton_Click(object sender, RoutedEventArgs e)
        {
            if (rigolDG2072 == null) return;

            try
            {
                // Enable harmonics if toggle is on
                bool enableHarmonics = Ch1HarmonicsToggle.IsChecked == true;
                rigolDG2072.SetHarmonicState(1, enableHarmonics);

                // Apply user-defined harmonics
                Ch1HarmonicCheckBox_Changed(sender, e);
                // Inside Ch1ApplyHarmonicsButton_Click and Ch2ApplyHarmonicsButton_Click methods,
                // after the line "Ch1HarmonicCheckBox_Changed(sender, e);" or "Ch2HarmonicCheckBox_Changed(sender, e);"
                // add this code for extra safety:

                // Find the highest enabled harmonic again in case the checkbox change event didn't fire
                bool[] enabled = new bool[7];
                enabled[0] = Ch1Harmonic2CheckBox.IsChecked == true;
                enabled[1] = Ch1Harmonic2CheckBox.IsChecked == true;
                enabled[2] = Ch1Harmonic2CheckBox.IsChecked == true; 
                enabled[3] = Ch1Harmonic2CheckBox.IsChecked == true; 
                enabled[4] = Ch1Harmonic2CheckBox.IsChecked == true; 
                enabled[5] = Ch1Harmonic2CheckBox.IsChecked == true; 
                enabled[6] = Ch1Harmonic8CheckBox.IsChecked == true; 

                int highestHarmonic = 2;
                for (int i = 6; i >= 0; i--)
                {
                    if (enabled[i])
                    {
                        highestHarmonic = i + 2;
                        break;
                    }
                }

                rigolDG2072.SetHarmonicOrder(1, highestHarmonic); // (or channel 2)
                // Update all amplitude and phase values
                for (int i = 1; i <= 8; i++)
                {
                    TextBox amplitudeTextBox = FindName($"Ch1Harmonic{i}AmplitudeTextBox") as TextBox;
                    TextBox phaseTextBox = FindName($"Ch1Harmonic{i}PhaseTextBox") as TextBox;

                    if (i == 1)
                    {
                        // Handle fundamental separately
                        if (amplitudeTextBox != null && !ch1IsPercentageMode && double.TryParse(amplitudeTextBox.Text, out double amplitude))
                        {
                            rigolDG2072.SetAmplitude(1, amplitude);
                        }

                        if (phaseTextBox != null && double.TryParse(phaseTextBox.Text, out double phase))
                        {
                            rigolDG2072.SetPhase(1, phase);
                        }
                    }
                    else
                    {
                        // Handle harmonics
                        if (amplitudeTextBox != null && double.TryParse(amplitudeTextBox.Text, out double amplitudeValue))
                        {
                            if (ch1IsPercentageMode)
                            {
                                // Convert percentage to absolute value
                                double fundamentalAmplitude = rigolDG2072.GetAmplitude(1);
                                amplitudeValue = (amplitudeValue / 100) * fundamentalAmplitude;
                            }

                            rigolDG2072.SetHarmonicAmplitude(1, i, amplitudeValue);
                        }

                        if (phaseTextBox != null && double.TryParse(phaseTextBox.Text, out double phaseValue))
                        {
                            rigolDG2072.SetHarmonicPhase(1, i, phaseValue);
                        }
                    }
                }

                // Update all displays after applying settings
                UpdateHarmonicAmplitudeDisplays(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying harmonic settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Ch2ApplyHarmonicsButton_Click(object sender, RoutedEventArgs e)
        {
            if (rigolDG2072 == null) return;

            try
            {
                // Enable harmonics if toggle is on
                bool enableHarmonics = Ch2HarmonicsToggle.IsChecked == true;
                rigolDG2072.SetHarmonicState(2, enableHarmonics);

                // Apply user-defined harmonics
                Ch2HarmonicCheckBox_Changed(sender, e);

                // Apply user-defined harmonics
                Ch2HarmonicCheckBox_Changed(sender, e);
                // Inside Ch2ApplyHarmonicsButton_Click and Ch2ApplyHarmonicsButton_Click methods,
                // after the line "Ch2HarmonicCheckBox_Changed(sender, e);
                // add this code for extra safety:

                // Find the highest enabled harmonic again in case the checkbox change event didn't fire
                bool[] enabled = new bool[7];
                enabled[0] = Ch2Harmonic2CheckBox.IsChecked == true;
                enabled[1] = Ch2Harmonic2CheckBox.IsChecked == true;
                enabled[2] = Ch2Harmonic2CheckBox.IsChecked == true;
                enabled[3] = Ch2Harmonic2CheckBox.IsChecked == true;
                enabled[4] = Ch2Harmonic2CheckBox.IsChecked == true;
                enabled[5] = Ch2Harmonic2CheckBox.IsChecked == true;
                enabled[6] = Ch2Harmonic8CheckBox.IsChecked == true;


                // Update all amplitude and phase values
                for (int i = 1; i <= 8; i++)
                {
                    TextBox amplitudeTextBox = FindName($"Ch2Harmonic{i}AmplitudeTextBox") as TextBox;
                    TextBox phaseTextBox = FindName($"Ch2Harmonic{i}PhaseTextBox") as TextBox;

                    if (i == 1)
                    {
                        // Handle fundamental separately
                        if (amplitudeTextBox != null && !ch2IsPercentageMode && double.TryParse(amplitudeTextBox.Text, out double amplitude))
                        {
                            rigolDG2072.SetAmplitude(2, amplitude);
                        }

                        if (phaseTextBox != null && double.TryParse(phaseTextBox.Text, out double phase))
                        {
                            rigolDG2072.SetPhase(2, phase);
                        }
                    }
                    else
                    {
                        // Handle harmonics
                        if (amplitudeTextBox != null && double.TryParse(amplitudeTextBox.Text, out double amplitudeValue))
                        {
                            if (ch2IsPercentageMode)
                            {
                                // Convert percentage to absolute value
                                double fundamentalAmplitude = rigolDG2072.GetAmplitude(2);
                                amplitudeValue = (amplitudeValue / 100) * fundamentalAmplitude;
                            }

                            rigolDG2072.SetHarmonicAmplitude(2, i, amplitudeValue);
                        }

                        if (phaseTextBox != null && double.TryParse(phaseTextBox.Text, out double phaseValue))
                        {
                            rigolDG2072.SetHarmonicPhase(2, i, phaseValue);
                        }
                    }
                }

                // Update all displays after applying settings
                UpdateHarmonicAmplitudeDisplays(2);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying harmonic settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Method to initialize and update harmonic UI when tab is shown
        public void UpdateHarmonicUI()
        {
            if (rigolDG2072 == null) return;

            try
            {

                // Get frequencies from device  // Line 395
                _ch1FundamentalFrequencyInHz = rigolDG2072.GetFrequency(1);
                _ch2FundamentalFrequencyInHz = rigolDG2072.GetFrequency(2);

                // Update frequency displays
                string ch1FreqUnit = ((ComboBoxItem)Ch1FundamentalFrequencyUnitComboBox.SelectedItem).Content.ToString();
                string ch2FreqUnit = ((ComboBoxItem)Ch2FundamentalFrequencyUnitComboBox.SelectedItem).Content.ToString();

                // Update amplitude displays
                string ch1AmpUnit = ((ComboBoxItem)Ch1FundamentalAmplitudeUnitComboBox.SelectedItem).Content.ToString();
                string ch2AmpUnit = ((ComboBoxItem)Ch2FundamentalAmplitudeUnitComboBox.SelectedItem).Content.ToString();

                double ch1Amplitude = rigolDG2072.GetAmplitude(1);
                double ch2Amplitude = rigolDG2072.GetAmplitude(2);

                Ch1FundamentalFrequencyTextBox.Text = ConvertFrequencyFromBaseUnits(_ch1FundamentalFrequencyInHz, ch1FreqUnit).ToString("G6");
                Ch2FundamentalFrequencyTextBox.Text = ConvertFrequencyFromBaseUnits(_ch2FundamentalFrequencyInHz, ch2FreqUnit).ToString("G6");

                // Display amplitude in appropriate units
                Ch1FundamentalAmplitudeTextBox.Text = ConvertVoltageFromBaseUnits(ch1Amplitude, ch1AmpUnit).ToString("F3");
                Ch2FundamentalAmplitudeTextBox.Text = ConvertVoltageFromBaseUnits(ch2Amplitude, ch2AmpUnit).ToString("F3");

                // Rest of method...
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating harmonic UI: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion


   #region Channel 2 Harmonic Controls











        #endregion


   #region Frequency Handling Methods

        private void Ch1FundamentalFrequencyTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (rigolDG2072 == null) return;

            try
            {
                if (double.TryParse(Ch1FundamentalFrequencyTextBox.Text, out double displayedFrequency))
                {
                    string unit = ((ComboBoxItem)Ch1FundamentalFrequencyUnitComboBox.SelectedItem).Content.ToString();
                    double baseFrequency = ConvertFrequencyToBaseUnits(displayedFrequency, unit);

                    _ch1FundamentalFrequencyInHz = baseFrequency;
                    rigolDG2072.SetFrequency(1, baseFrequency);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting frequency: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Ch2FundamentalFrequencyTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (rigolDG2072 == null) return;

            try
            {
                if (double.TryParse(Ch2FundamentalFrequencyTextBox.Text, out double displayedFrequency))
                {
                    string unit = ((ComboBoxItem)Ch2FundamentalFrequencyUnitComboBox.SelectedItem).Content.ToString();
                    double baseFrequency = ConvertFrequencyToBaseUnits(displayedFrequency, unit);

                    _ch2FundamentalFrequencyInHz = baseFrequency;
                    rigolDG2072.SetFrequency(2, baseFrequency);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting frequency: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Ch1FundamentalFrequencyUnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ch1FundamentalFrequencyTextBox == null || e.AddedItems.Count == 0) return;

            try
            {
                if (double.TryParse(Ch1FundamentalFrequencyTextBox.Text, out double displayedValue))
                {
                    string newUnit = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();
                    string oldUnit = e.RemovedItems.Count > 0 ?
                        ((ComboBoxItem)e.RemovedItems[0]).Content.ToString() : "Hz";

                    // Calculate new value based on the selected unit
                    Ch1FundamentalFrequencyTextBox.Text = ConvertFrequencyFromBaseUnits(_ch1FundamentalFrequencyInHz, newUnit).ToString("G6");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing frequency unit: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Ch2FundamentalFrequencyUnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ch2FundamentalFrequencyTextBox == null || e.AddedItems.Count == 0) return;

            try
            {
                if (double.TryParse(Ch2FundamentalFrequencyTextBox.Text, out double displayedValue))
                {
                    string newUnit = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();
                    string oldUnit = e.RemovedItems.Count > 0 ?
                        ((ComboBoxItem)e.RemovedItems[0]).Content.ToString() : "Hz";

                    // Calculate new value based on the selected unit
                    Ch2FundamentalFrequencyTextBox.Text = ConvertFrequencyFromBaseUnits(_ch2FundamentalFrequencyInHz, newUnit).ToString("G6");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing frequency unit: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private double ConvertFrequencyToBaseUnits(double value, string unit)
        {
            switch (unit)
            {
                case "MHz": return value * 1000000;
                case "kHz": return value * 1000;
                case "Hz": return value;
                case "mHz": return value / 1000;
                case "�Hz": return value / 1000000;
                default: return value;
            }
        }

        private double ConvertFrequencyFromBaseUnits(double baseValue, string targetUnit)
        {
            switch (targetUnit)
            {
                case "MHz": return baseValue / 1000000;
                case "kHz": return baseValue / 1000;
                case "Hz": return baseValue;
                case "mHz": return baseValue * 1000;
                case "�Hz": return baseValue * 1000000;
                default: return baseValue;
            }
        }

        private void Ch1FundamentalAmplitudeUnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Skip if in design mode or if controls or data aren't available
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) ||
                Ch1FundamentalAmplitudeTextBox == null ||
                e.AddedItems.Count == 0)
                return;

            if (rigolDG2072 == null)
            {
                // Optional: Log the error instead of showing a message box
                // This avoids popups during design time
                Console.WriteLine("Instrument not connected.");
                return;
            }

            try
            {
                if (double.TryParse(Ch1FundamentalAmplitudeTextBox.Text, out double displayedValue))
                {
                    string newUnit = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();
                    double amplitudeInVolts = rigolDG2072.GetAmplitude(1);
                    Ch1FundamentalAmplitudeTextBox.Text = ConvertVoltageFromBaseUnits(amplitudeInVolts, newUnit).ToString("F3");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing amplitude unit: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Ch1FundamentalAmplitudeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (rigolDG2072 == null) return;

            try
            {
                if (double.TryParse(Ch1FundamentalAmplitudeTextBox.Text, out double displayedAmplitude))
                {
                    string unit = ((ComboBoxItem)Ch1FundamentalAmplitudeUnitComboBox.SelectedItem).Content.ToString();
                    double baseAmplitude = ConvertVoltageToBaseUnits(displayedAmplitude, unit);

                    rigolDG2072.SetAmplitude(1, baseAmplitude);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting amplitude: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Ch2FundamentalAmplitudeUnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ch2FundamentalAmplitudeTextBox == null || e.AddedItems.Count == 0 || rigolDG2072 == null) return;

            try
            {
                if (double.TryParse(Ch2FundamentalAmplitudeTextBox.Text, out double displayedValue))
                {
                    string newUnit = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();

                    // Get current amplitude in volts
                    double amplitudeInVolts = rigolDG2072.GetAmplitude(2);

                    // Display in selected unit
                    Ch2FundamentalAmplitudeTextBox.Text = ConvertVoltageFromBaseUnits(amplitudeInVolts, newUnit).ToString("F3");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error changing amplitude unit: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Ch2FundamentalAmplitudeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (rigolDG2072 == null) return;

            try
            {
                if (double.TryParse(Ch2FundamentalAmplitudeTextBox.Text, out double displayedAmplitude))
                {
                    string unit = ((ComboBoxItem)Ch2FundamentalAmplitudeUnitComboBox.SelectedItem).Content.ToString();
                    double baseAmplitude = ConvertVoltageToBaseUnits(displayedAmplitude, unit);

                    rigolDG2072.SetAmplitude(2, baseAmplitude);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting amplitude: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }








        #endregion
        private double ConvertVoltageFromBaseUnits(double baseValue, string targetUnit)
        {
            switch (targetUnit)
            {
                case "mV": return baseValue * 1000;
                case "V": return baseValue;
                default: return baseValue;
            }
        }

        private double ConvertVoltageToBaseUnits(double value, string unit)
        {
            switch (unit)
            {
                case "mV": return value / 1000;
                case "V": return value;
                default: return value;
            }
        }

    }
}
