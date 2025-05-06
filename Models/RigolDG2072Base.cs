using MathNet.Numerics;
using System;
using System.Collections.Generic;

namespace DG2072_USB_Control
{
    public partial class RigolDG2072 : ILogProvider
    {
        private readonly VisaManager visaManager;
        private const string DefaultResourceName = "USB0::0x1AB1::0x0644::DG2P224100508::INSTR";

        public event EventHandler<string> LogEvent;

        public void Log(string message)
        {
            // Raise the LogEvent to notify subscribers
            LogEvent?.Invoke(this, message);
        }



        public RigolDG2072()
        {
            visaManager = new VisaManager();
            visaManager.LogEvent += (s, message) => LogEvent?.Invoke(this, message);
        }

        #region Connection Management

        public bool Connect(string resourceName = DefaultResourceName)
        {
            return visaManager.Connect(resourceName);
        }

        public bool Disconnect()
        {
            return visaManager.Disconnect();
        }

        public bool IsConnected => visaManager.IsConnected;

        public string GetIdentification()
        {
            return visaManager.SendQuery("*IDN?");
        }

        public List<string> FindResources()
        {
            return visaManager.FindResources();
        }

        #endregion

        #region Basic Channel Control

        public void SetOutput(int channel, bool state)
        {
            ValidateChannel(channel);
            visaManager.SendCommand($"OUTPUT{channel} {(state ? "ON" : "OFF")}");
        }

        public string GetOutputState(int channel)
        {
            ValidateChannel(channel);
            return visaManager.SendQuery($"OUTPUT{channel}?");
        }

        public void SetWaveform(int channel, string waveform)
        {
            ValidateChannel(channel);
            string waveformAbbrev = GetWaveformAbbreviation(waveform);
            string command = $"SOUR{channel}:APPL:{waveformAbbrev}";
            //// For SINE specifically, it should be SINusoid in the command
            //string waveformCommand = waveform.ToUpper();
            //if (waveformCommand == "SINE")
            //    waveformCommand = "SINusoid";

            // Get current parameters to include in the command
            double frequency = GetFrequency(channel);
            double amplitude = GetAmplitude(channel);
            double offset = GetOffset(channel);
            double phase = GetPhase(channel);

            // Apply with full parameter set
            visaManager.SendCommand($"SOURce{channel}:APPLy:{waveformAbbrev} {frequency},{amplitude},{offset},{phase}");

            // Log exactly what was sent for debugging
            Log($"Sent full waveform command: SOURce{channel}:APPLy:{waveformAbbrev} {frequency},{amplitude},{offset},{phase}");
        }

        public void SetFrequency(int channel, double frequency)
        {
            ValidateChannel(channel);
            visaManager.SendCommand($"SOURCE{channel}:FREQUENCY {frequency}");
        }

        public double GetFrequency(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:FREQUENCY?");
            if (double.TryParse(response, out double frequency))
            {
                return frequency;
            }
            return 0.0;
        }

        public void SetAmplitude(int channel, double amplitude)
        {
            ValidateChannel(channel);
            visaManager.SendCommand($"SOURCE{channel}:VOLTAGE {amplitude}");
        }

        public double GetAmplitude(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:VOLTAGE?");
            if (double.TryParse(response, out double amplitude))
            {
                return amplitude;
            }
            return 0.0;
        }

        public void SetOffset(int channel, double offset)
        {
            ValidateChannel(channel);
            visaManager.SendCommand($"SOURCE{channel}:VOLTAGE:OFFSET {offset}");
        }

        public double GetOffset(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:VOLTAGE:OFFSET?");
            if (double.TryParse(response, out double offset))
            {
                return offset;
            }
            return 0.0;
        }

        public void SetPhase(int channel, double phase)
        {
            ValidateChannel(channel);
            visaManager.SendCommand($"SOURCE{channel}:PHASE {phase}");
        }

        public double GetPhase(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:PHASE?");
            if (double.TryParse(response, out double phase))
            {
                return phase;
            }
            return 0.0;
        }

        public void ApplyWaveform(int channel, string waveform, double frequency, double amplitude, double offset, double phase)
        {
            ValidateChannel(channel);

            // Convert waveform name to abbreviated version
            string waveformAbbrev = GetWaveformAbbreviation(waveform);

            // Format command with proper abbreviations
            string command;

            // Special case for NOISE which only uses amplitude and offset parameters
            if (waveform.ToUpper() == "NOISE")
            {
                command = $"SOUR{channel}:APPL:{waveformAbbrev} {amplitude},{offset}";
            }
            else
            {
                command = $"SOUR{channel}:APPL:{waveformAbbrev} {frequency},{amplitude},{offset},{phase}";
            }

            visaManager.SendCommand(command);
        }


        #endregion

        #region Harmonic Control

        public void SetHarmonicState(int channel, bool state)
        {
            ValidateChannel(channel);
            visaManager.SendCommand($"SOURCE{channel}:HARMONIC:STATE {(state ? "ON" : "OFF")}");
        }

        public bool GetHarmonicState(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:HARMONIC:STATE?");
            return response.Trim().ToUpper() == "ON";
        }

        public void SetHarmonicType(int channel, string type)
        {
            ValidateChannel(channel);
            if (!IsValidHarmonicType(type))
            {
                throw new ArgumentException($"Invalid harmonic type: {type}. Must be EVEN, ODD, ALL, or USER.");
            }
            visaManager.SendCommand($"SOURCE{channel}:HARMONIC:TYPE {type.ToUpper()}");
        }

        public string GetHarmonicType(int channel)
        {
            ValidateChannel(channel);
            return visaManager.SendQuery($"SOURCE{channel}:HARMONIC:TYPE?");
        }

        public void SetHarmonicOrder(int channel, int order)
        {
            ValidateChannel(channel);
            if (order < 2 || order > 8)
            {
                throw new ArgumentException("Harmonic order must be between 2 and 8.");
            }
            visaManager.SendCommand($"SOURCE{channel}:HARMONIC:ORDER {order}");
        }

        public int GetHarmonicOrder(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:HARMONIC:ORDER?");
            if (double.TryParse(response, out double order))
            {
                return (int)order;
            }
            return 0;
        }

        public void SetHarmonicAmplitude(int channel, int harmonicNumber, double amplitude)
        {
            ValidateChannel(channel);
            if (harmonicNumber < 2 || harmonicNumber > 8)
            {
                throw new ArgumentException("Harmonic number must be between 2 and 8.");
            }
            visaManager.SendCommand($"SOURCE{channel}:HARMONIC:AMPL {harmonicNumber},{amplitude}");
        }

        public double GetHarmonicAmplitude(int channel, int harmonicNumber)
        {
            ValidateChannel(channel);
            if (harmonicNumber < 2 || harmonicNumber > 8)
            {
                throw new ArgumentException("Harmonic number must be between 2 and 8.");
            }
            string response = visaManager.SendQuery($"SOURCE{channel}:HARMONIC:AMPL? {harmonicNumber}");
            if (double.TryParse(response, out double amplitude))
            {
                return amplitude;
            }
            return 0.0;
        }

        public void SetHarmonicPhase(int channel, int harmonicNumber, double phase)
        {
            ValidateChannel(channel);
            if (harmonicNumber < 2 || harmonicNumber > 8)
            {
                throw new ArgumentException("Harmonic number must be between 2 and 8.");
            }
            visaManager.SendCommand($"SOURCE{channel}:HARMONIC:PHASE {harmonicNumber},{phase}");
        }

        public double GetHarmonicPhase(int channel, int harmonicNumber)
        {
            ValidateChannel(channel);
            if (harmonicNumber < 2 || harmonicNumber > 8)
            {
                throw new ArgumentException("Harmonic number must be between 2 and 8.");
            }
            string response = visaManager.SendQuery($"SOURCE{channel}:HARMONIC:PHASE? {harmonicNumber}");
            if (double.TryParse(response, out double phase))
            {
                return phase;
            }
            return 0.0;
        }

        public void SetUserDefinedHarmonics(int channel, bool[] enabledHarmonics)
        {
            ValidateChannel(channel);
            if (enabledHarmonics.Length != 7)
            {
                throw new ArgumentException("Enabled harmonics array must have exactly 7 elements (for harmonics 2-8).");
            }

            char[] userHarmonics = new char[8];
            userHarmonics[0] = 'X'; // Fundamental waveform is always enabled

            for (int i = 0; i < 7; i++)
            {
                userHarmonics[i + 1] = enabledHarmonics[i] ? '1' : '0';
            }

            string userHarmonicString = new string(userHarmonics);
            visaManager.SendCommand($"SOURCE{channel}:HARMONIC:USER {userHarmonicString}");
        }

        public string GetUserDefinedHarmonics(int channel)
        {
            ValidateChannel(channel);
            return visaManager.SendQuery($"SOURCE{channel}:HARMONIC:USER?");
        }

        #endregion

        #region Utility Functions

        // Generate a harmonic series with custom amplitudes using MathNet.Numerics
        public double[] GenerateHarmonicSeries(double fundamental, int maxHarmonic, double[] amplitudes, double[] phases)
        {
            if (amplitudes.Length != maxHarmonic || phases.Length != maxHarmonic)
            {
                throw new ArgumentException("Amplitudes and phases arrays must have the same length as maxHarmonic.");
            }

            const int samplesPerPeriod = 1000;
            double[] result = new double[samplesPerPeriod];
            double[] time = Generate.LinearSpaced(samplesPerPeriod, 0, 1);

            // Add fundamental
            for (int i = 0; i < samplesPerPeriod; i++)
            {
                result[i] = amplitudes[0] * Math.Sin(2 * Math.PI * time[i] + phases[0] * Math.PI / 180);
            }

            // Add harmonics
            for (int h = 1; h < maxHarmonic; h++)
            {
                for (int i = 0; i < samplesPerPeriod; i++)
                {
                    result[i] += amplitudes[h] * Math.Sin(2 * Math.PI * (h + 1) * time[i] + phases[h] * Math.PI / 180);
                }
            }

            return result;
        }

        protected void ValidateChannel(int channel)
        {
            if (channel < 1 || channel > 2)
            {
                throw new ArgumentException("Channel must be 1 or 2.");
            }
        }

        private bool IsValidHarmonicType(string type)
        {
            string upperType = type.ToUpper();
            return upperType == "EVEN" || upperType == "ODD" || upperType == "ALL" || upperType == "USER";
        }

        // Add this method at the end of the Utility Functions region
        private string GetWaveformAbbreviation(string waveform)
        {
            switch (waveform.ToUpper())
            {
                case "SINE":
                    return "SIN";
                case "SQUARE":
                    return "SQU";
                case "RAMP":
                    return "RAMP";
                case "PULSE":
                    return "PULSe";
                case "NOISE":
                    return "NOIS";
                case "HARMONIC":
                    return "HARM";
                default:
                    return waveform.ToUpper();
            }
        }

        #endregion
    }
}