using System.ComponentModel;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace Sdl.Community.Qualitivity.Panels.QualityMetrics
{
    partial class QualitivityRevisionControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            var dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(QualitivityRevisionControl));
            var dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            var dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl_qm = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.panel_container = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGridView_qm = new System.Windows.Forms.DataGridView();
            this.Column_recorded = new System.Windows.Forms.DataGridViewImageColumn();
            this.Column_status = new System.Windows.Forms.DataGridViewImageColumn();
            this.Column_segment_id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_qm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_severity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_content = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addNewQualityMetricToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editQualityMetricToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeQualityMetricToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.activateTheSegmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_add = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_edit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_remove = new System.Windows.Forms.ToolStripButton();
            this.panel_container_status = new System.Windows.Forms.Panel();
            this.panel16 = new System.Windows.Forms.Panel();
            this.panel19 = new System.Windows.Forms.Panel();
            this.label_status_text = new System.Windows.Forms.Label();
            this.panel18 = new System.Windows.Forms.Panel();
            this.panel17 = new System.Windows.Forms.Panel();
            this.comboBox_qm_filter = new System.Windows.Forms.ComboBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel13 = new System.Windows.Forms.Panel();
            this.textBox_comment = new System.Windows.Forms.TextBox();
            this.panel15 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel12 = new System.Windows.Forms.Panel();
            this.textBox_content = new System.Windows.Forms.TextBox();
            this.panel14 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.comboBox_severity = new System.Windows.Forms.ComboBox();
            this.panel11 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.comboBox_qm = new System.Windows.Forms.ComboBox();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.panel9 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.button_insert = new System.Windows.Forms.Button();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.olvColumn_client_name = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_project = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_activity_name = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_activity_description = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_activity_status = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_billable = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_documents = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_pem_total = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_hr_total = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_activity_total = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.miniToolStrip = new System.Windows.Forms.ToolStrip();
            this.panel1.SuspendLayout();
            this.tabControl_qm.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.panel_container.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_qm)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel_container_status.SuspendLayout();
            this.panel16.SuspendLayout();
            this.panel19.SuspendLayout();
            this.panel17.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel13.SuspendLayout();
            this.panel15.SuspendLayout();
            this.panel12.SuspendLayout();
            this.panel14.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AllowDrop = true;
            this.panel1.Controls.Add(this.tabControl_qm);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(418, 345);
            this.panel1.TabIndex = 8;
            // 
            // tabControl_qm
            // 
            this.tabControl_qm.Controls.Add(this.tabPage3);
            this.tabControl_qm.Controls.Add(this.tabPage4);
            this.tabControl_qm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_qm.ImageList = this.imageList2;
            this.tabControl_qm.Location = new System.Drawing.Point(0, 0);
            this.tabControl_qm.Name = "tabControl_qm";
            this.tabControl_qm.SelectedIndex = 0;
            this.tabControl_qm.Size = new System.Drawing.Size(418, 345);
            this.tabControl_qm.TabIndex = 64;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.panel_container);
            this.tabPage3.ImageIndex = 2;
            this.tabPage3.Location = new System.Drawing.Point(4, 23);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(410, 318);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Container ";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // panel_container
            // 
            this.panel_container.AllowDrop = true;
            this.panel_container.BackColor = System.Drawing.Color.Transparent;
            this.panel_container.Controls.Add(this.panel2);
            this.panel_container.Controls.Add(this.toolStrip1);
            this.panel_container.Controls.Add(this.panel_container_status);
            this.panel_container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_container.Location = new System.Drawing.Point(0, 0);
            this.panel_container.Name = "panel_container";
            this.panel_container.Padding = new System.Windows.Forms.Padding(1);
            this.panel_container.Size = new System.Drawing.Size(410, 318);
            this.panel_container.TabIndex = 5;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dataGridView_qm);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(1, 26);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(408, 265);
            this.panel2.TabIndex = 64;
            // 
            // dataGridView_qm
            // 
            this.dataGridView_qm.AllowDrop = true;
            this.dataGridView_qm.AllowUserToAddRows = false;
            this.dataGridView_qm.AllowUserToDeleteRows = false;
            this.dataGridView_qm.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridView_qm.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_qm.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column_recorded,
            this.Column_status,
            this.Column_segment_id,
            this.Column_qm,
            this.Column_severity,
            this.Column_content,
            this.Column_comment});
            this.dataGridView_qm.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridView_qm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_qm.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView_qm.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_qm.Name = "dataGridView_qm";
            this.dataGridView_qm.ReadOnly = true;
            this.dataGridView_qm.RowHeadersVisible = false;
            this.dataGridView_qm.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_qm.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView_qm.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView_qm.ShowCellErrors = false;
            this.dataGridView_qm.ShowEditingIcon = false;
            this.dataGridView_qm.ShowRowErrors = false;
            this.dataGridView_qm.Size = new System.Drawing.Size(408, 265);
            this.dataGridView_qm.TabIndex = 63;
            this.dataGridView_qm.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView_qm_RowsAdded);
            this.dataGridView_qm.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridView_qm_RowsRemoved);
            this.dataGridView_qm.SelectionChanged += new System.EventHandler(this.dataGridView_qm_SelectionChanged);
            this.dataGridView_qm.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dataGridView_qm_SortCompare);
            this.dataGridView_qm.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridView_qm_DragDrop);
            this.dataGridView_qm.DoubleClick += new System.EventHandler(this.dataGridView_qm_DoubleClick);
            this.dataGridView_qm.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridView_qm_KeyUp);
            // 
            // Column_recorded
            // 
            this.Column_recorded.FillWeight = 20F;
            this.Column_recorded.Frozen = true;
            this.Column_recorded.HeaderText = " ";
            this.Column_recorded.Image = ((System.Drawing.Image)(resources.GetObject("Column_recorded.Image")));
            this.Column_recorded.Name = "Column_recorded";
            this.Column_recorded.ReadOnly = true;
            this.Column_recorded.Width = 20;
            // 
            // Column_status
            // 
            this.Column_status.FillWeight = 20F;
            this.Column_status.Frozen = true;
            this.Column_status.HeaderText = " ";
            this.Column_status.Image = ((System.Drawing.Image)(resources.GetObject("Column_status.Image")));
            this.Column_status.Name = "Column_status";
            this.Column_status.ReadOnly = true;
            this.Column_status.Width = 20;
            // 
            // Column_segment_id
            // 
            dataGridViewCellStyle1.Format = "N0";
            dataGridViewCellStyle1.NullValue = null;
            this.Column_segment_id.DefaultCellStyle = dataGridViewCellStyle1;
            this.Column_segment_id.FillWeight = 50F;
            this.Column_segment_id.HeaderText = "SID";
            this.Column_segment_id.Name = "Column_segment_id";
            this.Column_segment_id.ReadOnly = true;
            this.Column_segment_id.Width = 45;
            // 
            // Column_qm
            // 
            this.Column_qm.HeaderText = "QM Type";
            this.Column_qm.Name = "Column_qm";
            this.Column_qm.ReadOnly = true;
            this.Column_qm.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column_qm.ToolTipText = "Quality Metrics";
            // 
            // Column_severity
            // 
            this.Column_severity.FillWeight = 55F;
            this.Column_severity.HeaderText = "Severity";
            this.Column_severity.Name = "Column_severity";
            this.Column_severity.ReadOnly = true;
            this.Column_severity.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column_severity.ToolTipText = "Severity";
            this.Column_severity.Width = 55;
            // 
            // Column_content
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Column_content.DefaultCellStyle = dataGridViewCellStyle2;
            this.Column_content.FillWeight = 125F;
            this.Column_content.HeaderText = "Content";
            this.Column_content.MinimumWidth = 60;
            this.Column_content.Name = "Column_content";
            this.Column_content.ReadOnly = true;
            // 
            // Column_comment
            // 
            this.Column_comment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Column_comment.DefaultCellStyle = dataGridViewCellStyle3;
            this.Column_comment.HeaderText = "Comment";
            this.Column_comment.Name = "Column_comment";
            this.Column_comment.ReadOnly = true;
            this.Column_comment.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewQualityMetricToolStripMenuItem,
            this.editQualityMetricToolStripMenuItem,
            this.removeQualityMetricToolStripMenuItem,
            this.toolStripSeparator1,
            this.activateTheSegmentToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(200, 98);
            // 
            // addNewQualityMetricToolStripMenuItem
            // 
            this.addNewQualityMetricToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addNewQualityMetricToolStripMenuItem.Image")));
            this.addNewQualityMetricToolStripMenuItem.Name = "addNewQualityMetricToolStripMenuItem";
            this.addNewQualityMetricToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.addNewQualityMetricToolStripMenuItem.Text = "Add new Quality Metric";
            this.addNewQualityMetricToolStripMenuItem.Click += new System.EventHandler(this.addNewQualityMetricToolStripMenuItem_Click);
            // 
            // editQualityMetricToolStripMenuItem
            // 
            this.editQualityMetricToolStripMenuItem.Enabled = false;
            this.editQualityMetricToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editQualityMetricToolStripMenuItem.Image")));
            this.editQualityMetricToolStripMenuItem.Name = "editQualityMetricToolStripMenuItem";
            this.editQualityMetricToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.editQualityMetricToolStripMenuItem.Text = "Edit Quality Metric";
            this.editQualityMetricToolStripMenuItem.Click += new System.EventHandler(this.editQualityMetricToolStripMenuItem_Click);
            // 
            // removeQualityMetricToolStripMenuItem
            // 
            this.removeQualityMetricToolStripMenuItem.Enabled = false;
            this.removeQualityMetricToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeQualityMetricToolStripMenuItem.Image")));
            this.removeQualityMetricToolStripMenuItem.Name = "removeQualityMetricToolStripMenuItem";
            this.removeQualityMetricToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.removeQualityMetricToolStripMenuItem.Text = "Remove Quality Metric";
            this.removeQualityMetricToolStripMenuItem.Click += new System.EventHandler(this.removeQualityMetricToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(196, 6);
            // 
            // activateTheSegmentToolStripMenuItem
            // 
            this.activateTheSegmentToolStripMenuItem.Name = "activateTheSegmentToolStripMenuItem";
            this.activateTheSegmentToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.activateTheSegmentToolStripMenuItem.Text = "Activate the Segment";
            this.activateTheSegmentToolStripMenuItem.Click += new System.EventHandler(this.activateTheSegmentToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_add,
            this.toolStripButton_edit,
            this.toolStripButton_remove});
            this.toolStrip1.Location = new System.Drawing.Point(1, 1);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(408, 25);
            this.toolStrip1.TabIndex = 62;
            // 
            // toolStripButton_add
            // 
            this.toolStripButton_add.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_add.Image")));
            this.toolStripButton_add.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_add.Name = "toolStripButton_add";
            this.toolStripButton_add.Size = new System.Drawing.Size(49, 22);
            this.toolStripButton_add.Text = "Add";
            this.toolStripButton_add.ToolTipText = "Add new Quality Metric";
            this.toolStripButton_add.Click += new System.EventHandler(this.toolStripButton_add_Click);
            // 
            // toolStripButton_edit
            // 
            this.toolStripButton_edit.Enabled = false;
            this.toolStripButton_edit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_edit.Image")));
            this.toolStripButton_edit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_edit.Name = "toolStripButton_edit";
            this.toolStripButton_edit.Size = new System.Drawing.Size(47, 22);
            this.toolStripButton_edit.Text = "Edit";
            this.toolStripButton_edit.ToolTipText = "Edit Quality Metric";
            this.toolStripButton_edit.Click += new System.EventHandler(this.toolStripButton_edit_Click);
            // 
            // toolStripButton_remove
            // 
            this.toolStripButton_remove.Enabled = false;
            this.toolStripButton_remove.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_remove.Image")));
            this.toolStripButton_remove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_remove.Name = "toolStripButton_remove";
            this.toolStripButton_remove.Size = new System.Drawing.Size(70, 22);
            this.toolStripButton_remove.Text = "Remove";
            this.toolStripButton_remove.ToolTipText = "Remove Quality Metric";
            this.toolStripButton_remove.Click += new System.EventHandler(this.toolStripButton_remove_Click);
            // 
            // panel_container_status
            // 
            this.panel_container_status.BackColor = System.Drawing.Color.Transparent;
            this.panel_container_status.Controls.Add(this.panel16);
            this.panel_container_status.Controls.Add(this.panel17);
            this.panel_container_status.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_container_status.Location = new System.Drawing.Point(1, 291);
            this.panel_container_status.Name = "panel_container_status";
            this.panel_container_status.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.panel_container_status.Size = new System.Drawing.Size(408, 26);
            this.panel_container_status.TabIndex = 0;
            // 
            // panel16
            // 
            this.panel16.Controls.Add(this.panel19);
            this.panel16.Controls.Add(this.panel18);
            this.panel16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel16.Location = new System.Drawing.Point(178, 0);
            this.panel16.Name = "panel16";
            this.panel16.Size = new System.Drawing.Size(230, 26);
            this.panel16.TabIndex = 5;
            // 
            // panel19
            // 
            this.panel19.Controls.Add(this.label_status_text);
            this.panel19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel19.Location = new System.Drawing.Point(35, 0);
            this.panel19.Name = "panel19";
            this.panel19.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.panel19.Size = new System.Drawing.Size(195, 26);
            this.panel19.TabIndex = 3;
            // 
            // label_status_text
            // 
            this.label_status_text.AutoSize = true;
            this.label_status_text.Dock = System.Windows.Forms.DockStyle.Right;
            this.label_status_text.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_status_text.ForeColor = System.Drawing.Color.Gray;
            this.label_status_text.Location = new System.Drawing.Point(176, 6);
            this.label_status_text.Name = "label_status_text";
            this.label_status_text.Size = new System.Drawing.Size(19, 13);
            this.label_status_text.TabIndex = 2;
            this.label_status_text.Text = "    ";
            this.label_status_text.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel18
            // 
            this.panel18.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel18.Location = new System.Drawing.Point(0, 0);
            this.panel18.Name = "panel18";
            this.panel18.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.panel18.Size = new System.Drawing.Size(35, 26);
            this.panel18.TabIndex = 2;
            // 
            // panel17
            // 
            this.panel17.Controls.Add(this.comboBox_qm_filter);
            this.panel17.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel17.Location = new System.Drawing.Point(2, 0);
            this.panel17.Name = "panel17";
            this.panel17.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.panel17.Size = new System.Drawing.Size(176, 26);
            this.panel17.TabIndex = 4;
            // 
            // comboBox_qm_filter
            // 
            this.comboBox_qm_filter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox_qm_filter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_qm_filter.FormattingEnabled = true;
            this.comboBox_qm_filter.Items.AddRange(new object[] {
            "Show All",
            "Filter on Selected Segment"});
            this.comboBox_qm_filter.Location = new System.Drawing.Point(0, 2);
            this.comboBox_qm_filter.Name = "comboBox_qm_filter";
            this.comboBox_qm_filter.Size = new System.Drawing.Size(176, 21);
            this.comboBox_qm_filter.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.panel6);
            this.tabPage4.Controls.Add(this.panel3);
            this.tabPage4.ImageIndex = 0;
            this.tabPage4.Location = new System.Drawing.Point(4, 23);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(410, 318);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Quick Insert ";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.panel13);
            this.panel6.Controls.Add(this.splitter1);
            this.panel6.Controls.Add(this.panel12);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(0, 64);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(410, 254);
            this.panel6.TabIndex = 1;
            // 
            // panel13
            // 
            this.panel13.Controls.Add(this.textBox_comment);
            this.panel13.Controls.Add(this.panel15);
            this.panel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel13.Location = new System.Drawing.Point(0, 48);
            this.panel13.Name = "panel13";
            this.panel13.Padding = new System.Windows.Forms.Padding(0, 0, 2, 2);
            this.panel13.Size = new System.Drawing.Size(410, 206);
            this.panel13.TabIndex = 2;
            // 
            // textBox_comment
            // 
            this.textBox_comment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_comment.Location = new System.Drawing.Point(61, 0);
            this.textBox_comment.Multiline = true;
            this.textBox_comment.Name = "textBox_comment";
            this.textBox_comment.Size = new System.Drawing.Size(347, 204);
            this.textBox_comment.TabIndex = 7;
            // 
            // panel15
            // 
            this.panel15.Controls.Add(this.label4);
            this.panel15.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel15.Location = new System.Drawing.Point(0, 0);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(61, 204);
            this.panel15.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Comment:";
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 44);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(410, 4);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // panel12
            // 
            this.panel12.Controls.Add(this.textBox_content);
            this.panel12.Controls.Add(this.panel14);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel12.Location = new System.Drawing.Point(0, 0);
            this.panel12.MinimumSize = new System.Drawing.Size(0, 24);
            this.panel12.Name = "panel12";
            this.panel12.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.panel12.Size = new System.Drawing.Size(410, 44);
            this.panel12.TabIndex = 0;
            // 
            // textBox_content
            // 
            this.textBox_content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_content.Location = new System.Drawing.Point(61, 0);
            this.textBox_content.Multiline = true;
            this.textBox_content.Name = "textBox_content";
            this.textBox_content.Size = new System.Drawing.Size(347, 44);
            this.textBox_content.TabIndex = 5;
            // 
            // panel14
            // 
            this.panel14.Controls.Add(this.label3);
            this.panel14.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel14.Location = new System.Drawing.Point(0, 0);
            this.panel14.Name = "panel14";
            this.panel14.Size = new System.Drawing.Size(61, 44);
            this.panel14.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Content:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(410, 64);
            this.panel3.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.panel10);
            this.panel5.Controls.Add(this.panel8);
            this.panel5.Controls.Add(this.panel7);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(313, 64);
            this.panel5.TabIndex = 9;
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.comboBox_severity);
            this.panel10.Controls.Add(this.panel11);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel10.Location = new System.Drawing.Point(0, 38);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(313, 26);
            this.panel10.TabIndex = 2;
            // 
            // comboBox_severity
            // 
            this.comboBox_severity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox_severity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_severity.FormattingEnabled = true;
            this.comboBox_severity.Location = new System.Drawing.Point(61, 0);
            this.comboBox_severity.Name = "comboBox_severity";
            this.comboBox_severity.Size = new System.Drawing.Size(252, 21);
            this.comboBox_severity.TabIndex = 3;
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.label2);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel11.Location = new System.Drawing.Point(0, 0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(61, 26);
            this.panel11.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Severity:";
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.comboBox_qm);
            this.panel8.Controls.Add(this.panel9);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Location = new System.Drawing.Point(0, 12);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(313, 26);
            this.panel8.TabIndex = 1;
            // 
            // comboBox_qm
            // 
            this.comboBox_qm.ContextMenuStrip = this.contextMenuStrip2;
            this.comboBox_qm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox_qm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_qm.FormattingEnabled = true;
            this.comboBox_qm.Location = new System.Drawing.Point(61, 0);
            this.comboBox_qm.Name = "comboBox_qm";
            this.comboBox_qm.Size = new System.Drawing.Size(252, 21);
            this.comboBox_qm.TabIndex = 1;
            this.comboBox_qm.SelectedIndexChanged += new System.EventHandler(this.comboBox_qm_SelectedIndexChanged);
            this.comboBox_qm.Click += new System.EventHandler(this.comboBox_qm_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStrip2.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip2_ItemClicked);
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.label1);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel9.Location = new System.Drawing.Point(0, 0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(61, 26);
            this.panel9.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "QM Type:";
            // 
            // panel7
            // 
            this.panel7.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel7.Location = new System.Drawing.Point(0, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(313, 12);
            this.panel7.TabIndex = 0;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.button_insert);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(313, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(97, 64);
            this.panel4.TabIndex = 8;
            // 
            // button_insert
            // 
            this.button_insert.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_insert.ImageIndex = 3;
            this.button_insert.ImageList = this.imageList2;
            this.button_insert.Location = new System.Drawing.Point(9, 11);
            this.button_insert.Name = "button_insert";
            this.button_insert.Size = new System.Drawing.Size(75, 23);
            this.button_insert.TabIndex = 8;
            this.button_insert.Text = "Add";
            this.button_insert.UseVisualStyleBackColor = true;
            this.button_insert.Click += new System.EventHandler(this.button_insert_Click);
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "Lightning-32(1).png");
            this.imageList2.Images.SetKeyName(1, "Lightning-32.png");
            this.imageList2.Images.SetKeyName(2, "List-32.png");
            this.imageList2.Images.SetKeyName(3, "Badge-Tick-32.png");
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "empty");
            this.imageList1.Images.SetKeyName(1, "Flag-Violet1");
            this.imageList1.Images.SetKeyName(2, "Flag-Blue1");
            this.imageList1.Images.SetKeyName(3, "comment");
            this.imageList1.Images.SetKeyName(4, "Flag-Green1");
            this.imageList1.Images.SetKeyName(5, "Flag-Yellow1");
            this.imageList1.Images.SetKeyName(6, "Flag-Yellow1");
            this.imageList1.Images.SetKeyName(7, "Flag-Blue");
            this.imageList1.Images.SetKeyName(8, "Flag-Green");
            this.imageList1.Images.SetKeyName(9, "Flag-Violet");
            this.imageList1.Images.SetKeyName(10, "Flag-Yellow");
            // 
            // olvColumn_client_name
            // 
            this.olvColumn_client_name.AspectName = "client_name";
            this.olvColumn_client_name.CellPadding = null;
            this.olvColumn_client_name.DisplayIndex = 0;
            this.olvColumn_client_name.Text = "Client";
            this.olvColumn_client_name.Width = 100;
            // 
            // olvColumn_project
            // 
            this.olvColumn_project.AspectName = "tp_name";
            this.olvColumn_project.CellPadding = null;
            this.olvColumn_project.DisplayIndex = 1;
            this.olvColumn_project.Text = "Project";
            this.olvColumn_project.Width = 110;
            // 
            // olvColumn_activity_name
            // 
            this.olvColumn_activity_name.AspectName = "name";
            this.olvColumn_activity_name.CellPadding = null;
            this.olvColumn_activity_name.DisplayIndex = 2;
            this.olvColumn_activity_name.HeaderImageKey = "(none)";
            this.olvColumn_activity_name.ImageAspectName = "";
            this.olvColumn_activity_name.Text = "Activity Name";
            this.olvColumn_activity_name.Width = 180;
            // 
            // olvColumn_activity_description
            // 
            this.olvColumn_activity_description.AspectName = "desc";
            this.olvColumn_activity_description.CellPadding = null;
            this.olvColumn_activity_description.DisplayIndex = 3;
            this.olvColumn_activity_description.Text = "Description";
            this.olvColumn_activity_description.Width = 80;
            // 
            // olvColumn_activity_status
            // 
            this.olvColumn_activity_status.AspectName = "status";
            this.olvColumn_activity_status.CellPadding = null;
            this.olvColumn_activity_status.DisplayIndex = 4;
            this.olvColumn_activity_status.Text = "Status";
            this.olvColumn_activity_status.Width = 78;
            // 
            // olvColumn_billable
            // 
            this.olvColumn_billable.AspectName = "billable";
            this.olvColumn_billable.CellPadding = null;
            this.olvColumn_billable.DisplayIndex = 5;
            this.olvColumn_billable.Text = "Billable";
            this.olvColumn_billable.Width = 75;
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "started";
            this.olvColumn5.CellPadding = null;
            this.olvColumn5.DisplayIndex = 6;
            this.olvColumn5.HeaderImageKey = "started";
            this.olvColumn5.Text = "From";
            this.olvColumn5.Width = 110;
            // 
            // olvColumn6
            // 
            this.olvColumn6.AspectName = "stopped";
            this.olvColumn6.CellPadding = null;
            this.olvColumn6.DisplayIndex = 7;
            this.olvColumn6.HeaderImageKey = "started";
            this.olvColumn6.Text = "To";
            this.olvColumn6.Width = 110;
            // 
            // olvColumn_documents
            // 
            this.olvColumn_documents.CellPadding = null;
            this.olvColumn_documents.DisplayIndex = 8;
            this.olvColumn_documents.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_documents.Text = "Documents";
            this.olvColumn_documents.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_documents.Width = 70;
            // 
            // olvColumn_pem_total
            // 
            this.olvColumn_pem_total.AspectName = "";
            this.olvColumn_pem_total.CellPadding = null;
            this.olvColumn_pem_total.DisplayIndex = 9;
            this.olvColumn_pem_total.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_pem_total.Text = "Language Rate Total";
            this.olvColumn_pem_total.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_pem_total.Width = 125;
            // 
            // olvColumn_hr_total
            // 
            this.olvColumn_hr_total.AspectName = "";
            this.olvColumn_hr_total.CellPadding = null;
            this.olvColumn_hr_total.DisplayIndex = 10;
            this.olvColumn_hr_total.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_hr_total.Text = "Hourly Rate Total";
            this.olvColumn_hr_total.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_hr_total.Width = 100;
            // 
            // olvColumn_activity_total
            // 
            this.olvColumn_activity_total.CellPadding = null;
            this.olvColumn_activity_total.DisplayIndex = 11;
            this.olvColumn_activity_total.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_activity_total.Text = "Activity Total";
            this.olvColumn_activity_total.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.olvColumn_activity_total.Width = 90;
            // 
            // miniToolStrip
            // 
            this.miniToolStrip.AutoSize = false;
            this.miniToolStrip.BackColor = System.Drawing.Color.Transparent;
            this.miniToolStrip.CanOverflow = false;
            this.miniToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.miniToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.miniToolStrip.Location = new System.Drawing.Point(175, 3);
            this.miniToolStrip.Name = "miniToolStrip";
            this.miniToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.miniToolStrip.Size = new System.Drawing.Size(559, 25);
            this.miniToolStrip.TabIndex = 62;
            // 
            // StudioTimeTrackerRevisionControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "StudioTimeTrackerRevisionControl";
            this.Size = new System.Drawing.Size(418, 345);
            this.panel1.ResumeLayout(false);
            this.tabControl_qm.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.panel_container.ResumeLayout(false);
            this.panel_container.PerformLayout();
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_qm)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel_container_status.ResumeLayout(false);
            this.panel16.ResumeLayout(false);
            this.panel19.ResumeLayout(false);
            this.panel19.PerformLayout();
            this.panel17.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel13.ResumeLayout(false);
            this.panel13.PerformLayout();
            this.panel15.ResumeLayout(false);
            this.panel15.PerformLayout();
            this.panel12.ResumeLayout(false);
            this.panel12.PerformLayout();
            this.panel14.ResumeLayout(false);
            this.panel14.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.panel11.PerformLayout();
            this.panel8.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        internal OLVColumn olvColumn_client_name;
        private OLVColumn olvColumn_project;
        internal OLVColumn olvColumn_activity_name;
        internal OLVColumn olvColumn_activity_description;
        private OLVColumn olvColumn_activity_status;
        private OLVColumn olvColumn_billable;
        private OLVColumn olvColumn5;
        private OLVColumn olvColumn6;
        private OLVColumn olvColumn_documents;
        private OLVColumn olvColumn_pem_total;
        private OLVColumn olvColumn_hr_total;
        private OLVColumn olvColumn_activity_total;
        internal ImageList imageList1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem addNewQualityMetricToolStripMenuItem;
        private ToolStripMenuItem editQualityMetricToolStripMenuItem;
        private ToolStripMenuItem removeQualityMetricToolStripMenuItem;
        private TabControl tabControl_qm;
        private TabPage tabPage3;
        private Panel panel_container;
        private Panel panel_container_status;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButton_add;
        private ToolStripButton toolStripButton_edit;
        private ToolStripButton toolStripButton_remove;
        private TabPage tabPage4;
        private ToolStrip miniToolStrip;
        internal DataGridView dataGridView_qm;
        private Panel panel2;
        private ImageList imageList2;
        private Panel panel3;
        private Panel panel5;
        internal ComboBox comboBox_severity;
        internal ComboBox comboBox_qm;
        private Label label2;
        private Label label1;
        private Panel panel4;
        private Button button_insert;
        private Panel panel6;
        private Panel panel10;
        private Panel panel11;
        private Panel panel8;
        private Panel panel9;
        private Panel panel7;
        private Panel panel13;
        private Splitter splitter1;
        private Panel panel12;
        private Panel panel15;
        private Label label4;
        private Panel panel14;
        private Label label3;
        internal TextBox textBox_content;
        internal TextBox textBox_comment;
        private Panel panel16;
        private Panel panel17;
        internal ComboBox comboBox_qm_filter;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem activateTheSegmentToolStripMenuItem;
        private Panel panel19;
        private Label label_status_text;
        private Panel panel18;
        private ContextMenuStrip contextMenuStrip2;
        private DataGridViewImageColumn Column_recorded;
        private DataGridViewImageColumn Column_status;
        private DataGridViewTextBoxColumn Column_segment_id;
        private DataGridViewTextBoxColumn Column_qm;
        private DataGridViewTextBoxColumn Column_severity;
        private DataGridViewTextBoxColumn Column_content;
        private DataGridViewTextBoxColumn Column_comment;



    }
}
