using System;
using System.Collections.Generic;
using MathNet.Numerics;

namespace DG2072_USB_Control
{
    public partial class RigolDG2072
    {
        #region Ramp Methods

        public void SetRampSymmetry(int channel, double symmetry)
        {
            ValidateChannel(channel);
            if (symmetry < 0 || symmetry > 100)
            {
                throw new ArgumentException("Ramp symmetry must be between 0 and 100%.");
            }
            visaManager.SendCommand($"SOURCE{channel}:FUNCTION:RAMP:SYMMETRY {symmetry}");
        }

        public double GetRampSymmetry(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:FUNCTION:RAMP:SYMMETRY?");
            if (double.TryParse(response, out double symmetry))
            {
                return symmetry;
            }
            return 50.0; // Default
        }

        #endregion

        #region Square Methods

        public void SetSquareDutyCycle(int channel, double dutyCycle)
        {
            ValidateChannel(channel);
            if (dutyCycle < 0 || dutyCycle > 100)
            {
                throw new ArgumentException("Square duty cycle must be between 0 and 100%.");
            }
            visaManager.SendCommand($"SOURCE{channel}:FUNCTION:SQUARE:DCYCLE {dutyCycle}");
        }

        public double GetSquareDutyCycle(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:FUNCTION:SQUARE:DCYCLE?");
            if (double.TryParse(response, out double dutyCycle))
            {
                return dutyCycle;
            }
            return 50.0; // Default
        }

        #endregion

        #region Pulse Methods
        //private void ApplyPulseWaveform(int channel, Dictionary<string, object> parameters)
        //{
        //    // Extract parameters with defaults
        //    double frequency = parameters.TryGetValue("Frequency", out object freqObj) ? Convert.ToDouble(freqObj) : 1000.0;
        //    double amplitude = parameters.TryGetValue("Amplitude", out object ampObj) ? Convert.ToDouble(ampObj) : 1.0;
        //    double offset = parameters.TryGetValue("Offset", out object offsetObj) ? Convert.ToDouble(offsetObj) : 0.0;
        //    double phase = parameters.TryGetValue("Phase", out object phaseObj) ? Convert.ToDouble(phaseObj) : 0.0;

        //    // Apply the pulse waveform
        //    string command = $"SOURce{channel}:APPLy:PULSe {frequency},{amplitude},{offset},{phase}";
        //    visaManager.SendCommand(command);

        //    // Set additional pulse-specific parameters after waveform is applied
        //    if (parameters.TryGetValue("Width", out object widthObj))
        //    {
        //        double width = Convert.ToDouble(widthObj);
        //        SetPulseWidth(channel, width);
        //    }

        //    if (parameters.TryGetValue("Period", out object periodObj))
        //    {
        //        double period = Convert.ToDouble(periodObj);
        //        SetPulsePeriod(channel, period);
        //    }

        //    if (parameters.TryGetValue("RiseTime", out object riseObj))
        //    {
        //        double riseTime = Convert.ToDouble(riseObj);
        //        SetPulseRiseTime(channel, riseTime);
        //    }

        //    if (parameters.TryGetValue("FallTime", out object fallObj))
        //    {
        //        double fallTime = Convert.ToDouble(fallObj);
        //        SetPulseFallTime(channel, fallTime);
        //    }

        //    // Remove duty cycle handling - duty cycle is not needed for pulse waveforms
        //    // The pulse is fully defined by period and width

        //    Log($"Applied PULSE to CH{channel} with Freq={frequency}Hz, Amp={amplitude}Vpp, Offset={offset}V, Phase={phase}°");
        //}

        public void SetPulseWidth(int channel, double width)
        {
            ValidateChannel(channel);
            visaManager.SendCommand($"SOURCE{channel}:PULSE:WIDTH {width}");
        }

        public double GetPulseWidth(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:PULSE:WIDTH?");
            if (double.TryParse(response, out double width))
            {
                return width;
            }
            return 0.0000005; // Default
        }

        public void SetPulseRiseTime(int channel, double riseTime)
        {
            ValidateChannel(channel);
            visaManager.SendCommand($"SOURCE{channel}:PULSE:TRANSITION:LEADING {riseTime}");
        }

        public double GetPulseRiseTime(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:PULSE:TRANSITION:LEADING?");
            if (double.TryParse(response, out double riseTime))
            {
                return riseTime;
            }
            return 0.00000002; // Default
        }

        public void SetPulseFallTime(int channel, double fallTime)
        {
            ValidateChannel(channel);
            visaManager.SendCommand($"SOURCE{channel}:PULSE:TRANSITION:TRAILING {fallTime}");
        }

        public double GetPulseFallTime(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:PULSE:TRANSITION:TRAILING?");
            if (double.TryParse(response, out double fallTime))
            {
                return fallTime;
            }
            return 0.00000002; // Default
        }

        public void SetPulseTransitionBoth(int channel, double transitionTime)
        {
            ValidateChannel(channel);
            visaManager.SendCommand($"SOURCE{channel}:PULSE:TRANSITION {transitionTime}");
        }

        #endregion


        #region Additional Pulse Methods

        public void SetPulsePeriod(int channel, double period)
        {
            ValidateChannel(channel);
            visaManager.SendCommand($"SOURCE{channel}:FUNCtion:PULSe:PERiod {period}");
        }

        public double GetPulsePeriod(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:FUNCtion:PULSe:PERiod?");
            if (double.TryParse(response, out double period))
            {
                return period;
            }
            return 0.001; // Default 1ms
        }

        // Calculate duty cycle from pulse width and period
        public double CalculatePulseDutyCycle(int channel)
        {
            double width = GetPulseWidth(channel);
            double period = GetPulsePeriod(channel);

            if (period > 0)
            {
                return (width / period) * 100.0;
            }
            return 50.0; // Default
        }

        #endregion



        #region PRBS Methods

        public void SetPRBSBitRate(int channel, double bitRate)
        {
            ValidateChannel(channel);
            visaManager.SendCommand($"SOURCE{channel}:FUNCTION:PRBS:BRATE {bitRate}");
        }

        public double GetPRBSBitRate(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:FUNCTION:PRBS:BRATE?");
            if (double.TryParse(response, out double bitRate))
            {
                return bitRate;
            }
            return 2000.0; // Default
        }

        public void SetPRBSDataType(int channel, string dataType)
        {
            ValidateChannel(channel);
            visaManager.SendCommand($"SOURCE{channel}:FUNCTION:PRBS:DATA {dataType}");
        }

        public string GetPRBSDataType(int channel)
        {
            ValidateChannel(channel);
            return visaManager.SendQuery($"SOURCE{channel}:FUNCTION:PRBS:DATA?");
        }

        #endregion

        #region Dual-Tone Methods

        public void SetDualToneFrequency1(int channel, double frequency)
        {
            ValidateChannel(channel);
            visaManager.SendCommand($"SOURCE{channel}:FUNCTION:DUALTONE:FREQ1 {frequency}");
        }

        public double GetDualToneFrequency1(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:FUNCTION:DUALTONE:FREQ1?");
            if (double.TryParse(response, out double frequency))
            {
                return frequency;
            }
            return 1000.0; // Default
        }

        public void SetDualToneFrequency2(int channel, double frequency)
        {
            ValidateChannel(channel);
            visaManager.SendCommand($"SOURCE{channel}:FUNCTION:DUALTONE:FREQ2 {frequency}");
        }

        public double GetDualToneFrequency2(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:FUNCTION:DUALTONE:FREQ2?");
            if (double.TryParse(response, out double frequency))
            {
                return frequency;
            }
            return 2000.0; // Default
        }

        public void SetDualToneCenterFrequency(int channel, double frequency)
        {
            ValidateChannel(channel);
            visaManager.SendCommand($"SOURCE{channel}:FUNCTION:DUALTONE:CENTFREQ {frequency}");
        }

        public double GetDualToneCenterFrequency(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:FUNCTION:DUALTONE:CENTFREQ?");
            if (double.TryParse(response, out double frequency))
            {
                return frequency;
            }
            return 1500.0; // Default
        }

        public void SetDualToneOffsetFrequency(int channel, double frequency)
        {
            ValidateChannel(channel);
            visaManager.SendCommand($"SOURCE{channel}:FUNCTION:DUALTONE:FREQDEV {frequency}");
        }

        public double GetDualToneOffsetFrequency(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:FUNCTION:DUALTONE:FREQDEV?");
            if (double.TryParse(response, out double frequency))
            {
                return frequency;
            }
            return 1000.0; // Default
        }

        #endregion

        #region RS232 Methods

        public void SetRS232BaudRate(int channel, int baudRate)
        {
            ValidateChannel(channel);
            visaManager.SendCommand($"SOURCE{channel}:FUNCTION:RS232:BAUD {baudRate}");
        }

        public int GetRS232BaudRate(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:FUNCTION:RS232:BAUD?");
            if (int.TryParse(response, out int baudRate))
            {
                return baudRate;
            }
            return 9600; // Default
        }

        public void SetRS232DataBits(int channel, int dataBits)
        {
            ValidateChannel(channel);
            if (dataBits != 7 && dataBits != 8)
            {
                throw new ArgumentException("RS232 data bits must be 7 or 8.");
            }
            visaManager.SendCommand($"SOURCE{channel}:FUNCTION:RS232:DBITS {dataBits}");
        }

        public int GetRS232DataBits(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:FUNCTION:RS232:DBITS?");
            if (int.TryParse(response, out int dataBits))
            {
                return dataBits;
            }
            return 8; // Default
        }

        public void SetRS232StopBits(int channel, double stopBits)
        {
            ValidateChannel(channel);
            if (stopBits != 1 && stopBits != 1.5 && stopBits != 2)
            {
                throw new ArgumentException("RS232 stop bits must be 1, 1.5, or 2.");
            }
            visaManager.SendCommand($"SOURCE{channel}:FUNCTION:RS232:SBITS {stopBits}");
        }

        public double GetRS232StopBits(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:FUNCTION:RS232:SBITS?");
            if (double.TryParse(response, out double stopBits))
            {
                return stopBits;
            }
            return 1.0; // Default
        }

        public void SetRS232CheckBit(int channel, string checkBit)
        {
            ValidateChannel(channel);
            if (checkBit != "NONE" && checkBit != "ODD" && checkBit != "EVEN")
            {
                throw new ArgumentException("RS232 check bit must be NONE, ODD, or EVEN.");
            }
            visaManager.SendCommand($"SOURCE{channel}:FUNCTION:RS232:PARITY {checkBit}");
        }

        public string GetRS232CheckBit(int channel)
        {
            ValidateChannel(channel);
            return visaManager.SendQuery($"SOURCE{channel}:FUNCTION:RS232:PARITY?");
        }

        public void SetRS232Data(int channel, int data)
        {
            ValidateChannel(channel);
            if (data < 0 || data > 255)
            {
                throw new ArgumentException("RS232 data must be between 0 and 255.");
            }
            visaManager.SendCommand($"SOURCE{channel}:FUNCTION:RS232:DATA {data}");
        }

        public int GetRS232Data(int channel)
        {
            ValidateChannel(channel);
            string response = visaManager.SendQuery($"SOURCE{channel}:FUNCTION:RS232:DATA?");
            if (int.TryParse(response, out int data))
            {
                return data;
            }
            return 0; // Default
        }

        #endregion
    }
}