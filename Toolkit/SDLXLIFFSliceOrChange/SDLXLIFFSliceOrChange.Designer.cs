using SDLXLIFFSliceOrChange.Data;
using System.Windows.Forms;

namespace SDLXLIFFSliceOrChange
{
    partial class SdlxliffSliceOrChange
    {

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
        public void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SdlxliffSliceOrChange));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.scrollablePanel = new System.Windows.Forms.Panel();
            this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panelCommands = new System.Windows.Forms.Panel();
            this.groupClear = new System.Windows.Forms.GroupBox();
            this.btnClearit = new System.Windows.Forms.Button();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.lblClearit = new System.Windows.Forms.Label();
            this.groupChange = new System.Windows.Forms.GroupBox();
            this.groupChangeToStatusOr = new System.Windows.Forms.GroupBox();
            this.ckCopySourceToTarget = new System.Windows.Forms.CheckBox();
            this.groupChangeLocked = new System.Windows.Forms.GroupBox();
            this.ckChangeToUnlocked = new System.Windows.Forms.RadioButton();
            this.ckChangeToLocked = new System.Windows.Forms.RadioButton();
            this.groupChangeTranslationStatus = new System.Windows.Forms.GroupBox();
            this.ckChangeToSignedOff = new System.Windows.Forms.RadioButton();
            this.ckChangeToSignOffRejected = new System.Windows.Forms.RadioButton();
            this.ckChangeToTranslationApproved = new System.Windows.Forms.RadioButton();
            this.ckChangeToTranslationRejected = new System.Windows.Forms.RadioButton();
            this.ckChangeToTranslated = new System.Windows.Forms.RadioButton();
            this.ckChangeToDraft = new System.Windows.Forms.RadioButton();
            this.ckChangeToNotTranslated = new System.Windows.Forms.RadioButton();
            this.btnChangeit = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.labelChangeComments = new System.Windows.Forms.Label();
            this.groupSlice = new System.Windows.Forms.GroupBox();
            this.ckMerge = new System.Windows.Forms.CheckBox();
            this.btnSliceit = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelSliceComments = new System.Windows.Forms.Label();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.CancelButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.pageStatuses = new System.Windows.Forms.TabPage();
            this.btnReverseSelection = new System.Windows.Forms.Button();
            this.groupDocumentStructure = new System.Windows.Forms.GroupBox();
            this.listDocumentStructure = new System.Windows.Forms.ListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelGenerateDSIComments = new System.Windows.Forms.Label();
            this.btnGenerateDSI = new System.Windows.Forms.Button();
            this.groupSystem = new System.Windows.Forms.GroupBox();
            this.ckPropagated = new System.Windows.Forms.CheckBox();
            this.ckSystemTranslationMemory = new System.Windows.Forms.CheckBox();
            this.ckSystemMachineTranslation = new System.Windows.Forms.CheckBox();
            this.groupTranslationOrigin = new System.Windows.Forms.GroupBox();
            this.ckNeuralMachineTranslation = new System.Windows.Forms.CheckBox();
            this.ckAutoPropagated = new System.Windows.Forms.CheckBox();
            this.ckAutomatedTranslation = new System.Windows.Forms.CheckBox();
            this.ckInteractive = new System.Windows.Forms.CheckBox();
            this.ckTranslationMemory = new System.Windows.Forms.CheckBox();
            this.groupScore = new System.Windows.Forms.GroupBox();
            this.txtMatchValuesExpression = new System.Windows.Forms.TextBox();
            this.ckMatchValues = new System.Windows.Forms.CheckBox();
            this.ckContextMatch = new System.Windows.Forms.CheckBox();
            this.ckPerfectMatch = new System.Windows.Forms.CheckBox();
            this.groupStatusesLocked = new System.Windows.Forms.GroupBox();
            this.ckUnlocked = new System.Windows.Forms.CheckBox();
            this.ckLocked = new System.Windows.Forms.CheckBox();
            this.groupStatusesTranslationStatus = new System.Windows.Forms.GroupBox();
            this.ckSignedOff = new System.Windows.Forms.CheckBox();
            this.ckSignOffRejected = new System.Windows.Forms.CheckBox();
            this.ckTranslationApproved = new System.Windows.Forms.CheckBox();
            this.ckTranslationRejected = new System.Windows.Forms.CheckBox();
            this.ckTranslated = new System.Windows.Forms.CheckBox();
            this.ckDraft = new System.Windows.Forms.CheckBox();
            this.ckNotTranslated = new System.Windows.Forms.CheckBox();
            this.pageSearch = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnExpandSearchResults = new System.Windows.Forms.Button();
            this.gridSearchResults = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnExpandStats = new System.Windows.Forms.Button();
            this.statsDataGridView = new System.Windows.Forms.DataGridView();
            this.btnFindAll = new System.Windows.Forms.Button();
            this.groupOptions = new System.Windows.Forms.GroupBox();
            this.ckSearchInTags = new System.Windows.Forms.CheckBox();
            this.ckSearchRegEx = new System.Windows.Forms.CheckBox();
            this.ckSearchMatchWholeWord = new System.Windows.Forms.CheckBox();
            this.ckSearchMatchCase = new System.Windows.Forms.CheckBox();
            this.groupSearch = new System.Windows.Forms.GroupBox();
            this.cmbOperator = new System.Windows.Forms.ComboBox();
            this.txtTargetSearch = new System.Windows.Forms.TextBox();
            this.labelSearchTarget = new System.Windows.Forms.Label();
            this.txtSourceSearch = new System.Windows.Forms.TextBox();
            this.labelSearchSource = new System.Windows.Forms.Label();
            this.pageReplace = new System.Windows.Forms.TabPage();
            this.groupReplace = new System.Windows.Forms.GroupBox();
            this.btnExpandReplaceResults = new System.Windows.Forms.Button();
            this.gridReplaceResults = new System.Windows.Forms.DataGridView();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnFindAllInReplace = new System.Windows.Forms.Button();
            this.btnReplace = new System.Windows.Forms.Button();
            this.txtReplaceTargetReplace = new System.Windows.Forms.TextBox();
            this.groupReplaceOptions = new System.Windows.Forms.GroupBox();
            this.ckReplaceUseRegEx = new System.Windows.Forms.CheckBox();
            this.ckReplaceMatchWholeWord = new System.Windows.Forms.CheckBox();
            this.ckReplaceMatchCase = new System.Windows.Forms.CheckBox();
            this.lblReplaceTargetReplace = new System.Windows.Forms.Label();
            this.txtReplaceTargetSearch = new System.Windows.Forms.TextBox();
            this.lblReplaceTargetSearch = new System.Windows.Forms.Label();
            this.txtReplaceSourceReplace = new System.Windows.Forms.TextBox();
            this.lblReplaceSourceReplace = new System.Windows.Forms.Label();
            this.txtReplaceSourceSearch = new System.Windows.Forms.TextBox();
            this.lblReplaceSourceSearch = new System.Windows.Forms.Label();
            this.browseTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbLanguages = new System.Windows.Forms.ComboBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnSelectFolder = new System.Windows.Forms.ToolStripButton();
            this.btnSelectSDLXLIFFFiles = new System.Windows.Forms.ToolStripButton();
            this.btnSelectProjectFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.browseLabel = new System.Windows.Forms.Label();
            this.gridXLIFFFiles = new System.Windows.Forms.DataGridView();
            this.colPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.selectFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.selectProjectFile = new System.Windows.Forms.OpenFileDialog();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.xLIFFFiles = new SDLXLIFFSliceOrChange.Data.XLIFFFiles();
            this.filesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.scrollablePanel.SuspendLayout();
            this.mainTableLayoutPanel.SuspendLayout();
            this.panelCommands.SuspendLayout();
            this.groupClear.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.groupChange.SuspendLayout();
            this.groupChangeToStatusOr.SuspendLayout();
            this.groupChangeLocked.SuspendLayout();
            this.groupChangeTranslationStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupSlice.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelStatus.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.pageStatuses.SuspendLayout();
            this.groupDocumentStructure.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupSystem.SuspendLayout();
            this.groupTranslationOrigin.SuspendLayout();
            this.groupScore.SuspendLayout();
            this.groupStatusesLocked.SuspendLayout();
            this.groupStatusesTranslationStatus.SuspendLayout();
            this.pageSearch.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridSearchResults)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statsDataGridView)).BeginInit();
            this.groupOptions.SuspendLayout();
            this.groupSearch.SuspendLayout();
            this.pageReplace.SuspendLayout();
            this.groupReplace.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridReplaceResults)).BeginInit();
            this.groupReplaceOptions.SuspendLayout();
            this.browseTableLayoutPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridXLIFFFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xLIFFFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.filesBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // scrollablePanel
            // 
            this.scrollablePanel.AutoScroll = true;
            this.scrollablePanel.Controls.Add(this.mainTableLayoutPanel);
            this.scrollablePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scrollablePanel.Location = new System.Drawing.Point(0, 0);
            this.scrollablePanel.Margin = new System.Windows.Forms.Padding(5);
            this.scrollablePanel.Name = "scrollablePanel";
            this.scrollablePanel.Size = new System.Drawing.Size(1435, 1853);
            this.scrollablePanel.TabIndex = 0;
            // 
            // mainTableLayoutPanel
            // 
            this.mainTableLayoutPanel.AutoSize = true;
            this.mainTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mainTableLayoutPanel.ColumnCount = 1;
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainTableLayoutPanel.Controls.Add(this.panelCommands, 0, 2);
            this.mainTableLayoutPanel.Controls.Add(this.panelStatus, 0, 3);
            this.mainTableLayoutPanel.Controls.Add(this.tabControl1, 0, 1);
            this.mainTableLayoutPanel.Controls.Add(this.browseTableLayoutPanel, 0, 0);
            this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayoutPanel.Margin = new System.Windows.Forms.Padding(5);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.RowCount = 4;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(1435, 1853);
            this.mainTableLayoutPanel.TabIndex = 11;
            // 
            // panelCommands
            // 
            this.panelCommands.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.panelCommands.Controls.Add(this.groupClear);
            this.panelCommands.Controls.Add(this.groupChange);
            this.panelCommands.Controls.Add(this.groupSlice);
            this.panelCommands.Location = new System.Drawing.Point(11, 945);
            this.panelCommands.Margin = new System.Windows.Forms.Padding(5);
            this.panelCommands.Name = "panelCommands";
            this.panelCommands.Size = new System.Drawing.Size(1412, 418);
            this.panelCommands.TabIndex = 9;
            // 
            // groupClear
            // 
            this.groupClear.Controls.Add(this.btnClearit);
            this.groupClear.Controls.Add(this.pictureBox3);
            this.groupClear.Controls.Add(this.lblClearit);
            this.groupClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupClear.ForeColor = System.Drawing.Color.Red;
            this.groupClear.Location = new System.Drawing.Point(1125, 18);
            this.groupClear.Margin = new System.Windows.Forms.Padding(5);
            this.groupClear.Name = "groupClear";
            this.groupClear.Padding = new System.Windows.Forms.Padding(5);
            this.groupClear.Size = new System.Drawing.Size(280, 396);
            this.groupClear.TabIndex = 6;
            this.groupClear.TabStop = false;
            this.groupClear.Text = "Clear";
            // 
            // btnClearit
            // 
            this.btnClearit.BackColor = System.Drawing.SystemColors.Control;
            this.btnClearit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearit.ForeColor = System.Drawing.Color.Red;
            this.btnClearit.Location = new System.Drawing.Point(10, 292);
            this.btnClearit.Margin = new System.Windows.Forms.Padding(5);
            this.btnClearit.Name = "btnClearit";
            this.btnClearit.Size = new System.Drawing.Size(228, 86);
            this.btnClearit.TabIndex = 2;
            this.btnClearit.Text = "Clearit!";
            this.btnClearit.UseVisualStyleBackColor = false;
            this.btnClearit.Click += new System.EventHandler(this.btnClearit_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.InitialImage = null;
            this.pictureBox3.Location = new System.Drawing.Point(10, 210);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(5);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(222, 75);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 1;
            this.pictureBox3.TabStop = false;
            // 
            // lblClearit
            // 
            this.lblClearit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblClearit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClearit.ForeColor = System.Drawing.Color.Blue;
            this.lblClearit.Location = new System.Drawing.Point(5, 31);
            this.lblClearit.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblClearit.Name = "lblClearit";
            this.lblClearit.Size = new System.Drawing.Size(270, 360);
            this.lblClearit.TabIndex = 0;
            this.lblClearit.Text = "Click Clearit! to clear all the translated segments based on your selection crite" +
    "ria.";
            // 
            // groupChange
            // 
            this.groupChange.Controls.Add(this.groupChangeToStatusOr);
            this.groupChange.Controls.Add(this.btnChangeit);
            this.groupChange.Controls.Add(this.pictureBox2);
            this.groupChange.Controls.Add(this.labelChangeComments);
            this.groupChange.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupChange.ForeColor = System.Drawing.Color.Red;
            this.groupChange.Location = new System.Drawing.Point(304, 10);
            this.groupChange.Margin = new System.Windows.Forms.Padding(5);
            this.groupChange.Name = "groupChange";
            this.groupChange.Padding = new System.Windows.Forms.Padding(5);
            this.groupChange.Size = new System.Drawing.Size(810, 402);
            this.groupChange.TabIndex = 5;
            this.groupChange.TabStop = false;
            this.groupChange.Text = "Change";
            // 
            // groupChangeToStatusOr
            // 
            this.groupChangeToStatusOr.Controls.Add(this.ckCopySourceToTarget);
            this.groupChangeToStatusOr.Controls.Add(this.groupChangeLocked);
            this.groupChangeToStatusOr.Controls.Add(this.groupChangeTranslationStatus);
            this.groupChangeToStatusOr.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupChangeToStatusOr.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupChangeToStatusOr.Location = new System.Drawing.Point(10, 33);
            this.groupChangeToStatusOr.Margin = new System.Windows.Forms.Padding(5);
            this.groupChangeToStatusOr.Name = "groupChangeToStatusOr";
            this.groupChangeToStatusOr.Padding = new System.Windows.Forms.Padding(5);
            this.groupChangeToStatusOr.Size = new System.Drawing.Size(530, 359);
            this.groupChangeToStatusOr.TabIndex = 0;
            this.groupChangeToStatusOr.TabStop = false;
            this.groupChangeToStatusOr.Text = "Change to Status and/or Lock value";
            // 
            // ckCopySourceToTarget
            // 
            this.ckCopySourceToTarget.AutoSize = true;
            this.ckCopySourceToTarget.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckCopySourceToTarget.Location = new System.Drawing.Point(282, 156);
            this.ckCopySourceToTarget.Margin = new System.Windows.Forms.Padding(5);
            this.ckCopySourceToTarget.Name = "ckCopySourceToTarget";
            this.ckCopySourceToTarget.Size = new System.Drawing.Size(224, 29);
            this.ckCopySourceToTarget.TabIndex = 2;
            this.ckCopySourceToTarget.Text = "Copy source to target";
            this.ckCopySourceToTarget.UseVisualStyleBackColor = true;
            // 
            // groupChangeLocked
            // 
            this.groupChangeLocked.Controls.Add(this.ckChangeToUnlocked);
            this.groupChangeLocked.Controls.Add(this.ckChangeToLocked);
            this.groupChangeLocked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupChangeLocked.Location = new System.Drawing.Point(271, 35);
            this.groupChangeLocked.Margin = new System.Windows.Forms.Padding(5);
            this.groupChangeLocked.Name = "groupChangeLocked";
            this.groupChangeLocked.Padding = new System.Windows.Forms.Padding(5);
            this.groupChangeLocked.Size = new System.Drawing.Size(245, 110);
            this.groupChangeLocked.TabIndex = 1;
            this.groupChangeLocked.TabStop = false;
            this.groupChangeLocked.Text = "Locked / Unlocked";
            // 
            // ckChangeToUnlocked
            // 
            this.ckChangeToUnlocked.AutoSize = true;
            this.ckChangeToUnlocked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckChangeToUnlocked.Location = new System.Drawing.Point(10, 70);
            this.ckChangeToUnlocked.Margin = new System.Windows.Forms.Padding(5);
            this.ckChangeToUnlocked.Name = "ckChangeToUnlocked";
            this.ckChangeToUnlocked.Size = new System.Drawing.Size(119, 29);
            this.ckChangeToUnlocked.TabIndex = 3;
            this.ckChangeToUnlocked.Text = "Unlocked";
            this.ckChangeToUnlocked.UseVisualStyleBackColor = true;
            this.ckChangeToUnlocked.CheckedChanged += new System.EventHandler(this.ckChangeToNotTranslated_CheckedChanged);
            this.ckChangeToUnlocked.Click += new System.EventHandler(this.ckChangeToNotTranslated_Click);
            // 
            // ckChangeToLocked
            // 
            this.ckChangeToLocked.AutoSize = true;
            this.ckChangeToLocked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckChangeToLocked.Location = new System.Drawing.Point(10, 33);
            this.ckChangeToLocked.Margin = new System.Windows.Forms.Padding(5);
            this.ckChangeToLocked.Name = "ckChangeToLocked";
            this.ckChangeToLocked.Size = new System.Drawing.Size(101, 29);
            this.ckChangeToLocked.TabIndex = 2;
            this.ckChangeToLocked.Text = "Locked";
            this.ckChangeToLocked.UseVisualStyleBackColor = true;
            this.ckChangeToLocked.CheckedChanged += new System.EventHandler(this.ckChangeToNotTranslated_CheckedChanged);
            this.ckChangeToLocked.Click += new System.EventHandler(this.ckChangeToNotTranslated_Click);
            // 
            // groupChangeTranslationStatus
            // 
            this.groupChangeTranslationStatus.Controls.Add(this.ckChangeToSignedOff);
            this.groupChangeTranslationStatus.Controls.Add(this.ckChangeToSignOffRejected);
            this.groupChangeTranslationStatus.Controls.Add(this.ckChangeToTranslationApproved);
            this.groupChangeTranslationStatus.Controls.Add(this.ckChangeToTranslationRejected);
            this.groupChangeTranslationStatus.Controls.Add(this.ckChangeToTranslated);
            this.groupChangeTranslationStatus.Controls.Add(this.ckChangeToDraft);
            this.groupChangeTranslationStatus.Controls.Add(this.ckChangeToNotTranslated);
            this.groupChangeTranslationStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupChangeTranslationStatus.Location = new System.Drawing.Point(10, 35);
            this.groupChangeTranslationStatus.Margin = new System.Windows.Forms.Padding(5);
            this.groupChangeTranslationStatus.Name = "groupChangeTranslationStatus";
            this.groupChangeTranslationStatus.Padding = new System.Windows.Forms.Padding(5);
            this.groupChangeTranslationStatus.Size = new System.Drawing.Size(250, 313);
            this.groupChangeTranslationStatus.TabIndex = 0;
            this.groupChangeTranslationStatus.TabStop = false;
            this.groupChangeTranslationStatus.Text = "Translation Status";
            // 
            // ckChangeToSignedOff
            // 
            this.ckChangeToSignedOff.AutoSize = true;
            this.ckChangeToSignedOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckChangeToSignedOff.Location = new System.Drawing.Point(10, 233);
            this.ckChangeToSignedOff.Margin = new System.Windows.Forms.Padding(5);
            this.ckChangeToSignedOff.Name = "ckChangeToSignedOff";
            this.ckChangeToSignedOff.Size = new System.Drawing.Size(130, 29);
            this.ckChangeToSignedOff.TabIndex = 6;
            this.ckChangeToSignedOff.Text = "Signed Off";
            this.ckChangeToSignedOff.UseVisualStyleBackColor = true;
            this.ckChangeToSignedOff.CheckedChanged += new System.EventHandler(this.ckChangeToNotTranslated_CheckedChanged);
            this.ckChangeToSignedOff.Click += new System.EventHandler(this.ckChangeToNotTranslated_Click);
            // 
            // ckChangeToSignOffRejected
            // 
            this.ckChangeToSignOffRejected.AutoSize = true;
            this.ckChangeToSignOffRejected.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckChangeToSignOffRejected.Location = new System.Drawing.Point(10, 273);
            this.ckChangeToSignOffRejected.Margin = new System.Windows.Forms.Padding(5);
            this.ckChangeToSignOffRejected.Name = "ckChangeToSignOffRejected";
            this.ckChangeToSignOffRejected.Size = new System.Drawing.Size(186, 29);
            this.ckChangeToSignOffRejected.TabIndex = 5;
            this.ckChangeToSignOffRejected.Text = "Sign-off Rejected";
            this.ckChangeToSignOffRejected.UseVisualStyleBackColor = true;
            this.ckChangeToSignOffRejected.CheckedChanged += new System.EventHandler(this.ckChangeToNotTranslated_CheckedChanged);
            this.ckChangeToSignOffRejected.Click += new System.EventHandler(this.ckChangeToNotTranslated_Click);
            // 
            // ckChangeToTranslationApproved
            // 
            this.ckChangeToTranslationApproved.AutoSize = true;
            this.ckChangeToTranslationApproved.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckChangeToTranslationApproved.Location = new System.Drawing.Point(10, 191);
            this.ckChangeToTranslationApproved.Margin = new System.Windows.Forms.Padding(5);
            this.ckChangeToTranslationApproved.Name = "ckChangeToTranslationApproved";
            this.ckChangeToTranslationApproved.Size = new System.Drawing.Size(224, 29);
            this.ckChangeToTranslationApproved.TabIndex = 4;
            this.ckChangeToTranslationApproved.Text = "Translation Approved";
            this.ckChangeToTranslationApproved.UseVisualStyleBackColor = true;
            this.ckChangeToTranslationApproved.CheckedChanged += new System.EventHandler(this.ckChangeToNotTranslated_CheckedChanged);
            this.ckChangeToTranslationApproved.Click += new System.EventHandler(this.ckChangeToNotTranslated_Click);
            // 
            // ckChangeToTranslationRejected
            // 
            this.ckChangeToTranslationRejected.AutoSize = true;
            this.ckChangeToTranslationRejected.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckChangeToTranslationRejected.Location = new System.Drawing.Point(10, 150);
            this.ckChangeToTranslationRejected.Margin = new System.Windows.Forms.Padding(5);
            this.ckChangeToTranslationRejected.Name = "ckChangeToTranslationRejected";
            this.ckChangeToTranslationRejected.Size = new System.Drawing.Size(215, 29);
            this.ckChangeToTranslationRejected.TabIndex = 3;
            this.ckChangeToTranslationRejected.Text = "Translation Rejected";
            this.ckChangeToTranslationRejected.UseVisualStyleBackColor = true;
            this.ckChangeToTranslationRejected.CheckedChanged += new System.EventHandler(this.ckChangeToNotTranslated_CheckedChanged);
            this.ckChangeToTranslationRejected.Click += new System.EventHandler(this.ckChangeToNotTranslated_Click);
            // 
            // ckChangeToTranslated
            // 
            this.ckChangeToTranslated.AutoSize = true;
            this.ckChangeToTranslated.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckChangeToTranslated.Location = new System.Drawing.Point(10, 110);
            this.ckChangeToTranslated.Margin = new System.Windows.Forms.Padding(5);
            this.ckChangeToTranslated.Name = "ckChangeToTranslated";
            this.ckChangeToTranslated.Size = new System.Drawing.Size(130, 29);
            this.ckChangeToTranslated.TabIndex = 2;
            this.ckChangeToTranslated.Text = "Translated";
            this.ckChangeToTranslated.UseVisualStyleBackColor = true;
            this.ckChangeToTranslated.CheckedChanged += new System.EventHandler(this.ckChangeToNotTranslated_CheckedChanged);
            this.ckChangeToTranslated.Click += new System.EventHandler(this.ckChangeToNotTranslated_Click);
            // 
            // ckChangeToDraft
            // 
            this.ckChangeToDraft.AutoSize = true;
            this.ckChangeToDraft.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckChangeToDraft.Location = new System.Drawing.Point(10, 70);
            this.ckChangeToDraft.Margin = new System.Windows.Forms.Padding(5);
            this.ckChangeToDraft.Name = "ckChangeToDraft";
            this.ckChangeToDraft.Size = new System.Drawing.Size(78, 29);
            this.ckChangeToDraft.TabIndex = 1;
            this.ckChangeToDraft.Text = "Draft";
            this.ckChangeToDraft.UseVisualStyleBackColor = true;
            this.ckChangeToDraft.CheckedChanged += new System.EventHandler(this.ckChangeToNotTranslated_CheckedChanged);
            this.ckChangeToDraft.Click += new System.EventHandler(this.ckChangeToNotTranslated_Click);
            // 
            // ckChangeToNotTranslated
            // 
            this.ckChangeToNotTranslated.AutoSize = true;
            this.ckChangeToNotTranslated.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckChangeToNotTranslated.Location = new System.Drawing.Point(10, 33);
            this.ckChangeToNotTranslated.Margin = new System.Windows.Forms.Padding(5);
            this.ckChangeToNotTranslated.Name = "ckChangeToNotTranslated";
            this.ckChangeToNotTranslated.Size = new System.Drawing.Size(157, 29);
            this.ckChangeToNotTranslated.TabIndex = 0;
            this.ckChangeToNotTranslated.Text = "Not translated";
            this.ckChangeToNotTranslated.UseVisualStyleBackColor = true;
            this.ckChangeToNotTranslated.CheckedChanged += new System.EventHandler(this.ckChangeToNotTranslated_CheckedChanged);
            this.ckChangeToNotTranslated.Click += new System.EventHandler(this.ckChangeToNotTranslated_Click);
            // 
            // btnChangeit
            // 
            this.btnChangeit.BackColor = System.Drawing.SystemColors.Control;
            this.btnChangeit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChangeit.ForeColor = System.Drawing.Color.Red;
            this.btnChangeit.Location = new System.Drawing.Point(551, 299);
            this.btnChangeit.Margin = new System.Windows.Forms.Padding(5);
            this.btnChangeit.Name = "btnChangeit";
            this.btnChangeit.Size = new System.Drawing.Size(242, 86);
            this.btnChangeit.TabIndex = 5;
            this.btnChangeit.Text = "Changeit!";
            this.btnChangeit.UseVisualStyleBackColor = false;
            this.btnChangeit.Click += new System.EventHandler(this.btnChangeit_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.InitialImage = null;
            this.pictureBox2.Location = new System.Drawing.Point(551, 217);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(242, 75);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // labelChangeComments
            // 
            this.labelChangeComments.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelChangeComments.ForeColor = System.Drawing.Color.Blue;
            this.labelChangeComments.Location = new System.Drawing.Point(556, 35);
            this.labelChangeComments.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelChangeComments.Name = "labelChangeComments";
            this.labelChangeComments.Size = new System.Drawing.Size(236, 144);
            this.labelChangeComments.TabIndex = 3;
            this.labelChangeComments.Text = "Click Changeit! to change the selected segments, to a specific Translation Status" +
    " or to lock or unlock them.";
            // 
            // groupSlice
            // 
            this.groupSlice.Controls.Add(this.ckMerge);
            this.groupSlice.Controls.Add(this.btnSliceit);
            this.groupSlice.Controls.Add(this.pictureBox1);
            this.groupSlice.Controls.Add(this.labelSliceComments);
            this.groupSlice.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupSlice.ForeColor = System.Drawing.Color.Red;
            this.groupSlice.Location = new System.Drawing.Point(7, 12);
            this.groupSlice.Margin = new System.Windows.Forms.Padding(5);
            this.groupSlice.Name = "groupSlice";
            this.groupSlice.Padding = new System.Windows.Forms.Padding(5);
            this.groupSlice.Size = new System.Drawing.Size(287, 401);
            this.groupSlice.TabIndex = 4;
            this.groupSlice.TabStop = false;
            this.groupSlice.Text = "Slice";
            // 
            // ckMerge
            // 
            this.ckMerge.AutoSize = true;
            this.ckMerge.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckMerge.ForeColor = System.Drawing.Color.Black;
            this.ckMerge.Location = new System.Drawing.Point(16, 180);
            this.ckMerge.Margin = new System.Windows.Forms.Padding(5);
            this.ckMerge.Name = "ckMerge";
            this.ckMerge.Size = new System.Drawing.Size(249, 29);
            this.ckMerge.TabIndex = 3;
            this.ckMerge.Text = "Merge result into one file";
            this.ckMerge.UseVisualStyleBackColor = true;
            // 
            // btnSliceit
            // 
            this.btnSliceit.BackColor = System.Drawing.SystemColors.Control;
            this.btnSliceit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSliceit.ForeColor = System.Drawing.Color.Red;
            this.btnSliceit.Location = new System.Drawing.Point(28, 299);
            this.btnSliceit.Margin = new System.Windows.Forms.Padding(5);
            this.btnSliceit.Name = "btnSliceit";
            this.btnSliceit.Size = new System.Drawing.Size(242, 86);
            this.btnSliceit.TabIndex = 2;
            this.btnSliceit.Text = "Sliceit!";
            this.btnSliceit.UseVisualStyleBackColor = false;
            this.btnSliceit.Click += new System.EventHandler(this.btnSliceit_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(28, 217);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(242, 75);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // labelSliceComments
            // 
            this.labelSliceComments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelSliceComments.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSliceComments.ForeColor = System.Drawing.Color.Blue;
            this.labelSliceComments.Location = new System.Drawing.Point(5, 31);
            this.labelSliceComments.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelSliceComments.Name = "labelSliceComments";
            this.labelSliceComments.Size = new System.Drawing.Size(277, 365);
            this.labelSliceComments.TabIndex = 0;
            this.labelSliceComments.Text = "Click Sliceit! to create a new SDLXLIFF file based on your selection criteria. Yo" +
    "u can add this file to your Project or share it with others to handle specific s" +
    "egments first.";
            // 
            // panelStatus
            // 
            this.panelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.panelStatus.Controls.Add(this.CancelButton);
            this.panelStatus.Controls.Add(this.progressBar);
            this.panelStatus.Location = new System.Drawing.Point(5, 1373);
            this.panelStatus.Margin = new System.Windows.Forms.Padding(5);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(1425, 475);
            this.panelStatus.TabIndex = 10;
            // 
            // CancelButton
            // 
            this.CancelButton.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelButton.ForeColor = System.Drawing.Color.Red;
            this.CancelButton.Location = new System.Drawing.Point(1144, 14);
            this.CancelButton.Margin = new System.Windows.Forms.Padding(5);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(271, 59);
            this.CancelButton.TabIndex = 4;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = false;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(23, 37);
            this.progressBar.Margin = new System.Windows.Forms.Padding(5);
            this.progressBar.MarqueeAnimationSpeed = 10;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1097, 24);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 8;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.tabControl1.Controls.Add(this.pageStatuses);
            this.tabControl1.Controls.Add(this.pageSearch);
            this.tabControl1.Controls.Add(this.pageReplace);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(15, 447);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1405, 488);
            this.tabControl1.TabIndex = 8;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // pageStatuses
            // 
            this.pageStatuses.BackColor = System.Drawing.SystemColors.Window;
            this.pageStatuses.Controls.Add(this.btnReverseSelection);
            this.pageStatuses.Controls.Add(this.groupDocumentStructure);
            this.pageStatuses.Controls.Add(this.groupSystem);
            this.pageStatuses.Controls.Add(this.groupTranslationOrigin);
            this.pageStatuses.Controls.Add(this.groupScore);
            this.pageStatuses.Controls.Add(this.groupStatusesLocked);
            this.pageStatuses.Controls.Add(this.groupStatusesTranslationStatus);
            this.pageStatuses.Location = new System.Drawing.Point(4, 34);
            this.pageStatuses.Margin = new System.Windows.Forms.Padding(5);
            this.pageStatuses.Name = "pageStatuses";
            this.pageStatuses.Padding = new System.Windows.Forms.Padding(5);
            this.pageStatuses.Size = new System.Drawing.Size(1397, 450);
            this.pageStatuses.TabIndex = 0;
            this.pageStatuses.Text = "Statuses";
            // 
            // btnReverseSelection
            // 
            this.btnReverseSelection.BackColor = System.Drawing.Color.White;
            this.btnReverseSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReverseSelection.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReverseSelection.Location = new System.Drawing.Point(10, 392);
            this.btnReverseSelection.Margin = new System.Windows.Forms.Padding(5);
            this.btnReverseSelection.Name = "btnReverseSelection";
            this.btnReverseSelection.Size = new System.Drawing.Size(301, 40);
            this.btnReverseSelection.TabIndex = 8;
            this.btnReverseSelection.Text = "Reverse selection";
            this.btnReverseSelection.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReverseSelection.UseVisualStyleBackColor = false;
            this.btnReverseSelection.Click += new System.EventHandler(this.btnReverseSelection_Click);
            // 
            // groupDocumentStructure
            // 
            this.groupDocumentStructure.Controls.Add(this.listDocumentStructure);
            this.groupDocumentStructure.Controls.Add(this.panel2);
            this.groupDocumentStructure.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupDocumentStructure.Location = new System.Drawing.Point(864, 10);
            this.groupDocumentStructure.Margin = new System.Windows.Forms.Padding(5);
            this.groupDocumentStructure.Name = "groupDocumentStructure";
            this.groupDocumentStructure.Padding = new System.Windows.Forms.Padding(5);
            this.groupDocumentStructure.Size = new System.Drawing.Size(494, 378);
            this.groupDocumentStructure.TabIndex = 7;
            this.groupDocumentStructure.TabStop = false;
            this.groupDocumentStructure.Text = "Document Structure";
            // 
            // listDocumentStructure
            // 
            this.listDocumentStructure.BackColor = System.Drawing.SystemColors.Window;
            this.listDocumentStructure.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listDocumentStructure.FormattingEnabled = true;
            this.listDocumentStructure.ItemHeight = 29;
            this.listDocumentStructure.Location = new System.Drawing.Point(5, 30);
            this.listDocumentStructure.Margin = new System.Windows.Forms.Padding(5);
            this.listDocumentStructure.Name = "listDocumentStructure";
            this.listDocumentStructure.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listDocumentStructure.Size = new System.Drawing.Size(224, 323);
            this.listDocumentStructure.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labelGenerateDSIComments);
            this.panel2.Controls.Add(this.btnGenerateDSI);
            this.panel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel2.Location = new System.Drawing.Point(243, 28);
            this.panel2.Margin = new System.Windows.Forms.Padding(5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(198, 345);
            this.panel2.TabIndex = 0;
            // 
            // labelGenerateDSIComments
            // 
            this.labelGenerateDSIComments.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGenerateDSIComments.ForeColor = System.Drawing.Color.Blue;
            this.labelGenerateDSIComments.Location = new System.Drawing.Point(5, 54);
            this.labelGenerateDSIComments.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelGenerateDSIComments.Name = "labelGenerateDSIComments";
            this.labelGenerateDSIComments.Size = new System.Drawing.Size(173, 250);
            this.labelGenerateDSIComments.TabIndex = 1;
            this.labelGenerateDSIComments.Text = "Click \"Generate DSI\" to generate a list of all different types of structure infor" +
    "mation used in the selected files. Hold Ctrl key down and select the ones you wa" +
    "nt with the mouse.";
            // 
            // btnGenerateDSI
            // 
            this.btnGenerateDSI.BackColor = System.Drawing.Color.White;
            this.btnGenerateDSI.Location = new System.Drawing.Point(5, 0);
            this.btnGenerateDSI.Margin = new System.Windows.Forms.Padding(5);
            this.btnGenerateDSI.Name = "btnGenerateDSI";
            this.btnGenerateDSI.Size = new System.Drawing.Size(172, 40);
            this.btnGenerateDSI.TabIndex = 0;
            this.btnGenerateDSI.Text = "Generate DSI";
            this.btnGenerateDSI.UseVisualStyleBackColor = false;
            this.btnGenerateDSI.Click += new System.EventHandler(this.btnGenerateDSI_Click);
            // 
            // groupSystem
            // 
            this.groupSystem.Controls.Add(this.ckPropagated);
            this.groupSystem.Controls.Add(this.ckSystemTranslationMemory);
            this.groupSystem.Controls.Add(this.ckSystemMachineTranslation);
            this.groupSystem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupSystem.Location = new System.Drawing.Point(612, 224);
            this.groupSystem.Margin = new System.Windows.Forms.Padding(5);
            this.groupSystem.Name = "groupSystem";
            this.groupSystem.Padding = new System.Windows.Forms.Padding(5);
            this.groupSystem.Size = new System.Drawing.Size(247, 164);
            this.groupSystem.TabIndex = 6;
            this.groupSystem.TabStop = false;
            this.groupSystem.Text = "System";
            // 
            // ckPropagated
            // 
            this.ckPropagated.AutoSize = true;
            this.ckPropagated.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckPropagated.Location = new System.Drawing.Point(10, 110);
            this.ckPropagated.Margin = new System.Windows.Forms.Padding(5);
            this.ckPropagated.Name = "ckPropagated";
            this.ckPropagated.Size = new System.Drawing.Size(139, 29);
            this.ckPropagated.TabIndex = 4;
            this.ckPropagated.Text = "Propagated";
            this.ckPropagated.UseVisualStyleBackColor = true;
            // 
            // ckSystemTranslationMemory
            // 
            this.ckSystemTranslationMemory.AutoSize = true;
            this.ckSystemTranslationMemory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckSystemTranslationMemory.Location = new System.Drawing.Point(10, 70);
            this.ckSystemTranslationMemory.Margin = new System.Windows.Forms.Padding(5);
            this.ckSystemTranslationMemory.Name = "ckSystemTranslationMemory";
            this.ckSystemTranslationMemory.Size = new System.Drawing.Size(211, 29);
            this.ckSystemTranslationMemory.TabIndex = 3;
            this.ckSystemTranslationMemory.Text = "Translation Memory";
            this.ckSystemTranslationMemory.UseVisualStyleBackColor = true;
            // 
            // ckSystemMachineTranslation
            // 
            this.ckSystemMachineTranslation.AutoSize = true;
            this.ckSystemMachineTranslation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckSystemMachineTranslation.Location = new System.Drawing.Point(10, 33);
            this.ckSystemMachineTranslation.Margin = new System.Windows.Forms.Padding(5);
            this.ckSystemMachineTranslation.Name = "ckSystemMachineTranslation";
            this.ckSystemMachineTranslation.Size = new System.Drawing.Size(215, 29);
            this.ckSystemMachineTranslation.TabIndex = 2;
            this.ckSystemMachineTranslation.Text = "Machine Translation";
            this.ckSystemMachineTranslation.UseVisualStyleBackColor = true;
            // 
            // groupTranslationOrigin
            // 
            this.groupTranslationOrigin.Controls.Add(this.ckNeuralMachineTranslation);
            this.groupTranslationOrigin.Controls.Add(this.ckAutoPropagated);
            this.groupTranslationOrigin.Controls.Add(this.ckAutomatedTranslation);
            this.groupTranslationOrigin.Controls.Add(this.ckInteractive);
            this.groupTranslationOrigin.Controls.Add(this.ckTranslationMemory);
            this.groupTranslationOrigin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupTranslationOrigin.Location = new System.Drawing.Point(612, 10);
            this.groupTranslationOrigin.Margin = new System.Windows.Forms.Padding(5);
            this.groupTranslationOrigin.Name = "groupTranslationOrigin";
            this.groupTranslationOrigin.Padding = new System.Windows.Forms.Padding(5);
            this.groupTranslationOrigin.Size = new System.Drawing.Size(247, 220);
            this.groupTranslationOrigin.TabIndex = 5;
            this.groupTranslationOrigin.TabStop = false;
            this.groupTranslationOrigin.Text = "Translation Origin";
            // 
            // ckNeuralMachineTranslation
            // 
            this.ckNeuralMachineTranslation.AutoSize = true;
            this.ckNeuralMachineTranslation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckNeuralMachineTranslation.Location = new System.Drawing.Point(10, 168);
            this.ckNeuralMachineTranslation.Margin = new System.Windows.Forms.Padding(5);
            this.ckNeuralMachineTranslation.Name = "ckNeuralMachineTranslation";
            this.ckNeuralMachineTranslation.Size = new System.Drawing.Size(130, 29);
            this.ckNeuralMachineTranslation.TabIndex = 4;
            this.ckNeuralMachineTranslation.Text = "Neural MT";
            this.ckNeuralMachineTranslation.UseVisualStyleBackColor = true;
            // 
            // ckAutoPropagated
            // 
            this.ckAutoPropagated.AutoSize = true;
            this.ckAutoPropagated.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckAutoPropagated.Location = new System.Drawing.Point(10, 133);
            this.ckAutoPropagated.Margin = new System.Windows.Forms.Padding(5);
            this.ckAutoPropagated.Name = "ckAutoPropagated";
            this.ckAutoPropagated.Size = new System.Drawing.Size(187, 29);
            this.ckAutoPropagated.TabIndex = 3;
            this.ckAutoPropagated.Text = "Auto-Propagated";
            this.ckAutoPropagated.UseVisualStyleBackColor = true;
            // 
            // ckAutomatedTranslation
            // 
            this.ckAutomatedTranslation.AutoSize = true;
            this.ckAutomatedTranslation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckAutomatedTranslation.Location = new System.Drawing.Point(10, 100);
            this.ckAutomatedTranslation.Margin = new System.Windows.Forms.Padding(5);
            this.ckAutomatedTranslation.Name = "ckAutomatedTranslation";
            this.ckAutomatedTranslation.Size = new System.Drawing.Size(235, 29);
            this.ckAutomatedTranslation.TabIndex = 2;
            this.ckAutomatedTranslation.Text = "Automated Translation";
            this.ckAutomatedTranslation.UseVisualStyleBackColor = true;
            // 
            // ckInteractive
            // 
            this.ckInteractive.AutoSize = true;
            this.ckInteractive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckInteractive.Location = new System.Drawing.Point(10, 65);
            this.ckInteractive.Margin = new System.Windows.Forms.Padding(5);
            this.ckInteractive.Name = "ckInteractive";
            this.ckInteractive.Size = new System.Drawing.Size(127, 29);
            this.ckInteractive.TabIndex = 1;
            this.ckInteractive.Text = "Interactive";
            this.ckInteractive.UseVisualStyleBackColor = true;
            // 
            // ckTranslationMemory
            // 
            this.ckTranslationMemory.AutoSize = true;
            this.ckTranslationMemory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckTranslationMemory.Location = new System.Drawing.Point(10, 33);
            this.ckTranslationMemory.Margin = new System.Windows.Forms.Padding(5);
            this.ckTranslationMemory.Name = "ckTranslationMemory";
            this.ckTranslationMemory.Size = new System.Drawing.Size(211, 29);
            this.ckTranslationMemory.TabIndex = 0;
            this.ckTranslationMemory.Text = "Translation Memory";
            this.ckTranslationMemory.UseVisualStyleBackColor = true;
            // 
            // groupScore
            // 
            this.groupScore.Controls.Add(this.txtMatchValuesExpression);
            this.groupScore.Controls.Add(this.ckMatchValues);
            this.groupScore.Controls.Add(this.ckContextMatch);
            this.groupScore.Controls.Add(this.ckPerfectMatch);
            this.groupScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupScore.Location = new System.Drawing.Point(327, 10);
            this.groupScore.Margin = new System.Windows.Forms.Padding(5);
            this.groupScore.Name = "groupScore";
            this.groupScore.Padding = new System.Windows.Forms.Padding(5);
            this.groupScore.Size = new System.Drawing.Size(268, 203);
            this.groupScore.TabIndex = 4;
            this.groupScore.TabStop = false;
            this.groupScore.Text = "Score";
            // 
            // txtMatchValuesExpression
            // 
            this.txtMatchValuesExpression.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMatchValuesExpression.Location = new System.Drawing.Point(9, 145);
            this.txtMatchValuesExpression.Margin = new System.Windows.Forms.Padding(5);
            this.txtMatchValuesExpression.Name = "txtMatchValuesExpression";
            this.txtMatchValuesExpression.Size = new System.Drawing.Size(210, 29);
            this.txtMatchValuesExpression.TabIndex = 5;
            this.txtMatchValuesExpression.Text = "<95";
            // 
            // ckMatchValues
            // 
            this.ckMatchValues.AutoSize = true;
            this.ckMatchValues.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckMatchValues.Location = new System.Drawing.Point(10, 110);
            this.ckMatchValues.Margin = new System.Windows.Forms.Padding(5);
            this.ckMatchValues.Name = "ckMatchValues";
            this.ckMatchValues.Size = new System.Drawing.Size(172, 29);
            this.ckMatchValues.TabIndex = 4;
            this.ckMatchValues.Text = "Match Value(s)";
            this.ckMatchValues.UseVisualStyleBackColor = true;
            // 
            // ckContextMatch
            // 
            this.ckContextMatch.AutoSize = true;
            this.ckContextMatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckContextMatch.Location = new System.Drawing.Point(10, 70);
            this.ckContextMatch.Margin = new System.Windows.Forms.Padding(5);
            this.ckContextMatch.Name = "ckContextMatch";
            this.ckContextMatch.Size = new System.Drawing.Size(165, 29);
            this.ckContextMatch.TabIndex = 3;
            this.ckContextMatch.Text = "Context Match";
            this.ckContextMatch.UseVisualStyleBackColor = true;
            // 
            // ckPerfectMatch
            // 
            this.ckPerfectMatch.AutoSize = true;
            this.ckPerfectMatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckPerfectMatch.Location = new System.Drawing.Point(10, 33);
            this.ckPerfectMatch.Margin = new System.Windows.Forms.Padding(5);
            this.ckPerfectMatch.Name = "ckPerfectMatch";
            this.ckPerfectMatch.Size = new System.Drawing.Size(158, 29);
            this.ckPerfectMatch.TabIndex = 2;
            this.ckPerfectMatch.Text = "Perfect Match";
            this.ckPerfectMatch.UseVisualStyleBackColor = true;
            // 
            // groupStatusesLocked
            // 
            this.groupStatusesLocked.Controls.Add(this.ckUnlocked);
            this.groupStatusesLocked.Controls.Add(this.ckLocked);
            this.groupStatusesLocked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupStatusesLocked.Location = new System.Drawing.Point(327, 224);
            this.groupStatusesLocked.Margin = new System.Windows.Forms.Padding(5);
            this.groupStatusesLocked.Name = "groupStatusesLocked";
            this.groupStatusesLocked.Padding = new System.Windows.Forms.Padding(5);
            this.groupStatusesLocked.Size = new System.Drawing.Size(268, 164);
            this.groupStatusesLocked.TabIndex = 2;
            this.groupStatusesLocked.TabStop = false;
            this.groupStatusesLocked.Text = "Locked / Unlocked";
            // 
            // ckUnlocked
            // 
            this.ckUnlocked.AutoSize = true;
            this.ckUnlocked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckUnlocked.Location = new System.Drawing.Point(10, 70);
            this.ckUnlocked.Margin = new System.Windows.Forms.Padding(5);
            this.ckUnlocked.Name = "ckUnlocked";
            this.ckUnlocked.Size = new System.Drawing.Size(120, 29);
            this.ckUnlocked.TabIndex = 3;
            this.ckUnlocked.Text = "Unlocked";
            this.ckUnlocked.UseVisualStyleBackColor = true;
            // 
            // ckLocked
            // 
            this.ckLocked.AutoSize = true;
            this.ckLocked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckLocked.Location = new System.Drawing.Point(10, 33);
            this.ckLocked.Margin = new System.Windows.Forms.Padding(5);
            this.ckLocked.Name = "ckLocked";
            this.ckLocked.Size = new System.Drawing.Size(102, 29);
            this.ckLocked.TabIndex = 2;
            this.ckLocked.Text = "Locked";
            this.ckLocked.UseVisualStyleBackColor = true;
            // 
            // groupStatusesTranslationStatus
            // 
            this.groupStatusesTranslationStatus.Controls.Add(this.ckSignedOff);
            this.groupStatusesTranslationStatus.Controls.Add(this.ckSignOffRejected);
            this.groupStatusesTranslationStatus.Controls.Add(this.ckTranslationApproved);
            this.groupStatusesTranslationStatus.Controls.Add(this.ckTranslationRejected);
            this.groupStatusesTranslationStatus.Controls.Add(this.ckTranslated);
            this.groupStatusesTranslationStatus.Controls.Add(this.ckDraft);
            this.groupStatusesTranslationStatus.Controls.Add(this.ckNotTranslated);
            this.groupStatusesTranslationStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupStatusesTranslationStatus.Location = new System.Drawing.Point(10, 10);
            this.groupStatusesTranslationStatus.Margin = new System.Windows.Forms.Padding(5);
            this.groupStatusesTranslationStatus.Name = "groupStatusesTranslationStatus";
            this.groupStatusesTranslationStatus.Padding = new System.Windows.Forms.Padding(5);
            this.groupStatusesTranslationStatus.Size = new System.Drawing.Size(301, 378);
            this.groupStatusesTranslationStatus.TabIndex = 1;
            this.groupStatusesTranslationStatus.TabStop = false;
            this.groupStatusesTranslationStatus.Text = "Translation Status";
            // 
            // ckSignedOff
            // 
            this.ckSignedOff.AutoSize = true;
            this.ckSignedOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckSignedOff.Location = new System.Drawing.Point(10, 271);
            this.ckSignedOff.Margin = new System.Windows.Forms.Padding(5);
            this.ckSignedOff.Name = "ckSignedOff";
            this.ckSignedOff.Size = new System.Drawing.Size(131, 29);
            this.ckSignedOff.TabIndex = 6;
            this.ckSignedOff.Text = "Signed Off";
            this.ckSignedOff.UseVisualStyleBackColor = true;
            // 
            // ckSignOffRejected
            // 
            this.ckSignOffRejected.AutoSize = true;
            this.ckSignOffRejected.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckSignOffRejected.Location = new System.Drawing.Point(10, 231);
            this.ckSignOffRejected.Margin = new System.Windows.Forms.Padding(5);
            this.ckSignOffRejected.Name = "ckSignOffRejected";
            this.ckSignOffRejected.Size = new System.Drawing.Size(187, 29);
            this.ckSignOffRejected.TabIndex = 5;
            this.ckSignOffRejected.Text = "Sign-off Rejected";
            this.ckSignOffRejected.UseVisualStyleBackColor = true;
            // 
            // ckTranslationApproved
            // 
            this.ckTranslationApproved.AutoSize = true;
            this.ckTranslationApproved.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckTranslationApproved.Location = new System.Drawing.Point(10, 191);
            this.ckTranslationApproved.Margin = new System.Windows.Forms.Padding(5);
            this.ckTranslationApproved.Name = "ckTranslationApproved";
            this.ckTranslationApproved.Size = new System.Drawing.Size(225, 29);
            this.ckTranslationApproved.TabIndex = 4;
            this.ckTranslationApproved.Text = "Translation Approved";
            this.ckTranslationApproved.UseVisualStyleBackColor = true;
            // 
            // ckTranslationRejected
            // 
            this.ckTranslationRejected.AutoSize = true;
            this.ckTranslationRejected.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckTranslationRejected.Location = new System.Drawing.Point(10, 150);
            this.ckTranslationRejected.Margin = new System.Windows.Forms.Padding(5);
            this.ckTranslationRejected.Name = "ckTranslationRejected";
            this.ckTranslationRejected.Size = new System.Drawing.Size(216, 29);
            this.ckTranslationRejected.TabIndex = 3;
            this.ckTranslationRejected.Text = "Translation Rejected";
            this.ckTranslationRejected.UseVisualStyleBackColor = true;
            // 
            // ckTranslated
            // 
            this.ckTranslated.AutoSize = true;
            this.ckTranslated.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckTranslated.Location = new System.Drawing.Point(10, 110);
            this.ckTranslated.Margin = new System.Windows.Forms.Padding(5);
            this.ckTranslated.Name = "ckTranslated";
            this.ckTranslated.Size = new System.Drawing.Size(131, 29);
            this.ckTranslated.TabIndex = 2;
            this.ckTranslated.Text = "Translated";
            this.ckTranslated.UseVisualStyleBackColor = true;
            // 
            // ckDraft
            // 
            this.ckDraft.AutoSize = true;
            this.ckDraft.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckDraft.Location = new System.Drawing.Point(10, 70);
            this.ckDraft.Margin = new System.Windows.Forms.Padding(5);
            this.ckDraft.Name = "ckDraft";
            this.ckDraft.Size = new System.Drawing.Size(79, 29);
            this.ckDraft.TabIndex = 1;
            this.ckDraft.Text = "Draft";
            this.ckDraft.UseVisualStyleBackColor = true;
            // 
            // ckNotTranslated
            // 
            this.ckNotTranslated.AutoSize = true;
            this.ckNotTranslated.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckNotTranslated.Location = new System.Drawing.Point(10, 33);
            this.ckNotTranslated.Margin = new System.Windows.Forms.Padding(5);
            this.ckNotTranslated.Name = "ckNotTranslated";
            this.ckNotTranslated.Size = new System.Drawing.Size(158, 29);
            this.ckNotTranslated.TabIndex = 0;
            this.ckNotTranslated.Text = "Not translated";
            this.ckNotTranslated.UseVisualStyleBackColor = true;
            // 
            // pageSearch
            // 
            this.pageSearch.Controls.Add(this.tabControl2);
            this.pageSearch.Controls.Add(this.btnFindAll);
            this.pageSearch.Controls.Add(this.groupOptions);
            this.pageSearch.Controls.Add(this.groupSearch);
            this.pageSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pageSearch.ForeColor = System.Drawing.SystemColors.ControlText;
            this.pageSearch.Location = new System.Drawing.Point(4, 34);
            this.pageSearch.Margin = new System.Windows.Forms.Padding(5);
            this.pageSearch.Name = "pageSearch";
            this.pageSearch.Padding = new System.Windows.Forms.Padding(5);
            this.pageSearch.Size = new System.Drawing.Size(1397, 450);
            this.pageSearch.TabIndex = 1;
            this.pageSearch.Text = "Search";
            this.pageSearch.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Controls.Add(this.tabPage2);
            this.tabControl2.Location = new System.Drawing.Point(296, 163);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1093, 284);
            this.tabControl2.TabIndex = 10;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnExpandSearchResults);
            this.tabPage1.Controls.Add(this.gridSearchResults);
            this.tabPage1.Location = new System.Drawing.Point(4, 33);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1085, 247);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Search Results";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnExpandSearchResults
            // 
            this.btnExpandSearchResults.Image = ((System.Drawing.Image)(resources.GetObject("btnExpandSearchResults.Image")));
            this.btnExpandSearchResults.Location = new System.Drawing.Point(1028, 8);
            this.btnExpandSearchResults.Margin = new System.Windows.Forms.Padding(5);
            this.btnExpandSearchResults.Name = "btnExpandSearchResults";
            this.btnExpandSearchResults.Size = new System.Drawing.Size(52, 47);
            this.btnExpandSearchResults.TabIndex = 10;
            this.btnExpandSearchResults.UseVisualStyleBackColor = true;
            this.btnExpandSearchResults.Click += new System.EventHandler(this.btnExpandSearchResults_Click);
            // 
            // gridSearchResults
            // 
            this.gridSearchResults.AllowUserToAddRows = false;
            this.gridSearchResults.AllowUserToDeleteRows = false;
            this.gridSearchResults.AllowUserToResizeRows = false;
            this.gridSearchResults.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridSearchResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSearchResults.GridColor = System.Drawing.SystemColors.Control;
            this.gridSearchResults.Location = new System.Drawing.Point(8, 4);
            this.gridSearchResults.Margin = new System.Windows.Forms.Padding(5);
            this.gridSearchResults.Name = "gridSearchResults";
            this.gridSearchResults.RowHeadersVisible = false;
            this.gridSearchResults.RowHeadersWidth = 72;
            this.gridSearchResults.Size = new System.Drawing.Size(1081, 238);
            this.gridSearchResults.TabIndex = 7;
            this.gridSearchResults.VirtualMode = true;
            this.gridSearchResults.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.gridSearchResults_CellValueNeeded);
            this.gridSearchResults.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gridSearchResults_DataBindingComplete);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnExpandStats);
            this.tabPage2.Controls.Add(this.statsDataGridView);
            this.tabPage2.Location = new System.Drawing.Point(4, 33);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1085, 247);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Statistics";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnExpandStats
            // 
            this.btnExpandStats.Image = ((System.Drawing.Image)(resources.GetObject("btnExpandStats.Image")));
            this.btnExpandStats.Location = new System.Drawing.Point(1025, 8);
            this.btnExpandStats.Margin = new System.Windows.Forms.Padding(5);
            this.btnExpandStats.Name = "btnExpandStats";
            this.btnExpandStats.Size = new System.Drawing.Size(52, 47);
            this.btnExpandStats.TabIndex = 11;
            this.btnExpandStats.UseVisualStyleBackColor = true;
            this.btnExpandStats.Click += new System.EventHandler(this.btnExpandStats_Click);
            // 
            // statsDataGridView
            // 
            
            // 
            // btnFindAll
            // 
            this.btnFindAll.BackColor = System.Drawing.SystemColors.Window;
            this.btnFindAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFindAll.Location = new System.Drawing.Point(5, 352);
            this.btnFindAll.Margin = new System.Windows.Forms.Padding(5);
            this.btnFindAll.Name = "btnFindAll";
            this.btnFindAll.Size = new System.Drawing.Size(270, 74);
            this.btnFindAll.TabIndex = 8;
            this.btnFindAll.Text = "Find All";
            this.btnFindAll.UseVisualStyleBackColor = false;
            this.btnFindAll.Click += new System.EventHandler(this.btnFindAll_Click);
            // 
            // groupOptions
            // 
            this.groupOptions.Controls.Add(this.ckSearchInTags);
            this.groupOptions.Controls.Add(this.ckSearchRegEx);
            this.groupOptions.Controls.Add(this.ckSearchMatchWholeWord);
            this.groupOptions.Controls.Add(this.ckSearchMatchCase);
            this.groupOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupOptions.Location = new System.Drawing.Point(12, 152);
            this.groupOptions.Margin = new System.Windows.Forms.Padding(5);
            this.groupOptions.Name = "groupOptions";
            this.groupOptions.Padding = new System.Windows.Forms.Padding(5);
            this.groupOptions.Size = new System.Drawing.Size(270, 192);
            this.groupOptions.TabIndex = 6;
            this.groupOptions.TabStop = false;
            this.groupOptions.Text = "Options";
            // 
            // ckSearchInTags
            // 
            this.ckSearchInTags.AutoSize = true;
            this.ckSearchInTags.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckSearchInTags.Location = new System.Drawing.Point(10, 154);
            this.ckSearchInTags.Margin = new System.Windows.Forms.Padding(5);
            this.ckSearchInTags.Name = "ckSearchInTags";
            this.ckSearchInTags.Size = new System.Drawing.Size(163, 29);
            this.ckSearchInTags.TabIndex = 3;
            this.ckSearchInTags.Text = "Search in tags";
            this.ckSearchInTags.UseVisualStyleBackColor = true;
            // 
            // ckSearchRegEx
            // 
            this.ckSearchRegEx.AutoSize = true;
            this.ckSearchRegEx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckSearchRegEx.Location = new System.Drawing.Point(10, 114);
            this.ckSearchRegEx.Margin = new System.Windows.Forms.Padding(5);
            this.ckSearchRegEx.Name = "ckSearchRegEx";
            this.ckSearchRegEx.Size = new System.Drawing.Size(248, 29);
            this.ckSearchRegEx.TabIndex = 2;
            this.ckSearchRegEx.Text = "Use regular expressions";
            this.ckSearchRegEx.UseVisualStyleBackColor = true;
            // 
            // ckSearchMatchWholeWord
            // 
            this.ckSearchMatchWholeWord.AutoSize = true;
            this.ckSearchMatchWholeWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckSearchMatchWholeWord.Location = new System.Drawing.Point(10, 74);
            this.ckSearchMatchWholeWord.Margin = new System.Windows.Forms.Padding(5);
            this.ckSearchMatchWholeWord.Name = "ckSearchMatchWholeWord";
            this.ckSearchMatchWholeWord.Size = new System.Drawing.Size(195, 29);
            this.ckSearchMatchWholeWord.TabIndex = 1;
            this.ckSearchMatchWholeWord.Text = "Match whole word";
            this.ckSearchMatchWholeWord.UseVisualStyleBackColor = true;
            // 
            // ckSearchMatchCase
            // 
            this.ckSearchMatchCase.AutoSize = true;
            this.ckSearchMatchCase.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckSearchMatchCase.Location = new System.Drawing.Point(10, 33);
            this.ckSearchMatchCase.Margin = new System.Windows.Forms.Padding(5);
            this.ckSearchMatchCase.Name = "ckSearchMatchCase";
            this.ckSearchMatchCase.Size = new System.Drawing.Size(139, 29);
            this.ckSearchMatchCase.TabIndex = 0;
            this.ckSearchMatchCase.Text = "Match case";
            this.ckSearchMatchCase.UseVisualStyleBackColor = true;
            // 
            // groupSearch
            // 
            this.groupSearch.Controls.Add(this.cmbOperator);
            this.groupSearch.Controls.Add(this.txtTargetSearch);
            this.groupSearch.Controls.Add(this.labelSearchTarget);
            this.groupSearch.Controls.Add(this.txtSourceSearch);
            this.groupSearch.Controls.Add(this.labelSearchSource);
            this.groupSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupSearch.Location = new System.Drawing.Point(10, 10);
            this.groupSearch.Margin = new System.Windows.Forms.Padding(5);
            this.groupSearch.Name = "groupSearch";
            this.groupSearch.Padding = new System.Windows.Forms.Padding(5);
            this.groupSearch.Size = new System.Drawing.Size(1346, 140);
            this.groupSearch.TabIndex = 6;
            this.groupSearch.TabStop = false;
            this.groupSearch.Text = "Search source OR / AND target";
            // 
            // cmbOperator
            // 
            this.cmbOperator.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbOperator.FormattingEnabled = true;
            this.cmbOperator.Items.AddRange(new object[] {
            "OR",
            "AND"});
            this.cmbOperator.Location = new System.Drawing.Point(688, 56);
            this.cmbOperator.Margin = new System.Windows.Forms.Padding(5);
            this.cmbOperator.Name = "cmbOperator";
            this.cmbOperator.Size = new System.Drawing.Size(92, 32);
            this.cmbOperator.TabIndex = 4;
            // 
            // txtTargetSearch
            // 
            this.txtTargetSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTargetSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTargetSearch.Location = new System.Drawing.Point(793, 56);
            this.txtTargetSearch.Margin = new System.Windows.Forms.Padding(5);
            this.txtTargetSearch.Multiline = true;
            this.txtTargetSearch.Name = "txtTargetSearch";
            this.txtTargetSearch.Size = new System.Drawing.Size(392, 72);
            this.txtTargetSearch.TabIndex = 3;
            // 
            // labelSearchTarget
            // 
            this.labelSearchTarget.AutoSize = true;
            this.labelSearchTarget.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSearchTarget.Location = new System.Drawing.Point(788, 28);
            this.labelSearchTarget.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelSearchTarget.Name = "labelSearchTarget";
            this.labelSearchTarget.Size = new System.Drawing.Size(133, 25);
            this.labelSearchTarget.TabIndex = 2;
            this.labelSearchTarget.Text = "Target search";
            // 
            // txtSourceSearch
            // 
            this.txtSourceSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSourceSearch.Location = new System.Drawing.Point(16, 56);
            this.txtSourceSearch.Margin = new System.Windows.Forms.Padding(5);
            this.txtSourceSearch.Multiline = true;
            this.txtSourceSearch.Name = "txtSourceSearch";
            this.txtSourceSearch.Size = new System.Drawing.Size(658, 72);
            this.txtSourceSearch.TabIndex = 1;
            // 
            // labelSearchSource
            // 
            this.labelSearchSource.AutoSize = true;
            this.labelSearchSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSearchSource.Location = new System.Drawing.Point(10, 28);
            this.labelSearchSource.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelSearchSource.Name = "labelSearchSource";
            this.labelSearchSource.Size = new System.Drawing.Size(139, 25);
            this.labelSearchSource.TabIndex = 0;
            this.labelSearchSource.Text = "Source search";
            // 
            // pageReplace
            // 
            this.pageReplace.Controls.Add(this.groupReplace);
            this.pageReplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pageReplace.ForeColor = System.Drawing.SystemColors.ControlText;
            this.pageReplace.Location = new System.Drawing.Point(4, 34);
            this.pageReplace.Margin = new System.Windows.Forms.Padding(5);
            this.pageReplace.Name = "pageReplace";
            this.pageReplace.Padding = new System.Windows.Forms.Padding(5);
            this.pageReplace.Size = new System.Drawing.Size(1397, 450);
            this.pageReplace.TabIndex = 2;
            this.pageReplace.Text = "Replace";
            this.pageReplace.UseVisualStyleBackColor = true;
            // 
            // groupReplace
            // 
            this.groupReplace.Controls.Add(this.btnExpandReplaceResults);
            this.groupReplace.Controls.Add(this.gridReplaceResults);
            this.groupReplace.Controls.Add(this.btnPreview);
            this.groupReplace.Controls.Add(this.btnFindAllInReplace);
            this.groupReplace.Controls.Add(this.btnReplace);
            this.groupReplace.Controls.Add(this.txtReplaceTargetReplace);
            this.groupReplace.Controls.Add(this.groupReplaceOptions);
            this.groupReplace.Controls.Add(this.lblReplaceTargetReplace);
            this.groupReplace.Controls.Add(this.txtReplaceTargetSearch);
            this.groupReplace.Controls.Add(this.lblReplaceTargetSearch);
            this.groupReplace.Controls.Add(this.txtReplaceSourceReplace);
            this.groupReplace.Controls.Add(this.lblReplaceSourceReplace);
            this.groupReplace.Controls.Add(this.txtReplaceSourceSearch);
            this.groupReplace.Controls.Add(this.lblReplaceSourceSearch);
            this.groupReplace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupReplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupReplace.Location = new System.Drawing.Point(5, 5);
            this.groupReplace.Margin = new System.Windows.Forms.Padding(5);
            this.groupReplace.Name = "groupReplace";
            this.groupReplace.Padding = new System.Windows.Forms.Padding(5);
            this.groupReplace.Size = new System.Drawing.Size(1387, 440);
            this.groupReplace.TabIndex = 6;
            this.groupReplace.TabStop = false;
            this.groupReplace.Text = "Replace in source OR target";
            // 
            // btnExpandReplaceResults
            // 
            this.btnExpandReplaceResults.Image = ((System.Drawing.Image)(resources.GetObject("btnExpandReplaceResults.Image")));
            this.btnExpandReplaceResults.Location = new System.Drawing.Point(1323, 131);
            this.btnExpandReplaceResults.Margin = new System.Windows.Forms.Padding(5);
            this.btnExpandReplaceResults.Name = "btnExpandReplaceResults";
            this.btnExpandReplaceResults.Size = new System.Drawing.Size(47, 51);
            this.btnExpandReplaceResults.TabIndex = 13;
            this.btnExpandReplaceResults.UseVisualStyleBackColor = true;
            this.btnExpandReplaceResults.Click += new System.EventHandler(this.btnExpandReplaceResults_Click);
            // 
            // gridReplaceResults
            // 
            this.gridReplaceResults.AllowUserToAddRows = false;
            this.gridReplaceResults.AllowUserToDeleteRows = false;
            this.gridReplaceResults.AllowUserToResizeRows = false;
            this.gridReplaceResults.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridReplaceResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridReplaceResults.GridColor = System.Drawing.SystemColors.Control;
            this.gridReplaceResults.Location = new System.Drawing.Point(296, 130);
            this.gridReplaceResults.Margin = new System.Windows.Forms.Padding(5);
            this.gridReplaceResults.Name = "gridReplaceResults";
            this.gridReplaceResults.RowHeadersVisible = false;
            this.gridReplaceResults.RowHeadersWidth = 72;
            this.gridReplaceResults.Size = new System.Drawing.Size(1076, 294);
            this.gridReplaceResults.TabIndex = 12;
            this.gridReplaceResults.VirtualMode = true;
            this.gridReplaceResults.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.gridReplaceResults_CellValueNeeded);
            this.gridReplaceResults.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gridReplaceResults_DataBindingComplete);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.Window;
            this.btnPreview.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPreview.Location = new System.Drawing.Point(16, 298);
            this.btnPreview.Margin = new System.Windows.Forms.Padding(5);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(270, 49);
            this.btnPreview.TabIndex = 11;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnFindAllInReplace
            // 
            this.btnFindAllInReplace.BackColor = System.Drawing.SystemColors.Window;
            this.btnFindAllInReplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFindAllInReplace.Location = new System.Drawing.Point(16, 242);
            this.btnFindAllInReplace.Margin = new System.Windows.Forms.Padding(5);
            this.btnFindAllInReplace.Name = "btnFindAllInReplace";
            this.btnFindAllInReplace.Size = new System.Drawing.Size(270, 49);
            this.btnFindAllInReplace.TabIndex = 10;
            this.btnFindAllInReplace.Text = "Find All";
            this.btnFindAllInReplace.UseVisualStyleBackColor = false;
            this.btnFindAllInReplace.Click += new System.EventHandler(this.btnFindAllInReplace_Click);
            // 
            // btnReplace
            // 
            this.btnReplace.BackColor = System.Drawing.SystemColors.Window;
            this.btnReplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReplace.Location = new System.Drawing.Point(16, 357);
            this.btnReplace.Margin = new System.Windows.Forms.Padding(5);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(270, 49);
            this.btnReplace.TabIndex = 8;
            this.btnReplace.Text = "Replace All";
            this.btnReplace.UseVisualStyleBackColor = false;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // txtReplaceTargetReplace
            // 
            this.txtReplaceTargetReplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReplaceTargetReplace.Location = new System.Drawing.Point(1008, 84);
            this.txtReplaceTargetReplace.Margin = new System.Windows.Forms.Padding(5);
            this.txtReplaceTargetReplace.Multiline = true;
            this.txtReplaceTargetReplace.Name = "txtReplaceTargetReplace";
            this.txtReplaceTargetReplace.Size = new System.Drawing.Size(361, 32);
            this.txtReplaceTargetReplace.TabIndex = 9;
            // 
            // groupReplaceOptions
            // 
            this.groupReplaceOptions.Controls.Add(this.ckReplaceUseRegEx);
            this.groupReplaceOptions.Controls.Add(this.ckReplaceMatchWholeWord);
            this.groupReplaceOptions.Controls.Add(this.ckReplaceMatchCase);
            this.groupReplaceOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupReplaceOptions.Location = new System.Drawing.Point(16, 38);
            this.groupReplaceOptions.Margin = new System.Windows.Forms.Padding(5);
            this.groupReplaceOptions.Name = "groupReplaceOptions";
            this.groupReplaceOptions.Padding = new System.Windows.Forms.Padding(5);
            this.groupReplaceOptions.Size = new System.Drawing.Size(270, 196);
            this.groupReplaceOptions.TabIndex = 6;
            this.groupReplaceOptions.TabStop = false;
            this.groupReplaceOptions.Text = "Options";
            // 
            // ckReplaceUseRegEx
            // 
            this.ckReplaceUseRegEx.AutoSize = true;
            this.ckReplaceUseRegEx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckReplaceUseRegEx.Location = new System.Drawing.Point(10, 114);
            this.ckReplaceUseRegEx.Margin = new System.Windows.Forms.Padding(5);
            this.ckReplaceUseRegEx.Name = "ckReplaceUseRegEx";
            this.ckReplaceUseRegEx.Size = new System.Drawing.Size(248, 29);
            this.ckReplaceUseRegEx.TabIndex = 2;
            this.ckReplaceUseRegEx.Text = "Use regular expressions";
            this.ckReplaceUseRegEx.UseVisualStyleBackColor = true;
            // 
            // ckReplaceMatchWholeWord
            // 
            this.ckReplaceMatchWholeWord.AutoSize = true;
            this.ckReplaceMatchWholeWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckReplaceMatchWholeWord.Location = new System.Drawing.Point(10, 74);
            this.ckReplaceMatchWholeWord.Margin = new System.Windows.Forms.Padding(5);
            this.ckReplaceMatchWholeWord.Name = "ckReplaceMatchWholeWord";
            this.ckReplaceMatchWholeWord.Size = new System.Drawing.Size(195, 29);
            this.ckReplaceMatchWholeWord.TabIndex = 1;
            this.ckReplaceMatchWholeWord.Text = "Match whole word";
            this.ckReplaceMatchWholeWord.UseVisualStyleBackColor = true;
            // 
            // ckReplaceMatchCase
            // 
            this.ckReplaceMatchCase.AutoSize = true;
            this.ckReplaceMatchCase.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckReplaceMatchCase.Location = new System.Drawing.Point(10, 33);
            this.ckReplaceMatchCase.Margin = new System.Windows.Forms.Padding(5);
            this.ckReplaceMatchCase.Name = "ckReplaceMatchCase";
            this.ckReplaceMatchCase.Size = new System.Drawing.Size(139, 29);
            this.ckReplaceMatchCase.TabIndex = 0;
            this.ckReplaceMatchCase.Text = "Match case";
            this.ckReplaceMatchCase.UseVisualStyleBackColor = true;
            // 
            // lblReplaceTargetReplace
            // 
            this.lblReplaceTargetReplace.AutoSize = true;
            this.lblReplaceTargetReplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReplaceTargetReplace.Location = new System.Drawing.Point(880, 96);
            this.lblReplaceTargetReplace.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblReplaceTargetReplace.Name = "lblReplaceTargetReplace";
            this.lblReplaceTargetReplace.Size = new System.Drawing.Size(122, 25);
            this.lblReplaceTargetReplace.TabIndex = 8;
            this.lblReplaceTargetReplace.Text = "Replace with";
            // 
            // txtReplaceTargetSearch
            // 
            this.txtReplaceTargetSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReplaceTargetSearch.Location = new System.Drawing.Point(434, 84);
            this.txtReplaceTargetSearch.Margin = new System.Windows.Forms.Padding(5);
            this.txtReplaceTargetSearch.Multiline = true;
            this.txtReplaceTargetSearch.Name = "txtReplaceTargetSearch";
            this.txtReplaceTargetSearch.Size = new System.Drawing.Size(392, 32);
            this.txtReplaceTargetSearch.TabIndex = 7;
            this.txtReplaceTargetSearch.Validating += new System.ComponentModel.CancelEventHandler(this.txtReplaceTargetSearch_Validating);
            // 
            // lblReplaceTargetSearch
            // 
            this.lblReplaceTargetSearch.AutoSize = true;
            this.lblReplaceTargetSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReplaceTargetSearch.Location = new System.Drawing.Point(290, 89);
            this.lblReplaceTargetSearch.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblReplaceTargetSearch.Name = "lblReplaceTargetSearch";
            this.lblReplaceTargetSearch.Size = new System.Drawing.Size(133, 25);
            this.lblReplaceTargetSearch.TabIndex = 6;
            this.lblReplaceTargetSearch.Text = "Target search";
            // 
            // txtReplaceSourceReplace
            // 
            this.txtReplaceSourceReplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReplaceSourceReplace.Location = new System.Drawing.Point(1008, 38);
            this.txtReplaceSourceReplace.Margin = new System.Windows.Forms.Padding(5);
            this.txtReplaceSourceReplace.Multiline = true;
            this.txtReplaceSourceReplace.Name = "txtReplaceSourceReplace";
            this.txtReplaceSourceReplace.Size = new System.Drawing.Size(361, 32);
            this.txtReplaceSourceReplace.TabIndex = 5;
            // 
            // lblReplaceSourceReplace
            // 
            this.lblReplaceSourceReplace.AutoSize = true;
            this.lblReplaceSourceReplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReplaceSourceReplace.Location = new System.Drawing.Point(880, 44);
            this.lblReplaceSourceReplace.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblReplaceSourceReplace.Name = "lblReplaceSourceReplace";
            this.lblReplaceSourceReplace.Size = new System.Drawing.Size(122, 25);
            this.lblReplaceSourceReplace.TabIndex = 4;
            this.lblReplaceSourceReplace.Text = "Replace with";
            // 
            // txtReplaceSourceSearch
            // 
            this.txtReplaceSourceSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReplaceSourceSearch.Location = new System.Drawing.Point(434, 38);
            this.txtReplaceSourceSearch.Margin = new System.Windows.Forms.Padding(5);
            this.txtReplaceSourceSearch.Multiline = true;
            this.txtReplaceSourceSearch.Name = "txtReplaceSourceSearch";
            this.txtReplaceSourceSearch.Size = new System.Drawing.Size(392, 32);
            this.txtReplaceSourceSearch.TabIndex = 1;
            this.txtReplaceSourceSearch.Validating += new System.ComponentModel.CancelEventHandler(this.txtReplaceSourceSearch_Validating);
            // 
            // lblReplaceSourceSearch
            // 
            this.lblReplaceSourceSearch.AutoSize = true;
            this.lblReplaceSourceSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReplaceSourceSearch.Location = new System.Drawing.Point(290, 44);
            this.lblReplaceSourceSearch.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblReplaceSourceSearch.Name = "lblReplaceSourceSearch";
            this.lblReplaceSourceSearch.Size = new System.Drawing.Size(139, 25);
            this.lblReplaceSourceSearch.TabIndex = 0;
            this.lblReplaceSourceSearch.Text = "Source search";
            // 
            // browseTableLayoutPanel
            // 
            this.browseTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.browseTableLayoutPanel.ColumnCount = 1;
            this.browseTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.browseTableLayoutPanel.Controls.Add(this.panel1, 0, 1);
            this.browseTableLayoutPanel.Controls.Add(this.browseLabel, 0, 0);
            this.browseTableLayoutPanel.Controls.Add(this.gridXLIFFFiles, 0, 2);
            this.browseTableLayoutPanel.Location = new System.Drawing.Point(14, 5);
            this.browseTableLayoutPanel.Margin = new System.Windows.Forms.Padding(5);
            this.browseTableLayoutPanel.MaximumSize = new System.Drawing.Size(1407, 480);
            this.browseTableLayoutPanel.Name = "browseTableLayoutPanel";
            this.browseTableLayoutPanel.RowCount = 3;
            this.browseTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.browseTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.browseTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.browseTableLayoutPanel.Size = new System.Drawing.Size(1407, 432);
            this.browseTableLayoutPanel.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.cmbLanguages);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(5, 30);
            this.panel1.Margin = new System.Windows.Forms.Padding(5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1397, 54);
            this.panel1.TabIndex = 5;
            // 
            // cmbLanguages
            // 
            this.cmbLanguages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbLanguages.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbLanguages.FormattingEnabled = true;
            this.cmbLanguages.Location = new System.Drawing.Point(1130, 9);
            this.cmbLanguages.Margin = new System.Windows.Forms.Padding(5);
            this.cmbLanguages.Name = "cmbLanguages";
            this.cmbLanguages.Size = new System.Drawing.Size(264, 32);
            this.cmbLanguages.TabIndex = 2;
            this.cmbLanguages.SelectedIndexChanged += new System.EventHandler(this.cmbLanguages_SelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.CanOverflow = false;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSelectFolder,
            this.btnSelectSDLXLIFFFiles,
            this.btnSelectProjectFile,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.toolStrip1.Size = new System.Drawing.Size(1125, 54);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Image = ((System.Drawing.Image)(resources.GetObject("btnSelectFolder.Image")));
            this.btnSelectFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(181, 48);
            this.btnSelectFolder.Text = "Select folder ...";
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // btnSelectSDLXLIFFFiles
            // 
            this.btnSelectSDLXLIFFFiles.Image = ((System.Drawing.Image)(resources.GetObject("btnSelectSDLXLIFFFiles.Image")));
            this.btnSelectSDLXLIFFFiles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelectSDLXLIFFFiles.Name = "btnSelectSDLXLIFFFiles";
            this.btnSelectSDLXLIFFFiles.Size = new System.Drawing.Size(230, 48);
            this.btnSelectSDLXLIFFFiles.Text = "Select sdlxliff files ...";
            this.btnSelectSDLXLIFFFiles.Click += new System.EventHandler(this.btnSelectSDLXLIFFFiles_Click);
            // 
            // btnSelectProjectFile
            // 
            this.btnSelectProjectFile.Image = ((System.Drawing.Image)(resources.GetObject("btnSelectProjectFile.Image")));
            this.btnSelectProjectFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelectProjectFile.Name = "btnSelectProjectFile";
            this.btnSelectProjectFile.Size = new System.Drawing.Size(225, 48);
            this.btnSelectProjectFile.Text = "Select project file ...";
            this.btnSelectProjectFile.Click += new System.EventHandler(this.btnSelectProjectFile_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(238, 48);
            this.toolStripButton1.Text = "Clear selected files ...";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // browseLabel
            // 
            this.browseLabel.AutoSize = true;
            this.browseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.browseLabel.Location = new System.Drawing.Point(5, 0);
            this.browseLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.browseLabel.Name = "browseLabel";
            this.browseLabel.Size = new System.Drawing.Size(232, 25);
            this.browseLabel.TabIndex = 0;
            this.browseLabel.Text = "Browse for SDLXLIFFs";
            // 
            // gridXLIFFFiles
            // 
            this.gridXLIFFFiles.AllowDrop = true;
            this.gridXLIFFFiles.AllowUserToAddRows = false;
            this.gridXLIFFFiles.AllowUserToDeleteRows = false;
            this.gridXLIFFFiles.AllowUserToOrderColumns = true;
            this.gridXLIFFFiles.AllowUserToResizeRows = false;
            this.gridXLIFFFiles.BackgroundColor = System.Drawing.SystemColors.Control;
            this.gridXLIFFFiles.ColumnHeadersHeight = 40;
            this.gridXLIFFFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPath,
            this.colName,
            this.colSize,
            this.colDate});
            this.gridXLIFFFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridXLIFFFiles.GridColor = System.Drawing.SystemColors.Control;
            this.gridXLIFFFiles.Location = new System.Drawing.Point(5, 94);
            this.gridXLIFFFiles.Margin = new System.Windows.Forms.Padding(5);
            this.gridXLIFFFiles.MinimumSize = new System.Drawing.Size(175, 0);
            this.gridXLIFFFiles.Name = "gridXLIFFFiles";
            this.gridXLIFFFiles.RowHeadersWidth = 72;
            this.gridXLIFFFiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridXLIFFFiles.Size = new System.Drawing.Size(1397, 333);
            this.gridXLIFFFiles.TabIndex = 2;
            this.gridXLIFFFiles.SelectionChanged += new System.EventHandler(this.gridXLIFFFiles_SelectionChanged);
            this.gridXLIFFFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.gridXLIFFFiles_DragDrop);
            this.gridXLIFFFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.gridXLIFFFiles_DragEnter);
            // 
            // colPath
            // 
            this.colPath.DataPropertyName = "Path";
            this.colPath.HeaderText = "Path";
            this.colPath.MinimumWidth = 9;
            this.colPath.Name = "colPath";
            this.colPath.Visible = false;
            this.colPath.Width = 175;
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colName.DataPropertyName = "Name";
            this.colName.HeaderText = "Name";
            this.colName.MinimumWidth = 9;
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // colSize
            // 
            this.colSize.DataPropertyName = "Size";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colSize.DefaultCellStyle = dataGridViewCellStyle2;
            this.colSize.HeaderText = "Size";
            this.colSize.MinimumWidth = 9;
            this.colSize.Name = "colSize";
            this.colSize.ReadOnly = true;
            this.colSize.Width = 175;
            // 
            // colDate
            // 
            this.colDate.DataPropertyName = "Date";
            this.colDate.HeaderText = "Date";
            this.colDate.MinimumWidth = 9;
            this.colDate.Name = "colDate";
            this.colDate.Width = 190;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // xLIFFFiles
            // 
            this.xLIFFFiles.DataSetName = "XLIFFFiles";
            this.xLIFFFiles.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // filesBindingSource
            // 
            this.filesBindingSource.DataMember = "Files";
            this.filesBindingSource.DataSource = this.xLIFFFiles;
            // 
            // SdlxliffSliceOrChange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(168F, 168F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.Controls.Add(this.scrollablePanel);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "SdlxliffSliceOrChange";
            this.Size = new System.Drawing.Size(1435, 1853);
            this.Load += new System.EventHandler(this.SDLXLIFFSliceOrChange_Load);
            this.SizeChanged += new System.EventHandler(this.SDLXLIFFSliceOrChange_SizeChanged);
            this.scrollablePanel.ResumeLayout(false);
            this.scrollablePanel.PerformLayout();
            this.mainTableLayoutPanel.ResumeLayout(false);
            this.panelCommands.ResumeLayout(false);
            this.groupClear.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.groupChange.ResumeLayout(false);
            this.groupChangeToStatusOr.ResumeLayout(false);
            this.groupChangeToStatusOr.PerformLayout();
            this.groupChangeLocked.ResumeLayout(false);
            this.groupChangeLocked.PerformLayout();
            this.groupChangeTranslationStatus.ResumeLayout(false);
            this.groupChangeTranslationStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupSlice.ResumeLayout(false);
            this.groupSlice.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelStatus.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.pageStatuses.ResumeLayout(false);
            this.groupDocumentStructure.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupSystem.ResumeLayout(false);
            this.groupSystem.PerformLayout();
            this.groupTranslationOrigin.ResumeLayout(false);
            this.groupTranslationOrigin.PerformLayout();
            this.groupScore.ResumeLayout(false);
            this.groupScore.PerformLayout();
            this.groupStatusesLocked.ResumeLayout(false);
            this.groupStatusesLocked.PerformLayout();
            this.groupStatusesTranslationStatus.ResumeLayout(false);
            this.groupStatusesTranslationStatus.PerformLayout();
            this.pageSearch.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridSearchResults)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.statsDataGridView)).EndInit();
            this.groupOptions.ResumeLayout(false);
            this.groupOptions.PerformLayout();
            this.groupSearch.ResumeLayout(false);
            this.groupSearch.PerformLayout();
            this.pageReplace.ResumeLayout(false);
            this.groupReplace.ResumeLayout(false);
            this.groupReplace.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridReplaceResults)).EndInit();
            this.groupReplaceOptions.ResumeLayout(false);
            this.groupReplaceOptions.PerformLayout();
            this.browseTableLayoutPanel.ResumeLayout(false);
            this.browseTableLayoutPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridXLIFFFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xLIFFFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.filesBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.FolderBrowserDialog selectFolder;
        public XLIFFFiles xLIFFFiles;
        public System.Windows.Forms.OpenFileDialog selectProjectFile;
        public System.Windows.Forms.BindingSource filesBindingSource;
        private System.ComponentModel.IContainer components;
        public System.Windows.Forms.TabControl tabControl1;
        public System.Windows.Forms.TabPage pageStatuses;
        public System.Windows.Forms.Button btnReverseSelection;
        public System.Windows.Forms.GroupBox groupDocumentStructure;
        public System.Windows.Forms.ListBox listDocumentStructure;
        public System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.Label labelGenerateDSIComments;
        public System.Windows.Forms.Button btnGenerateDSI;
        public System.Windows.Forms.GroupBox groupSystem;
        public System.Windows.Forms.CheckBox ckPropagated;
        public System.Windows.Forms.CheckBox ckSystemTranslationMemory;
        public System.Windows.Forms.CheckBox ckSystemMachineTranslation;
        public System.Windows.Forms.GroupBox groupTranslationOrigin;
        public System.Windows.Forms.CheckBox ckAutoPropagated;
        public System.Windows.Forms.CheckBox ckAutomatedTranslation;
        public System.Windows.Forms.CheckBox ckInteractive;
        public System.Windows.Forms.CheckBox ckTranslationMemory;
        public System.Windows.Forms.GroupBox groupScore;
        public System.Windows.Forms.TextBox txtMatchValuesExpression;
        public System.Windows.Forms.CheckBox ckMatchValues;
        public System.Windows.Forms.CheckBox ckContextMatch;
        public System.Windows.Forms.CheckBox ckPerfectMatch;
        public System.Windows.Forms.GroupBox groupStatusesLocked;
        public System.Windows.Forms.CheckBox ckUnlocked;
        public System.Windows.Forms.CheckBox ckLocked;
        public System.Windows.Forms.GroupBox groupStatusesTranslationStatus;
        public System.Windows.Forms.CheckBox ckSignedOff;
        public System.Windows.Forms.CheckBox ckSignOffRejected;
        public System.Windows.Forms.CheckBox ckTranslationApproved;
        public System.Windows.Forms.CheckBox ckTranslationRejected;
        public System.Windows.Forms.CheckBox ckTranslated;
        public System.Windows.Forms.CheckBox ckDraft;
        public System.Windows.Forms.CheckBox ckNotTranslated;
        public System.Windows.Forms.TabPage pageSearch;
        public System.Windows.Forms.Button btnFindAll;
        public System.Windows.Forms.DataGridView gridSearchResults;
        public System.Windows.Forms.GroupBox groupOptions;
        public System.Windows.Forms.CheckBox ckSearchInTags;
        public System.Windows.Forms.CheckBox ckSearchRegEx;
        public System.Windows.Forms.CheckBox ckSearchMatchWholeWord;
        public System.Windows.Forms.CheckBox ckSearchMatchCase;
        public System.Windows.Forms.GroupBox groupSearch;
        public System.Windows.Forms.ComboBox cmbOperator;
        public System.Windows.Forms.TextBox txtTargetSearch;
        public System.Windows.Forms.Label labelSearchTarget;
        public System.Windows.Forms.TextBox txtSourceSearch;
        public System.Windows.Forms.Label labelSearchSource;
        public System.Windows.Forms.TabPage pageReplace;
        public System.Windows.Forms.GroupBox groupReplace;
        public System.Windows.Forms.Button btnExpandReplaceResults;
        public System.Windows.Forms.DataGridView gridReplaceResults;
        public System.Windows.Forms.Button btnPreview;
        public System.Windows.Forms.Button btnFindAllInReplace;
        public System.Windows.Forms.Button btnReplace;
        public System.Windows.Forms.TextBox txtReplaceTargetReplace;
        public System.Windows.Forms.GroupBox groupReplaceOptions;
        public System.Windows.Forms.CheckBox ckReplaceUseRegEx;
        public System.Windows.Forms.CheckBox ckReplaceMatchWholeWord;
        public System.Windows.Forms.CheckBox ckReplaceMatchCase;
        public System.Windows.Forms.Label lblReplaceTargetReplace;
        public System.Windows.Forms.TextBox txtReplaceTargetSearch;
        public System.Windows.Forms.Label lblReplaceTargetSearch;
        public System.Windows.Forms.TextBox txtReplaceSourceReplace;
        public System.Windows.Forms.Label lblReplaceSourceReplace;
        public System.Windows.Forms.TextBox txtReplaceSourceSearch;
        public System.Windows.Forms.Label lblReplaceSourceSearch;
        private System.Windows.Forms.Panel panelCommands;
        public System.Windows.Forms.GroupBox groupClear;
        public System.Windows.Forms.Button btnClearit;
        public System.Windows.Forms.PictureBox pictureBox3;
        public System.Windows.Forms.Label lblClearit;
        public System.Windows.Forms.GroupBox groupChange;
        public System.Windows.Forms.GroupBox groupChangeToStatusOr;
        private System.Windows.Forms.CheckBox ckCopySourceToTarget;
        public System.Windows.Forms.GroupBox groupChangeLocked;
        public System.Windows.Forms.RadioButton ckChangeToUnlocked;
        public System.Windows.Forms.RadioButton ckChangeToLocked;
        public System.Windows.Forms.GroupBox groupChangeTranslationStatus;
        public System.Windows.Forms.RadioButton ckChangeToSignedOff;
        public System.Windows.Forms.RadioButton ckChangeToSignOffRejected;
        public System.Windows.Forms.RadioButton ckChangeToTranslationApproved;
        public System.Windows.Forms.RadioButton ckChangeToTranslationRejected;
        public System.Windows.Forms.RadioButton ckChangeToTranslated;
        public System.Windows.Forms.RadioButton ckChangeToDraft;
        public System.Windows.Forms.RadioButton ckChangeToNotTranslated;
        public System.Windows.Forms.Button btnChangeit;
        public System.Windows.Forms.PictureBox pictureBox2;
        public System.Windows.Forms.Label labelChangeComments;
        public System.Windows.Forms.GroupBox groupSlice;
        private System.Windows.Forms.CheckBox ckMerge;
        public System.Windows.Forms.Button btnSliceit;
        public System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.Label labelSliceComments;
        private System.Windows.Forms.TableLayoutPanel browseTableLayoutPanel;
        public System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.ComboBox cmbLanguages;
        public System.Windows.Forms.ToolStrip toolStrip1;
        public System.Windows.Forms.ToolStripButton btnSelectFolder;
        public System.Windows.Forms.ToolStripButton btnSelectSDLXLIFFFiles;
        public System.Windows.Forms.ToolStripButton btnSelectProjectFile;
        public System.Windows.Forms.ToolStripButton toolStripButton1;
        public System.Windows.Forms.DataGridView gridXLIFFFiles;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.Label browseLabel;
        private System.Windows.Forms.TableLayoutPanel mainTableLayoutPanel;
        private System.Windows.Forms.Panel scrollablePanel;
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ErrorProvider errorProvider1;
		public System.Windows.Forms.CheckBox ckNeuralMachineTranslation;
        public Button CancelButton;
        private TabControl tabControl2;
        private TabPage tabPage1;
        private TabPage tabPage2;
        public Button btnExpandSearchResults;
        private DataGridView statsDataGridView;
        public Button btnExpandStats;
    }
}

