using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Iot.Device.Ft4222;
using System.Device.Spi;
using Iot.Device.FtCommon;
using System.Device.Gpio;
using System.Globalization;
using System.Threading;
using System.Web;

namespace ADF4368_Register
{
    public partial class MainForm: Form
    {

        string filepath = string.Empty;
        Dictionary<string, string> regDB = new Dictionary<string, string>();

        int selectedHex;

        string datavalue = string.Empty;

        // Detect all connected FTDI FT4222H devices
        List<FtDevice> devices;
        Ft4222Spi spiDriver;
        GpioController gpioController;
       // SpiConnectionSettings spiSettings;
        
        
        const int Gpio3 = 3;
        //const int ADF4368_SPI_WRITE_CMD = 0x0000;
        //const int ADF4368_SPI_READ_CMD = 0x8000;


        bool initflag = false;

        DataTable dt = new DataTable();
        public MainForm()
        {
            InitializeComponent();
            label2.Text = string.Empty;
            InitDataTable();
            LoadComboBox();
            InitFTDI();

            initflag = true;
        }

        private void LoadComboBox()
        {
            for (int i = 0x0000; i <= 0x0063; i++)
            {
                if (i >= 0x0007 && i <= 0x0009)
                    continue; // Skip values 0x0007 to 0x0009
                comboBox1.Items.Add($"0x{i:X4}"); // Format as hexadecimal with 4 digits
            }

            comboBox1.SelectedIndex = 0; // Select the first item by default
        }

        private void Cmd_Exit_Click(object sender, EventArgs e)
        {

            if (gpioController.Read(Gpio3) == 1)
                gpioController.Write(Gpio3, PinValue.Low);

            Application.ExitThread();
        }

        public void InitDataTable()
        {
            dt.Columns.Add("Index", typeof(int));
            dt.Columns.Add("Register", typeof(string));
            dt.Columns.Add("Value", typeof(string));
            dt.Columns.Add("Value byte", typeof(byte));
            dataGridView1.DataSource = dt;
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
            dataGridView1.AllowUserToAddRows = false;
            
        }

        private void Cmd_Import_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ft = new OpenFileDialog())
            {
                try
                {
                    ft.InitialDirectory = Directory.GetCurrentDirectory();
                    ft.Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*";
                    ft.FilterIndex = 0;

                    if (ft.ShowDialog() == DialogResult.OK)
                    {
                        filepath = ft.FileName;
                        label2.Text = filepath;
                        ParsingFile(filepath);
                        int index = 0;

                        foreach(var kvp in regDB)
                        {
                            index++;
                            dt.Rows.Add(index, kvp.Key, kvp.Value, Convert.ToByte(kvp.Value, 16));
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Warning");
                }
            }

        }

        public void ParsingFile(string file)
        {
            if (!File.Exists(file))
            {
                MessageBox.Show("File not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                foreach (var line in File.ReadLines(file))
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 2)
                    {
                        string registerAddress = parts[0].Trim();
                        string registerValue = parts[1].Trim();

                        int value = Convert.ToInt32(registerAddress, 16);
                        string convertedregister = $"0x{value:X4}"; // Ensure at least 4 digits

                        //byte registerValue = Convert.ToByte(parts[1], 16);//parts[1].Trim();
                        //registers.Add((registerAddress, registerValue));
                        regDB.Add(convertedregister, registerValue);
                    }
                }
            }

        }

        private void Cmd_WriteAll_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedValue = comboBox1.SelectedItem.ToString();
            selectedHex = Convert.ToInt32(selectedValue.Substring(2), 16); // Convert hex string to int

            // Disable the button for values 0x0002 to 0x000D
            Cmd_Write.Enabled = !((selectedHex >= 0x0002 && selectedHex <= 0x000D) || (selectedHex >= 0x0054 && selectedHex <= 0x0063));
            textValue.Enabled = !((selectedHex >= 0x0002 && selectedHex <= 0x000D) || (selectedHex >= 0x0054 && selectedHex <= 0x0063));


            if (initflag)
            {
                //gpioController.Write(Gpio3, PinValue.High);
                byte nulladress = 0x0000;
                WriteRegister(spiDriver, nulladress, 0x18);

                byte valbyte = ReadRegister(spiDriver, (ushort)selectedHex);
                textValue.Text = $"0x{valbyte:X2}";

                //gpioController.Write(Gpio3, PinValue.Low);
            }
            


        }

        private void Cmd_ReadAll_Click(object sender, EventArgs e)
        {
            foreach(var indstring in comboBox1.Items)
            {
                string getcombo = indstring.ToString();
                selectedHex = Convert.ToInt32(getcombo.Substring(2), 16);
                byte valbyte = ReadRegister(spiDriver, (ushort)selectedHex);

            }
        }

        static byte ReadRegister(Ft4222Spi spi, ushort registerAddress)
        {
            // Format the read command: MSB of 24-bit address is 1 (read operation)
            ushort readCommand = (ushort)(registerAddress | 0x8000); // Set MSB (bit 23) to 1

            byte[] writeBuffer = new byte[]
            {
                (byte)(readCommand >> 8),  // High byte of register address
                (byte)(readCommand & 0xFF), // Low byte of register address
                0x00  // Dummy byte for reading data
            };

            byte[] readBuffer = new byte[3]; // 3 bytes: Register Address + Dummy + Read Data

            //spi.Write(writeBuffer);
            //Thread.Sleep(100);
            //spi.Read(readBuffer);
            // Perform SPI transaction
            spi.TransferFullDuplex(writeBuffer, readBuffer);

            byte receivedData = readBuffer[2]; // Extract the received data byte

            //Console.WriteLine($"SPI Read: Register 0x{registerAddress:X4} -> 0x{receivedData:X2}");
            return receivedData;
        }

        private void InitFTDI()
        {
            devices = FtCommon.GetDevices();

            if (devices.Count == 0)
            {
                label4.Text ="FTDI STATUS: " + "No FT4222H devices found.";
                return;
            }
            else 
            {
                var (chip, dll) = Ft4222Common.GetVersions();
                label4.Text = "FTDI STATUS: " + $"Detected {devices.Count} FT4222H device(s): Chip Version {chip}, Dll version {dll}";
            }

            //Init ADF4368 board config GPIO3 to OUTPUT and Configuration of 0x0000 register with 0x18 value for 4 wire mode
            gpioController = new GpioController(PinNumberingScheme.Board, new Ft4222Gpio());
            // Opening GPIO3
            gpioController.OpenPin(Gpio3);
            gpioController.SetPinMode(Gpio3, PinMode.Output);

            //SET GPIO3 HIGH
            gpioController.Write(Gpio3, PinValue.High);

            //SPI configuration 
            var spiSettings = new SpiConnectionSettings(0, 1)
            {
                ClockFrequency = 3750000,//ClockDiv16, // 7500000 ClockDiv8, 1875000 ClockDiv32// SPI Clock: System Clock / 16
                Mode = SpiMode.Mode0, // SPI Mode 0 (CPOL = 0, CPHA = 0)
                DataBitLength = 8, // 8-bit data per transaction
                
            };

            // Configure GPIO3 as an output
            spiDriver = new Ft4222Spi(spiSettings);
            
            byte nulladress = 0x0000;
            WriteRegister(spiDriver, nulladress, 0x18);

            // Example: Write data to register 0x10 with value 0xAB
            byte registerAddress = 0x000C; // Modify as needed
            byte receive = ReadRegister(spiDriver, registerAddress); ;


            //SET GPIO3 HIGH
            //gpioController.Write(Gpio3, PinValue.Low);
        }

        static void WriteRegister(Ft4222Spi spi, ushort registerAddress, byte data)
        {
            // Format the write command: MSB of 24-bit address is 0 (write operation)
            byte[] buffer = new byte[]
            {
                (byte)(registerAddress >> 8),  // High byte of register address
                (byte)(registerAddress & 0xFF), // Low byte of register address
                data  // 8-bit data
            };

            spi.Write(buffer);
            Console.WriteLine($"SPI Write: Register 0x{registerAddress:X4} <- 0x{data:X2}");
        }

        private void CleanHexValues()
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row["Register"] != DBNull.Value)
                {
                    string hexString = row["Register"].ToString();
                    int value = Convert.ToInt32(hexString, 16);
                    row["Register"] = $"0x{value:X4}"; // Ensure at least 4 digits
                }
            }
        }

        private void UpdateRowValue(byte searchValue, byte newValue)
        {
            string hexSearch = $"0x{searchValue:X2}";

            foreach (DataRow row in dt.Rows)
            {
                if (row["Register"].ToString() == hexSearch)
                {                    
                    row["Value"] = $"0x{newValue:X2}"; // Update the hex column to reflect the new value
                    row["Value byte"] = newValue; // Update the integer column
                    break; // Exit loop after updating the first match
                }
            }
        }

        private void Cmd_Write_Click(object sender, EventArgs e)
        {
            string regadress = comboBox1.SelectedItem?.ToString(); // Get selected value as string            
            byte databyte = Convert.ToByte(datavalue);
            if (int.TryParse(regadress, out int intValue))
            {
                ushort nvalue = (ushort)intValue;
                //SET GPIO3 HIGH
                gpioController.Write(Gpio3, PinValue.High);

                WriteRegister(spiDriver, nvalue, databyte);

                //SET GPIO3 Low
                gpioController.Write(Gpio3, PinValue.Low);
            }
        }

        private void textValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 13)
            {
                if (IsHexString(textValue.Text))
                {
                    datavalue = textValue.Text;
                }
                else
                {
                    textValue.Clear();
                    datavalue = string.Empty;
                }
            }
        }

        private bool IsHexString(string input)
        {
            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                input = input.Substring(2); // Remove "0x" prefix

            return int.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _);
        }
    }
}
