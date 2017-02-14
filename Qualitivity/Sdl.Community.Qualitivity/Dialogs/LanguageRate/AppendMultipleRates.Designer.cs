using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Community.Qualitivity.Dialogs.LanguageRate
{
    partial class AppendMultipleRates
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(AppendMultipleRates));
            this.panel6 = new System.Windows.Forms.Panel();
            this.textBox_header_title = new System.Windows.Forms.TextBox();
            this.pictureBox_header_image = new System.Windows.Forms.PictureBox();
            this.button_save = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.comboBox_sourceLanguages = new System.Windows.Forms.ComboBox();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header_image)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.White;
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.textBox_header_title);
            this.panel6.Controls.Add(this.pictureBox_header_image);
            this.panel6.Location = new System.Drawing.Point(12, 12);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(467, 56);
            this.panel6.TabIndex = 58;
            // 
            // textBox_header_title
            // 
            this.textBox_header_title.BackColor = System.Drawing.Color.White;
            this.textBox_header_title.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_header_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_header_title.Location = new System.Drawing.Point(85, 15);
            this.textBox_header_title.Name = "textBox_header_title";
            this.textBox_header_title.ReadOnly = true;
            this.textBox_header_title.Size = new System.Drawing.Size(285, 25);
            this.textBox_header_title.TabIndex = 3;
            this.textBox_header_title.TabStop = false;
            this.textBox_header_title.Text = "Multiple language Rates";
            // 
            // pictureBox_header_image
            // 
            this.pictureBox_header_image.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_header_image.Image")));
            this.pictureBox_header_image.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_header_image.InitialImage")));
            this.pictureBox_header_image.Location = new System.Drawing.Point(17, 6);
            this.pictureBox_header_image.Name = "pictureBox_header_image";
            this.pictureBox_header_image.Size = new System.Drawing.Size(56, 40);
            this.pictureBox_header_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_header_image.TabIndex = 0;
            this.pictureBox_header_image.TabStop = false;
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(323, 198);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 60;
            this.button_save.Text = "&OK";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(404, 198);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_cancel.TabIndex = 59;
            this.button_cancel.Text = "&Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label29);
            this.groupBox1.Controls.Add(this.comboBox_sourceLanguages);
            this.groupBox1.Location = new System.Drawing.Point(13, 77);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(466, 115);
            this.groupBox1.TabIndex = 61;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Language";
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Location = new System.Drawing.Point(73, 59);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(356, 30);
            this.label7.TabIndex = 73;
            this.label7.Text = "Note: Add a \'Language Rate\' for each target language combination associated with " +
    "the selected source language";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(23, 38);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(44, 13);
            this.label29.TabIndex = 56;
            this.label29.Text = "Source:";
            // 
            // comboBox_sourceLanguages
            // 
            this.comboBox_sourceLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_sourceLanguages.FormattingEnabled = true;
            this.comboBox_sourceLanguages.Location = new System.Drawing.Point(73, 34);
            this.comboBox_sourceLanguages.Name = "comboBox_sourceLanguages";
            this.comboBox_sourceLanguages.Size = new System.Drawing.Size(346, 21);
            this.comboBox_sourceLanguages.TabIndex = 57;
            // 
            // AppendMultipleRates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 226);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.panel6);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AppendMultipleRates";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Multiple Language Rates";
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header_image)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel6;
        private TextBox textBox_header_title;
        private PictureBox pictureBox_header_image;
        private Button button_save;
        private Button button_cancel;
        private GroupBox groupBox1;
        private Label label29;
        internal ComboBox comboBox_sourceLanguages;
        private Label label7;
    }
}