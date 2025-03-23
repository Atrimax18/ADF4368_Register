# ADF4368 Register Control

This is a Windows Forms application for interfacing with Analog Devices' **ADF4368** evaluation board via **FTDI FT4222H** SPI bridge. It allows register-level configuration, reading, and writing using SPI over USB.

## Features

- Connects to ADF4368 using FTDI FT4222H (1.8VDC - Voltage Level) via SPI
- Detects available FTDI devices
- Reads and writes individual or all registers
- Loads register values from CSV files
- Allows manual data entry and editing of register values
- GUI interface with DataGridView and ComboBox for ease of use
- Visual indication of FTDI device detection and status

## Requirements

1. FTDI Drivers download latest here [link](https://ftdichip.com/drivers/d2xx-drivers/).
2. LibFT4222 Driver library can be downloaded here [link](https://ftdichip.com/software-examples/ft4222h-software-examples/).
3. Windows OS
4. FTDI FT4222H hardware connected via USB
5. ADF4368 evaluation board
6. Required NuGet packages:
  - `System.Device.Gpio`
  - `Iot.Device.Bindings`
  - `Iot.Device.Ft4222`
  - `Iot.Device.FtCommon`

## Setup and Usage

### 1. Connect Hardware
- Connect the FT4222H USB bridge to the PC.
- Wire the SPI and GPIO connections to the ADF4368 evaluation board.
- Ensure GPIO3 is connected to the board enable/power control TP9 (CE).

### 2. Build and Run
- Open the project in Visual Studio.
- Restore NuGet packages.
- Build and run the project.

### 3. Application Flow

#### FTDI Detection
- On launch, the app detects connected FT4222H devices.
- Initializes SPI communication in 4-wire mode (register `0x0000` with value `0x18`).

#### CSV Import
- Create `*.csv` file via ACE application from Analog Devices.
- Click **Import** to load register values from a `.csv` file.
- Format: `0xADDRESS,0xVALUE,...` (3 columns per line).

#### Register Operations
- **ComboBox**: Select a register (excluding restricted ranges).
- **Text Box**: Enter a hex value (e.g., `0x1A`) and press Enter.
- **Write**: Write selected value to selected register.
- **Read All**: Read values from all valid registers and populate the table.
- **Write All**: Write all values from the table to corresponding registers.

#### Power Toggle
- A simple switch labeled **"RF POWER ON"/"RF POWER OFF"** toggles its state (no hardware change in current version). - TODO
- Right now PD_ALL bit must be changed in register `0x002B`.

## Register Access Logic

- **Read**: 3-byte SPI transaction:
  - First 2 bytes: Register address with MSB = 1 (read)
  - Third byte: Dummy byte
- **Write**: 3-byte SPI transaction:
  - First 2 bytes: Register address with MSB = 0 (write)
  - Third byte: Data byte

## Notes

- Some register ranges (e.g., `0x0002–0x000D` and `0x0054–0x0063`) are disabled for writing.
- On exit, GPIO3 is set LOW to safely power off the board.

## Screenshot (Optional)
![App Screenshot](./screenshot.png)

## License

MIT License
