using System.ComponentModel;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace Sdl.Community.Qualitivity.Dialogs
{
    partial class ActivityReportWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActivityReportWizard));
            this.panel_button_control = new System.Windows.Forms.Panel();
            this.panel33 = new System.Windows.Forms.Panel();
            this.button_wizard_cancel = new System.Windows.Forms.Button();
            this.button_wizard_finish = new System.Windows.Forms.Button();
            this.button_wizard_next = new System.Windows.Forms.Button();
            this.button_wizard_back = new System.Windows.Forms.Button();
            this.button_wizard_help = new System.Windows.Forms.Button();
            this.panel_leftbar = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.panel_welcome = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.textBox16 = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox_w0_title = new System.Windows.Forms.TextBox();
            this.panel_project_activties = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel16 = new System.Windows.Forms.Panel();
            this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn_activity_name = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_source = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_target = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_activity_description = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_activity_status = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_billable = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_documents = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_pem_total = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_hr_total = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_custom_total = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_activity_total = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel12 = new System.Windows.Forms.Panel();
            this.linkLabel_uncheck_all_listview = new System.Windows.Forms.LinkLabel();
            this.linkLabel_check_all_listview = new System.Windows.Forms.LinkLabel();
            this.panel17 = new System.Windows.Forms.Panel();
            this.label_activity_count = new System.Windows.Forms.Label();
            this.panel11 = new System.Windows.Forms.Panel();
            this.panel14 = new System.Windows.Forms.Panel();
            this.panel15 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel13 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.panel_report_options = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel21 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox_report_compresson_name = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox_report_compression = new System.Windows.Forms.CheckBox();
            this.panel20 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_open_output_folder_when_complete = new System.Windows.Forms.CheckBox();
            this.button_select_folder = new System.Windows.Forms.Button();
            this.textBox_output_folder = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel19 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_project_activity_export_to_file_xml = new System.Windows.Forms.CheckBox();
            this.checkBox_project_activity_export_to_file_single = new System.Windows.Forms.CheckBox();
            this.checkBox_project_activity_export_to_file_excel = new System.Windows.Forms.CheckBox();
            this.checkBox_project_activity_export_to_file = new System.Windows.Forms.CheckBox();
            this.checkBox_project_activity_quality_metric_report = new System.Windows.Forms.CheckBox();
            this.checkBox_project_activity_document_report = new System.Windows.Forms.CheckBox();
            this.checkBox_project_activity_report_single = new System.Windows.Forms.CheckBox();
            this.checkBox_project_activity_report = new System.Windows.Forms.CheckBox();
            this.panel18 = new System.Windows.Forms.Panel();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.panel8 = new System.Windows.Forms.Panel();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.panel_processing = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label_progress_percentage = new System.Windows.Forms.Label();
            this.progressBar_import_progress = new System.Windows.Forms.ProgressBar();
            this.label_progress_message = new System.Windows.Forms.Label();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.panel10 = new System.Windows.Forms.Panel();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.panel_button_control.SuspendLayout();
            this.panel33.SuspendLayout();
            this.panel_leftbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel_welcome.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel_project_activties.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel16.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
            this.panel12.SuspendLayout();
            this.panel17.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel14.SuspendLayout();
            this.panel15.SuspendLayout();
            this.panel13.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel_report_options.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel21.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel20.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel19.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel18.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel_processing.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel10.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_button_control
            // 
            this.panel_button_control.Controls.Add(this.panel33);
            this.panel_button_control.Controls.Add(this.button_wizard_help);
            this.panel_button_control.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_button_control.Location = new System.Drawing.Point(0, 480);
            this.panel_button_control.Name = "panel_button_control";
            this.panel_button_control.Size = new System.Drawing.Size(1101, 44);
            this.panel_button_control.TabIndex = 43;
            // 
            // panel33
            // 
            this.panel33.Controls.Add(this.button_wizard_cancel);
            this.panel33.Controls.Add(this.button_wizard_finish);
            this.panel33.Controls.Add(this.button_wizard_next);
            this.panel33.Controls.Add(this.button_wizard_back);
            this.panel33.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel33.Location = new System.Drawing.Point(732, 0);
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
            // panel_leftbar
            // 
            this.panel_leftbar.Controls.Add(this.pictureBox2);
            this.panel_leftbar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel_leftbar.Location = new System.Drawing.Point(0, 0);
            this.panel_leftbar.Name = "panel_leftbar";
            this.panel_leftbar.Padding = new System.Windows.Forms.Padding(5);
            this.panel_leftbar.Size = new System.Drawing.Size(180, 480);
            this.panel_leftbar.TabIndex = 45;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(5, 5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(170, 470);
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // panel_welcome
            // 
            this.panel_welcome.Controls.Add(this.panel3);
            this.panel_welcome.Controls.Add(this.panel2);
            this.panel_welcome.Location = new System.Drawing.Point(311, 37);
            this.panel_welcome.Name = "panel_welcome";
            this.panel_welcome.Padding = new System.Windows.Forms.Padding(5);
            this.panel_welcome.Size = new System.Drawing.Size(208, 150);
            this.panel_welcome.TabIndex = 46;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.textBox16);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(5, 44);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(5, 20, 5, 5);
            this.panel3.Size = new System.Drawing.Size(198, 101);
            this.panel3.TabIndex = 1;
            // 
            // textBox16
            // 
            this.textBox16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox16.Location = new System.Drawing.Point(5, 20);
            this.textBox16.Multiline = true;
            this.textBox16.Name = "textBox16";
            this.textBox16.ReadOnly = true;
            this.textBox16.Size = new System.Drawing.Size(188, 76);
            this.textBox16.TabIndex = 5;
            this.textBox16.Text = resources.GetString("textBox16.Text");
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBox_w0_title);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(5, 5);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(5);
            this.panel2.Size = new System.Drawing.Size(198, 39);
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
            this.textBox_w0_title.Size = new System.Drawing.Size(188, 29);
            this.textBox_w0_title.TabIndex = 0;
            this.textBox_w0_title.Text = "Qualitivity Activity Report Wizard";
            // 
            // panel_project_activties
            // 
            this.panel_project_activties.Controls.Add(this.panel5);
            this.panel_project_activties.Controls.Add(this.panel6);
            this.panel_project_activties.Location = new System.Drawing.Point(535, 37);
            this.panel_project_activties.Name = "panel_project_activties";
            this.panel_project_activties.Padding = new System.Windows.Forms.Padding(5);
            this.panel_project_activties.Size = new System.Drawing.Size(195, 150);
            this.panel_project_activties.TabIndex = 47;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.panel16);
            this.panel5.Controls.Add(this.panel12);
            this.panel5.Controls.Add(this.panel11);
            this.panel5.Controls.Add(this.textBox2);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(5, 44);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(185, 101);
            this.panel5.TabIndex = 1;
            // 
            // panel16
            // 
            this.panel16.Controls.Add(this.objectListView1);
            this.panel16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel16.Location = new System.Drawing.Point(0, 68);
            this.panel16.Name = "panel16";
            this.panel16.Padding = new System.Windows.Forms.Padding(5);
            this.panel16.Size = new System.Drawing.Size(185, 8);
            this.panel16.TabIndex = 3;
            // 
            // objectListView1
            // 
            this.objectListView1.AllColumns.Add(this.olvColumn_activity_name);
            this.objectListView1.AllColumns.Add(this.olvColumn_source);
            this.objectListView1.AllColumns.Add(this.olvColumn_target);
            this.objectListView1.AllColumns.Add(this.olvColumn_activity_description);
            this.objectListView1.AllColumns.Add(this.olvColumn_activity_status);
            this.objectListView1.AllColumns.Add(this.olvColumn_billable);
            this.objectListView1.AllColumns.Add(this.olvColumn5);
            this.objectListView1.AllColumns.Add(this.olvColumn6);
            this.objectListView1.AllColumns.Add(this.olvColumn_documents);
            this.objectListView1.AllColumns.Add(this.olvColumn_pem_total);
            this.objectListView1.AllColumns.Add(this.olvColumn_hr_total);
            this.objectListView1.AllColumns.Add(this.olvColumn_custom_total);
            this.objectListView1.AllColumns.Add(this.olvColumn_activity_total);
            this.objectListView1.CheckBoxes = true;
            this.objectListView1.CheckedAspectName = "IsChecked";
            this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn_activity_name,
            this.olvColumn_source,
            this.olvColumn_target,
            this.olvColumn_activity_description,
            this.olvColumn_activity_status,
            this.olvColumn_billable,
            this.olvColumn5,
            this.olvColumn6,
            this.olvColumn_documents,
            this.olvColumn_pem_total,
            this.olvColumn_hr_total,
            this.olvColumn_custom_total,
            this.olvColumn_activity_total});
            this.objectListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListView1.EmptyListMsg = "Empty!";
            this.objectListView1.EmptyListMsgFont = new System.Drawing.Font("Palatino Linotype", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.objectListView1.FullRowSelect = true;
            this.objectListView1.HeaderUsesThemes = false;
            this.objectListView1.HeaderWordWrap = true;
            this.objectListView1.HideSelection = false;
            this.objectListView1.IncludeColumnHeadersInCopy = true;
            this.objectListView1.Location = new System.Drawing.Point(5, 5);
            this.objectListView1.Name = "objectListView1";
            this.objectListView1.OwnerDraw = true;
            this.objectListView1.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.objectListView1.ShowCommandMenuOnRightClick = true;
            this.objectListView1.ShowHeaderInAllViews = false;
            this.objectListView1.ShowItemToolTips = true;
            this.objectListView1.Size = new System.Drawing.Size(175, 0);
            this.objectListView1.SmallImageList = this.imageList1;
            this.objectListView1.SpaceBetweenGroups = 20;
            this.objectListView1.TabIndex = 5;
            this.objectListView1.UseAlternatingBackColors = true;
            this.objectListView1.UseCellFormatEvents = true;
            this.objectListView1.UseCompatibleStateImageBehavior = false;
            this.objectListView1.UseFilterIndicator = true;
            this.objectListView1.UseFiltering = true;
            this.objectListView1.View = System.Windows.Forms.View.Details;
            this.objectListView1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.objectListView1_ItemCheck);
            this.objectListView1.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.objectListView1_ItemChecked);
            // 
            // olvColumn_activity_name
            // 
            this.olvColumn_activity_name.AspectName = "Name";
            this.olvColumn_activity_name.CellPadding = null;
            this.olvColumn_activity_name.HeaderImageKey = "(none)";
            this.olvColumn_activity_name.ImageAspectName = "";
            this.olvColumn_activity_name.Text = "Activity Name";
            this.olvColumn_activity_name.Width = 180;
            // 
            // olvColumn_source
            // 
            this.olvColumn_source.CellPadding = null;
            this.olvColumn_source.Text = "Source";
            // 
            // olvColumn_target
            // 
            this.olvColumn_target.CellPadding = null;
            this.olvColumn_target.Text = "Target";
            // 
            // olvColumn_activity_description
            // 
            this.olvColumn_activity_description.AspectName = "Description";
            this.olvColumn_activity_description.CellPadding = null;
            this.olvColumn_activity_description.Text = "Description";
            this.olvColumn_activity_description.Width = 80;
            // 
            // olvColumn_activity_status
            // 
            this.olvColumn_activity_status.AspectName = "ActivityStatus";
            this.olvColumn_activity_status.CellPadding = null;
            this.olvColumn_activity_status.Text = "Status";
            this.olvColumn_activity_status.Width = 78;
            // 
            // olvColumn_billable
            // 
            this.olvColumn_billable.AspectName = "Billable";
            this.olvColumn_billable.CellPadding = null;
            this.olvColumn_billable.Text = "Billable";
            this.olvColumn_billable.Width = 75;
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "Started";
            this.olvColumn5.CellPadding = null;
            this.olvColumn5.HeaderImageKey = "started";
            this.olvColumn5.Text = "From";
            this.olvColumn5.Width = 110;
            // 
            // olvColumn6
            // 
            this.olvColumn6.AspectName = "Stopped";
            this.olvColumn6.CellPadding = null;
            this.olvColumn6.HeaderImageKey = "started";
            this.olvColumn6.Text = "To";
            this.olvColumn6.Width = 110;
            // 
            // olvColumn_documents
            // 
            this.olvColumn_documents.CellPadding = null;
            this.olvColumn_documents.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_documents.Text = "Documents";
            this.olvColumn_documents.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_documents.Width = 70;
            // 
            // olvColumn_pem_total
            // 
            this.olvColumn_pem_total.AspectName = "";
            this.olvColumn_pem_total.CellPadding = null;
            this.olvColumn_pem_total.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_pem_total.Text = "Language Rate Total";
            this.olvColumn_pem_total.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_pem_total.Width = 125;
            // 
            // olvColumn_hr_total
            // 
            this.olvColumn_hr_total.AspectName = "";
            this.olvColumn_hr_total.CellPadding = null;
            this.olvColumn_hr_total.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_hr_total.Text = "Hourly Rate Total";
            this.olvColumn_hr_total.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_hr_total.Width = 100;
            // 
            // olvColumn_custom_total
            // 
            this.olvColumn_custom_total.CellPadding = null;
            this.olvColumn_custom_total.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_custom_total.Text = "Custom Rate Total";
            this.olvColumn_custom_total.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_custom_total.Width = 125;
            // 
            // olvColumn_activity_total
            // 
            this.olvColumn_activity_total.CellPadding = null;
            this.olvColumn_activity_total.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_activity_total.Text = "Activity Total";
            this.olvColumn_activity_total.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_activity_total.Width = 90;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "calendar");
            this.imageList1.Images.SetKeyName(1, "client");
            this.imageList1.Images.SetKeyName(2, "xno1");
            this.imageList1.Images.SetKeyName(3, "xno2");
            this.imageList1.Images.SetKeyName(4, "iyes");
            this.imageList1.Images.SetKeyName(5, "ino");
            this.imageList1.Images.SetKeyName(6, "vno");
            this.imageList1.Images.SetKeyName(7, "vyes");
            this.imageList1.Images.SetKeyName(8, "tick");
            this.imageList1.Images.SetKeyName(9, "flag_red");
            this.imageList1.Images.SetKeyName(10, "flag_blue");
            this.imageList1.Images.SetKeyName(11, "flag_green");
            this.imageList1.Images.SetKeyName(12, "started");
            this.imageList1.Images.SetKeyName(13, "stopped");
            this.imageList1.Images.SetKeyName(14, "question_blue");
            this.imageList1.Images.SetKeyName(15, "hours");
            this.imageList1.Images.SetKeyName(16, "words");
            this.imageList1.Images.SetKeyName(17, "af-ZA.gif");
            this.imageList1.Images.SetKeyName(18, "ar.gif");
            this.imageList1.Images.SetKeyName(19, "ar-AE.gif");
            this.imageList1.Images.SetKeyName(20, "ar-BH.gif");
            this.imageList1.Images.SetKeyName(21, "ar-DZ.gif");
            this.imageList1.Images.SetKeyName(22, "ar-EG.gif");
            this.imageList1.Images.SetKeyName(23, "ar-IQ.gif");
            this.imageList1.Images.SetKeyName(24, "ar-JO.gif");
            this.imageList1.Images.SetKeyName(25, "ar-KW.gif");
            this.imageList1.Images.SetKeyName(26, "ar-LB.gif");
            this.imageList1.Images.SetKeyName(27, "ar-LY.gif");
            this.imageList1.Images.SetKeyName(28, "ar-MA.gif");
            this.imageList1.Images.SetKeyName(29, "ar-OM.gif");
            this.imageList1.Images.SetKeyName(30, "ar-QA.gif");
            this.imageList1.Images.SetKeyName(31, "ar-SA.gif");
            this.imageList1.Images.SetKeyName(32, "ar-SY.gif");
            this.imageList1.Images.SetKeyName(33, "ar-TN.gif");
            this.imageList1.Images.SetKeyName(34, "ar-YE.gif");
            this.imageList1.Images.SetKeyName(35, "be.gif");
            this.imageList1.Images.SetKeyName(36, "be-BY.gif");
            this.imageList1.Images.SetKeyName(37, "bg.gif");
            this.imageList1.Images.SetKeyName(38, "bg-BG.gif");
            this.imageList1.Images.SetKeyName(39, "bp.gif");
            this.imageList1.Images.SetKeyName(40, "bs.gif");
            this.imageList1.Images.SetKeyName(41, "bs-Cyrl-BA.gif");
            this.imageList1.Images.SetKeyName(42, "bs-Latn-BA.gif");
            this.imageList1.Images.SetKeyName(43, "ca.gif");
            this.imageList1.Images.SetKeyName(44, "ca-ES.gif");
            this.imageList1.Images.SetKeyName(45, "cf.gif");
            this.imageList1.Images.SetKeyName(46, "ch.gif");
            this.imageList1.Images.SetKeyName(47, "cs.gif");
            this.imageList1.Images.SetKeyName(48, "cs-CZ.gif");
            this.imageList1.Images.SetKeyName(49, "da.gif");
            this.imageList1.Images.SetKeyName(50, "da-DK.gif");
            this.imageList1.Images.SetKeyName(51, "de.gif");
            this.imageList1.Images.SetKeyName(52, "de-AT.gif");
            this.imageList1.Images.SetKeyName(53, "de-CH.gif");
            this.imageList1.Images.SetKeyName(54, "de-DE.gif");
            this.imageList1.Images.SetKeyName(55, "de-LI.gif");
            this.imageList1.Images.SetKeyName(56, "de-LU.gif");
            this.imageList1.Images.SetKeyName(57, "el.gif");
            this.imageList1.Images.SetKeyName(58, "el-GR.gif");
            this.imageList1.Images.SetKeyName(59, "empty.png");
            this.imageList1.Images.SetKeyName(60, "en.gif");
            this.imageList1.Images.SetKeyName(61, "en-AU.gif");
            this.imageList1.Images.SetKeyName(62, "en-BZ.gif");
            this.imageList1.Images.SetKeyName(63, "en-CA.gif");
            this.imageList1.Images.SetKeyName(64, "en-GB.gif");
            this.imageList1.Images.SetKeyName(65, "en-IE.gif");
            this.imageList1.Images.SetKeyName(66, "en-JM.gif");
            this.imageList1.Images.SetKeyName(67, "en-NZ.gif");
            this.imageList1.Images.SetKeyName(68, "en-PH.gif");
            this.imageList1.Images.SetKeyName(69, "en-TT.gif");
            this.imageList1.Images.SetKeyName(70, "en-US.gif");
            this.imageList1.Images.SetKeyName(71, "en-ZW.gif");
            this.imageList1.Images.SetKeyName(72, "es.gif");
            this.imageList1.Images.SetKeyName(73, "es-AR.gif");
            this.imageList1.Images.SetKeyName(74, "es-BO.gif");
            this.imageList1.Images.SetKeyName(75, "es-CL.gif");
            this.imageList1.Images.SetKeyName(76, "es-CO.gif");
            this.imageList1.Images.SetKeyName(77, "es-CR.gif");
            this.imageList1.Images.SetKeyName(78, "es-DO.gif");
            this.imageList1.Images.SetKeyName(79, "es-EC.gif");
            this.imageList1.Images.SetKeyName(80, "es-ES.gif");
            this.imageList1.Images.SetKeyName(81, "es-GT.gif");
            this.imageList1.Images.SetKeyName(82, "es-HN.gif");
            this.imageList1.Images.SetKeyName(83, "es-MX.gif");
            this.imageList1.Images.SetKeyName(84, "es-NI.gif");
            this.imageList1.Images.SetKeyName(85, "es-PA.gif");
            this.imageList1.Images.SetKeyName(86, "es-PE.gif");
            this.imageList1.Images.SetKeyName(87, "es-PR.gif");
            this.imageList1.Images.SetKeyName(88, "es-PY.gif");
            this.imageList1.Images.SetKeyName(89, "es-SV.gif");
            this.imageList1.Images.SetKeyName(90, "es-UY.gif");
            this.imageList1.Images.SetKeyName(91, "es-VE.gif");
            this.imageList1.Images.SetKeyName(92, "et.gif");
            this.imageList1.Images.SetKeyName(93, "et-EE.gif");
            this.imageList1.Images.SetKeyName(94, "eu.gif");
            this.imageList1.Images.SetKeyName(95, "eu-ES.gif");
            this.imageList1.Images.SetKeyName(96, "fa.gif");
            this.imageList1.Images.SetKeyName(97, "fa-IR.gif");
            this.imageList1.Images.SetKeyName(98, "fi.gif");
            this.imageList1.Images.SetKeyName(99, "fi-FI.gif");
            this.imageList1.Images.SetKeyName(100, "fl.gif");
            this.imageList1.Images.SetKeyName(101, "fr.gif");
            this.imageList1.Images.SetKeyName(102, "fr-BE.gif");
            this.imageList1.Images.SetKeyName(103, "fr-CA.gif");
            this.imageList1.Images.SetKeyName(104, "fr-CH.gif");
            this.imageList1.Images.SetKeyName(105, "fr-FR.gif");
            this.imageList1.Images.SetKeyName(106, "fr-LU.gif");
            this.imageList1.Images.SetKeyName(107, "ga.gif");
            this.imageList1.Images.SetKeyName(108, "gb.gif");
            this.imageList1.Images.SetKeyName(109, "he.gif");
            this.imageList1.Images.SetKeyName(110, "he-IL.gif");
            this.imageList1.Images.SetKeyName(111, "hi.gif");
            this.imageList1.Images.SetKeyName(112, "hi-IN.gif");
            this.imageList1.Images.SetKeyName(113, "hr.gif");
            this.imageList1.Images.SetKeyName(114, "hr-HR.gif");
            this.imageList1.Images.SetKeyName(115, "hu.gif");
            this.imageList1.Images.SetKeyName(116, "hu-HU.gif");
            this.imageList1.Images.SetKeyName(117, "id-ID.gif");
            this.imageList1.Images.SetKeyName(118, "in.gif");
            this.imageList1.Images.SetKeyName(119, "is.gif");
            this.imageList1.Images.SetKeyName(120, "is-IS.gif");
            this.imageList1.Images.SetKeyName(121, "it.gif");
            this.imageList1.Images.SetKeyName(122, "it-CH.gif");
            this.imageList1.Images.SetKeyName(123, "it-IT.gif");
            this.imageList1.Images.SetKeyName(124, "ja.gif");
            this.imageList1.Images.SetKeyName(125, "ja-JP.gif");
            this.imageList1.Images.SetKeyName(126, "kk.gif");
            this.imageList1.Images.SetKeyName(127, "kk-KZ.gif");
            this.imageList1.Images.SetKeyName(128, "ko.gif");
            this.imageList1.Images.SetKeyName(129, "ko-KR.gif");
            this.imageList1.Images.SetKeyName(130, "ls.gif");
            this.imageList1.Images.SetKeyName(131, "lt.gif");
            this.imageList1.Images.SetKeyName(132, "lt-LT.gif");
            this.imageList1.Images.SetKeyName(133, "lv.gif");
            this.imageList1.Images.SetKeyName(134, "lv-LV.gif");
            this.imageList1.Images.SetKeyName(135, "lx.gif");
            this.imageList1.Images.SetKeyName(136, "mk.gif");
            this.imageList1.Images.SetKeyName(137, "mk-MK.gif");
            this.imageList1.Images.SetKeyName(138, "ms.gif");
            this.imageList1.Images.SetKeyName(139, "ms-MY.gif");
            this.imageList1.Images.SetKeyName(140, "nb-NO.gif");
            this.imageList1.Images.SetKeyName(141, "nl.gif");
            this.imageList1.Images.SetKeyName(142, "nl-BE.gif");
            this.imageList1.Images.SetKeyName(143, "nl-NL.gif");
            this.imageList1.Images.SetKeyName(144, "nn-NO.gif");
            this.imageList1.Images.SetKeyName(145, "no.gif");
            this.imageList1.Images.SetKeyName(146, "pl.gif");
            this.imageList1.Images.SetKeyName(147, "pl-PL.gif");
            this.imageList1.Images.SetKeyName(148, "pt.gif");
            this.imageList1.Images.SetKeyName(149, "pt-BR.gif");
            this.imageList1.Images.SetKeyName(150, "pt-PT.gif");
            this.imageList1.Images.SetKeyName(151, "ro.gif");
            this.imageList1.Images.SetKeyName(152, "ro-RO.gif");
            this.imageList1.Images.SetKeyName(153, "ru.gif");
            this.imageList1.Images.SetKeyName(154, "ru-RU.gif");
            this.imageList1.Images.SetKeyName(155, "sd.gif");
            this.imageList1.Images.SetKeyName(156, "sf.gif");
            this.imageList1.Images.SetKeyName(157, "si.gif");
            this.imageList1.Images.SetKeyName(158, "sk.gif");
            this.imageList1.Images.SetKeyName(159, "sk-SK.gif");
            this.imageList1.Images.SetKeyName(160, "sl.gif");
            this.imageList1.Images.SetKeyName(161, "sl-SI.gif");
            this.imageList1.Images.SetKeyName(162, "sp.gif");
            this.imageList1.Images.SetKeyName(163, "sq.gif");
            this.imageList1.Images.SetKeyName(164, "sq-AL.gif");
            this.imageList1.Images.SetKeyName(165, "sr.gif");
            this.imageList1.Images.SetKeyName(166, "sr-Cyrl-CS.gif");
            this.imageList1.Images.SetKeyName(167, "sr-Latn-CS.gif");
            this.imageList1.Images.SetKeyName(168, "sv.gif");
            this.imageList1.Images.SetKeyName(169, "sv-FI.gif");
            this.imageList1.Images.SetKeyName(170, "sv-SE.gif");
            this.imageList1.Images.SetKeyName(171, "syr-SY.gif");
            this.imageList1.Images.SetKeyName(172, "th.gif");
            this.imageList1.Images.SetKeyName(173, "th-TH.gif");
            this.imageList1.Images.SetKeyName(174, "tr.gif");
            this.imageList1.Images.SetKeyName(175, "tr-TR.gif");
            this.imageList1.Images.SetKeyName(176, "uk.gif");
            this.imageList1.Images.SetKeyName(177, "uk-UA.gif");
            this.imageList1.Images.SetKeyName(178, "ur-PK.gif");
            this.imageList1.Images.SetKeyName(179, "uz-Cyrl-UZ.gif");
            this.imageList1.Images.SetKeyName(180, "uz-Latn-UZ.gif");
            this.imageList1.Images.SetKeyName(181, "vi.gif");
            this.imageList1.Images.SetKeyName(182, "vi-VN.gif");
            this.imageList1.Images.SetKeyName(183, "xh-ZA.gif");
            this.imageList1.Images.SetKeyName(184, "zh.gif");
            this.imageList1.Images.SetKeyName(185, "zh-CN.gif");
            this.imageList1.Images.SetKeyName(186, "zh-HK.gif");
            this.imageList1.Images.SetKeyName(187, "zh-MO.gif");
            this.imageList1.Images.SetKeyName(188, "zh-SG.gif");
            this.imageList1.Images.SetKeyName(189, "zh-TW.gif");
            this.imageList1.Images.SetKeyName(190, "zt.gif");
            // 
            // panel12
            // 
            this.panel12.Controls.Add(this.linkLabel_uncheck_all_listview);
            this.panel12.Controls.Add(this.linkLabel_check_all_listview);
            this.panel12.Controls.Add(this.panel17);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel12.Location = new System.Drawing.Point(0, 76);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(185, 25);
            this.panel12.TabIndex = 2;
            // 
            // linkLabel_uncheck_all_listview
            // 
            this.linkLabel_uncheck_all_listview.AutoSize = true;
            this.linkLabel_uncheck_all_listview.Location = new System.Drawing.Point(75, 6);
            this.linkLabel_uncheck_all_listview.Name = "linkLabel_uncheck_all_listview";
            this.linkLabel_uncheck_all_listview.Size = new System.Drawing.Size(65, 13);
            this.linkLabel_uncheck_all_listview.TabIndex = 2;
            this.linkLabel_uncheck_all_listview.TabStop = true;
            this.linkLabel_uncheck_all_listview.Text = "Uncheck All";
            this.linkLabel_uncheck_all_listview.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_uncheck_all_listview_LinkClicked);
            // 
            // linkLabel_check_all_listview
            // 
            this.linkLabel_check_all_listview.AutoSize = true;
            this.linkLabel_check_all_listview.Location = new System.Drawing.Point(5, 6);
            this.linkLabel_check_all_listview.Name = "linkLabel_check_all_listview";
            this.linkLabel_check_all_listview.Size = new System.Drawing.Size(52, 13);
            this.linkLabel_check_all_listview.TabIndex = 2;
            this.linkLabel_check_all_listview.TabStop = true;
            this.linkLabel_check_all_listview.Text = "Check All";
            this.linkLabel_check_all_listview.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_check_all_listview_LinkClicked);
            // 
            // panel17
            // 
            this.panel17.Controls.Add(this.label_activity_count);
            this.panel17.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel17.Location = new System.Drawing.Point(-15, 0);
            this.panel17.Name = "panel17";
            this.panel17.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.panel17.Size = new System.Drawing.Size(200, 25);
            this.panel17.TabIndex = 1;
            // 
            // label_activity_count
            // 
            this.label_activity_count.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_activity_count.Location = new System.Drawing.Point(0, 0);
            this.label_activity_count.Name = "label_activity_count";
            this.label_activity_count.Size = new System.Drawing.Size(195, 25);
            this.label_activity_count.TabIndex = 0;
            this.label_activity_count.Text = "Total Activities {0}; Selected {1}";
            this.label_activity_count.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.panel14);
            this.panel11.Controls.Add(this.panel13);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel11.Location = new System.Drawing.Point(0, 0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(185, 68);
            this.panel11.TabIndex = 1;
            // 
            // panel14
            // 
            this.panel14.Controls.Add(this.panel15);
            this.panel14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel14.Location = new System.Drawing.Point(0, 30);
            this.panel14.Name = "panel14";
            this.panel14.Padding = new System.Windows.Forms.Padding(5);
            this.panel14.Size = new System.Drawing.Size(185, 38);
            this.panel14.TabIndex = 17;
            // 
            // panel15
            // 
            this.panel15.Controls.Add(this.label1);
            this.panel15.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel15.Location = new System.Drawing.Point(5, 5);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(200, 28);
            this.panel15.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 28);
            this.label1.TabIndex = 16;
            this.label1.Text = "Project Activites";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // panel13
            // 
            this.panel13.Controls.Add(this.label3);
            this.panel13.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel13.Location = new System.Drawing.Point(0, 0);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(185, 30);
            this.panel13.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(185, 30);
            this.label3.TabIndex = 0;
            this.label3.Text = "Select the project activities containing the data that will be included in the re" +
    "ports and click continue.";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox2
            // 
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Location = new System.Drawing.Point(0, 0);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(185, 101);
            this.textBox2.TabIndex = 0;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.textBox3);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(5, 5);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new System.Windows.Forms.Padding(5);
            this.panel6.Size = new System.Drawing.Size(185, 39);
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
            this.textBox3.Size = new System.Drawing.Size(175, 29);
            this.textBox3.TabIndex = 0;
            this.textBox3.Text = "Select Project Activities";
            // 
            // panel_report_options
            // 
            this.panel_report_options.Controls.Add(this.panel7);
            this.panel_report_options.Controls.Add(this.panel8);
            this.panel_report_options.Location = new System.Drawing.Point(311, 202);
            this.panel_report_options.Name = "panel_report_options";
            this.panel_report_options.Padding = new System.Windows.Forms.Padding(5);
            this.panel_report_options.Size = new System.Drawing.Size(203, 143);
            this.panel_report_options.TabIndex = 48;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.panel21);
            this.panel7.Controls.Add(this.panel20);
            this.panel7.Controls.Add(this.panel19);
            this.panel7.Controls.Add(this.panel18);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(5, 44);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(193, 94);
            this.panel7.TabIndex = 1;
            // 
            // panel21
            // 
            this.panel21.Controls.Add(this.groupBox3);
            this.panel21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel21.Location = new System.Drawing.Point(0, 327);
            this.panel21.Name = "panel21";
            this.panel21.Padding = new System.Windows.Forms.Padding(5);
            this.panel21.Size = new System.Drawing.Size(193, 0);
            this.panel21.TabIndex = 17;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox_report_compresson_name);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.checkBox_report_compression);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(5, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(183, 0);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Compression";
            // 
            // textBox_report_compresson_name
            // 
            this.textBox_report_compresson_name.Location = new System.Drawing.Point(140, 51);
            this.textBox_report_compresson_name.Name = "textBox_report_compresson_name";
            this.textBox_report_compresson_name.Size = new System.Drawing.Size(580, 20);
            this.textBox_report_compresson_name.TabIndex = 16;
            this.textBox_report_compresson_name.TextChanged += new System.EventHandler(this.textBox_report_compresson_name_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(42, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Zip Archive Name:";
            // 
            // checkBox_report_compression
            // 
            this.checkBox_report_compression.AutoSize = true;
            this.checkBox_report_compression.Checked = true;
            this.checkBox_report_compression.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_report_compression.Location = new System.Drawing.Point(20, 28);
            this.checkBox_report_compression.Name = "checkBox_report_compression";
            this.checkBox_report_compression.Size = new System.Drawing.Size(400, 17);
            this.checkBox_report_compression.TabIndex = 14;
            this.checkBox_report_compression.Text = "Include the reports in a zip archive once the report creation process is complete" +
    "";
            this.checkBox_report_compression.UseVisualStyleBackColor = true;
            this.checkBox_report_compression.CheckedChanged += new System.EventHandler(this.checkBox_report_compression_CheckedChanged);
            // 
            // panel20
            // 
            this.panel20.Controls.Add(this.groupBox2);
            this.panel20.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel20.Location = new System.Drawing.Point(0, 241);
            this.panel20.Name = "panel20";
            this.panel20.Padding = new System.Windows.Forms.Padding(5);
            this.panel20.Size = new System.Drawing.Size(193, 86);
            this.panel20.TabIndex = 16;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_open_output_folder_when_complete);
            this.groupBox2.Controls.Add(this.button_select_folder);
            this.groupBox2.Controls.Add(this.textBox_output_folder);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(5, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(183, 76);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Output Options";
            // 
            // checkBox_open_output_folder_when_complete
            // 
            this.checkBox_open_output_folder_when_complete.AutoSize = true;
            this.checkBox_open_output_folder_when_complete.Checked = true;
            this.checkBox_open_output_folder_when_complete.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_open_output_folder_when_complete.Location = new System.Drawing.Point(97, 50);
            this.checkBox_open_output_folder_when_complete.Name = "checkBox_open_output_folder_when_complete";
            this.checkBox_open_output_folder_when_complete.Size = new System.Drawing.Size(269, 17);
            this.checkBox_open_output_folder_when_complete.TabIndex = 12;
            this.checkBox_open_output_folder_when_complete.Text = "Open the output folder once the reports are created";
            this.checkBox_open_output_folder_when_complete.UseVisualStyleBackColor = true;
            this.checkBox_open_output_folder_when_complete.CheckedChanged += new System.EventHandler(this.checkBox_open_output_folder_when_complete_CheckedChanged);
            // 
            // button_select_folder
            // 
            this.button_select_folder.Location = new System.Drawing.Point(726, 25);
            this.button_select_folder.Name = "button_select_folder";
            this.button_select_folder.Size = new System.Drawing.Size(93, 23);
            this.button_select_folder.TabIndex = 11;
            this.button_select_folder.Text = "Select Folder...";
            this.button_select_folder.UseVisualStyleBackColor = true;
            this.button_select_folder.Click += new System.EventHandler(this.button_select_folder_Click);
            // 
            // textBox_output_folder
            // 
            this.textBox_output_folder.Location = new System.Drawing.Point(97, 26);
            this.textBox_output_folder.Name = "textBox_output_folder";
            this.textBox_output_folder.Size = new System.Drawing.Size(623, 20);
            this.textBox_output_folder.TabIndex = 10;
            this.textBox_output_folder.TextChanged += new System.EventHandler(this.textBox_output_folder_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Output Folder:";
            // 
            // panel19
            // 
            this.panel19.Controls.Add(this.groupBox1);
            this.panel19.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel19.Location = new System.Drawing.Point(0, 32);
            this.panel19.Name = "panel19";
            this.panel19.Padding = new System.Windows.Forms.Padding(5);
            this.panel19.Size = new System.Drawing.Size(193, 209);
            this.panel19.TabIndex = 15;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_project_activity_export_to_file_xml);
            this.groupBox1.Controls.Add(this.checkBox_project_activity_export_to_file_single);
            this.groupBox1.Controls.Add(this.checkBox_project_activity_export_to_file_excel);
            this.groupBox1.Controls.Add(this.checkBox_project_activity_export_to_file);
            this.groupBox1.Controls.Add(this.checkBox_project_activity_quality_metric_report);
            this.groupBox1.Controls.Add(this.checkBox_project_activity_document_report);
            this.groupBox1.Controls.Add(this.checkBox_project_activity_report_single);
            this.groupBox1.Controls.Add(this.checkBox_project_activity_report);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(5, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(183, 199);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Report Options";
            // 
            // checkBox_project_activity_export_to_file_xml
            // 
            this.checkBox_project_activity_export_to_file_xml.AutoSize = true;
            this.checkBox_project_activity_export_to_file_xml.Checked = true;
            this.checkBox_project_activity_export_to_file_xml.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_project_activity_export_to_file_xml.Location = new System.Drawing.Point(45, 173);
            this.checkBox_project_activity_export_to_file_xml.Name = "checkBox_project_activity_export_to_file_xml";
            this.checkBox_project_activity_export_to_file_xml.Size = new System.Drawing.Size(118, 17);
            this.checkBox_project_activity_export_to_file_xml.TabIndex = 8;
            this.checkBox_project_activity_export_to_file_xml.Text = "Export to xml format";
            this.checkBox_project_activity_export_to_file_xml.UseVisualStyleBackColor = true;
            this.checkBox_project_activity_export_to_file_xml.CheckedChanged += new System.EventHandler(this.checkBox_project_activity_export_to_file_xml_CheckedChanged);
            // 
            // checkBox_project_activity_export_to_file_single
            // 
            this.checkBox_project_activity_export_to_file_single.AutoSize = true;
            this.checkBox_project_activity_export_to_file_single.Checked = true;
            this.checkBox_project_activity_export_to_file_single.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_project_activity_export_to_file_single.Location = new System.Drawing.Point(45, 134);
            this.checkBox_project_activity_export_to_file_single.Name = "checkBox_project_activity_export_to_file_single";
            this.checkBox_project_activity_export_to_file_single.Size = new System.Drawing.Size(198, 17);
            this.checkBox_project_activity_export_to_file_single.TabIndex = 6;
            this.checkBox_project_activity_export_to_file_single.Text = "Include all activities in a single report";
            this.checkBox_project_activity_export_to_file_single.UseVisualStyleBackColor = true;
            this.checkBox_project_activity_export_to_file_single.CheckedChanged += new System.EventHandler(this.checkBox_project_activity_export_to_file_excel_CheckedChanged);
            // 
            // checkBox_project_activity_export_to_file_excel
            // 
            this.checkBox_project_activity_export_to_file_excel.AutoSize = true;
            this.checkBox_project_activity_export_to_file_excel.Checked = true;
            this.checkBox_project_activity_export_to_file_excel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_project_activity_export_to_file_excel.Location = new System.Drawing.Point(45, 153);
            this.checkBox_project_activity_export_to_file_excel.Name = "checkBox_project_activity_export_to_file_excel";
            this.checkBox_project_activity_export_to_file_excel.Size = new System.Drawing.Size(128, 17);
            this.checkBox_project_activity_export_to_file_excel.TabIndex = 7;
            this.checkBox_project_activity_export_to_file_excel.Text = "Export to excel format";
            this.checkBox_project_activity_export_to_file_excel.UseVisualStyleBackColor = true;
            this.checkBox_project_activity_export_to_file_excel.CheckedChanged += new System.EventHandler(this.checkBox_project_activity_export_to_file_excel_CheckedChanged);
            // 
            // checkBox_project_activity_export_to_file
            // 
            this.checkBox_project_activity_export_to_file.AutoSize = true;
            this.checkBox_project_activity_export_to_file.Checked = true;
            this.checkBox_project_activity_export_to_file.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_project_activity_export_to_file.Location = new System.Drawing.Point(20, 112);
            this.checkBox_project_activity_export_to_file.Name = "checkBox_project_activity_export_to_file";
            this.checkBox_project_activity_export_to_file.Size = new System.Drawing.Size(176, 17);
            this.checkBox_project_activity_export_to_file.TabIndex = 5;
            this.checkBox_project_activity_export_to_file.Text = "Export the activity records to file";
            this.checkBox_project_activity_export_to_file.UseVisualStyleBackColor = true;
            this.checkBox_project_activity_export_to_file.CheckedChanged += new System.EventHandler(this.checkBox_project_activity_export_to_file_CheckedChanged);
            // 
            // checkBox_project_activity_quality_metric_report
            // 
            this.checkBox_project_activity_quality_metric_report.AutoSize = true;
            this.checkBox_project_activity_quality_metric_report.Checked = true;
            this.checkBox_project_activity_quality_metric_report.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_project_activity_quality_metric_report.Location = new System.Drawing.Point(20, 91);
            this.checkBox_project_activity_quality_metric_report.Name = "checkBox_project_activity_quality_metric_report";
            this.checkBox_project_activity_quality_metric_report.Size = new System.Drawing.Size(192, 17);
            this.checkBox_project_activity_quality_metric_report.TabIndex = 4;
            this.checkBox_project_activity_quality_metric_report.Text = "Create activity quality metric reports";
            this.checkBox_project_activity_quality_metric_report.UseVisualStyleBackColor = true;
            this.checkBox_project_activity_quality_metric_report.CheckedChanged += new System.EventHandler(this.checkBox_project_activity_quality_metric_report_CheckedChanged);
            // 
            // checkBox_project_activity_document_report
            // 
            this.checkBox_project_activity_document_report.AutoSize = true;
            this.checkBox_project_activity_document_report.Checked = true;
            this.checkBox_project_activity_document_report.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_project_activity_document_report.Location = new System.Drawing.Point(20, 70);
            this.checkBox_project_activity_document_report.Name = "checkBox_project_activity_document_report";
            this.checkBox_project_activity_document_report.Size = new System.Drawing.Size(224, 17);
            this.checkBox_project_activity_document_report.TabIndex = 3;
            this.checkBox_project_activity_document_report.Text = "Create activity document overview reports";
            this.checkBox_project_activity_document_report.UseVisualStyleBackColor = true;
            this.checkBox_project_activity_document_report.CheckedChanged += new System.EventHandler(this.checkBox_project_activity_document_report_CheckedChanged);
            // 
            // checkBox_project_activity_report_single
            // 
            this.checkBox_project_activity_report_single.AutoSize = true;
            this.checkBox_project_activity_report_single.Checked = true;
            this.checkBox_project_activity_report_single.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_project_activity_report_single.Location = new System.Drawing.Point(45, 49);
            this.checkBox_project_activity_report_single.Name = "checkBox_project_activity_report_single";
            this.checkBox_project_activity_report_single.Size = new System.Drawing.Size(198, 17);
            this.checkBox_project_activity_report_single.TabIndex = 2;
            this.checkBox_project_activity_report_single.Text = "Include all activities in a single report";
            this.checkBox_project_activity_report_single.UseVisualStyleBackColor = true;
            this.checkBox_project_activity_report_single.CheckedChanged += new System.EventHandler(this.checkBox_project_activity_report_single_CheckedChanged);
            // 
            // checkBox_project_activity_report
            // 
            this.checkBox_project_activity_report.AutoSize = true;
            this.checkBox_project_activity_report.Checked = true;
            this.checkBox_project_activity_report.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_project_activity_report.Location = new System.Drawing.Point(20, 28);
            this.checkBox_project_activity_report.Name = "checkBox_project_activity_report";
            this.checkBox_project_activity_report.Size = new System.Drawing.Size(128, 17);
            this.checkBox_project_activity_report.TabIndex = 1;
            this.checkBox_project_activity_report.Text = "Create activity reports";
            this.checkBox_project_activity_report.UseVisualStyleBackColor = true;
            this.checkBox_project_activity_report.CheckedChanged += new System.EventHandler(this.checkBox_project_activity_report_CheckedChanged);
            // 
            // panel18
            // 
            this.panel18.Controls.Add(this.textBox4);
            this.panel18.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel18.Location = new System.Drawing.Point(0, 0);
            this.panel18.Name = "panel18";
            this.panel18.Size = new System.Drawing.Size(193, 32);
            this.panel18.TabIndex = 14;
            // 
            // textBox4
            // 
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Location = new System.Drawing.Point(7, 11);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(842, 18);
            this.textBox4.TabIndex = 8;
            this.textBox4.TabStop = false;
            this.textBox4.Text = "Select the type of reports that you would like to create from the selected activi" +
    "ties and choose the ouptut + compression options.   Click on continue to proceed" +
    ".";
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.textBox6);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Location = new System.Drawing.Point(5, 5);
            this.panel8.Name = "panel8";
            this.panel8.Padding = new System.Windows.Forms.Padding(5);
            this.panel8.Size = new System.Drawing.Size(193, 39);
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
            this.textBox6.Size = new System.Drawing.Size(183, 29);
            this.textBox6.TabIndex = 0;
            this.textBox6.Text = "Report Options";
            // 
            // panel_processing
            // 
            this.panel_processing.Controls.Add(this.panel4);
            this.panel_processing.Controls.Add(this.panel10);
            this.panel_processing.Location = new System.Drawing.Point(535, 202);
            this.panel_processing.Name = "panel_processing";
            this.panel_processing.Padding = new System.Windows.Forms.Padding(5);
            this.panel_processing.Size = new System.Drawing.Size(195, 145);
            this.panel_processing.TabIndex = 53;
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
            this.panel4.Size = new System.Drawing.Size(185, 96);
            this.panel4.TabIndex = 6;
            // 
            // label_progress_percentage
            // 
            this.label_progress_percentage.AutoSize = true;
            this.label_progress_percentage.Location = new System.Drawing.Point(817, 127);
            this.label_progress_percentage.Name = "label_progress_percentage";
            this.label_progress_percentage.Size = new System.Drawing.Size(21, 13);
            this.label_progress_percentage.TabIndex = 11;
            this.label_progress_percentage.Text = "0%";
            // 
            // progressBar_import_progress
            // 
            this.progressBar_import_progress.Location = new System.Drawing.Point(33, 122);
            this.progressBar_import_progress.Name = "progressBar_import_progress";
            this.progressBar_import_progress.Size = new System.Drawing.Size(778, 23);
            this.progressBar_import_progress.TabIndex = 10;
            // 
            // label_progress_message
            // 
            this.label_progress_message.AutoSize = true;
            this.label_progress_message.Location = new System.Drawing.Point(33, 102);
            this.label_progress_message.Name = "label_progress_message";
            this.label_progress_message.Size = new System.Drawing.Size(102, 13);
            this.label_progress_message.TabIndex = 9;
            this.label_progress_message.Text = "0 entries processed.";
            // 
            // textBox7
            // 
            this.textBox7.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox7.Location = new System.Drawing.Point(19, 5);
            this.textBox7.Multiline = true;
            this.textBox7.Name = "textBox7";
            this.textBox7.ReadOnly = true;
            this.textBox7.Size = new System.Drawing.Size(608, 36);
            this.textBox7.TabIndex = 8;
            this.textBox7.Text = "The reports are being created.  Click Finish when the process is complete.";
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.textBox8);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel10.Location = new System.Drawing.Point(5, 5);
            this.panel10.Name = "panel10";
            this.panel10.Padding = new System.Windows.Forms.Padding(5);
            this.panel10.Size = new System.Drawing.Size(185, 39);
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
            this.textBox8.Size = new System.Drawing.Size(175, 29);
            this.textBox8.TabIndex = 0;
            this.textBox8.Text = "Creating the reports";
            // 
            // ActivityReportWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1101, 524);
            this.Controls.Add(this.panel_project_activties);
            this.Controls.Add(this.panel_processing);
            this.Controls.Add(this.panel_report_options);
            this.Controls.Add(this.panel_welcome);
            this.Controls.Add(this.panel_leftbar);
            this.Controls.Add(this.panel_button_control);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ActivityReportWizard";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Qualitivity Activity Report Wizard";
            this.Load += new System.EventHandler(this.AddressDetails_Load);
            this.panel_button_control.ResumeLayout(false);
            this.panel33.ResumeLayout(false);
            this.panel_leftbar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel_welcome.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel_project_activties.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel16.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
            this.panel12.ResumeLayout(false);
            this.panel12.PerformLayout();
            this.panel17.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.panel14.ResumeLayout(false);
            this.panel15.ResumeLayout(false);
            this.panel13.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel_report_options.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel21.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel20.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel19.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel18.ResumeLayout(false);
            this.panel18.PerformLayout();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            this.panel_processing.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel_button_control;
        private Panel panel33;
        private Button button_wizard_cancel;
        private Button button_wizard_finish;
        private Button button_wizard_next;
        private Button button_wizard_back;
        private Button button_wizard_help;
        private Panel panel_leftbar;
        private PictureBox pictureBox2;
        private Panel panel_welcome;
        private Panel panel3;
        private Panel panel2;
        private TextBox textBox_w0_title;
        private Panel panel_project_activties;
        private Panel panel5;
        private TextBox textBox2;
        private Panel panel6;
        private TextBox textBox3;
        private Panel panel_report_options;
        private Panel panel7;
        private Panel panel8;
        private TextBox textBox6;
        private Panel panel_processing;
        private Panel panel4;
        private Label label_progress_percentage;
        private ProgressBar progressBar_import_progress;
        private Label label_progress_message;
        private TextBox textBox7;
        private Panel panel10;
        private TextBox textBox8;
        private Panel panel11;
        private Panel panel12;
        private Panel panel14;
        private Panel panel15;
        private Label label1;
        private Panel panel13;
        private Label label3;
        private Panel panel16;
        internal ImageList imageList1;
        private Panel panel17;
        private Label label_activity_count;
        internal ObjectListView objectListView1;
        internal OLVColumn olvColumn_activity_name;
        private OLVColumn olvColumn_source;
        private OLVColumn olvColumn_target;
        internal OLVColumn olvColumn_activity_description;
        private OLVColumn olvColumn_activity_status;
        private OLVColumn olvColumn_billable;
        private OLVColumn olvColumn5;
        private OLVColumn olvColumn6;
        private OLVColumn olvColumn_documents;
        private OLVColumn olvColumn_pem_total;
        private OLVColumn olvColumn_hr_total;
        private OLVColumn olvColumn_custom_total;
        private OLVColumn olvColumn_activity_total;
        private LinkLabel linkLabel_uncheck_all_listview;
        private LinkLabel linkLabel_check_all_listview;
        private TextBox textBox16;
        private Panel panel21;
        private GroupBox groupBox3;
        private TextBox textBox_report_compresson_name;
        private Label label4;
        private CheckBox checkBox_report_compression;
        private Panel panel20;
        private GroupBox groupBox2;
        private CheckBox checkBox_open_output_folder_when_complete;
        private Button button_select_folder;
        private TextBox textBox_output_folder;
        private Label label2;
        private Panel panel19;
        private GroupBox groupBox1;
        private CheckBox checkBox_project_activity_export_to_file_xml;
        private CheckBox checkBox_project_activity_export_to_file_excel;
        private CheckBox checkBox_project_activity_export_to_file;
        private CheckBox checkBox_project_activity_quality_metric_report;
        private CheckBox checkBox_project_activity_document_report;
        private CheckBox checkBox_project_activity_report_single;
        private CheckBox checkBox_project_activity_report;
        private Panel panel18;
        private TextBox textBox4;
        private CheckBox checkBox_project_activity_export_to_file_single;


    }
}