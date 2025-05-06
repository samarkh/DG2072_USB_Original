using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MathNet.Numerics;
using System.Collections.Generic;
using System.ComponentModel;

namespace DG2072_USB_Control
{
    public partial class MainWindow : System.Windows.Window, INotifyPropertyChanged
    {
        // VISA instrument handle
        private IntPtr instrumentHandle = IntPtr.Zero;
        private bool isConnected = false;
        private const string InstrumentAddress = "USB0::0x1AB1::0x0644::DG2P224100508::INSTR";
        private RigolDG2072 rigolDG2072;


        // Frequency
        private double _ch1FrequencyInHz = 1000.0;  // Default 1000 Hz
        private double _ch2FrequencyInHz = 1000.0;  // Default 1000 Hz

        // Amplitude and Offset
        private double _ch1AmplitudeInVolts = 1.0;  // Default 1.0 V
        private double _ch1OffsetInVolts = 0.0;     // Default 0.0 V
        private double _ch2AmplitudeInVolts = 1.0;  // Default 1.0 V
        private double _ch2OffsetInVolts = 0.0;     // Default 0.0 V

        //Pulse parameters
        private double _ch1PulsePeriodInSeconds = 0.001;  // Default 1ms
        private double _ch2PulsePeriodInSeconds = 0.001;  // Default 1ms
        private double _ch1PulseWidthInSeconds = 0.0005;  // Default 500µs
        private double _ch2PulseWidthInSeconds = 0.0005;  // Default 500µs
        private double _ch1PulseRiseTimeInSeconds = 0.00000002;  // Default 20ns
        private double _ch2PulseRiseTimeInSeconds = 0.00000002;  // Default 20ns
        private double _ch1PulseFallTimeInSeconds = 0.00000002;  // Default 20ns
        private double _ch2PulseFallTimeInSeconds = 0.00000002;  // Default 20ns



        // Constants for VISA operations
        private const int VI_NULL = 0;
        private const int VI_SUCCESS = 0;
        private const int VI_TRUE = 1;
        private const int VI_FALSE = 0;
        private const int VI_INFINITE = -1;
        private const int VI_TMO_IMMEDIATE = 0;
        private const int VI_GPIB_REN_DEASSERT = 0;
        private const int VI_GPIB_REN_ASSERT = 1;
        private const int VI_GPIB_REN_DEASSERT_GTL = 2;
        private const int VI_GPIB_REN_ASSERT_ADDRESS = 3;
        private const int VI_GPIB_REN_ASSERT_LLO = 4;
        private const int VI_GPIB_REN_ASSERT_ADDRESS_LLO = 5;
        private const int VI_GPIB_REN_ADDRESS_GTL = 6;



        private double ConvertVoltageToBaseUnits(double value, string unit)
        {
            double result;
            switch (unit)
            {
                case "V": result = value; break;
                case "mV": result = value / 1000; break;
                default: result = value; break;
            }
            LogMessage($"Converting {value} {unit} to {result} V");
            return result;
        }

        private double ConvertVoltageFromBaseUnits(double baseValue, string targetUnit)
        {
            switch (targetUnit)
            {
                case "V": return baseValue;
                case "mV": return baseValue * 1000;
                default: return baseValue;
            }
        }


        // Implement INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _ch1SelectedWaveform = "Sine";
        public string Ch1SelectedWaveform
        {
            get { return _ch1SelectedWaveform; }
            set
            {
                if (_ch1SelectedWaveform != value)
                {
                    _ch1SelectedWaveform = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Ch1SelectedWaveform)));
                }
            }
        }

        private string _ch2SelectedWaveform = "Sine";
        public string Ch2SelectedWaveform
        {
            get { return _ch2SelectedWaveform; }
            set
            {
                if (_ch2SelectedWaveform != value)
                {
                    _ch2SelectedWaveform = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Ch2SelectedWaveform)));
                }
            }
        }


        public MainWindow()
        {
            InitializeComponent();

            DataContext = this; // Add this line    
            // Initialize RigolDG2072 class
            rigolDG2072 = new RigolDG2072();
            rigolDG2072.LogEvent += (s, message) => LogMessage(message);

            // Set the RigolDG2072 instance to the WaveformFunctionTab control
            WaveformFunctionTabControl.InstrumentController = rigolDG2072;

            // Initialize ComboBoxes for basic controls
            Ch1WaveformComboBox.SelectedIndex = 0;
            Ch2WaveformComboBox.SelectedIndex = 0;

            Ch1AdvancedWaveformComboBox.SelectedIndex = 0;
            Ch2AdvancedWaveformComboBox.SelectedIndex = 0;

            // Initialize frequency unit ComboBoxes
            Ch1FrequencyUnitComboBox.SelectedIndex = 2; // Hz by default
            Ch2FrequencyUnitComboBox.SelectedIndex = 2; // Hz by default

            // Initialize voltage unit ComboBoxes
            Ch1AmplitudeUnitComboBox.SelectedIndex = 0; // V by default
            Ch2AmplitudeUnitComboBox.SelectedIndex = 0; // V by default
            Ch1OffsetUnitComboBox.SelectedIndex = 0;    // V by default
            Ch2OffsetUnitComboBox.SelectedIndex = 0;    // V by default

            // Initialize time unit ComboBoxes for Pulse parameters
            Ch1PulsePeriodUnitComboBox.SelectedIndex = 3;     // ms by default
            Ch1PulseWidthUnitComboBox.SelectedIndex = 3;      // ms by default
            Ch1PulseRiseTimeUnitComboBox.SelectedIndex = 5;   // ns by default
            Ch1PulseFallTimeUnitComboBox.SelectedIndex = 5;   // ns by default

            Ch2PulsePeriodUnitComboBox.SelectedIndex = 3;     // ms by default
            Ch2PulseWidthUnitComboBox.SelectedIndex = 3;      // ms by default
            Ch2PulseRiseTimeUnitComboBox.SelectedIndex = 5;   // ns by default
            Ch2PulseFallTimeUnitComboBox.SelectedIndex = 5;   // ns by defaul




        }

        // Update this method to set the stored base frequency value when retrieving from the device
        private void UpdateFrequencyDisplay(int channel)
        {
            try
            {
                double deviceFrequency = rigolDG2072.GetFrequency(channel);

                if (channel == 1)
                {
                    // Store the base frequency from the device
                    _ch1FrequencyInHz = deviceFrequency;

                    string selectedUnit = ((ComboBoxItem)Ch1FrequencyUnitComboBox.SelectedItem).Content.ToString();
                    double displayFrequency = ConvertFrequencyFromBaseUnits(deviceFrequency, selectedUnit);
                    Ch1FrequencyTextBox.Text = displayFrequency.ToString("F3");
                }
                else if (channel == 2)
                {
                    // Store the base frequency from the device
                    _ch2FrequencyInHz = deviceFrequency;

                    string selectedUnit = ((ComboBoxItem)Ch2FrequencyUnitComboBox.SelectedItem).Content.ToString();
                    double displayFrequency = ConvertFrequencyFromBaseUnits(deviceFrequency, selectedUnit);
                    Ch2FrequencyTextBox.Text = displayFrequency.ToString("F3");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error updating frequency display: {ex.Message}");
            }
        }

        private void UpdateAmplitudeDisplay(int channel)
        {
            try
            {
                double deviceAmplitude = rigolDG2072.GetAmplitude(channel);

                if (channel == 1)
                {
                    _ch1AmplitudeInVolts = deviceAmplitude;
                    string selectedUnit = ((ComboBoxItem)Ch1AmplitudeUnitComboBox.SelectedItem).Content.ToString();
                    double displayAmplitude = ConvertVoltageFromBaseUnits(deviceAmplitude, selectedUnit);
                    Ch1AmplitudeTextBox.Text = displayAmplitude.ToString("F3");
                }
                else if (channel == 2)
                {
                    _ch2AmplitudeInVolts = deviceAmplitude;
                    string selectedUnit = ((ComboBoxItem)Ch2AmplitudeUnitComboBox.SelectedItem).Content.ToString();
                    double displayAmplitude = ConvertVoltageFromBaseUnits(deviceAmplitude, selectedUnit);
                    Ch2AmplitudeTextBox.Text = displayAmplitude.ToString("F3");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error updating amplitude display: {ex.Message}");
            }
        }

        private void UpdateOffsetDisplay(int channel)
        {
            try
            {
                double deviceOffset = rigolDG2072.GetOffset(channel);

                if (channel == 1)
                {
                    _ch1OffsetInVolts = deviceOffset;
                    string selectedUnit = ((ComboBoxItem)Ch1OffsetUnitComboBox.SelectedItem).Content.ToString();
                    double displayOffset = ConvertVoltageFromBaseUnits(deviceOffset, selectedUnit);
                    Ch1OffsetTextBox.Text = displayOffset.ToString("F3");
                }
                else if (channel == 2)
                {
                    _ch2OffsetInVolts = deviceOffset;
                    string selectedUnit = ((ComboBoxItem)Ch2OffsetUnitComboBox.SelectedItem).Content.ToString();
                    double displayOffset = ConvertVoltageFromBaseUnits(deviceOffset, selectedUnit);
                    Ch2OffsetTextBox.Text = displayOffset.ToString("F3");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error updating offset display: {ex.Message}");
            }
        }




        //-------------------------------------------------------------------------------------
        #region VISA P/Invoke Declarations

        [DllImport("visa32.dll")]
        private static extern int viOpenDefaultRM(out IntPtr sesn);

        [DllImport("visa32.dll")]
        private static extern int viOpen(IntPtr sesn, string rsrcName, int accessMode, int openTimeout, out IntPtr vi);

        [DllImport("visa32.dll")]
        private static extern int viClose(IntPtr vi);

        [DllImport("visa32.dll")]
        private static extern int viWrite(IntPtr vi, byte[] buf, int count, out int retCount);

        [DllImport("visa32.dll")]
        private static extern int viRead(IntPtr vi, byte[] buf, int count, out int retCount);

        [DllImport("visa32.dll")]
        private static extern int viGpibControlREN(IntPtr vi, int mode);

        #endregion


        //-------------------------------------------------------------------------------------
        #region VISA Communication Methods



        private void ConnectionToggle_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectionToggle.IsChecked == true)
            {
                // Connect
                if (Connect())
                {
                    ConnectionStatusTextBlock.Text = "Connected";
                    ConnectionStatusTextBlock.Foreground = System.Windows.Media.Brushes.Green;
                    ConnectionToggle.Content = "Disconnect";
                    IdentifyButton.IsEnabled = true;


                    // Short delay to ensure connection is fully established
                    

                    // Enable channel output toggles
                    Ch1OutputToggle.IsEnabled = true;
                    Ch2OutputToggle.IsEnabled = true;

                    // Update channel output toggle states
                    UpdateChannelOutputToggleStates();

                    // Update displays with current device values
                    UpdateFrequencyDisplay(1);
                    UpdateFrequencyDisplay(2);
                    UpdateAmplitudeDisplay(1);
                    UpdateAmplitudeDisplay(2);
                    UpdateOffsetDisplay(1);
                    UpdateOffsetDisplay(2);
                }
                else
                {
                    // Connection failed, revert toggle state
                    ConnectionToggle.IsChecked = false;
                }
            }
            else
            {
                // Disconnect
                if (Disconnect())
                {
                    ConnectionStatusTextBlock.Text = "Disconnected";
                    ConnectionStatusTextBlock.Foreground = System.Windows.Media.Brushes.Red;
                    ConnectionToggle.Content = "Connect";
                    IdentifyButton.IsEnabled = false;

                    // Disable channel output toggles
                    Ch1OutputToggle.IsEnabled = false;
                    Ch2OutputToggle.IsEnabled = false;
                }
                else
                {
                    // Disconnection failed, revert toggle state
                    ConnectionToggle.IsChecked = true;
                }
            }
        }


        private bool Connect()
        {
            try
            {
                bool result = rigolDG2072.Connect();
                if (result)
                {
                    isConnected = true;
                    LogMessage("Connected to Rigol DG2072");
                }
                return result;
            }
            catch (Exception ex)
            {
                LogMessage($"Connection error: {ex.Message}");
                return false;
            }
        }

        private bool Disconnect()
        {
            try
            {
                bool result = rigolDG2072.Disconnect();
                if (result)
                {
                    isConnected = false;
                    LogMessage("Disconnected from Rigol DG2072");
                }
                return result;
            }
            catch (Exception ex)
            {
                LogMessage($"Disconnection error: {ex.Message}");
                return false;
            }
        }

        private string SendCommand(string command)
        {
            if (!isConnected || instrumentHandle == IntPtr.Zero)
            {
                LogMessage("Not connected to the instrument.");
                return string.Empty;
            }

            try
            {
                // Send command
                byte[] buffer = Encoding.ASCII.GetBytes(command);
                int retCount = 0;
                int status = viWrite(instrumentHandle, buffer, buffer.Length, out retCount);

                if (status != VI_SUCCESS)
                {
                    LogMessage($"Failed to write to the instrument. Error code: {status}");
                    return string.Empty;
                }

                LogMessage($"Command sent: {command}");
                return string.Empty; // No response expected for most commands
            }
            catch (Exception ex)
            {
                LogMessage($"Send command error: {ex.Message}");
                return string.Empty;
            }
        }

        private string SendQuery(string query)
        {
            if (!isConnected || instrumentHandle == IntPtr.Zero)
            {
                LogMessage("Not connected to the instrument.");
                return string.Empty;
            }

            try
            {
                // Send query
                byte[] writeBuffer = Encoding.ASCII.GetBytes(query);
                int retWriteCount = 0;
                int status = viWrite(instrumentHandle, writeBuffer, writeBuffer.Length, out retWriteCount);

                if (status != VI_SUCCESS)
                {
                    LogMessage($"Failed to write query to the instrument. Error code: {status}");
                    return string.Empty;
                }

                // Read response
                byte[] readBuffer = new byte[1024];
                int retReadCount = 0;
                status = viRead(instrumentHandle, readBuffer, readBuffer.Length, out retReadCount);

                if (status != VI_SUCCESS)
                {
                    LogMessage($"Failed to read from the instrument. Error code: {status}");
                    return string.Empty;
                }

                string response = Encoding.ASCII.GetString(readBuffer, 0, retReadCount).Trim();
                LogMessage($"Query: {query}, Response: {response}");
                return response;
            }
            catch (Exception ex)
            {
                LogMessage($"Query error: {ex.Message}");
                return string.Empty;
            }
        }

        public void LogMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                CommandLogTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
                CommandLogTextBox.ScrollToEnd();
            });
        }

        #endregion


        //-------------------------------------------------------------------------------------
        #region Unit Conversion Methods

        private double ConvertFrequencyToBaseUnits(double value, string unit)
        {
            double result;
            switch (unit)
            {
                case "Hz": result = value; break;
                case "kHz": result = value * 1000; break;
                case "MHz": result = value * 1000000; break;
                case "mHz": result = value / 1000; break;
                case "µHz": result = value / 1000000; break;
                default: result = value; break;
            }
            LogMessage($"Converting {value} {unit} to {result} Hz");
            return result;
        }


        private double ConvertFrequencyFromBaseUnits(double baseValue, string targetUnit)
        {
            switch (targetUnit)
            {
                case "Hz": return baseValue;
                case "kHz": return baseValue / 1000;
                case "MHz": return baseValue / 1000000;
                case "mHz": return baseValue * 1000;
                case "µHz": return baseValue * 1000000;
                default: return baseValue;
            }
        }

        // Method to suggest a better unit based on frequency value
        private string SuggestBestFrequencyUnit(double frequencyInHz)
        {
            if (frequencyInHz >= 1000000) return "MHz";
            if (frequencyInHz >= 1000) return "kHz";
            if (frequencyInHz >= 1) return "Hz";
            if (frequencyInHz >= 0.001) return "mHz";
            return "µHz";
        }
        
        // Method to automatically select the most appropriate unit (optional enhancement)
        private void AutoSelectBestFrequencyUnit(int channel, double frequencyInHz)
        {
            string bestUnit = SuggestBestFrequencyUnit(frequencyInHz);

            if (channel == 1)
            {
                foreach (ComboBoxItem item in Ch1FrequencyUnitComboBox.Items)
                {
                    if (item.Content.ToString() == bestUnit)
                    {
                        Ch1FrequencyUnitComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
            else if (channel == 2)
            {
                foreach (ComboBoxItem item in Ch2FrequencyUnitComboBox.Items)
                {
                    if (item.Content.ToString() == bestUnit)
                    {
                        Ch2FrequencyUnitComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private double ConvertTimeToBaseUnits(double value, string unit)
        {
            double result;
            switch (unit)
            {
                case "Ms": result = value * 1000000; break;  // Megaseconds
                case "ks": result = value * 1000; break;     // Kiloseconds
                case "s": result = value; break;             // Seconds (base unit)
                case "ms": result = value / 1000; break;     // Milliseconds
                case "µs": result = value / 1000000; break;  // Microseconds
                case "ns": result = value / 1000000000; break; // Nanoseconds
                default: result = value; break;
            }
            LogMessage($"Converting {value} {unit} to {result} s");
            return result;
        }

        private double ConvertTimeFromBaseUnits(double baseValue, string targetUnit)
        {
            switch (targetUnit)
            {
                case "Ms": return baseValue / 1000000;       // Megaseconds
                case "ks": return baseValue / 1000;          // Kiloseconds
                case "s": return baseValue;                  // Seconds (base unit)
                case "ms": return baseValue * 1000;          // Milliseconds
                case "µs": return baseValue * 1000000;       // Microseconds
                case "ns": return baseValue * 1000000000;    // Nanoseconds
                default: return baseValue;
            }
        }

        // Method to suggest best time unit based on the time value
        private string SuggestBestTimeUnit(double timeInSeconds)
        {
            if (timeInSeconds >= 1000000) return "Ms";
            if (timeInSeconds >= 1000) return "ks";
            if (timeInSeconds >= 1) return "s";
            if (timeInSeconds >= 0.001) return "ms";
            if (timeInSeconds >= 0.000001) return "µs";
            return "ns";
        }

        // Method to automatically select the most appropriate unit
        private void AutoSelectBestTimeUnit(ComboBox unitComboBox, double timeInSeconds)
        {
            string bestUnit = SuggestBestTimeUnit(timeInSeconds);

            foreach (ComboBoxItem item in unitComboBox.Items)
            {
                if (item.Content.ToString() == bestUnit)
                {
                    unitComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        //
        // Add to MainWindow.xaml.cs in a suitable region, perhaps in a #region unit conversion handlers section

        private void Ch1PulsePeriodUnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ch1PulsePeriodTextBox == null || e.AddedItems.Count == 0) return;

            if (double.TryParse(Ch1PulsePeriodTextBox.Text, out double displayedValue))
            {
                string newUnit = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();
                Ch1PulsePeriodTextBox.Text = ConvertTimeFromBaseUnits(_ch1PulsePeriodInSeconds, newUnit).ToString("G6");
            }
        }

        private void Ch1PulseWidthUnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ch1PulseWidthTextBox == null || e.AddedItems.Count == 0) return;

            if (double.TryParse(Ch1PulseWidthTextBox.Text, out double displayedValue))
            {
                string newUnit = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();
                Ch1PulseWidthTextBox.Text = ConvertTimeFromBaseUnits(_ch1PulseWidthInSeconds, newUnit).ToString("G6");
            }
        }

        private void Ch1PulseRiseTimeUnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ch1PulseRiseTimeTextBox == null || e.AddedItems.Count == 0) return;

            if (double.TryParse(Ch1PulseRiseTimeTextBox.Text, out double displayedValue))
            {
                string newUnit = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();
                Ch1PulseRiseTimeTextBox.Text = ConvertTimeFromBaseUnits(_ch1PulseRiseTimeInSeconds, newUnit).ToString("G6");
            }
        }

        private void Ch1PulseFallTimeUnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ch1PulseFallTimeTextBox == null || e.AddedItems.Count == 0) return;

            if (double.TryParse(Ch1PulseFallTimeTextBox.Text, out double displayedValue))
            {
                string newUnit = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();
                Ch1PulseFallTimeTextBox.Text = ConvertTimeFromBaseUnits(_ch1PulseFallTimeInSeconds, newUnit).ToString("G6");
            }
        }

        private void Ch2PulsePeriodUnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ch2PulsePeriodTextBox == null || e.AddedItems.Count == 0) return;

            if (double.TryParse(Ch2PulsePeriodTextBox.Text, out double displayedValue))
            {
                string newUnit = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();
                Ch2PulsePeriodTextBox.Text = ConvertTimeFromBaseUnits(_ch2PulsePeriodInSeconds, newUnit).ToString("G6");
            }
        }

        private void Ch2PulseWidthUnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ch2PulseWidthTextBox == null || e.AddedItems.Count == 0) return;

            if (double.TryParse(Ch2PulseWidthTextBox.Text, out double displayedValue))
            {
                string newUnit = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();
                Ch2PulseWidthTextBox.Text = ConvertTimeFromBaseUnits(_ch2PulseWidthInSeconds, newUnit).ToString("G6");
            }
        }

        private void Ch2PulseRiseTimeUnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ch2PulseRiseTimeTextBox == null || e.AddedItems.Count == 0) return;

            if (double.TryParse(Ch2PulseRiseTimeTextBox.Text, out double displayedValue))
            {
                string newUnit = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();
                Ch2PulseRiseTimeTextBox.Text = ConvertTimeFromBaseUnits(_ch2PulseRiseTimeInSeconds, newUnit).ToString("G6");
            }
        }

        private void Ch2PulseFallTimeUnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ch2PulseFallTimeTextBox == null || e.AddedItems.Count == 0) return;

            if (double.TryParse(Ch2PulseFallTimeTextBox.Text, out double displayedValue))
            {
                string newUnit = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();
                Ch2PulseFallTimeTextBox.Text = ConvertTimeFromBaseUnits(_ch2PulseFallTimeInSeconds, newUnit).ToString("G6");
            }
        }

        //
        private void Ch1PhaseTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch1PhaseTextBox.Text, out double phase))
            {
                rigolDG2072.SetPhase(1, phase);
                LogMessage($"Set CH1 phase to {phase}°");
            }
            else
            {
                LogMessage("Invalid phase value for CH1");
            }
        }

        private void Ch2AmplitudeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch2AmplitudeTextBox.Text, out double displayedAmplitude))
            {
                string unit = ((ComboBoxItem)Ch2AmplitudeUnitComboBox.SelectedItem).Content.ToString();
                double baseAmplitude = ConvertVoltageToBaseUnits(displayedAmplitude, unit);

                _ch2AmplitudeInVolts = baseAmplitude;
                rigolDG2072.SetAmplitude(2, baseAmplitude);
                LogMessage($"Set CH2 amplitude to {displayedAmplitude} {unit} ({baseAmplitude} V)");
            }
            else
            {
                LogMessage("Invalid amplitude value for CH2");
                string unit = ((ComboBoxItem)Ch2AmplitudeUnitComboBox.SelectedItem).Content.ToString();
                double displayValue = ConvertVoltageFromBaseUnits(_ch2AmplitudeInVolts, unit);
                Ch2AmplitudeTextBox.Text = displayValue.ToString("F3");
            }
        }

        private void Ch2AmplitudeUnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ch2AmplitudeTextBox == null || e.AddedItems.Count == 0 || e.RemovedItems.Count == 0) return;

            if (double.TryParse(Ch2AmplitudeTextBox.Text, out double displayedValue))
            {
                string oldUnit = ((ComboBoxItem)e.RemovedItems[0]).Content.ToString();
                string newUnit = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();

                double newAmplitudeInVolts = ConvertVoltageToBaseUnits(displayedValue, newUnit);
                _ch2AmplitudeInVolts = newAmplitudeInVolts;

                if (isConnected)
                {
                    rigolDG2072.SetAmplitude(2, newAmplitudeInVolts);
                    LogMessage($"Unit change: Set CH2 amplitude to {displayedValue} {newUnit} ({newAmplitudeInVolts} V)");
                }
            }
        }

        private void Ch2OffsetTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch2OffsetTextBox.Text, out double displayedOffset))
            {
                string unit = ((ComboBoxItem)Ch2OffsetUnitComboBox.SelectedItem).Content.ToString();
                double baseOffset = ConvertVoltageToBaseUnits(displayedOffset, unit);

                _ch2OffsetInVolts = baseOffset;
                rigolDG2072.SetOffset(2, baseOffset);
                LogMessage($"Set CH2 offset to {displayedOffset} {unit} ({baseOffset} V)");
            }
            else
            {
                LogMessage("Invalid offset value for CH2");
                string unit = ((ComboBoxItem)Ch2OffsetUnitComboBox.SelectedItem).Content.ToString();
                double displayValue = ConvertVoltageFromBaseUnits(_ch2OffsetInVolts, unit);
                Ch2OffsetTextBox.Text = displayValue.ToString("F3");
            }
        }

        private void Ch2OffsetUnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ch2OffsetTextBox == null || e.AddedItems.Count == 0 || e.RemovedItems.Count == 0) return;

            if (double.TryParse(Ch2OffsetTextBox.Text, out double displayedValue))
            {
                string oldUnit = ((ComboBoxItem)e.RemovedItems[0]).Content.ToString();
                string newUnit = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();

                double newOffsetInVolts = ConvertVoltageToBaseUnits(displayedValue, newUnit);
                _ch2OffsetInVolts = newOffsetInVolts;

                if (isConnected)
                {
                    rigolDG2072.SetOffset(2, newOffsetInVolts);
                    LogMessage($"Unit change: Set CH2 offset to {displayedValue} {newUnit} ({newOffsetInVolts} V)");
                }
            }
        }

        #endregion

        //-------------------------------------------------------------------------------------
        #region Event Handlers - Window and Connection

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Auto-connect on window load
            ConnectionToggle.IsChecked = true;
            ConnectionToggle_Click(ConnectionToggle, e);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Disconnect on window closing
            if (isConnected)
            {
                ConnectionToggle.IsChecked = false;
                ConnectionToggle_Click(ConnectionToggle, new RoutedEventArgs());
            }
        }


        private void IdentifyButton_Click(object sender, RoutedEventArgs e)
        {
            string response = rigolDG2072.GetIdentification();
            if (!string.IsNullOrEmpty(response))
            {
                MessageBox.Show($"Instrument Identification:\n{response}", "Instrument Identification", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        private void ClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            CommandLogTextBox.Clear();
        }

        private void UpdateChannelOutputToggleStates()
        {
            if (!isConnected) return;

            try
            {
                // Update CH1 toggle state
                string ch1State = rigolDG2072.GetOutputState(1);
                bool ch1IsOn = ch1State.ToUpper().Contains("ON");
                Ch1OutputToggle.IsChecked = ch1IsOn;
                Ch1OutputToggle.Content = ch1IsOn ? "ON" : "OFF";

                // Update CH2 toggle state
                string ch2State = rigolDG2072.GetOutputState(2);
                bool ch2IsOn = ch2State.ToUpper().Contains("ON");
                Ch2OutputToggle.IsChecked = ch2IsOn;
                Ch2OutputToggle.Content = ch2IsOn ? "ON" : "OFF";
            }
            catch (Exception ex)
            {
                LogMessage($"Error updating output toggle states: {ex.Message}");
            }
        }






        #endregion

        //-------------------------------------------------------------------------------------
        #region Channel 1 Basic Controls

        private void Ch1OutputToggle_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            try
            {
                bool isOn = Ch1OutputToggle.IsChecked == true;
                rigolDG2072.SetOutput(1, isOn);
                Ch1OutputToggle.Content = isOn ? "ON" : "OFF";
                LogMessage($"Set CH1 output {(isOn ? "ON" : "OFF")}");
            }
            catch (Exception ex)
            {
                LogMessage($"Error toggling CH1 output: {ex.Message}");
            }
        }

        // Update this method to keep track of the base frequency value
        private void Ch1FrequencyTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch1FrequencyTextBox.Text, out double displayedFrequency))
            {
                string unit = ((ComboBoxItem)Ch1FrequencyUnitComboBox.SelectedItem).Content.ToString();
                double baseFrequency = ConvertFrequencyToBaseUnits(displayedFrequency, unit);

                // Store the calculated base frequency
                _ch1FrequencyInHz = baseFrequency;

                // Set the device frequency
                rigolDG2072.SetFrequency(1, baseFrequency);
                LogMessage($"Set CH1 frequency to {displayedFrequency} {unit} ({baseFrequency} Hz)");
            }
            else
            {
                LogMessage("Invalid frequency value for CH1");
                // Revert to the previous valid value
                string unit = ((ComboBoxItem)Ch1FrequencyUnitComboBox.SelectedItem).Content.ToString();
                double displayValue = ConvertFrequencyFromBaseUnits(_ch1FrequencyInHz, unit);
                Ch1FrequencyTextBox.Text = displayValue.ToString("F3");
            }
        }

        // Update the unit selection change handlers to keep the displayed value the same
        // but change the actual frequency

        private void Ch1FrequencyUnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ch1FrequencyTextBox == null || e.AddedItems.Count == 0 || e.RemovedItems.Count == 0) return;

            // Keep the same displayed numeric value
            if (double.TryParse(Ch1FrequencyTextBox.Text, out double displayedValue))
            {
                // Get the old and new units
                string oldUnit = ((ComboBoxItem)e.RemovedItems[0]).Content.ToString();
                string newUnit = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();

                // Calculate the new base frequency in Hz
                // First convert displayed value to Hz using the new unit
                double newFrequencyInHz = ConvertFrequencyToBaseUnits(displayedValue, newUnit);

                // Update the stored frequency in Hz
                _ch1FrequencyInHz = newFrequencyInHz;

                // Set the actual device frequency if connected
                if (isConnected)
                {
                    rigolDG2072.SetFrequency(1, newFrequencyInHz);
                    LogMessage($"Unit change: Set CH1 frequency to {displayedValue} {newUnit} ({newFrequencyInHz} Hz)");
                }
            }
        }


        private void Ch1WaveformComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isConnected || Ch1WaveformComboBox.SelectedItem == null) return;

            ComboBoxItem selectedItem = Ch1WaveformComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string waveform = selectedItem.Content.ToString();

            // Show appropriate parameter controls for the selected waveform
            ShowWaveformSpecificControls(1, waveform);

            // Apply waveform with current parameters
            try
            {
                Dictionary<string, object> parameters = GetChannelParameters(1);
                rigolDG2072.ApplyWaveformWithSpecificParams(1, waveform, parameters);
                LogMessage($"Applied {waveform} to CH1");
            }
            catch (Exception ex)
            {
                LogMessage($"Error applying waveform: {ex.Message}");
            }

            // Update frequency display with current device value
            UpdateFrequencyDisplay(1);
        }

        // Amplitude LostFocus handlers
        private void Ch1AmplitudeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch1AmplitudeTextBox.Text, out double displayedAmplitude))
            {
                string unit = ((ComboBoxItem)Ch1AmplitudeUnitComboBox.SelectedItem).Content.ToString();
                double baseAmplitude = ConvertVoltageToBaseUnits(displayedAmplitude, unit);

                _ch1AmplitudeInVolts = baseAmplitude;
                rigolDG2072.SetAmplitude(1, baseAmplitude);
                LogMessage($"Set CH1 amplitude to {displayedAmplitude} {unit} ({baseAmplitude} V)");
            }
            else
            {
                LogMessage("Invalid amplitude value for CH1");
                string unit = ((ComboBoxItem)Ch1AmplitudeUnitComboBox.SelectedItem).Content.ToString();
                double displayValue = ConvertVoltageFromBaseUnits(_ch1AmplitudeInVolts, unit);
                Ch1AmplitudeTextBox.Text = displayValue.ToString("F3");
            }
        }


        // Offset LostFocus handlers
        private void Ch1OffsetTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch1OffsetTextBox.Text, out double displayedOffset))
            {
                string unit = ((ComboBoxItem)Ch1OffsetUnitComboBox.SelectedItem).Content.ToString();
                double baseOffset = ConvertVoltageToBaseUnits(displayedOffset, unit);

                _ch1OffsetInVolts = baseOffset;
                rigolDG2072.SetOffset(1, baseOffset);
                LogMessage($"Set CH1 offset to {displayedOffset} {unit} ({baseOffset} V)");
            }
            else
            {
                LogMessage("Invalid offset value for CH1");
                string unit = ((ComboBoxItem)Ch1OffsetUnitComboBox.SelectedItem).Content.ToString();
                double displayValue = ConvertVoltageFromBaseUnits(_ch1OffsetInVolts, unit);
                Ch1OffsetTextBox.Text = displayValue.ToString("F3");
            }
        }



        private void Ch1AmplitudeUnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ch1AmplitudeTextBox == null || e.AddedItems.Count == 0 || e.RemovedItems.Count == 0) return;

            if (double.TryParse(Ch1AmplitudeTextBox.Text, out double displayedValue))
            {
                string oldUnit = ((ComboBoxItem)e.RemovedItems[0]).Content.ToString();
                string newUnit = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();

                double newAmplitudeInVolts = ConvertVoltageToBaseUnits(displayedValue, newUnit);
                _ch1AmplitudeInVolts = newAmplitudeInVolts;

                if (isConnected)
                {
                    rigolDG2072.SetAmplitude(1, newAmplitudeInVolts);
                    LogMessage($"Unit change: Set CH1 amplitude to {displayedValue} {newUnit} ({newAmplitudeInVolts} V)");
                }
            }
        }



        private void Ch1OffsetUnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ch1OffsetTextBox == null || e.AddedItems.Count == 0 || e.RemovedItems.Count == 0) return;

            if (double.TryParse(Ch1OffsetTextBox.Text, out double displayedValue))
            {
                string oldUnit = ((ComboBoxItem)e.RemovedItems[0]).Content.ToString();
                string newUnit = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();

                double newOffsetInVolts = ConvertVoltageToBaseUnits(displayedValue, newUnit);
                _ch1OffsetInVolts = newOffsetInVolts;

                if (isConnected)
                {
                    rigolDG2072.SetOffset(1, newOffsetInVolts);
                    LogMessage($"Unit change: Set CH1 offset to {displayedValue} {newUnit} ({newOffsetInVolts} V)");
                }
            }
        }
 






        private void Ch1ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            ComboBoxItem selectedItem = Ch1WaveformComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string waveform = selectedItem.Content.ToString();
            Dictionary<string, object> parameters = GetChannelParameters(1);

            try
            {
                rigolDG2072.ApplyWaveformWithSpecificParams(1, waveform, parameters);
                LogMessage($"Applied {waveform} settings to CH1");
            }
            catch (Exception ex)
            {
                LogMessage($"Error applying CH1 settings: {ex.Message}");
            }

            // Update displays with current device values
            UpdateFrequencyDisplay(1);
        }




        #endregion

        //-------------------------------------------------------------------------------------
        #region Channel 2 Basic Controls

        private void Ch2OutputToggle_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            try
            {
                bool isOn = Ch2OutputToggle.IsChecked == true;
                rigolDG2072.SetOutput(2, isOn);
                Ch2OutputToggle.Content = isOn ? "ON" : "OFF";
                LogMessage($"Set CH2 output {(isOn ? "ON" : "OFF")}");
            }
            catch (Exception ex)
            {
                LogMessage($"Error toggling CH2 output: {ex.Message}");
            }
        }

        private void Ch2WaveformComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isConnected || Ch2WaveformComboBox.SelectedItem == null) return;

            ComboBoxItem selectedItem = Ch2WaveformComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string waveform = selectedItem.Content.ToString();
            Ch2SelectedWaveform = waveform; // Add this line

            // Show appropriate parameter controls for the selected waveform
            ShowWaveformSpecificControls(2, waveform);
            
            // Apply waveform with current parameters
            try
            {
                Dictionary<string, object> parameters = GetChannelParameters(2);
                rigolDG2072.ApplyWaveformWithSpecificParams(2, waveform, parameters);
                LogMessage($"Applied {waveform} to CH2");
            }
            catch (Exception ex)
            {
                LogMessage($"Error applying waveform: {ex.Message}");
            }
            // Update frequency display with current device value
            UpdateFrequencyDisplay(2);

        }

        // Update this method to keep track of the base frequency value
        private void Ch2FrequencyTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch2FrequencyTextBox.Text, out double displayedFrequency))
            {
                string unit = ((ComboBoxItem)Ch2FrequencyUnitComboBox.SelectedItem).Content.ToString();
                double baseFrequency = ConvertFrequencyToBaseUnits(displayedFrequency, unit);

                // Store the calculated base frequency
                _ch2FrequencyInHz = baseFrequency;

                // Set the device frequency
                rigolDG2072.SetFrequency(2, baseFrequency);
                LogMessage($"Set CH2 frequency to {displayedFrequency} {unit} ({baseFrequency} Hz)");
            }
            else
            {
                LogMessage("Invalid frequency value for CH2");
                // Revert to the previous valid value
                string unit = ((ComboBoxItem)Ch2FrequencyUnitComboBox.SelectedItem).Content.ToString();
                double displayValue = ConvertFrequencyFromBaseUnits(_ch2FrequencyInHz, unit);
                Ch2FrequencyTextBox.Text = displayValue.ToString("F3");
            }
        }

        // Update this method to properly handle frequency unit changes without changing the actual frequency

        // Update the unit selection change handlers to keep the displayed value the same
        // but change the actual frequency
        private void Ch2FrequencyUnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Ch2FrequencyTextBox == null || e.AddedItems.Count == 0 || e.RemovedItems.Count == 0) return;

            // Keep the same displayed numeric value
            if (double.TryParse(Ch2FrequencyTextBox.Text, out double displayedValue))
            {
                // Get the old and new units
                string oldUnit = ((ComboBoxItem)e.RemovedItems[0]).Content.ToString();
                string newUnit = ((ComboBoxItem)e.AddedItems[0]).Content.ToString();

                // Calculate the new base frequency in Hz
                // First convert displayed value to Hz using the new unit
                double newFrequencyInHz = ConvertFrequencyToBaseUnits(displayedValue, newUnit);

                // Update the stored frequency in Hz
                _ch2FrequencyInHz = newFrequencyInHz;

                // Set the actual device frequency if connected
                if (isConnected)
                {
                    rigolDG2072.SetFrequency(2, newFrequencyInHz);
                    LogMessage($"Unit change: Set CH2 frequency to {displayedValue} {newUnit} ({newFrequencyInHz} Hz)");
                }
            }
        }


        private void Ch2PhaseTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            if (double.TryParse(Ch2PhaseTextBox.Text, out double phase))
            {
                rigolDG2072.SetPhase(2, phase);
            }
            else
            {
                LogMessage("Invalid phase value for CH2");
            }
        }

        private void Ch2ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected) return;

            ComboBoxItem selectedItem = Ch2WaveformComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string waveform = selectedItem.Content.ToString();
            Dictionary<string, object> parameters = GetChannelParameters(2);

            try
            {
                rigolDG2072.ApplyWaveformWithSpecificParams(2, waveform, parameters);
                LogMessage($"Applied {waveform} settings to CH2");
            }
            catch (Exception ex)
            {
                LogMessage($"Error applying CH2 settings: {ex.Message}");
            }

            // Update displays with current device values
            UpdateFrequencyDisplay(2);
        }





        }

    #endregion
}
