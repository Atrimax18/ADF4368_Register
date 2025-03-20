namespace ADF4368_Register
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Cmd_Exit = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Cmd_Import = new System.Windows.Forms.Button();
            this.Cmd_Write = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textValue = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Cmd_ReadAll = new System.Windows.Forms.Button();
            this.Cmd_WriteAll = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // Cmd_Exit
            // 
            this.Cmd_Exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cmd_Exit.Location = new System.Drawing.Point(673, 543);
            this.Cmd_Exit.Name = "Cmd_Exit";
            this.Cmd_Exit.Size = new System.Drawing.Size(115, 68);
            this.Cmd_Exit.TabIndex = 0;
            this.Cmd_Exit.Text = "Exit";
            this.Cmd_Exit.UseVisualStyleBackColor = true;
            this.Cmd_Exit.Click += new System.EventHandler(this.Cmd_Exit_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dataGridView1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 138);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(548, 473);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ADF4368 REGISTER DATA:";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(6, 19);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(536, 448);
            this.dataGridView1.TabIndex = 0;
            // 
            // Cmd_Import
            // 
            this.Cmd_Import.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cmd_Import.Location = new System.Drawing.Point(673, 146);
            this.Cmd_Import.Name = "Cmd_Import";
            this.Cmd_Import.Size = new System.Drawing.Size(115, 51);
            this.Cmd_Import.TabIndex = 2;
            this.Cmd_Import.Text = "Import Files";
            this.Cmd_Import.UseVisualStyleBackColor = true;
            this.Cmd_Import.Click += new System.EventHandler(this.Cmd_Import_Click);
            // 
            // Cmd_Write
            // 
            this.Cmd_Write.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cmd_Write.Location = new System.Drawing.Point(445, 45);
            this.Cmd_Write.Name = "Cmd_Write";
            this.Cmd_Write.Size = new System.Drawing.Size(115, 34);
            this.Cmd_Write.TabIndex = 3;
            this.Cmd_Write.Text = "Write Register";
            this.Cmd_Write.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "file path: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(80, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "filename";
            // 
            // textValue
            // 
            this.textValue.Location = new System.Drawing.Point(317, 53);
            this.textValue.Name = "textValue";
            this.textValue.Size = new System.Drawing.Size(115, 20);
            this.textValue.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(15, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(155, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "REGISTER ADDRES:";
            // 
            // Cmd_ReadAll
            // 
            this.Cmd_ReadAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cmd_ReadAll.Location = new System.Drawing.Point(673, 203);
            this.Cmd_ReadAll.Name = "Cmd_ReadAll";
            this.Cmd_ReadAll.Size = new System.Drawing.Size(115, 51);
            this.Cmd_ReadAll.TabIndex = 8;
            this.Cmd_ReadAll.Text = "Read All";
            this.Cmd_ReadAll.UseVisualStyleBackColor = true;
            this.Cmd_ReadAll.Click += new System.EventHandler(this.Cmd_ReadAll_Click);
            // 
            // Cmd_WriteAll
            // 
            this.Cmd_WriteAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cmd_WriteAll.Location = new System.Drawing.Point(674, 271);
            this.Cmd_WriteAll.Name = "Cmd_WriteAll";
            this.Cmd_WriteAll.Size = new System.Drawing.Size(113, 47);
            this.Cmd_WriteAll.TabIndex = 9;
            this.Cmd_WriteAll.Text = "Write All";
            this.Cmd_WriteAll.UseVisualStyleBackColor = true;
            this.Cmd_WriteAll.Click += new System.EventHandler(this.Cmd_WriteAll_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(176, 53);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 24);
            this.comboBox1.TabIndex = 10;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 624);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "FTDI STATUS:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 646);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.Cmd_WriteAll);
            this.Controls.Add(this.Cmd_ReadAll);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textValue);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Cmd_Write);
            this.Controls.Add(this.Cmd_Import);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Cmd_Exit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "ADF4368 Register Control";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Cmd_Exit;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Cmd_Import;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button Cmd_Write;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Cmd_ReadAll;
        private System.Windows.Forms.Button Cmd_WriteAll;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label4;
    }
}

