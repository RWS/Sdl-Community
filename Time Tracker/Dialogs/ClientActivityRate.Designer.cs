using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Community.Studio.Time.Tracker.Dialogs
{
    partial class ClientActivityRate
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientActivityRate));
            this.panel4 = new System.Windows.Forms.Panel();
            this.textBox_header = new System.Windows.Forms.TextBox();
            this.pictureBox_header = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_id = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button_save = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label_adjustment = new System.Windows.Forms.Label();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDown_hourlyRate_default = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown_hourlyRate_client = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label_currency_client = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hourlyRate_default)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hourlyRate_client)).BeginInit();
            this.SuspendLayout();
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.Window;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.textBox_header);
            this.panel4.Controls.Add(this.pictureBox_header);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(499, 56);
            this.panel4.TabIndex = 38;
            // 
            // textBox_header
            // 
            this.textBox_header.BackColor = System.Drawing.Color.White;
            this.textBox_header.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_header.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_header.Location = new System.Drawing.Point(72, 16);
            this.textBox_header.Name = "textBox_header";
            this.textBox_header.ReadOnly = true;
            this.textBox_header.Size = new System.Drawing.Size(288, 25);
            this.textBox_header.TabIndex = 3;
            this.textBox_header.TabStop = false;
            this.textBox_header.Text = "Client Activity Rate";
            // 
            // pictureBox_header
            // 
            this.pictureBox_header.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_header.Image")));
            this.pictureBox_header.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_header.InitialImage")));
            this.pictureBox_header.Location = new System.Drawing.Point(19, 7);
            this.pictureBox_header.Name = "pictureBox_header";
            this.pictureBox_header.Size = new System.Drawing.Size(42, 42);
            this.pictureBox_header.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_header.TabIndex = 0;
            this.pictureBox_header.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label_id);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 238);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(499, 45);
            this.panel1.TabIndex = 39;
            // 
            // label_id
            // 
            this.label_id.AutoSize = true;
            this.label_id.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_id.Location = new System.Drawing.Point(17, 16);
            this.label_id.Name = "label_id";
            this.label_id.Size = new System.Drawing.Size(16, 13);
            this.label_id.TabIndex = 1;
            this.label_id.Text = "...";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button_save);
            this.panel2.Controls.Add(this.button_cancel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(319, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(180, 45);
            this.panel2.TabIndex = 0;
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(12, 11);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 9;
            this.button_save.Text = "&OK";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(93, 11);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_cancel.TabIndex = 8;
            this.button_cancel.Text = "&Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label_adjustment);
            this.groupBox1.Controls.Add(this.textBox_name);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numericUpDown_hourlyRate_default);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericUpDown_hourlyRate_client);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label_currency_client);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(12, 62);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(475, 169);
            this.groupBox1.TabIndex = 40;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Details";
            // 
            // label_adjustment
            // 
            this.label_adjustment.AutoSize = true;
            this.label_adjustment.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_adjustment.Location = new System.Drawing.Point(271, 107);
            this.label_adjustment.Name = "label_adjustment";
            this.label_adjustment.Size = new System.Drawing.Size(16, 15);
            this.label_adjustment.TabIndex = 8;
            this.label_adjustment.Text = "...";
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(131, 35);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.ReadOnly = true;
            this.textBox_name.Size = new System.Drawing.Size(303, 20);
            this.textBox_name.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(53, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Activity Name:";
            // 
            // numericUpDown_hourlyRate_default
            // 
            this.numericUpDown_hourlyRate_default.DecimalPlaces = 2;
            this.numericUpDown_hourlyRate_default.Location = new System.Drawing.Point(131, 79);
            this.numericUpDown_hourlyRate_default.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown_hourlyRate_default.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numericUpDown_hourlyRate_default.Name = "numericUpDown_hourlyRate_default";
            this.numericUpDown_hourlyRate_default.ReadOnly = true;
            this.numericUpDown_hourlyRate_default.Size = new System.Drawing.Size(102, 20);
            this.numericUpDown_hourlyRate_default.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Gray;
            this.label2.Location = new System.Drawing.Point(271, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(166, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "(default hourly rate for the activity)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Default Hourly Rate:";
            // 
            // numericUpDown_hourlyRate_client
            // 
            this.numericUpDown_hourlyRate_client.DecimalPlaces = 2;
            this.numericUpDown_hourlyRate_client.Location = new System.Drawing.Point(131, 105);
            this.numericUpDown_hourlyRate_client.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown_hourlyRate_client.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.numericUpDown_hourlyRate_client.Name = "numericUpDown_hourlyRate_client";
            this.numericUpDown_hourlyRate_client.Size = new System.Drawing.Size(102, 20);
            this.numericUpDown_hourlyRate_client.TabIndex = 5;
            this.numericUpDown_hourlyRate_client.ValueChanged += new System.EventHandler(this.numericUpDown_hourlyRate_client_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Location = new System.Drawing.Point(235, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "EUR";
            // 
            // label_currency_client
            // 
            this.label_currency_client.AutoSize = true;
            this.label_currency_client.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_currency_client.Location = new System.Drawing.Point(235, 109);
            this.label_currency_client.Name = "label_currency_client";
            this.label_currency_client.Size = new System.Drawing.Size(30, 13);
            this.label_currency_client.TabIndex = 4;
            this.label_currency_client.Text = "EUR";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(33, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Client Hourly Rate:";
            // 
            // ClientActivity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 283);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClientActivity";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Client Activity Rate";
            this.Load += new System.EventHandler(this.Activity_Load);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hourlyRate_default)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hourlyRate_client)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel4;
        private TextBox textBox_header;
        private PictureBox pictureBox_header;
        private Panel panel1;
        private Panel panel2;
        private Button button_save;
        private Button button_cancel;
        private GroupBox groupBox1;
        private NumericUpDown numericUpDown_hourlyRate_client;
        private Label label5;
        private Label label_id;
        private NumericUpDown numericUpDown_hourlyRate_default;
        private Label label1;
        private TextBox textBox_name;
        private Label label3;
        private Label label4;
        private Label label_currency_client;
        private Label label_adjustment;
        private Label label2;
    }
}