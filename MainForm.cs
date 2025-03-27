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
using static Iot.Device.HardwareMonitor.OpenHardwareMonitor;
using UnitsNet;
using Iot.Device.Mcp25xxx.Register;
using System.Net;

namespace ADF4368_Register
{
    public partial class MainForm: Form
    {

        string filepath = string.Empty;
        //Dictionary<string, string> regDB = new Dictionary<string, string>();

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

            //initflag = true;
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
            if (initflag)
            {
                if (gpioController.Read(Gpio3) == 1)
                    gpioController.Write(Gpio3, PinValue.Low);
            }

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
                        if (dt.Rows.Count != 0)
                            dt.Clear();
                        
                        filepath = ft.FileName;
                        label2.Text = filepath;
                        ParsingFile(filepath);
                        if (dt.Rows.Count != 0)
                        {
                            Cmd_WriteAll.Enabled = true;
                            Cmd_PowerSwitch.Enabled = true;
                            Cmd_Export.Enabled = true;
                            comboBox2.Enabled = true;
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
                
                int index = 1;
                foreach (var line in File.ReadLines(file))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(',');

                    int value = Convert.ToInt32(parts[0].Trim().Substring(2), 16);

                    if (parts.Length == 3)
                    {
                        dt.Rows.Add(index++.ToString() , $"0x{value:X4}", parts[1].Trim(), Convert.ToByte(parts[1].Trim(), 16));
                    }
                }
            }

        }

        private void Cmd_WriteAll_Click(object sender, EventArgs e)
        {
            var filteredRows = dt.AsEnumerable().Where(row =>
            {
                string hexStr = row["Register"].ToString();      // e.g. "0x0053"
                int reg = Convert.ToInt32(hexStr.Substring(2), 16); // Convert to int
                return reg >= 0x10 && reg <= 0x53;}).OrderByDescending(row => Convert.ToInt32(row["Register"].ToString().Substring(2), 16) // Sort descending
            );

            foreach (var row in filteredRows)
            {
                string reghex = row["Register"].ToString();
                string val = row["Value"].ToString();
                byte paddress = Convert.ToByte(reghex.Replace("0x", ""), 16);
                ushort regValue = Convert.ToUInt16(reghex.Replace("0x", ""), 16);
                byte databyte = Convert.ToByte(val.Replace("0x", ""), 16);

                WriteRegister(spiDriver, regValue, databyte);

                if (paddress == 0x002B)
                {
                    CheckPowerRegister(paddress);
                }                
            }            
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
                byte valbyte = ReadRegister(spiDriver, (ushort)selectedHex);
                textValue.Text = $"0x{valbyte:X2}";                
            }
        }

        private void Cmd_ReadAll_Click(object sender, EventArgs e)
        {
            int index = 1; byte paddress = 0;

            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.DataSource = null;
                dt.Clear();
                
                dataGridView1.DataSource = dt;
            }                
            
            foreach (var indstring in comboBox1.Items)
            {
                
                string getcombo = indstring.ToString();
                selectedHex = Convert.ToInt32(getcombo.Substring(2), 16);
                paddress = Convert.ToByte(getcombo.Replace("0x", ""), 16);
                byte valbyte = ReadRegister(spiDriver, (ushort)selectedHex);
                
                DataRow row = dt.NewRow();
                row["Index"] = index++.ToString();
                row["Register"] = getcombo;
                row["Value"] = $"0x{valbyte:X2}";
                row["Value byte"] = valbyte;
                dt.Rows.Add(row);

                if (paddress == 0x002B)
                {
                    CheckPowerRegister(paddress);
                }
            }

            if (dt.Rows.Count != 0)
            {
                Cmd_WriteAll.Enabled = true;
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
                label4.ForeColor = Color.Red;
                label4.Text ="FTDI STATUS: " + "No FT4222H devices found.";
                comboBox1.Enabled = false;
                initflag = false;
                Cmd_Write.Enabled = false;
                Cmd_Import.Enabled = false;
                textValue.Enabled = false;  
                Cmd_ReadAll.Enabled = false;
                return;
            }
            else 
            {
                var (chip, dll) = Ft4222Common.GetVersions();
                label4.ForeColor= Color.Green;
                label4.Text = "FTDI STATUS: " + $"Detected {devices.Count} FT4222H device(s): Chip Version {chip}, Dll version {dll}";
                initflag = true;
                comboBox1.Enabled = true;
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

            // Setup SPI communication FTDI A
            spiDriver = new Ft4222Spi(spiSettings);
            
            byte nulladress = 0x0000;
            WriteRegister(spiDriver, nulladress, 0x18);

            // Example: Write data to register 0x10 with value 0xAB
            byte registerAddress = 0x000C; // Modify as needed
            byte receive = ReadRegister(spiDriver, registerAddress);
            
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
        
        private void Cmd_Write_Click(object sender, EventArgs e)
        {
            string regaddress = comboBox1.SelectedItem?.ToString(); // Get selected value as string
                                                                    // 
            byte poweraddress = Convert.ToByte(regaddress.Replace("0x", ""), 16);
            ushort regValue = Convert.ToUInt16(regaddress.Replace("0x", ""), 16);
            byte databyte = Convert.ToByte(datavalue.Replace("0x", ""), 16);
            
            WriteRegister(spiDriver, regValue, databyte);

            if (poweraddress == 0x002B)
            {
                CheckPowerRegister(poweraddress);
            }
        }

        private void textValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 13)
            {
                if (IsHexString(textValue.Text))
                {
                    datavalue = textValue.Text.ToUpper();
                    Cmd_Write.Focus();
                }
                else
                {
                    textValue.Clear();
                    datavalue = string.Empty;
                    textValue.Focus();
                }
            }
        }

        private bool IsHexString(string input)
        {
            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                input = input.Substring(2); // Remove "0x" prefix

            return int.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _);
        }

        private void Cmd_PowerSwitch_Click(object sender, EventArgs e)
        {
            byte poweraddress = 0x002B;
            if (Cmd_PowerSwitch.Text.Equals("RF POWER ON"))
            {               
                
                WriteRegister(spiDriver, poweraddress, 0x00);
                Thread.Sleep(100);
                
                CheckPowerRegister(poweraddress);
            }
            else
            {               
                WriteRegister(spiDriver, poweraddress, 0x83);
                Thread.Sleep(100);
                CheckPowerRegister(poweraddress);
            }
        }


        private void CheckPowerRegister(byte address)
        {
            byte powerreturn = ReadRegister(spiDriver, address);

            if (powerreturn == 0x00)
            {
                Cmd_PowerSwitch.Text = "RF POWER OFF";
                radioButton2.Checked = true;
                timer1.Start();
            }
            else
            {
                Cmd_PowerSwitch.Text = "RF POWER ON";
                radioButton2.Checked = false;
                timer1.Stop();  
            }
        }

        private void Cmd_Export_Click(object sender, EventArgs e)
        {
            SaveDataTableToCsv(dt);
        }

        private int RFLockSampling(byte address, int monitoredBitIndex)
        {
            
            byte lockdata = ReadRegister(spiDriver, address);
            int bitValue = (lockdata >> monitoredBitIndex) & 0x01;

            return bitValue;
        }

        private void SaveDataTableToCsv(DataTable table)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.InitialDirectory = Directory.GetCurrentDirectory();
                saveDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveDialog.Title = "Save CSV File";
                saveDialog.FileName = "register_dump.csv";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StringBuilder csvContent = new StringBuilder();

                        // Optional: Add header row
                        // csvContent.AppendLine("Address,Value,Label");

                        foreach (DataRow row in table.Rows)
                        {
                            string regStr = row["Register"].ToString(); // e.g. "0x0023"
                            string valStr = row["Value"].ToString();    // e.g. "0x0F"

                            int regInt = Convert.ToInt32(regStr.Substring(2), 16); // Convert "0x0023" to int
                            string regHexFull = $"0x{regInt:X8}"; // Format to 8-digit hex

                            // You can dynamically name or hardcode the label
                            string label = "RegMap1";

                            csvContent.AppendLine($"{regHexFull},{valStr},{label}");
                        }

                        File.WriteAllText(saveDialog.FileName, csvContent.ToString());
                        MessageBox.Show("CSV exported successfully!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving CSV: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(RFLockSampling(0x0058, 0) == 1)
            {
                radioButton1.Checked = true;
                radioButton1.Text = "RF LOCKED";
            }
            else
            {
                radioButton1.Checked = false;
                radioButton1.Text = "RF UNLOCKED";
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

            int indvalue = comboBox2.SelectedIndex;
            byte data = 0b0000000;
            byte reisterval = 0x002E;

            switch (indvalue)
            {

                case 0:
                    data = 0b00000000;
                    break;

                case 1:
                    data = 0b00010000;
                    break;
                case 2:
                    data = 0b00100000;
                    break;
                case 3:
                    data = 0b00110000;
                    break;
                case 4:
                    data = 0b01000000;
                    break;
                case 5:
                    data = 0b01010000;
                    break;
            }

            WriteRegister(spiDriver, (ushort)reisterval, data);
            Thread.Sleep(100);

            byte checkbyte = ReadRegister(spiDriver, (ushort)reisterval);

            if (checkbyte == data)
                MessageBox.Show("MUXOUT updated successfully!!", "Information");
            else
                MessageBox.Show("MUXOUT Not updated!!!", "Information");            
        }
    }
}
