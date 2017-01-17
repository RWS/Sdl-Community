using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Community.Studio.Time.Tracker.Dialogs
{
    partial class AddressDetails
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_addressCountry = new System.Windows.Forms.TextBox();
            this.textBox_addressZip = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_addressState = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_addressStreet = new System.Windows.Forms.TextBox();
            this.textBox_addressCity = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button_save = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_addressCountry);
            this.groupBox1.Controls.Add(this.textBox_addressZip);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBox_addressState);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBox_addressStreet);
            this.groupBox1.Controls.Add(this.textBox_addressCity);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(13, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(353, 191);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Address details";
            // 
            // textBox_addressCountry
            // 
            this.textBox_addressCountry.Location = new System.Drawing.Point(105, 153);
            this.textBox_addressCountry.Name = "textBox_addressCountry";
            this.textBox_addressCountry.Size = new System.Drawing.Size(229, 20);
            this.textBox_addressCountry.TabIndex = 9;
            this.textBox_addressCountry.TextChanged += new System.EventHandler(this.textBox_nameLast_TextChanged);
            // 
            // textBox_addressZip
            // 
            this.textBox_addressZip.Location = new System.Drawing.Point(105, 128);
            this.textBox_addressZip.Name = "textBox_addressZip";
            this.textBox_addressZip.Size = new System.Drawing.Size(229, 20);
            this.textBox_addressZip.TabIndex = 7;
            this.textBox_addressZip.TextChanged += new System.EventHandler(this.textBox_nameLast_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 131);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Zip/Postal Code:";
            // 
            // textBox_addressState
            // 
            this.textBox_addressState.Location = new System.Drawing.Point(105, 102);
            this.textBox_addressState.Name = "textBox_addressState";
            this.textBox_addressState.Size = new System.Drawing.Size(229, 20);
            this.textBox_addressState.TabIndex = 5;
            this.textBox_addressState.TextChanged += new System.EventHandler(this.textBox_nameMiddle_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "State/Province:";
            // 
            // textBox_addressStreet
            // 
            this.textBox_addressStreet.Location = new System.Drawing.Point(105, 31);
            this.textBox_addressStreet.Multiline = true;
            this.textBox_addressStreet.Name = "textBox_addressStreet";
            this.textBox_addressStreet.Size = new System.Drawing.Size(229, 39);
            this.textBox_addressStreet.TabIndex = 1;
            this.textBox_addressStreet.TextChanged += new System.EventHandler(this.textBox_addressStreet_TextChanged);
            // 
            // textBox_addressCity
            // 
            this.textBox_addressCity.Location = new System.Drawing.Point(105, 76);
            this.textBox_addressCity.Name = "textBox_addressCity";
            this.textBox_addressCity.Size = new System.Drawing.Size(229, 20);
            this.textBox_addressCity.TabIndex = 3;
            this.textBox_addressCity.TextChanged += new System.EventHandler(this.textBox_addressCity_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(75, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "City:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 156);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Country/Region:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(64, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Street:";
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(210, 207);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 11;
            this.button_save.Text = "&OK";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(291, 207);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_cancel.TabIndex = 10;
            this.button_cancel.Text = "&Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // AddressDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 239);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddressDetails";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Address";
            this.Load += new System.EventHandler(this.AddressDetails_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox groupBox1;
        private Label label1;
        private Label label2;
        private Label label4;
        private Label label3;
        private Label label5;
        private Button button_save;
        private Button button_cancel;
        internal TextBox textBox_addressCity;
        internal TextBox textBox_addressState;
        internal TextBox textBox_addressZip;
        internal TextBox textBox_addressStreet;
        internal TextBox textBox_addressCountry;
    }
}