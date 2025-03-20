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

namespace ADF4368_Register
{
    public partial class MainForm: Form
    {

        string filepath = string.Empty;
        Dictionary<string, string> regDB = new Dictionary<string, string>();

        DataTable dt = new DataTable();
        public MainForm()
        {
            InitializeComponent();
            label2.Text = string.Empty;
            InitDataTable();
        }

        private void Cmd_Exit_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
        }

        public void InitDataTable()
        {
            dt.Columns.Add("Address", typeof(string));
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

                        foreach(var kvp in regDB)
                        {
                            dt.Rows.Add(kvp.Key, kvp.Value, Convert.ToByte(kvp.Value, 16));
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
                        //byte registerValue = Convert.ToByte(parts[1], 16);//parts[1].Trim();
                        //registers.Add((registerAddress, registerValue));
                        regDB.Add(registerAddress, registerValue);
                    }
                }
            }

        }

        private void Cmd_WriteAll_Click(object sender, EventArgs e)
        {

        }
    }
}
