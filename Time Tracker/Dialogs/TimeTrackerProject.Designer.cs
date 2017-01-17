using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Community.Studio.Time.Tracker.Dialogs
{
    partial class TimeTrackerProject
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(TimeTrackerProject));
            this.panel4 = new System.Windows.Forms.Panel();
            this.textBox_header = new System.Windows.Forms.TextBox();
            this.pictureBox_header = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_id = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button_save = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkLabel_view_client_profile = new System.Windows.Forms.LinkLabel();
            this.dateTimePicker_completed = new System.Windows.Forms.DateTimePicker();
            this.label_completed = new System.Windows.Forms.Label();
            this.dateTimePicker_due = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.dateTimePicker_created = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox_project_status = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox_client = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_description = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_project_name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBox_sdl_projects = new System.Windows.Forms.ComboBox();
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
            this.panel4.Size = new System.Drawing.Size(607, 56);
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
            this.textBox_header.Text = "Time Tracker Project";
            // 
            // pictureBox_header
            // 
            this.pictureBox_header.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_header.Image")));
            this.pictureBox_header.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_header.InitialImage")));
            this.pictureBox_header.Location = new System.Drawing.Point(18, 6);
            this.pictureBox_header.Name = "pictureBox_header";
            this.pictureBox_header.Size = new System.Drawing.Size(47, 44);
            this.pictureBox_header.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_header.TabIndex = 0;
            this.pictureBox_header.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label_id);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 407);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(607, 45);
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
            this.panel2.Location = new System.Drawing.Point(427, 0);
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
            this.groupBox1.Controls.Add(this.linkLabel_view_client_profile);
            this.groupBox1.Controls.Add(this.dateTimePicker_completed);
            this.groupBox1.Controls.Add(this.label_completed);
            this.groupBox1.Controls.Add(this.dateTimePicker_due);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.dateTimePicker_created);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.comboBox_project_status);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.comboBox_sdl_projects);
            this.groupBox1.Controls.Add(this.comboBox_client);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBox_description);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox_project_name);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 74);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(584, 331);
            this.groupBox1.TabIndex = 40;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Project Details";
            // 
            // linkLabel_view_client_profile
            // 
            this.linkLabel_view_client_profile.AutoSize = true;
            this.linkLabel_view_client_profile.Location = new System.Drawing.Point(482, 183);
            this.linkLabel_view_client_profile.Name = "linkLabel_view_client_profile";
            this.linkLabel_view_client_profile.Size = new System.Drawing.Size(65, 13);
            this.linkLabel_view_client_profile.TabIndex = 6;
            this.linkLabel_view_client_profile.TabStop = true;
            this.linkLabel_view_client_profile.Text = "View Details";
            this.linkLabel_view_client_profile.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_view_client_profile_LinkClicked);
            // 
            // dateTimePicker_completed
            // 
            this.dateTimePicker_completed.Location = new System.Drawing.Point(90, 293);
            this.dateTimePicker_completed.Name = "dateTimePicker_completed";
            this.dateTimePicker_completed.Size = new System.Drawing.Size(202, 20);
            this.dateTimePicker_completed.TabIndex = 14;
            this.dateTimePicker_completed.ValueChanged += new System.EventHandler(this.dateTimePicker_completed_ValueChanged);
            // 
            // label_completed
            // 
            this.label_completed.AutoSize = true;
            this.label_completed.Location = new System.Drawing.Point(26, 297);
            this.label_completed.Name = "label_completed";
            this.label_completed.Size = new System.Drawing.Size(60, 13);
            this.label_completed.TabIndex = 13;
            this.label_completed.Text = "Completed:";
            // 
            // dateTimePicker_due
            // 
            this.dateTimePicker_due.Location = new System.Drawing.Point(90, 266);
            this.dateTimePicker_due.Name = "dateTimePicker_due";
            this.dateTimePicker_due.Size = new System.Drawing.Size(202, 20);
            this.dateTimePicker_due.TabIndex = 12;
            this.dateTimePicker_due.ValueChanged += new System.EventHandler(this.dateTimePicker_due_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(56, 270);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Due:";
            // 
            // dateTimePicker_created
            // 
            this.dateTimePicker_created.Enabled = false;
            this.dateTimePicker_created.Location = new System.Drawing.Point(90, 240);
            this.dateTimePicker_created.Name = "dateTimePicker_created";
            this.dateTimePicker_created.Size = new System.Drawing.Size(202, 20);
            this.dateTimePicker_created.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(39, 244);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Created:";
            // 
            // comboBox_project_status
            // 
            this.comboBox_project_status.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_project_status.FormattingEnabled = true;
            this.comboBox_project_status.Items.AddRange(new object[] {
            "In progress",
            "Completed"});
            this.comboBox_project_status.Location = new System.Drawing.Point(90, 206);
            this.comboBox_project_status.Name = "comboBox_project_status";
            this.comboBox_project_status.Size = new System.Drawing.Size(202, 21);
            this.comboBox_project_status.TabIndex = 8;
            this.comboBox_project_status.SelectedIndexChanged += new System.EventHandler(this.comboBox_project_status_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(46, 210);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Status:";
            // 
            // comboBox_client
            // 
            this.comboBox_client.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_client.FormattingEnabled = true;
            this.comboBox_client.Location = new System.Drawing.Point(90, 178);
            this.comboBox_client.Name = "comboBox_client";
            this.comboBox_client.Size = new System.Drawing.Size(386, 21);
            this.comboBox_client.Sorted = true;
            this.comboBox_client.TabIndex = 5;
            this.comboBox_client.SelectedIndexChanged += new System.EventHandler(this.comboBox_client_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(50, 182);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Client:";
            // 
            // textBox_description
            // 
            this.textBox_description.Location = new System.Drawing.Point(90, 95);
            this.textBox_description.Multiline = true;
            this.textBox_description.Name = "textBox_description";
            this.textBox_description.Size = new System.Drawing.Size(462, 77);
            this.textBox_description.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Description:";
            // 
            // textBox_project_name
            // 
            this.textBox_project_name.Location = new System.Drawing.Point(90, 69);
            this.textBox_project_name.Name = "textBox_project_name";
            this.textBox_project_name.Size = new System.Drawing.Size(463, 20);
            this.textBox_project_name.TabIndex = 1;
            this.textBox_project_name.TextChanged += new System.EventHandler(this.textBox_project_name_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(48, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Studio Project:";
            // 
            // comboBox_sdl_projects
            // 
            this.comboBox_sdl_projects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_sdl_projects.FormattingEnabled = true;
            this.comboBox_sdl_projects.Location = new System.Drawing.Point(90, 42);
            this.comboBox_sdl_projects.Name = "comboBox_sdl_projects";
            this.comboBox_sdl_projects.Size = new System.Drawing.Size(462, 21);
            this.comboBox_sdl_projects.Sorted = true;
            this.comboBox_sdl_projects.TabIndex = 5;
            this.comboBox_sdl_projects.SelectedIndexChanged += new System.EventHandler(this.comboBox_sdl_projects_SelectedIndexChanged);
            // 
            // TimeTrackerProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 452);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TimeTrackerProject";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Time Tracker Project";
            this.Load += new System.EventHandler(this.TimeTrackerProject_Load);
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
        private Label label_id;
        private GroupBox groupBox1;
        private TextBox textBox_project_name;
        private Label label1;
        private TextBox textBox_description;
        private Label label2;
        private ComboBox comboBox_client;
        private Label label3;
        private ComboBox comboBox_project_status;
        private Label label4;
        private DateTimePicker dateTimePicker_created;
        private Label label5;
        private Label label7;
        private DateTimePicker dateTimePicker_completed;
        private Label label_completed;
        private DateTimePicker dateTimePicker_due;
        private LinkLabel linkLabel_view_client_profile;
        private ComboBox comboBox_sdl_projects;
        private Label label6;
    }
}