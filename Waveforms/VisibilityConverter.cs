using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DG2072_USB_Control.Waveforms
{
    public class VisibilityConverter : IValueConverter
    {
        public object ParameterType { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Visibility.Collapsed;

            string waveformType = value.ToString().ToUpper();
            string controlType = parameter.ToString().ToUpper();

            // Define which controls should be visible for which waveform types
            switch (controlType)
            {
                case "FREQUENCY":
                    return (waveformType == "DC" || waveformType == "NOISE" ||
                            waveformType == "RS232")
                           ? Visibility.Collapsed : Visibility.Visible;

                case "PHASE":
                    return (waveformType == "DC" || waveformType == "NOISE" ||
                            waveformType == "RS232" || waveformType == "PRBS")
                           ? Visibility.Collapsed : Visibility.Visible;

                case "RAMP":
                    return waveformType == "RAMP" ? Visibility.Visible : Visibility.Collapsed;

                case "SQUARE":
                    return waveformType == "SQUARE" ? Visibility.Visible : Visibility.Collapsed;

                case "PULSE":
                    return waveformType == "PULSE" ? Visibility.Visible : Visibility.Collapsed;

                case "DUALTONE":
                    return waveformType == "DUAL-TONE" ? Visibility.Visible : Visibility.Collapsed;

                case "PRBS":
                    return waveformType == "PRBS" ? Visibility.Visible : Visibility.Collapsed;

                case "RS232":
                    return waveformType == "RS232" ? Visibility.Visible : Visibility.Collapsed;

                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}