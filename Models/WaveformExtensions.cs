using System;
using System.Collections.Generic;

namespace DG2072_USB_Control
{
    // New partial class to extend the RigolDG2072 with waveform-specific methods
    public partial class RigolDG2072
    {
        // Dictionary to map waveform names to their command abbreviations
        private readonly Dictionary<string, string> waveformToCommandMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Sine", "SINusoid" },
            { "Square", "SQUare" },
            { "Ramp", "RAMP" },
            { "Pulse", "PULSe" },
            { "Noise", "NOISe" },
            { "DC", "DC" },
            { "Dual-Tone", "DUALTone" },
            { "Harmonic", "HARMonic" },
            { "PRBS", "PRBS" },
            { "RS232", "RS232" },
            { "User", "USER" }
        };

        #region Waveform Application Methods

        /// <summary>
        /// Apply a waveform with its specific parameters
        /// </summary>
        public void ApplyWaveformWithSpecificParams(int channel, string waveform, Dictionary<string, object> parameters)
        {
            ValidateChannel(channel);

            if (!waveformToCommandMap.TryGetValue(waveform, out string waveformCommand))
            {
                throw new ArgumentException($"Unsupported waveform: {waveform}");
            }

            // Different waveforms have different parameter requirements
            switch (waveform.ToUpper())
            {
                case "SINE":
                    ApplySineWaveform(channel, parameters);
                    break;
                case "SQUARE":
                    ApplySquareWaveform(channel, parameters);
                    break;
                case "RAMP":
                    ApplyRampWaveform(channel, parameters);
                    break;
                case "PULSE":
                    ApplyPulseWaveform(channel, parameters);
                    break;
                case "NOISE":
                    ApplyNoiseWaveform(channel, parameters);
                    break;
                case "DC":
                    ApplyDCWaveform(channel, parameters);
                    break;
                case "DUAL-TONE":
                    ApplyDualToneWaveform(channel, parameters);
                    break;
                case "HARMONIC":
                    ApplyHarmonicWaveform(channel, parameters);
                    break;
                case "PRBS":
                    ApplyPRBSWaveform(channel, parameters);
                    break;
                case "RS232":
                    ApplyRS232Waveform(channel, parameters);
                    break;
                case "USER":
                    ApplyUserWaveform(channel, parameters);
                    break;
                default:
                    throw new ArgumentException($"Unsupported waveform: {waveform}");
            }
        }

        private void ApplySineWaveform(int channel, Dictionary<string, object> parameters)
        {
            // Extract parameters with defaults
            double frequency = parameters.TryGetValue("Frequency", out object freqObj) ? Convert.ToDouble(freqObj) : 1000.0;
            double amplitude = parameters.TryGetValue("Amplitude", out object ampObj) ? Convert.ToDouble(ampObj) : 1.0;
            double offset = parameters.TryGetValue("Offset", out object offsetObj) ? Convert.ToDouble(offsetObj) : 0.0;
            double phase = parameters.TryGetValue("Phase", out object phaseObj) ? Convert.ToDouble(phaseObj) : 0.0;

            // Apply the sine waveform
            string command = $"SOURce{channel}:APPLy:SINusoid {frequency},{amplitude},{offset},{phase}";
            visaManager.SendCommand(command);
            Log($"Applied SINE to CH{channel} with Freq={frequency}Hz, Amp={amplitude}Vpp, Offset={offset}V, Phase={phase}°");
        }

        private void ApplySquareWaveform(int channel, Dictionary<string, object> parameters)
        {
            // Extract parameters with defaults
            double frequency = parameters.TryGetValue("Frequency", out object freqObj) ? Convert.ToDouble(freqObj) : 1000.0;
            double amplitude = parameters.TryGetValue("Amplitude", out object ampObj) ? Convert.ToDouble(ampObj) : 1.0;
            double offset = parameters.TryGetValue("Offset", out object offsetObj) ? Convert.ToDouble(offsetObj) : 0.0;
            double phase = parameters.TryGetValue("Phase", out object phaseObj) ? Convert.ToDouble(phaseObj) : 0.0;
            double dutyCycle = parameters.TryGetValue("DutyCycle", out object dutyObj) ? Convert.ToDouble(dutyObj) : 50.0;

            // Apply the square waveform
            string command = $"SOURce{channel}:APPLy:SQUare {frequency},{amplitude},{offset},{phase}";
            visaManager.SendCommand(command);

            // Set duty cycle after waveform is applied
            if (parameters.ContainsKey("DutyCycle"))
            {
                SetSquareDutyCycle(channel, dutyCycle);
            }

            Log($"Applied SQUARE to CH{channel} with Freq={frequency}Hz, Amp={amplitude}Vpp, Offset={offset}V, Phase={phase}°, Duty={dutyCycle}%");
        }

        private void ApplyRampWaveform(int channel, Dictionary<string, object> parameters)
        {
            // Extract parameters with defaults
            double frequency = parameters.TryGetValue("Frequency", out object freqObj) ? Convert.ToDouble(freqObj) : 1000.0;
            double amplitude = parameters.TryGetValue("Amplitude", out object ampObj) ? Convert.ToDouble(ampObj) : 1.0;
            double offset = parameters.TryGetValue("Offset", out object offsetObj) ? Convert.ToDouble(offsetObj) : 0.0;
            double phase = parameters.TryGetValue("Phase", out object phaseObj) ? Convert.ToDouble(phaseObj) : 0.0;
            double symmetry = parameters.TryGetValue("Symmetry", out object symObj) ? Convert.ToDouble(symObj) : 50.0;

            // Apply the ramp waveform
            string command = $"SOURce{channel}:APPLy:RAMP {frequency},{amplitude},{offset},{phase}";
            visaManager.SendCommand(command);

            // Set symmetry after waveform is applied
            if (parameters.ContainsKey("Symmetry"))
            {
                SetRampSymmetry(channel, symmetry);
            }

            Log($"Applied RAMP to CH{channel} with Freq={frequency}Hz, Amp={amplitude}Vpp, Offset={offset}V, Phase={phase}°, Symmetry={symmetry}%");
        }
       
        private void ApplyPulseWaveform(int channel, Dictionary<string, object> parameters)
        {
            // Extract parameters with defaults
            double frequency = parameters.TryGetValue("Frequency", out object freqObj) ? Convert.ToDouble(freqObj) : 1000.0;
            double amplitude = parameters.TryGetValue("Amplitude", out object ampObj) ? Convert.ToDouble(ampObj) : 1.0;
            double offset = parameters.TryGetValue("Offset", out object offsetObj) ? Convert.ToDouble(offsetObj) : 0.0;
            double phase = parameters.TryGetValue("Phase", out object phaseObj) ? Convert.ToDouble(phaseObj) : 0.0;

            // Apply the pulse waveform
            string command = $"SOURce{channel}:APPLy:PULSe {frequency},{amplitude},{offset},{phase}";
            visaManager.SendCommand(command);

            // Set additional pulse-specific parameters after waveform is applied
            if (parameters.TryGetValue("Period", out object periodObj))
            {
                double period = Convert.ToDouble(periodObj);
                SetPulsePeriod(channel, period);
            }

            if (parameters.TryGetValue("Width", out object widthObj))
            {
                double width = Convert.ToDouble(widthObj);
                SetPulseWidth(channel, width);
            }

            if (parameters.TryGetValue("RiseTime", out object riseObj))
            {
                double riseTime = Convert.ToDouble(riseObj);
                SetPulseRiseTime(channel, riseTime);
            }

            if (parameters.TryGetValue("FallTime", out object fallObj))
            {
                double fallTime = Convert.ToDouble(fallObj);
                SetPulseFallTime(channel, fallTime);
            }

            // Note: We don't need to handle duty cycle for pulse waveforms
            // as they are fully defined by period and width

            Log($"Applied PULSE to CH{channel} with Freq={frequency}Hz, Amp={amplitude}Vpp, Offset={offset}V, Phase={phase}°");
        }


        private void ApplyNoiseWaveform(int channel, Dictionary<string, object> parameters)
        {
            // Extract parameters with defaults (Note: Noise does not use frequency or phase)
            double amplitude = parameters.TryGetValue("Amplitude", out object ampObj) ? Convert.ToDouble(ampObj) : 1.0;
            double offset = parameters.TryGetValue("Offset", out object offsetObj) ? Convert.ToDouble(offsetObj) : 0.0;

            // Apply the noise waveform
            string command = $"SOURce{channel}:APPLy:NOISe {amplitude},{offset}";
            visaManager.SendCommand(command);
            Log($"Applied NOISE to CH{channel} with Amp={amplitude}Vpp, Offset={offset}V");
        }

        private void ApplyDCWaveform(int channel, Dictionary<string, object> parameters)
        {
            // Extract parameters with defaults (Note: DC only uses offset)
            double offset = parameters.TryGetValue("Offset", out object offsetObj) ? Convert.ToDouble(offsetObj) : 0.0;

            // DC requires placeholders for freq and amp, but only offset is used
            // Change from:
            // string command = $"SOURce{channel}:APPLy:DC 0,0,{offset}";
            // To:
            string command = $"SOURce{channel}:APPLy:DC DEF,DEF,{offset}";

            visaManager.SendCommand(command);
            Log($"Applied DC to CH{channel} with Offset={offset}V");
        }

        private void ApplyDualToneWaveform(int channel, Dictionary<string, object> parameters)
        {
            // Extract parameters with defaults
            double frequency = parameters.TryGetValue("Frequency", out object freqObj) ? Convert.ToDouble(freqObj) : 1000.0;
            double amplitude = parameters.TryGetValue("Amplitude", out object ampObj) ? Convert.ToDouble(ampObj) : 1.0;
            double offset = parameters.TryGetValue("Offset", out object offsetObj) ? Convert.ToDouble(offsetObj) : 0.0;

            // Apply the dual-tone waveform
            string command = $"SOURce{channel}:APPLy:DUALTone {frequency},{amplitude},{offset}";
            visaManager.SendCommand(command);

            // Set additional dual-tone specific parameters
            if (parameters.TryGetValue("Frequency1", out object freq1Obj))
            {
                double freq1 = Convert.ToDouble(freq1Obj);
                SetDualToneFrequency1(channel, freq1);
            }

            if (parameters.TryGetValue("Frequency2", out object freq2Obj))
            {
                double freq2 = Convert.ToDouble(freq2Obj);
                SetDualToneFrequency2(channel, freq2);
            }

            if (parameters.TryGetValue("CenterFrequency", out object centerFreqObj))
            {
                double centerFreq = Convert.ToDouble(centerFreqObj);
                SetDualToneCenterFrequency(channel, centerFreq);
            }

            if (parameters.TryGetValue("OffsetFrequency", out object offsetFreqObj))
            {
                double offsetFreq = Convert.ToDouble(offsetFreqObj);
                SetDualToneOffsetFrequency(channel, offsetFreq);
            }

            Log($"Applied DUAL-TONE to CH{channel} with Amp={amplitude}Vpp, Offset={offset}V");
        }

        private void ApplyHarmonicWaveform(int channel, Dictionary<string, object> parameters)
        {
            // Extract parameters with defaults
            double frequency = parameters.TryGetValue("Frequency", out object freqObj) ? Convert.ToDouble(freqObj) : 1000.0;
            double amplitude = parameters.TryGetValue("Amplitude", out object ampObj) ? Convert.ToDouble(ampObj) : 1.0;
            double offset = parameters.TryGetValue("Offset", out object offsetObj) ? Convert.ToDouble(offsetObj) : 0.0;
            double phase = parameters.TryGetValue("Phase", out object phaseObj) ? Convert.ToDouble(phaseObj) : 0.0;

            // Apply the harmonic waveform (this sets up the fundamental sine wave)
            string command = $"SOURce{channel}:APPLy:HARMonic {frequency},{amplitude},{offset},{phase}";
            visaManager.SendCommand(command);

            // Harmonic settings are managed through the WaveformFunctionTab
            Log($"Applied HARMONIC to CH{channel} with Freq={frequency}Hz, Amp={amplitude}Vpp, Offset={offset}V, Phase={phase}°");
        }

        private void ApplyPRBSWaveform(int channel, Dictionary<string, object> parameters)
        {
            // Extract parameters with defaults
            double bitRate = parameters.TryGetValue("BitRate", out object rateObj) ? Convert.ToDouble(rateObj) : 2000.0;
            double amplitude = parameters.TryGetValue("Amplitude", out object ampObj) ? Convert.ToDouble(ampObj) : 1.0;
            double offset = parameters.TryGetValue("Offset", out object offsetObj) ? Convert.ToDouble(offsetObj) : 0.0;

            // Apply the PRBS waveform
            string command = $"SOURce{channel}:APPLy:PRBS {bitRate},{amplitude},{offset}";
            visaManager.SendCommand(command);

            // Set PRBS data type if specified
            if (parameters.TryGetValue("DataType", out object dataTypeObj))
            {
                string dataType = dataTypeObj.ToString();
                SetPRBSDataType(channel, dataType);
            }

            Log($"Applied PRBS to CH{channel} with BitRate={bitRate}bps, Amp={amplitude}Vpp, Offset={offset}V");
        }

        private void ApplyRS232Waveform(int channel, Dictionary<string, object> parameters)
        {
            // Extract parameters with defaults
            double amplitude = parameters.TryGetValue("Amplitude", out object ampObj) ? Convert.ToDouble(ampObj) : 1.0;
            double offset = parameters.TryGetValue("Offset", out object offsetObj) ? Convert.ToDouble(offsetObj) : 0.0;

            // Apply the RS232 waveform
            string command = $"SOURce{channel}:APPLy:RS232 {amplitude},{offset}";
            visaManager.SendCommand(command);

            // Set RS232 specific parameters
            if (parameters.TryGetValue("BaudRate", out object baudObj))
            {
                int baudRate = Convert.ToInt32(baudObj);
                SetRS232BaudRate(channel, baudRate);
            }

            if (parameters.TryGetValue("DataBits", out object dataBitsObj))
            {
                int dataBits = Convert.ToInt32(dataBitsObj);
                SetRS232DataBits(channel, dataBits);
            }

            if (parameters.TryGetValue("StopBits", out object stopBitsObj))
            {
                double stopBits = Convert.ToDouble(stopBitsObj);
                SetRS232StopBits(channel, stopBits);
            }

            if (parameters.TryGetValue("Parity", out object parityObj))
            {
                string parity = parityObj.ToString();
                SetRS232CheckBit(channel, parity);
            }

            if (parameters.TryGetValue("Data", out object dataObj))
            {
                int data = Convert.ToInt32(dataObj);
                SetRS232Data(channel, data);
            }

            Log($"Applied RS232 to CH{channel} with Amp={amplitude}Vpp, Offset={offset}V");
        }

        private void ApplyUserWaveform(int channel, Dictionary<string, object> parameters)
        {
            // Extract parameters with defaults
            double frequency = parameters.TryGetValue("Frequency", out object freqObj) ? Convert.ToDouble(freqObj) : 1000.0;
            double amplitude = parameters.TryGetValue("Amplitude", out object ampObj) ? Convert.ToDouble(ampObj) : 1.0;
            double offset = parameters.TryGetValue("Offset", out object offsetObj) ? Convert.ToDouble(offsetObj) : 0.0;
            double phase = parameters.TryGetValue("Phase", out object phaseObj) ? Convert.ToDouble(phaseObj) : 0.0;

            // Apply the user waveform
            string command = $"SOURce{channel}:APPLy:USER {frequency},{amplitude},{offset},{phase}";
            visaManager.SendCommand(command);
            Log($"Applied USER waveform to CH{channel} with Freq={frequency}Hz, Amp={amplitude}Vpp, Offset={offset}V, Phase={phase}°");
        }

        #endregion
    }
}
