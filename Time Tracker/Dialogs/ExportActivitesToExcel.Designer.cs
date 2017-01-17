using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Community.Studio.Time.Tracker.Dialogs
{
    partial class ExportActivitesToExcel
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportActivitesToExcel));
            this.panel4 = new System.Windows.Forms.Panel();
            this.textBox_header = new System.Windows.Forms.TextBox();
            this.pictureBox_header = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_id = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button_save = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.button_browse_folder_path = new System.Windows.Forms.Button();
            this.textBox_folder_path = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.checkBox_view_reprot_when_complete = new System.Windows.Forms.CheckBox();
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
            this.panel4.Size = new System.Drawing.Size(641, 56);
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
            this.textBox_header.Size = new System.Drawing.Size(344, 25);
            this.textBox_header.TabIndex = 3;
            this.textBox_header.TabStop = false;
            this.textBox_header.Text = "Export Activites to Excel format";
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
            this.panel1.Location = new System.Drawing.Point(0, 273);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(641, 45);
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
            this.panel2.Location = new System.Drawing.Point(461, 0);
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
            this.groupBox1.Controls.Add(this.checkBox_view_reprot_when_complete);
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Controls.Add(this.button_browse_folder_path);
            this.groupBox1.Controls.Add(this.textBox_folder_path);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Location = new System.Drawing.Point(12, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(617, 204);
            this.groupBox1.TabIndex = 40;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Export Properties";
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(72, 159);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(142, 17);
            this.radioButton3.TabIndex = 4;
            this.radioButton3.Text = "Export all activity records";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(72, 136);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(187, 17);
            this.radioButton2.TabIndex = 4;
            this.radioButton2.Text = "Export all activites currently filtered";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(72, 113);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(212, 17);
            this.radioButton1.TabIndex = 4;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Export only the selected activity records";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // button_browse_folder_path
            // 
            this.button_browse_folder_path.Location = new System.Drawing.Point(568, 36);
            this.button_browse_folder_path.Name = "button_browse_folder_path";
            this.button_browse_folder_path.Size = new System.Drawing.Size(27, 23);
            this.button_browse_folder_path.TabIndex = 3;
            this.button_browse_folder_path.Text = "...";
            this.button_browse_folder_path.UseVisualStyleBackColor = true;
            this.button_browse_folder_path.Click += new System.EventHandler(this.button_browse_folder_path_Click);
            // 
            // textBox_folder_path
            // 
            this.textBox_folder_path.Location = new System.Drawing.Point(72, 38);
            this.textBox_folder_path.Name = "textBox_folder_path";
            this.textBox_folder_path.Size = new System.Drawing.Size(490, 20);
            this.textBox_folder_path.TabIndex = 2;
            this.textBox_folder_path.TextChanged += new System.EventHandler(this.textBox_folder_path_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(16, 41);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(50, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "Save As:";
            // 
            // checkBox_view_reprot_when_complete
            // 
            this.checkBox_view_reprot_when_complete.AutoSize = true;
            this.checkBox_view_reprot_when_complete.Checked = true;
            this.checkBox_view_reprot_when_complete.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_view_reprot_when_complete.Location = new System.Drawing.Point(72, 65);
            this.checkBox_view_reprot_when_complete.Name = "checkBox_view_reprot_when_complete";
            this.checkBox_view_reprot_when_complete.Size = new System.Drawing.Size(205, 17);
            this.checkBox_view_reprot_when_complete.TabIndex = 5;
            this.checkBox_view_reprot_when_complete.Text = "View report after file has been created";
            this.checkBox_view_reprot_when_complete.UseVisualStyleBackColor = true;
            // 
            // ExportActivitesToExcel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 318);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportActivitesToExcel";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Export Activites";
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
        private Label label_id;
        private Label label13;
        private TextBox textBox_folder_path;
        private Button button_browse_folder_path;
        private RadioButton radioButton3;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private CheckBox checkBox_view_reprot_when_complete;
    }
}