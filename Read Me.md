# DG2072_USB_Control

A .NET 8.0 WPF application for controlling Rigol DG2072 function/arbitrary waveform generator via USB using VISA instrument control.

## Features

- Connect to Rigol DG2072 via USB using VISA
- Control basic waveform parameters for both channels:
  - Waveform type (Sine, Square, Ramp, Pulse, Noise, Harmonic)
  - Frequency
  - Amplitude
  - Offset
  - Phase
- Control harmonic waveform generation:
  - Enable/disable harmonics
  - Set harmonic type (Even, Odd, All, User-defined)
  - Set harmonic order (2-8)
  - Configure amplitude and phase for individual harmonics
  - Custom user-defined harmonic selection
- Command logging for debugging and monitoring

## Requirements

- .NET 8.0 or later
- Visual Studio 2022 or later
- VISA Runtime (NI-VISA, Keysight VISA, or similar)
- Rigol DG2072 function/arbitrary waveform generator with USB connection

## Installation

1. Clone or download the repository
2. Open the solution in Visual Studio
3. Restore NuGet packages (MathNet.Numerics)
4. Build the solution
5. Run the application

## Usage

1. Connect the Rigol DG2072 to your computer via USB
2. Start the application
3. The application will attempt to connect to the device automatically
4. If connection fails, click the "Connect" button to retry
5. Use the tabs to access different control panels:
   - Basic Controls: Adjust fundamental waveform parameters
   - Harmonic: Configure and control harmonic waveform generation

## Architecture

The application is structured into several key components:

- `MainWindow`: The main UI and application entry point
- `VisaManager`: Handles low-level VISA communication with the instrument
- `RigolDG2072`: Provides a high-level API for controlling the device

## VISA Communication

The application uses P/Invoke to directly call the VISA library functions:

- `viOpenDefaultRM`: Opens the default VISA resource manager
- `viOpen`: Opens a connection to a specific instrument
- `viClose`: Closes a VISA connection
- `viWrite`: Sends commands to the instrument
- `viRead`: Reads responses from the instrument
- `viGpibControlREN`: Controls the Remote Enable line for remote/local operation

## Harmonic Generation

The Rigol DG2072 has built-in capabilities to generate harmonic waveforms. This application provides a user-friendly interface to control these functions through SCPI commands:

- Set harmonic type (Even, Odd, All, User)
- Set harmonic order (2-8)
- Configure amplitude and phase for individual harmonics
- Create user-defined harmonic patterns

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgements

- MathNet.Numerics for mathematical functions
- Rigol for the DG2072 programming guide
