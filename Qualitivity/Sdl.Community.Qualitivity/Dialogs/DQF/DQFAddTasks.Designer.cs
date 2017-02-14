using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Community.Qualitivity.Dialogs.DQF
{
    partial class DqfAddTasks
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
            this.components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(DqfAddTasks));
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel33 = new System.Windows.Forms.Panel();
            this.button_wizard_cancel = new System.Windows.Forms.Button();
            this.button_wizard_finish = new System.Windows.Forms.Button();
            this.button_wizard_next = new System.Windows.Forms.Button();
            this.button_wizard_back = new System.Windows.Forms.Button();
            this.button_wizard_help = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.panel_information = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.textBox16 = new System.Windows.Forms.TextBox();
            this.textBox_information_message = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox_w0_title = new System.Windows.Forms.TextBox();
            this.panel_dqf_project = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.richTextBox_dqf_project_info = new System.Windows.Forms.RichTextBox();
            this.treeView_dqf_projects = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.panel_dqf_documents = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.linkLabel_uncheckall = new System.Windows.Forms.LinkLabel();
            this.linkLabel_checkall = new System.Windows.Forms.LinkLabel();
            this.listView_documents = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label2 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.panel8 = new System.Windows.Forms.Panel();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.panel_processing_the_upload = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label_progress_percentage = new System.Windows.Forms.Label();
            this.progressBar_import_progress = new System.Windows.Forms.ProgressBar();
            this.label_progress_message = new System.Windows.Forms.Label();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.panel10 = new System.Windows.Forms.Panel();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.panel9.SuspendLayout();
            this.panel33.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel_information.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel_dqf_project.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel_dqf_documents.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel_processing_the_upload.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel10.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.panel33);
            this.panel9.Controls.Add(this.button_wizard_help);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel9.Location = new System.Drawing.Point(0, 413);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(884, 44);
            this.panel9.TabIndex = 43;
            // 
            // panel33
            // 
            this.panel33.Controls.Add(this.button_wizard_cancel);
            this.panel33.Controls.Add(this.button_wizard_finish);
            this.panel33.Controls.Add(this.button_wizard_next);
            this.panel33.Controls.Add(this.button_wizard_back);
            this.panel33.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel33.Location = new System.Drawing.Point(515, 0);
            this.panel33.Name = "panel33";
            this.panel33.Size = new System.Drawing.Size(369, 44);
            this.panel33.TabIndex = 1;
            // 
            // button_wizard_cancel
            // 
            this.button_wizard_cancel.Location = new System.Drawing.Point(279, 9);
            this.button_wizard_cancel.Name = "button_wizard_cancel";
            this.button_wizard_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_wizard_cancel.TabIndex = 3;
            this.button_wizard_cancel.Text = "&Cancel";
            this.button_wizard_cancel.UseVisualStyleBackColor = true;
            this.button_wizard_cancel.Click += new System.EventHandler(this.button_wizard_cancel_Click);
            // 
            // button_wizard_finish
            // 
            this.button_wizard_finish.Location = new System.Drawing.Point(198, 9);
            this.button_wizard_finish.Name = "button_wizard_finish";
            this.button_wizard_finish.Size = new System.Drawing.Size(75, 23);
            this.button_wizard_finish.TabIndex = 4;
            this.button_wizard_finish.Text = "&Finish";
            this.button_wizard_finish.UseVisualStyleBackColor = true;
            this.button_wizard_finish.Click += new System.EventHandler(this.button_wizard_finish_Click);
            // 
            // button_wizard_next
            // 
            this.button_wizard_next.Location = new System.Drawing.Point(117, 9);
            this.button_wizard_next.Name = "button_wizard_next";
            this.button_wizard_next.Size = new System.Drawing.Size(75, 23);
            this.button_wizard_next.TabIndex = 1;
            this.button_wizard_next.Text = "&Next >";
            this.button_wizard_next.UseVisualStyleBackColor = true;
            this.button_wizard_next.Click += new System.EventHandler(this.button_wizard_next_Click);
            // 
            // button_wizard_back
            // 
            this.button_wizard_back.Location = new System.Drawing.Point(36, 9);
            this.button_wizard_back.Name = "button_wizard_back";
            this.button_wizard_back.Size = new System.Drawing.Size(75, 23);
            this.button_wizard_back.TabIndex = 2;
            this.button_wizard_back.Text = "< &Back";
            this.button_wizard_back.UseVisualStyleBackColor = true;
            this.button_wizard_back.Click += new System.EventHandler(this.button_wizard_back_Click);
            // 
            // button_wizard_help
            // 
            this.button_wizard_help.Location = new System.Drawing.Point(22, 9);
            this.button_wizard_help.Name = "button_wizard_help";
            this.button_wizard_help.Size = new System.Drawing.Size(75, 23);
            this.button_wizard_help.TabIndex = 0;
            this.button_wizard_help.Text = "&Help";
            this.button_wizard_help.UseVisualStyleBackColor = true;
            this.button_wizard_help.Click += new System.EventHandler(this.button_wizard_help_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(180, 413);
            this.panel1.TabIndex = 45;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(5, 5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(170, 403);
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // panel_information
            // 
            this.panel_information.Controls.Add(this.panel3);
            this.panel_information.Controls.Add(this.panel2);
            this.panel_information.Location = new System.Drawing.Point(300, 37);
            this.panel_information.Name = "panel_information";
            this.panel_information.Padding = new System.Windows.Forms.Padding(5);
            this.panel_information.Size = new System.Drawing.Size(224, 159);
            this.panel_information.TabIndex = 46;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.textBox16);
            this.panel3.Controls.Add(this.textBox_information_message);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(5, 44);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(214, 110);
            this.panel3.TabIndex = 1;
            // 
            // textBox16
            // 
            this.textBox16.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox16.Location = new System.Drawing.Point(20, 11);
            this.textBox16.Multiline = true;
            this.textBox16.Name = "textBox16";
            this.textBox16.ReadOnly = true;
            this.textBox16.Size = new System.Drawing.Size(630, 258);
            this.textBox16.TabIndex = 4;
            this.textBox16.Text = resources.GetString("textBox16.Text");
            // 
            // textBox_information_message
            // 
            this.textBox_information_message.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_information_message.Location = new System.Drawing.Point(0, 0);
            this.textBox_information_message.Multiline = true;
            this.textBox_information_message.Name = "textBox_information_message";
            this.textBox_information_message.ReadOnly = true;
            this.textBox_information_message.Size = new System.Drawing.Size(214, 110);
            this.textBox_information_message.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBox_w0_title);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(5, 5);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(5);
            this.panel2.Size = new System.Drawing.Size(214, 39);
            this.panel2.TabIndex = 0;
            // 
            // textBox_w0_title
            // 
            this.textBox_w0_title.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_w0_title.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_w0_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_w0_title.Location = new System.Drawing.Point(5, 5);
            this.textBox_w0_title.Multiline = true;
            this.textBox_w0_title.Name = "textBox_w0_title";
            this.textBox_w0_title.ReadOnly = true;
            this.textBox_w0_title.Size = new System.Drawing.Size(204, 29);
            this.textBox_w0_title.TabIndex = 0;
            this.textBox_w0_title.Text = "DQF Project Task Wizard";
            // 
            // panel_dqf_project
            // 
            this.panel_dqf_project.Controls.Add(this.panel5);
            this.panel_dqf_project.Controls.Add(this.panel6);
            this.panel_dqf_project.Location = new System.Drawing.Point(300, 202);
            this.panel_dqf_project.Name = "panel_dqf_project";
            this.panel_dqf_project.Padding = new System.Windows.Forms.Padding(5);
            this.panel_dqf_project.Size = new System.Drawing.Size(224, 139);
            this.panel_dqf_project.TabIndex = 47;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label1);
            this.panel5.Controls.Add(this.richTextBox_dqf_project_info);
            this.panel5.Controls.Add(this.treeView_dqf_projects);
            this.panel5.Controls.Add(this.textBox1);
            this.panel5.Controls.Add(this.textBox2);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(5, 44);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(214, 90);
            this.panel5.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(17, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "DQF Projects";
            // 
            // richTextBox_dqf_project_info
            // 
            this.richTextBox_dqf_project_info.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBox_dqf_project_info.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox_dqf_project_info.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_dqf_project_info.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.richTextBox_dqf_project_info.Location = new System.Drawing.Point(244, 64);
            this.richTextBox_dqf_project_info.Name = "richTextBox_dqf_project_info";
            this.richTextBox_dqf_project_info.ReadOnly = true;
            this.richTextBox_dqf_project_info.Size = new System.Drawing.Size(420, 276);
            this.richTextBox_dqf_project_info.TabIndex = 12;
            this.richTextBox_dqf_project_info.Text = "...";
            this.richTextBox_dqf_project_info.WordWrap = false;
            // 
            // treeView_dqf_projects
            // 
            this.treeView_dqf_projects.FullRowSelect = true;
            this.treeView_dqf_projects.HideSelection = false;
            this.treeView_dqf_projects.ImageIndex = 0;
            this.treeView_dqf_projects.ImageList = this.imageList1;
            this.treeView_dqf_projects.Indent = 10;
            this.treeView_dqf_projects.Location = new System.Drawing.Point(20, 64);
            this.treeView_dqf_projects.Name = "treeView_dqf_projects";
            this.treeView_dqf_projects.SelectedImageIndex = 0;
            this.treeView_dqf_projects.ShowLines = false;
            this.treeView_dqf_projects.ShowRootLines = false;
            this.treeView_dqf_projects.Size = new System.Drawing.Size(218, 276);
            this.treeView_dqf_projects.TabIndex = 5;
            this.treeView_dqf_projects.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_dqf_projects_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Light-Blue-Circle-32.png");
            this.imageList1.Images.SetKeyName(1, "Orange-Circle-32.png");
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(20, 11);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(630, 28);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = "Select the DQF Project and click continue.";
            // 
            // textBox2
            // 
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Location = new System.Drawing.Point(0, 0);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(214, 90);
            this.textBox2.TabIndex = 0;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.textBox3);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(5, 5);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new System.Windows.Forms.Padding(5);
            this.panel6.Size = new System.Drawing.Size(214, 39);
            this.panel6.TabIndex = 0;
            // 
            // textBox3
            // 
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(5, 5);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(204, 29);
            this.textBox3.TabIndex = 0;
            this.textBox3.Text = "DQF Projects";
            // 
            // panel_dqf_documents
            // 
            this.panel_dqf_documents.Controls.Add(this.panel7);
            this.panel_dqf_documents.Controls.Add(this.panel8);
            this.panel_dqf_documents.Location = new System.Drawing.Point(530, 37);
            this.panel_dqf_documents.Name = "panel_dqf_documents";
            this.panel_dqf_documents.Padding = new System.Windows.Forms.Padding(5);
            this.panel_dqf_documents.Size = new System.Drawing.Size(200, 159);
            this.panel_dqf_documents.TabIndex = 48;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.linkLabel_uncheckall);
            this.panel7.Controls.Add(this.linkLabel_checkall);
            this.panel7.Controls.Add(this.listView_documents);
            this.panel7.Controls.Add(this.label2);
            this.panel7.Controls.Add(this.textBox4);
            this.panel7.Controls.Add(this.textBox5);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(5, 44);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(190, 110);
            this.panel7.TabIndex = 1;
            // 
            // linkLabel_uncheckall
            // 
            this.linkLabel_uncheckall.AutoSize = true;
            this.linkLabel_uncheckall.Location = new System.Drawing.Point(94, 320);
            this.linkLabel_uncheckall.Name = "linkLabel_uncheckall";
            this.linkLabel_uncheckall.Size = new System.Drawing.Size(65, 13);
            this.linkLabel_uncheckall.TabIndex = 15;
            this.linkLabel_uncheckall.TabStop = true;
            this.linkLabel_uncheckall.Text = "Uncheck All";
            this.linkLabel_uncheckall.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_uncheckall_LinkClicked);
            // 
            // linkLabel_checkall
            // 
            this.linkLabel_checkall.AutoSize = true;
            this.linkLabel_checkall.Location = new System.Drawing.Point(23, 320);
            this.linkLabel_checkall.Name = "linkLabel_checkall";
            this.linkLabel_checkall.Size = new System.Drawing.Size(52, 13);
            this.linkLabel_checkall.TabIndex = 15;
            this.linkLabel_checkall.TabStop = true;
            this.linkLabel_checkall.Text = "Check All";
            this.linkLabel_checkall.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_checkall_LinkClicked);
            // 
            // listView_documents
            // 
            this.listView_documents.CheckBoxes = true;
            this.listView_documents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader4,
            this.columnHeader2,
            this.columnHeader3});
            this.listView_documents.FullRowSelect = true;
            this.listView_documents.Location = new System.Drawing.Point(20, 65);
            this.listView_documents.Name = "listView_documents";
            this.listView_documents.Size = new System.Drawing.Size(661, 252);
            this.listView_documents.TabIndex = 14;
            this.listView_documents.UseCompatibleStateImageBehavior = false;
            this.listView_documents.View = System.Windows.Forms.View.Details;
            this.listView_documents.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listView_documents_ItemCheck);
            this.listView_documents.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView_documents_ItemChecked);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Status";
            this.columnHeader4.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Target Language";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Total Segments";
            this.columnHeader3.Width = 100;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Documents";
            // 
            // textBox4
            // 
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Location = new System.Drawing.Point(20, 11);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(630, 28);
            this.textBox4.TabIndex = 4;
            this.textBox4.Text = "Select the documents that contain segments that you want to submit to the TAUS DQ" +
    "F platform.   Click on continue to proceed.";
            // 
            // textBox5
            // 
            this.textBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox5.Location = new System.Drawing.Point(0, 0);
            this.textBox5.Multiline = true;
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.Size = new System.Drawing.Size(190, 110);
            this.textBox5.TabIndex = 0;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.textBox6);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Location = new System.Drawing.Point(5, 5);
            this.panel8.Name = "panel8";
            this.panel8.Padding = new System.Windows.Forms.Padding(5);
            this.panel8.Size = new System.Drawing.Size(190, 39);
            this.panel8.TabIndex = 0;
            // 
            // textBox6
            // 
            this.textBox6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox6.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox6.Location = new System.Drawing.Point(5, 5);
            this.textBox6.Multiline = true;
            this.textBox6.Name = "textBox6";
            this.textBox6.ReadOnly = true;
            this.textBox6.Size = new System.Drawing.Size(180, 29);
            this.textBox6.TabIndex = 0;
            this.textBox6.Text = "Activity Documents";
            // 
            // panel_processing_the_upload
            // 
            this.panel_processing_the_upload.Controls.Add(this.panel4);
            this.panel_processing_the_upload.Controls.Add(this.panel10);
            this.panel_processing_the_upload.Location = new System.Drawing.Point(525, 202);
            this.panel_processing_the_upload.Name = "panel_processing_the_upload";
            this.panel_processing_the_upload.Padding = new System.Windows.Forms.Padding(5);
            this.panel_processing_the_upload.Size = new System.Drawing.Size(205, 139);
            this.panel_processing_the_upload.TabIndex = 53;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label_progress_percentage);
            this.panel4.Controls.Add(this.progressBar_import_progress);
            this.panel4.Controls.Add(this.label_progress_message);
            this.panel4.Controls.Add(this.textBox7);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(5, 44);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(195, 90);
            this.panel4.TabIndex = 6;
            // 
            // label_progress_percentage
            // 
            this.label_progress_percentage.AutoSize = true;
            this.label_progress_percentage.Location = new System.Drawing.Point(612, 130);
            this.label_progress_percentage.Name = "label_progress_percentage";
            this.label_progress_percentage.Size = new System.Drawing.Size(21, 13);
            this.label_progress_percentage.TabIndex = 11;
            this.label_progress_percentage.Text = "0%";
            // 
            // progressBar_import_progress
            // 
            this.progressBar_import_progress.Location = new System.Drawing.Point(33, 125);
            this.progressBar_import_progress.Name = "progressBar_import_progress";
            this.progressBar_import_progress.Size = new System.Drawing.Size(573, 23);
            this.progressBar_import_progress.TabIndex = 10;
            // 
            // label_progress_message
            // 
            this.label_progress_message.AutoSize = true;
            this.label_progress_message.Location = new System.Drawing.Point(30, 102);
            this.label_progress_message.Name = "label_progress_message";
            this.label_progress_message.Size = new System.Drawing.Size(102, 13);
            this.label_progress_message.TabIndex = 9;
            this.label_progress_message.Text = "0 entries processed.";
            // 
            // textBox7
            // 
            this.textBox7.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox7.Location = new System.Drawing.Point(19, 3);
            this.textBox7.Multiline = true;
            this.textBox7.Name = "textBox7";
            this.textBox7.ReadOnly = true;
            this.textBox7.Size = new System.Drawing.Size(608, 36);
            this.textBox7.TabIndex = 8;
            this.textBox7.Text = "The document segments are being uploaded to the TAUS DQF platform.  Click Finish " +
    "when the process is complete.";
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.textBox8);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel10.Location = new System.Drawing.Point(5, 5);
            this.panel10.Name = "panel10";
            this.panel10.Padding = new System.Windows.Forms.Padding(5);
            this.panel10.Size = new System.Drawing.Size(195, 39);
            this.panel10.TabIndex = 5;
            // 
            // textBox8
            // 
            this.textBox8.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox8.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox8.Location = new System.Drawing.Point(5, 5);
            this.textBox8.Multiline = true;
            this.textBox8.Name = "textBox8";
            this.textBox8.ReadOnly = true;
            this.textBox8.Size = new System.Drawing.Size(185, 29);
            this.textBox8.TabIndex = 0;
            this.textBox8.Text = "Processing the segments";
            // 
            // DQFAddTasks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 457);
            this.Controls.Add(this.panel_processing_the_upload);
            this.Controls.Add(this.panel_dqf_documents);
            this.Controls.Add(this.panel_dqf_project);
            this.Controls.Add(this.panel_information);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel9);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DqfAddTasks";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DQF Project Task Wizard";
            this.Load += new System.EventHandler(this.AddressDetails_Load);
            this.panel9.ResumeLayout(false);
            this.panel33.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel_information.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel_dqf_project.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel_dqf_documents.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panel_processing_the_upload.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel9;
        private Panel panel33;
        private Button button_wizard_cancel;
        private Button button_wizard_finish;
        private Button button_wizard_next;
        private Button button_wizard_back;
        private Button button_wizard_help;
        private Panel panel1;
        private PictureBox pictureBox2;
        private Panel panel_information;
        private Panel panel3;
        private TextBox textBox16;
        private TextBox textBox_information_message;
        private Panel panel2;
        private TextBox textBox_w0_title;
        private Panel panel_dqf_project;
        private Panel panel5;
        private TextBox textBox1;
        private TextBox textBox2;
        private Panel panel6;
        private TextBox textBox3;
        public TreeView treeView_dqf_projects;
        private RichTextBox richTextBox_dqf_project_info;
        private Label label1;
        private Panel panel_dqf_documents;
        private Panel panel7;
        private Label label2;
        private TextBox textBox4;
        private TextBox textBox5;
        private Panel panel8;
        private TextBox textBox6;
        private LinkLabel linkLabel_uncheckall;
        private LinkLabel linkLabel_checkall;
        private ListView listView_documents;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ImageList imageList1;
        private Panel panel_processing_the_upload;
        private Panel panel4;
        private Label label_progress_percentage;
        private ProgressBar progressBar_import_progress;
        private Label label_progress_message;
        private TextBox textBox7;
        private Panel panel10;
        private TextBox textBox8;


    }
}