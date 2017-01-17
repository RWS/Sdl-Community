using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Community.Studio.Time.Tracker.Dialogs
{
    partial class ClientProfile
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientProfile));
            this.panel4 = new System.Windows.Forms.Panel();
            this.textBox_header = new System.Windows.Forms.TextBox();
            this.pictureBox_header = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_id = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button_save = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label_webPageAddress = new System.Windows.Forms.Label();
            this.textBox_companyFaxNumber = new System.Windows.Forms.TextBox();
            this.textBox_companyPhoneNumber = new System.Windows.Forms.TextBox();
            this.textBox_internetWebPageAddress = new System.Windows.Forms.TextBox();
            this.textBox_companyAddress = new System.Windows.Forms.TextBox();
            this.button_companyAddress = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_internetEmail = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_companyVatCode = new System.Windows.Forms.TextBox();
            this.textBox_companyTaxCode = new System.Windows.Forms.TextBox();
            this.textBox_companyName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
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
            this.panel4.Size = new System.Drawing.Size(510, 56);
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
            this.textBox_header.Text = "Client Profile";
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
            this.panel1.Location = new System.Drawing.Point(0, 426);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(510, 45);
            this.panel1.TabIndex = 39;
            // 
            // label_id
            // 
            this.label_id.AutoSize = true;
            this.label_id.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_id.Location = new System.Drawing.Point(17, 21);
            this.label_id.Name = "label_id";
            this.label_id.Size = new System.Drawing.Size(16, 13);
            this.label_id.TabIndex = 2;
            this.label_id.Text = "...";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button_save);
            this.panel2.Controls.Add(this.button_cancel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(330, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(180, 45);
            this.panel2.TabIndex = 0;
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(12, 11);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 18;
            this.button_save.Text = "&OK";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(93, 11);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_cancel.TabIndex = 17;
            this.button_cancel.Text = "&Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label_webPageAddress);
            this.groupBox1.Controls.Add(this.textBox_companyFaxNumber);
            this.groupBox1.Controls.Add(this.textBox_companyPhoneNumber);
            this.groupBox1.Controls.Add(this.textBox_internetWebPageAddress);
            this.groupBox1.Controls.Add(this.textBox_companyAddress);
            this.groupBox1.Controls.Add(this.button_companyAddress);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBox_internetEmail);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBox_companyVatCode);
            this.groupBox1.Controls.Add(this.textBox_companyTaxCode);
            this.groupBox1.Controls.Add(this.textBox_companyName);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(12, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(486, 357);
            this.groupBox1.TabIndex = 40;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Client Profile Details";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(74, 244);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "E-Mail:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(46, 326);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "Fax Number:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(32, 297);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Phone Number:";
            // 
            // label_webPageAddress
            // 
            this.label_webPageAddress.AutoSize = true;
            this.label_webPageAddress.Location = new System.Drawing.Point(53, 271);
            this.label_webPageAddress.Name = "label_webPageAddress";
            this.label_webPageAddress.Size = new System.Drawing.Size(60, 13);
            this.label_webPageAddress.TabIndex = 11;
            this.label_webPageAddress.Text = "Web page:";
            // 
            // textBox_companyFaxNumber
            // 
            this.textBox_companyFaxNumber.Location = new System.Drawing.Point(120, 322);
            this.textBox_companyFaxNumber.Name = "textBox_companyFaxNumber";
            this.textBox_companyFaxNumber.Size = new System.Drawing.Size(314, 20);
            this.textBox_companyFaxNumber.TabIndex = 16;
            // 
            // textBox_companyPhoneNumber
            // 
            this.textBox_companyPhoneNumber.Location = new System.Drawing.Point(120, 293);
            this.textBox_companyPhoneNumber.Name = "textBox_companyPhoneNumber";
            this.textBox_companyPhoneNumber.Size = new System.Drawing.Size(314, 20);
            this.textBox_companyPhoneNumber.TabIndex = 14;
            // 
            // textBox_internetWebPageAddress
            // 
            this.textBox_internetWebPageAddress.Location = new System.Drawing.Point(120, 267);
            this.textBox_internetWebPageAddress.Name = "textBox_internetWebPageAddress";
            this.textBox_internetWebPageAddress.Size = new System.Drawing.Size(314, 20);
            this.textBox_internetWebPageAddress.TabIndex = 12;
            // 
            // textBox_companyAddress
            // 
            this.textBox_companyAddress.Location = new System.Drawing.Point(121, 70);
            this.textBox_companyAddress.Multiline = true;
            this.textBox_companyAddress.Name = "textBox_companyAddress";
            this.textBox_companyAddress.ReadOnly = true;
            this.textBox_companyAddress.Size = new System.Drawing.Size(314, 84);
            this.textBox_companyAddress.TabIndex = 4;
            // 
            // button_companyAddress
            // 
            this.button_companyAddress.Location = new System.Drawing.Point(30, 71);
            this.button_companyAddress.Name = "button_companyAddress";
            this.button_companyAddress.Size = new System.Drawing.Size(83, 23);
            this.button_companyAddress.TabIndex = 3;
            this.button_companyAddress.Text = "Address...";
            this.button_companyAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_companyAddress.UseVisualStyleBackColor = true;
            this.button_companyAddress.Click += new System.EventHandler(this.button_companyAddress_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(54, 196);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "VAT Code:";
            // 
            // textBox_internetEmail
            // 
            this.textBox_internetEmail.Location = new System.Drawing.Point(120, 240);
            this.textBox_internetEmail.Name = "textBox_internetEmail";
            this.textBox_internetEmail.Size = new System.Drawing.Size(314, 20);
            this.textBox_internetEmail.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(54, 173);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "TAX Code:";
            // 
            // textBox_companyVatCode
            // 
            this.textBox_companyVatCode.Location = new System.Drawing.Point(121, 192);
            this.textBox_companyVatCode.Name = "textBox_companyVatCode";
            this.textBox_companyVatCode.Size = new System.Drawing.Size(314, 20);
            this.textBox_companyVatCode.TabIndex = 8;
            // 
            // textBox_companyTaxCode
            // 
            this.textBox_companyTaxCode.Location = new System.Drawing.Point(121, 169);
            this.textBox_companyTaxCode.Name = "textBox_companyTaxCode";
            this.textBox_companyTaxCode.Size = new System.Drawing.Size(314, 20);
            this.textBox_companyTaxCode.TabIndex = 6;
            // 
            // textBox_companyName
            // 
            this.textBox_companyName.Location = new System.Drawing.Point(120, 38);
            this.textBox_companyName.Name = "textBox_companyName";
            this.textBox_companyName.Size = new System.Drawing.Size(314, 20);
            this.textBox_companyName.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(46, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Client Name:";
            // 
            // CompanyProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 471);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CompanyProfile";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Company Profile";
            this.Load += new System.EventHandler(this.CompanyProfile_Load);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private Label label6;
        private Label label9;
        private Label label8;
        private Label label_webPageAddress;
        private TextBox textBox_companyFaxNumber;
        private TextBox textBox_companyPhoneNumber;
        private TextBox textBox_internetWebPageAddress;
        private TextBox textBox_companyAddress;
        private Button button_companyAddress;
        private Label label7;
        private TextBox textBox_internetEmail;
        private Label label5;
        private TextBox textBox_companyVatCode;
        private TextBox textBox_companyTaxCode;
        private TextBox textBox_companyName;
        private Label label4;
        private Label label_id;
    }
}