using SDLXLIFFSliceOrChange.Data;

namespace SDLXLIFFSliceOrChange
{
    partial class SDLXLIFFSliceOrChange
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SDLXLIFFSliceOrChange));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.xLIFFFiles = new XLIFFFiles();
            this.selectFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.selectProjectFile = new System.Windows.Forms.OpenFileDialog();
            this.filesBindingSource = new System.Windows.Forms.BindingSource(this.components);
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
            this.btnExpandSearchResults = new System.Windows.Forms.Button();
            this.btnFindAll = new System.Windows.Forms.Button();
            this.gridSearchResults = new System.Windows.Forms.DataGridView();
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
            this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.xLIFFFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.filesBindingSource)).BeginInit();
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
            ((System.ComponentModel.ISupportInitialize)(this.gridSearchResults)).BeginInit();
            this.groupOptions.SuspendLayout();
            this.groupSearch.SuspendLayout();
            this.pageReplace.SuspendLayout();
            this.groupReplace.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridReplaceResults)).BeginInit();
            this.groupReplaceOptions.SuspendLayout();
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
            this.browseTableLayoutPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridXLIFFFiles)).BeginInit();
            this.mainTableLayoutPanel.SuspendLayout();
            this.panelStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
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
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.tabControl1.Controls.Add(this.pageStatuses);
            this.tabControl1.Controls.Add(this.pageSearch);
            this.tabControl1.Controls.Add(this.pageReplace);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(7, 256);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(803, 279);
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
            this.pageStatuses.Location = new System.Drawing.Point(4, 24);
            this.pageStatuses.Name = "pageStatuses";
            this.pageStatuses.Padding = new System.Windows.Forms.Padding(3);
            this.pageStatuses.Size = new System.Drawing.Size(795, 251);
            this.pageStatuses.TabIndex = 0;
            this.pageStatuses.Text = "Statuses";
            // 
            // btnReverseSelection
            // 
            this.btnReverseSelection.BackColor = System.Drawing.Color.White;
            this.btnReverseSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReverseSelection.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReverseSelection.Location = new System.Drawing.Point(6, 224);
            this.btnReverseSelection.Name = "btnReverseSelection";
            this.btnReverseSelection.Size = new System.Drawing.Size(172, 23);
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
            this.groupDocumentStructure.Location = new System.Drawing.Point(494, 6);
            this.groupDocumentStructure.Name = "groupDocumentStructure";
            this.groupDocumentStructure.Size = new System.Drawing.Size(282, 216);
            this.groupDocumentStructure.TabIndex = 7;
            this.groupDocumentStructure.TabStop = false;
            this.groupDocumentStructure.Text = "Document Structure";
            // 
            // listDocumentStructure
            // 
            this.listDocumentStructure.BackColor = System.Drawing.SystemColors.Window;
            this.listDocumentStructure.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listDocumentStructure.FormattingEnabled = true;
            this.listDocumentStructure.ItemHeight = 16;
            this.listDocumentStructure.Location = new System.Drawing.Point(3, 17);
            this.listDocumentStructure.Name = "listDocumentStructure";
            this.listDocumentStructure.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listDocumentStructure.Size = new System.Drawing.Size(130, 196);
            this.listDocumentStructure.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labelGenerateDSIComments);
            this.panel2.Controls.Add(this.btnGenerateDSI);
            this.panel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel2.Location = new System.Drawing.Point(139, 16);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(113, 197);
            this.panel2.TabIndex = 0;
            // 
            // labelGenerateDSIComments
            // 
            this.labelGenerateDSIComments.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGenerateDSIComments.ForeColor = System.Drawing.Color.Blue;
            this.labelGenerateDSIComments.Location = new System.Drawing.Point(3, 31);
            this.labelGenerateDSIComments.Name = "labelGenerateDSIComments";
            this.labelGenerateDSIComments.Size = new System.Drawing.Size(99, 143);
            this.labelGenerateDSIComments.TabIndex = 1;
            this.labelGenerateDSIComments.Text = "Click \"Generate DSI\" to generate a list of all different types of structure infor" +
    "mation used in the selected files. Hold Ctrl key down and select the ones you wa" +
    "nt with the mouse.";
            // 
            // btnGenerateDSI
            // 
            this.btnGenerateDSI.BackColor = System.Drawing.Color.White;
            this.btnGenerateDSI.Location = new System.Drawing.Point(3, 0);
            this.btnGenerateDSI.Name = "btnGenerateDSI";
            this.btnGenerateDSI.Size = new System.Drawing.Size(98, 23);
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
            this.groupSystem.Location = new System.Drawing.Point(350, 128);
            this.groupSystem.Name = "groupSystem";
            this.groupSystem.Size = new System.Drawing.Size(141, 94);
            this.groupSystem.TabIndex = 6;
            this.groupSystem.TabStop = false;
            this.groupSystem.Text = "System";
            // 
            // ckPropagated
            // 
            this.ckPropagated.AutoSize = true;
            this.ckPropagated.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckPropagated.Location = new System.Drawing.Point(6, 63);
            this.ckPropagated.Name = "ckPropagated";
            this.ckPropagated.Size = new System.Drawing.Size(81, 17);
            this.ckPropagated.TabIndex = 4;
            this.ckPropagated.Text = "Propagated";
            this.ckPropagated.UseVisualStyleBackColor = true;
            // 
            // ckSystemTranslationMemory
            // 
            this.ckSystemTranslationMemory.AutoSize = true;
            this.ckSystemTranslationMemory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckSystemTranslationMemory.Location = new System.Drawing.Point(6, 40);
            this.ckSystemTranslationMemory.Name = "ckSystemTranslationMemory";
            this.ckSystemTranslationMemory.Size = new System.Drawing.Size(118, 17);
            this.ckSystemTranslationMemory.TabIndex = 3;
            this.ckSystemTranslationMemory.Text = "Translation Memory";
            this.ckSystemTranslationMemory.UseVisualStyleBackColor = true;
            // 
            // ckSystemMachineTranslation
            // 
            this.ckSystemMachineTranslation.AutoSize = true;
            this.ckSystemMachineTranslation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckSystemMachineTranslation.Location = new System.Drawing.Point(6, 19);
            this.ckSystemMachineTranslation.Name = "ckSystemMachineTranslation";
            this.ckSystemMachineTranslation.Size = new System.Drawing.Size(122, 17);
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
            this.groupTranslationOrigin.Location = new System.Drawing.Point(350, 6);
            this.groupTranslationOrigin.Name = "groupTranslationOrigin";
            this.groupTranslationOrigin.Size = new System.Drawing.Size(141, 126);
            this.groupTranslationOrigin.TabIndex = 5;
            this.groupTranslationOrigin.TabStop = false;
            this.groupTranslationOrigin.Text = "Translation Origin";
            // 
            // ckNeuralMachineTranslation
            // 
            this.ckNeuralMachineTranslation.AutoSize = true;
            this.ckNeuralMachineTranslation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckNeuralMachineTranslation.Location = new System.Drawing.Point(6, 96);
            this.ckNeuralMachineTranslation.Name = "ckNeuralMachineTranslation";
            this.ckNeuralMachineTranslation.Size = new System.Drawing.Size(76, 17);
            this.ckNeuralMachineTranslation.TabIndex = 4;
            this.ckNeuralMachineTranslation.Text = "Neural MT";
            this.ckNeuralMachineTranslation.UseVisualStyleBackColor = true;
            // 
            // ckAutoPropagated
            // 
            this.ckAutoPropagated.AutoSize = true;
            this.ckAutoPropagated.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckAutoPropagated.Location = new System.Drawing.Point(6, 76);
            this.ckAutoPropagated.Name = "ckAutoPropagated";
            this.ckAutoPropagated.Size = new System.Drawing.Size(106, 17);
            this.ckAutoPropagated.TabIndex = 3;
            this.ckAutoPropagated.Text = "Auto-Propagated";
            this.ckAutoPropagated.UseVisualStyleBackColor = true;
            // 
            // ckAutomatedTranslation
            // 
            this.ckAutomatedTranslation.AutoSize = true;
            this.ckAutomatedTranslation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckAutomatedTranslation.Location = new System.Drawing.Point(6, 57);
            this.ckAutomatedTranslation.Name = "ckAutomatedTranslation";
            this.ckAutomatedTranslation.Size = new System.Drawing.Size(132, 17);
            this.ckAutomatedTranslation.TabIndex = 2;
            this.ckAutomatedTranslation.Text = "Automated Translation";
            this.ckAutomatedTranslation.UseVisualStyleBackColor = true;
            // 
            // ckInteractive
            // 
            this.ckInteractive.AutoSize = true;
            this.ckInteractive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckInteractive.Location = new System.Drawing.Point(6, 37);
            this.ckInteractive.Name = "ckInteractive";
            this.ckInteractive.Size = new System.Drawing.Size(76, 17);
            this.ckInteractive.TabIndex = 1;
            this.ckInteractive.Text = "Interactive";
            this.ckInteractive.UseVisualStyleBackColor = true;
            // 
            // ckTranslationMemory
            // 
            this.ckTranslationMemory.AutoSize = true;
            this.ckTranslationMemory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckTranslationMemory.Location = new System.Drawing.Point(6, 19);
            this.ckTranslationMemory.Name = "ckTranslationMemory";
            this.ckTranslationMemory.Size = new System.Drawing.Size(118, 17);
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
            this.groupScore.Location = new System.Drawing.Point(187, 6);
            this.groupScore.Name = "groupScore";
            this.groupScore.Size = new System.Drawing.Size(153, 116);
            this.groupScore.TabIndex = 4;
            this.groupScore.TabStop = false;
            this.groupScore.Text = "Score";
            // 
            // txtMatchValuesExpression
            // 
            this.txtMatchValuesExpression.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMatchValuesExpression.Location = new System.Drawing.Point(5, 83);
            this.txtMatchValuesExpression.Name = "txtMatchValuesExpression";
            this.txtMatchValuesExpression.Size = new System.Drawing.Size(122, 20);
            this.txtMatchValuesExpression.TabIndex = 5;
            this.txtMatchValuesExpression.Text = "<95";
            // 
            // ckMatchValues
            // 
            this.ckMatchValues.AutoSize = true;
            this.ckMatchValues.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckMatchValues.Location = new System.Drawing.Point(6, 63);
            this.ckMatchValues.Name = "ckMatchValues";
            this.ckMatchValues.Size = new System.Drawing.Size(97, 17);
            this.ckMatchValues.TabIndex = 4;
            this.ckMatchValues.Text = "Match Value(s)";
            this.ckMatchValues.UseVisualStyleBackColor = true;
            // 
            // ckContextMatch
            // 
            this.ckContextMatch.AutoSize = true;
            this.ckContextMatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckContextMatch.Location = new System.Drawing.Point(6, 40);
            this.ckContextMatch.Name = "ckContextMatch";
            this.ckContextMatch.Size = new System.Drawing.Size(95, 17);
            this.ckContextMatch.TabIndex = 3;
            this.ckContextMatch.Text = "Context Match";
            this.ckContextMatch.UseVisualStyleBackColor = true;
            // 
            // ckPerfectMatch
            // 
            this.ckPerfectMatch.AutoSize = true;
            this.ckPerfectMatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckPerfectMatch.Location = new System.Drawing.Point(6, 19);
            this.ckPerfectMatch.Name = "ckPerfectMatch";
            this.ckPerfectMatch.Size = new System.Drawing.Size(93, 17);
            this.ckPerfectMatch.TabIndex = 2;
            this.ckPerfectMatch.Text = "Perfect Match";
            this.ckPerfectMatch.UseVisualStyleBackColor = true;
            // 
            // groupStatusesLocked
            // 
            this.groupStatusesLocked.Controls.Add(this.ckUnlocked);
            this.groupStatusesLocked.Controls.Add(this.ckLocked);
            this.groupStatusesLocked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupStatusesLocked.Location = new System.Drawing.Point(187, 128);
            this.groupStatusesLocked.Name = "groupStatusesLocked";
            this.groupStatusesLocked.Size = new System.Drawing.Size(153, 94);
            this.groupStatusesLocked.TabIndex = 2;
            this.groupStatusesLocked.TabStop = false;
            this.groupStatusesLocked.Text = "Locked / Unlocked";
            // 
            // ckUnlocked
            // 
            this.ckUnlocked.AutoSize = true;
            this.ckUnlocked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckUnlocked.Location = new System.Drawing.Point(6, 40);
            this.ckUnlocked.Name = "ckUnlocked";
            this.ckUnlocked.Size = new System.Drawing.Size(72, 17);
            this.ckUnlocked.TabIndex = 3;
            this.ckUnlocked.Text = "Unlocked";
            this.ckUnlocked.UseVisualStyleBackColor = true;
            // 
            // ckLocked
            // 
            this.ckLocked.AutoSize = true;
            this.ckLocked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckLocked.Location = new System.Drawing.Point(6, 19);
            this.ckLocked.Name = "ckLocked";
            this.ckLocked.Size = new System.Drawing.Size(62, 17);
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
            this.groupStatusesTranslationStatus.Location = new System.Drawing.Point(6, 6);
            this.groupStatusesTranslationStatus.Name = "groupStatusesTranslationStatus";
            this.groupStatusesTranslationStatus.Size = new System.Drawing.Size(172, 216);
            this.groupStatusesTranslationStatus.TabIndex = 1;
            this.groupStatusesTranslationStatus.TabStop = false;
            this.groupStatusesTranslationStatus.Text = "Translation Status";
            // 
            // ckSignedOff
            // 
            this.ckSignedOff.AutoSize = true;
            this.ckSignedOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckSignedOff.Location = new System.Drawing.Point(6, 155);
            this.ckSignedOff.Name = "ckSignedOff";
            this.ckSignedOff.Size = new System.Drawing.Size(76, 17);
            this.ckSignedOff.TabIndex = 6;
            this.ckSignedOff.Text = "Signed Off";
            this.ckSignedOff.UseVisualStyleBackColor = true;
            // 
            // ckSignOffRejected
            // 
            this.ckSignOffRejected.AutoSize = true;
            this.ckSignOffRejected.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckSignOffRejected.Location = new System.Drawing.Point(6, 132);
            this.ckSignOffRejected.Name = "ckSignOffRejected";
            this.ckSignOffRejected.Size = new System.Drawing.Size(108, 17);
            this.ckSignOffRejected.TabIndex = 5;
            this.ckSignOffRejected.Text = "Sign-off Rejected";
            this.ckSignOffRejected.UseVisualStyleBackColor = true;
            // 
            // ckTranslationApproved
            // 
            this.ckTranslationApproved.AutoSize = true;
            this.ckTranslationApproved.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckTranslationApproved.Location = new System.Drawing.Point(6, 109);
            this.ckTranslationApproved.Name = "ckTranslationApproved";
            this.ckTranslationApproved.Size = new System.Drawing.Size(127, 17);
            this.ckTranslationApproved.TabIndex = 4;
            this.ckTranslationApproved.Text = "Translation Approved";
            this.ckTranslationApproved.UseVisualStyleBackColor = true;
            // 
            // ckTranslationRejected
            // 
            this.ckTranslationRejected.AutoSize = true;
            this.ckTranslationRejected.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckTranslationRejected.Location = new System.Drawing.Point(6, 86);
            this.ckTranslationRejected.Name = "ckTranslationRejected";
            this.ckTranslationRejected.Size = new System.Drawing.Size(124, 17);
            this.ckTranslationRejected.TabIndex = 3;
            this.ckTranslationRejected.Text = "Translation Rejected";
            this.ckTranslationRejected.UseVisualStyleBackColor = true;
            // 
            // ckTranslated
            // 
            this.ckTranslated.AutoSize = true;
            this.ckTranslated.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckTranslated.Location = new System.Drawing.Point(6, 63);
            this.ckTranslated.Name = "ckTranslated";
            this.ckTranslated.Size = new System.Drawing.Size(76, 17);
            this.ckTranslated.TabIndex = 2;
            this.ckTranslated.Text = "Translated";
            this.ckTranslated.UseVisualStyleBackColor = true;
            // 
            // ckDraft
            // 
            this.ckDraft.AutoSize = true;
            this.ckDraft.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckDraft.Location = new System.Drawing.Point(6, 40);
            this.ckDraft.Name = "ckDraft";
            this.ckDraft.Size = new System.Drawing.Size(49, 17);
            this.ckDraft.TabIndex = 1;
            this.ckDraft.Text = "Draft";
            this.ckDraft.UseVisualStyleBackColor = true;
            // 
            // ckNotTranslated
            // 
            this.ckNotTranslated.AutoSize = true;
            this.ckNotTranslated.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckNotTranslated.Location = new System.Drawing.Point(6, 19);
            this.ckNotTranslated.Name = "ckNotTranslated";
            this.ckNotTranslated.Size = new System.Drawing.Size(92, 17);
            this.ckNotTranslated.TabIndex = 0;
            this.ckNotTranslated.Text = "Not translated";
            this.ckNotTranslated.UseVisualStyleBackColor = true;
            // 
            // pageSearch
            // 
            this.pageSearch.Controls.Add(this.btnExpandSearchResults);
            this.pageSearch.Controls.Add(this.btnFindAll);
            this.pageSearch.Controls.Add(this.gridSearchResults);
            this.pageSearch.Controls.Add(this.groupOptions);
            this.pageSearch.Controls.Add(this.groupSearch);
            this.pageSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pageSearch.ForeColor = System.Drawing.SystemColors.ControlText;
            this.pageSearch.Location = new System.Drawing.Point(4, 24);
            this.pageSearch.Name = "pageSearch";
            this.pageSearch.Padding = new System.Windows.Forms.Padding(3);
            this.pageSearch.Size = new System.Drawing.Size(795, 251);
            this.pageSearch.TabIndex = 1;
            this.pageSearch.Text = "Search";
            this.pageSearch.UseVisualStyleBackColor = true;
            // 
            // btnExpandSearchResults
            // 
            this.btnExpandSearchResults.Image = ((System.Drawing.Image)(resources.GetObject("btnExpandSearchResults.Image")));
            this.btnExpandSearchResults.Location = new System.Drawing.Point(745, 96);
            this.btnExpandSearchResults.Name = "btnExpandSearchResults";
            this.btnExpandSearchResults.Size = new System.Drawing.Size(30, 27);
            this.btnExpandSearchResults.TabIndex = 9;
            this.btnExpandSearchResults.UseVisualStyleBackColor = true;
            this.btnExpandSearchResults.Click += new System.EventHandler(this.btnExpandSearchResults_Click);
            // 
            // btnFindAll
            // 
            this.btnFindAll.BackColor = System.Drawing.SystemColors.Window;
            this.btnFindAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFindAll.Location = new System.Drawing.Point(3, 201);
            this.btnFindAll.Name = "btnFindAll";
            this.btnFindAll.Size = new System.Drawing.Size(154, 42);
            this.btnFindAll.TabIndex = 8;
            this.btnFindAll.Text = "Find All";
            this.btnFindAll.UseVisualStyleBackColor = false;
            this.btnFindAll.Click += new System.EventHandler(this.btnFindAll_Click);
            // 
            // gridSearchResults
            // 
            this.gridSearchResults.AllowUserToAddRows = false;
            this.gridSearchResults.AllowUserToDeleteRows = false;
            this.gridSearchResults.AllowUserToResizeRows = false;
            this.gridSearchResults.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridSearchResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSearchResults.GridColor = System.Drawing.SystemColors.Control;
            this.gridSearchResults.Location = new System.Drawing.Point(163, 96);
            this.gridSearchResults.Name = "gridSearchResults";
            this.gridSearchResults.RowHeadersVisible = false;
            this.gridSearchResults.Size = new System.Drawing.Size(612, 151);
            this.gridSearchResults.TabIndex = 7;
            this.gridSearchResults.VirtualMode = true;
            this.gridSearchResults.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.gridSearchResults_CellValueNeeded);
            this.gridSearchResults.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gridSearchResults_DataBindingComplete);
            // 
            // groupOptions
            // 
            this.groupOptions.Controls.Add(this.ckSearchInTags);
            this.groupOptions.Controls.Add(this.ckSearchRegEx);
            this.groupOptions.Controls.Add(this.ckSearchMatchWholeWord);
            this.groupOptions.Controls.Add(this.ckSearchMatchCase);
            this.groupOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupOptions.Location = new System.Drawing.Point(7, 87);
            this.groupOptions.Name = "groupOptions";
            this.groupOptions.Size = new System.Drawing.Size(154, 110);
            this.groupOptions.TabIndex = 6;
            this.groupOptions.TabStop = false;
            this.groupOptions.Text = "Options";
            // 
            // ckSearchInTags
            // 
            this.ckSearchInTags.AutoSize = true;
            this.ckSearchInTags.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckSearchInTags.Location = new System.Drawing.Point(6, 88);
            this.ckSearchInTags.Name = "ckSearchInTags";
            this.ckSearchInTags.Size = new System.Drawing.Size(94, 17);
            this.ckSearchInTags.TabIndex = 3;
            this.ckSearchInTags.Text = "Search in tags";
            this.ckSearchInTags.UseVisualStyleBackColor = true;
            // 
            // ckSearchRegEx
            // 
            this.ckSearchRegEx.AutoSize = true;
            this.ckSearchRegEx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckSearchRegEx.Location = new System.Drawing.Point(6, 65);
            this.ckSearchRegEx.Name = "ckSearchRegEx";
            this.ckSearchRegEx.Size = new System.Drawing.Size(138, 17);
            this.ckSearchRegEx.TabIndex = 2;
            this.ckSearchRegEx.Text = "Use regular expressions";
            this.ckSearchRegEx.UseVisualStyleBackColor = true;
            // 
            // ckSearchMatchWholeWord
            // 
            this.ckSearchMatchWholeWord.AutoSize = true;
            this.ckSearchMatchWholeWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckSearchMatchWholeWord.Location = new System.Drawing.Point(6, 42);
            this.ckSearchMatchWholeWord.Name = "ckSearchMatchWholeWord";
            this.ckSearchMatchWholeWord.Size = new System.Drawing.Size(113, 17);
            this.ckSearchMatchWholeWord.TabIndex = 1;
            this.ckSearchMatchWholeWord.Text = "Match whole word";
            this.ckSearchMatchWholeWord.UseVisualStyleBackColor = true;
            // 
            // ckSearchMatchCase
            // 
            this.ckSearchMatchCase.AutoSize = true;
            this.ckSearchMatchCase.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckSearchMatchCase.Location = new System.Drawing.Point(6, 19);
            this.ckSearchMatchCase.Name = "ckSearchMatchCase";
            this.ckSearchMatchCase.Size = new System.Drawing.Size(82, 17);
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
            this.groupSearch.Location = new System.Drawing.Point(6, 6);
            this.groupSearch.Name = "groupSearch";
            this.groupSearch.Size = new System.Drawing.Size(769, 80);
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
            this.cmbOperator.Location = new System.Drawing.Point(393, 32);
            this.cmbOperator.Name = "cmbOperator";
            this.cmbOperator.Size = new System.Drawing.Size(54, 21);
            this.cmbOperator.TabIndex = 4;
            // 
            // txtTargetSearch
            // 
            this.txtTargetSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTargetSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTargetSearch.Location = new System.Drawing.Point(453, 32);
            this.txtTargetSearch.Multiline = true;
            this.txtTargetSearch.Name = "txtTargetSearch";
            this.txtTargetSearch.Size = new System.Drawing.Size(226, 43);
            this.txtTargetSearch.TabIndex = 3;
            // 
            // labelSearchTarget
            // 
            this.labelSearchTarget.AutoSize = true;
            this.labelSearchTarget.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSearchTarget.Location = new System.Drawing.Point(450, 16);
            this.labelSearchTarget.Name = "labelSearchTarget";
            this.labelSearchTarget.Size = new System.Drawing.Size(73, 13);
            this.labelSearchTarget.TabIndex = 2;
            this.labelSearchTarget.Text = "Target search";
            // 
            // txtSourceSearch
            // 
            this.txtSourceSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSourceSearch.Location = new System.Drawing.Point(9, 32);
            this.txtSourceSearch.Multiline = true;
            this.txtSourceSearch.Name = "txtSourceSearch";
            this.txtSourceSearch.Size = new System.Drawing.Size(378, 43);
            this.txtSourceSearch.TabIndex = 1;
            // 
            // labelSearchSource
            // 
            this.labelSearchSource.AutoSize = true;
            this.labelSearchSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSearchSource.Location = new System.Drawing.Point(6, 16);
            this.labelSearchSource.Name = "labelSearchSource";
            this.labelSearchSource.Size = new System.Drawing.Size(76, 13);
            this.labelSearchSource.TabIndex = 0;
            this.labelSearchSource.Text = "Source search";
            // 
            // pageReplace
            // 
            this.pageReplace.Controls.Add(this.groupReplace);
            this.pageReplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pageReplace.ForeColor = System.Drawing.SystemColors.ControlText;
            this.pageReplace.Location = new System.Drawing.Point(4, 24);
            this.pageReplace.Name = "pageReplace";
            this.pageReplace.Padding = new System.Windows.Forms.Padding(3);
            this.pageReplace.Size = new System.Drawing.Size(795, 251);
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
            this.groupReplace.Location = new System.Drawing.Point(3, 3);
            this.groupReplace.Name = "groupReplace";
            this.groupReplace.Size = new System.Drawing.Size(789, 245);
            this.groupReplace.TabIndex = 6;
            this.groupReplace.TabStop = false;
            this.groupReplace.Text = "Replace in source OR target";
            // 
            // btnExpandReplaceResults
            // 
            this.btnExpandReplaceResults.Image = ((System.Drawing.Image)(resources.GetObject("btnExpandReplaceResults.Image")));
            this.btnExpandReplaceResults.Location = new System.Drawing.Point(756, 75);
            this.btnExpandReplaceResults.Name = "btnExpandReplaceResults";
            this.btnExpandReplaceResults.Size = new System.Drawing.Size(27, 29);
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
            this.gridReplaceResults.Location = new System.Drawing.Point(169, 74);
            this.gridReplaceResults.Name = "gridReplaceResults";
            this.gridReplaceResults.RowHeadersVisible = false;
            this.gridReplaceResults.Size = new System.Drawing.Size(615, 168);
            this.gridReplaceResults.TabIndex = 12;
            this.gridReplaceResults.VirtualMode = true;
            this.gridReplaceResults.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.gridReplaceResults_CellValueNeeded);
            this.gridReplaceResults.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.gridReplaceResults_DataBindingComplete);
            // 
            // btnPreview
            // 
            this.btnPreview.BackColor = System.Drawing.SystemColors.Window;
            this.btnPreview.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPreview.Location = new System.Drawing.Point(9, 170);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(154, 28);
            this.btnPreview.TabIndex = 11;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnFindAllInReplace
            // 
            this.btnFindAllInReplace.BackColor = System.Drawing.SystemColors.Window;
            this.btnFindAllInReplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFindAllInReplace.Location = new System.Drawing.Point(9, 138);
            this.btnFindAllInReplace.Name = "btnFindAllInReplace";
            this.btnFindAllInReplace.Size = new System.Drawing.Size(154, 28);
            this.btnFindAllInReplace.TabIndex = 10;
            this.btnFindAllInReplace.Text = "Find All";
            this.btnFindAllInReplace.UseVisualStyleBackColor = false;
            this.btnFindAllInReplace.Click += new System.EventHandler(this.btnFindAllInReplace_Click);
            // 
            // btnReplace
            // 
            this.btnReplace.BackColor = System.Drawing.SystemColors.Window;
            this.btnReplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReplace.Location = new System.Drawing.Point(9, 204);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(154, 28);
            this.btnReplace.TabIndex = 8;
            this.btnReplace.Text = "Replace All";
            this.btnReplace.UseVisualStyleBackColor = false;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // txtReplaceTargetReplace
            // 
            this.txtReplaceTargetReplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReplaceTargetReplace.Location = new System.Drawing.Point(576, 48);
            this.txtReplaceTargetReplace.Multiline = true;
            this.txtReplaceTargetReplace.Name = "txtReplaceTargetReplace";
            this.txtReplaceTargetReplace.Size = new System.Drawing.Size(208, 20);
            this.txtReplaceTargetReplace.TabIndex = 9;
            // 
            // groupReplaceOptions
            // 
            this.groupReplaceOptions.Controls.Add(this.ckReplaceUseRegEx);
            this.groupReplaceOptions.Controls.Add(this.ckReplaceMatchWholeWord);
            this.groupReplaceOptions.Controls.Add(this.ckReplaceMatchCase);
            this.groupReplaceOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupReplaceOptions.Location = new System.Drawing.Point(9, 22);
            this.groupReplaceOptions.Name = "groupReplaceOptions";
            this.groupReplaceOptions.Size = new System.Drawing.Size(154, 112);
            this.groupReplaceOptions.TabIndex = 6;
            this.groupReplaceOptions.TabStop = false;
            this.groupReplaceOptions.Text = "Options";
            // 
            // ckReplaceUseRegEx
            // 
            this.ckReplaceUseRegEx.AutoSize = true;
            this.ckReplaceUseRegEx.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckReplaceUseRegEx.Location = new System.Drawing.Point(6, 65);
            this.ckReplaceUseRegEx.Name = "ckReplaceUseRegEx";
            this.ckReplaceUseRegEx.Size = new System.Drawing.Size(138, 17);
            this.ckReplaceUseRegEx.TabIndex = 2;
            this.ckReplaceUseRegEx.Text = "Use regular expressions";
            this.ckReplaceUseRegEx.UseVisualStyleBackColor = true;
            // 
            // ckReplaceMatchWholeWord
            // 
            this.ckReplaceMatchWholeWord.AutoSize = true;
            this.ckReplaceMatchWholeWord.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckReplaceMatchWholeWord.Location = new System.Drawing.Point(6, 42);
            this.ckReplaceMatchWholeWord.Name = "ckReplaceMatchWholeWord";
            this.ckReplaceMatchWholeWord.Size = new System.Drawing.Size(113, 17);
            this.ckReplaceMatchWholeWord.TabIndex = 1;
            this.ckReplaceMatchWholeWord.Text = "Match whole word";
            this.ckReplaceMatchWholeWord.UseVisualStyleBackColor = true;
            // 
            // ckReplaceMatchCase
            // 
            this.ckReplaceMatchCase.AutoSize = true;
            this.ckReplaceMatchCase.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckReplaceMatchCase.Location = new System.Drawing.Point(6, 19);
            this.ckReplaceMatchCase.Name = "ckReplaceMatchCase";
            this.ckReplaceMatchCase.Size = new System.Drawing.Size(82, 17);
            this.ckReplaceMatchCase.TabIndex = 0;
            this.ckReplaceMatchCase.Text = "Match case";
            this.ckReplaceMatchCase.UseVisualStyleBackColor = true;
            // 
            // lblReplaceTargetReplace
            // 
            this.lblReplaceTargetReplace.AutoSize = true;
            this.lblReplaceTargetReplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReplaceTargetReplace.Location = new System.Drawing.Point(503, 55);
            this.lblReplaceTargetReplace.Name = "lblReplaceTargetReplace";
            this.lblReplaceTargetReplace.Size = new System.Drawing.Size(69, 13);
            this.lblReplaceTargetReplace.TabIndex = 8;
            this.lblReplaceTargetReplace.Text = "Replace with";
            // 
            // txtReplaceTargetSearch
            // 
            this.txtReplaceTargetSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReplaceTargetSearch.Location = new System.Drawing.Point(248, 48);
            this.txtReplaceTargetSearch.Multiline = true;
            this.txtReplaceTargetSearch.Name = "txtReplaceTargetSearch";
            this.txtReplaceTargetSearch.Size = new System.Drawing.Size(226, 20);
            this.txtReplaceTargetSearch.TabIndex = 7;
            this.txtReplaceTargetSearch.Validating += new System.ComponentModel.CancelEventHandler(this.txtReplaceTargetSearch_Validating);
            // 
            // lblReplaceTargetSearch
            // 
            this.lblReplaceTargetSearch.AutoSize = true;
            this.lblReplaceTargetSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReplaceTargetSearch.Location = new System.Drawing.Point(166, 51);
            this.lblReplaceTargetSearch.Name = "lblReplaceTargetSearch";
            this.lblReplaceTargetSearch.Size = new System.Drawing.Size(73, 13);
            this.lblReplaceTargetSearch.TabIndex = 6;
            this.lblReplaceTargetSearch.Text = "Target search";
            // 
            // txtReplaceSourceReplace
            // 
            this.txtReplaceSourceReplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReplaceSourceReplace.Location = new System.Drawing.Point(576, 22);
            this.txtReplaceSourceReplace.Multiline = true;
            this.txtReplaceSourceReplace.Name = "txtReplaceSourceReplace";
            this.txtReplaceSourceReplace.Size = new System.Drawing.Size(208, 20);
            this.txtReplaceSourceReplace.TabIndex = 5;
            // 
            // lblReplaceSourceReplace
            // 
            this.lblReplaceSourceReplace.AutoSize = true;
            this.lblReplaceSourceReplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReplaceSourceReplace.Location = new System.Drawing.Point(503, 25);
            this.lblReplaceSourceReplace.Name = "lblReplaceSourceReplace";
            this.lblReplaceSourceReplace.Size = new System.Drawing.Size(69, 13);
            this.lblReplaceSourceReplace.TabIndex = 4;
            this.lblReplaceSourceReplace.Text = "Replace with";
            // 
            // txtReplaceSourceSearch
            // 
            this.txtReplaceSourceSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReplaceSourceSearch.Location = new System.Drawing.Point(248, 22);
            this.txtReplaceSourceSearch.Multiline = true;
            this.txtReplaceSourceSearch.Name = "txtReplaceSourceSearch";
            this.txtReplaceSourceSearch.Size = new System.Drawing.Size(226, 20);
            this.txtReplaceSourceSearch.TabIndex = 1;
            this.txtReplaceSourceSearch.Validating += new System.ComponentModel.CancelEventHandler(this.txtReplaceSourceSearch_Validating);
            // 
            // lblReplaceSourceSearch
            // 
            this.lblReplaceSourceSearch.AutoSize = true;
            this.lblReplaceSourceSearch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReplaceSourceSearch.Location = new System.Drawing.Point(166, 25);
            this.lblReplaceSourceSearch.Name = "lblReplaceSourceSearch";
            this.lblReplaceSourceSearch.Size = new System.Drawing.Size(76, 13);
            this.lblReplaceSourceSearch.TabIndex = 0;
            this.lblReplaceSourceSearch.Text = "Source search";
            // 
            // panelCommands
            // 
            this.panelCommands.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.panelCommands.Controls.Add(this.groupClear);
            this.panelCommands.Controls.Add(this.groupChange);
            this.panelCommands.Controls.Add(this.groupSlice);
            this.panelCommands.Location = new System.Drawing.Point(5, 541);
            this.panelCommands.Name = "panelCommands";
            this.panelCommands.Size = new System.Drawing.Size(807, 239);
            this.panelCommands.TabIndex = 9;
            // 
            // groupClear
            // 
            this.groupClear.Controls.Add(this.btnClearit);
            this.groupClear.Controls.Add(this.pictureBox3);
            this.groupClear.Controls.Add(this.lblClearit);
            this.groupClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupClear.ForeColor = System.Drawing.Color.Red;
            this.groupClear.Location = new System.Drawing.Point(643, 10);
            this.groupClear.Name = "groupClear";
            this.groupClear.Size = new System.Drawing.Size(160, 226);
            this.groupClear.TabIndex = 6;
            this.groupClear.TabStop = false;
            this.groupClear.Text = "Clear";
            // 
            // btnClearit
            // 
            this.btnClearit.BackColor = System.Drawing.SystemColors.Control;
            this.btnClearit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearit.ForeColor = System.Drawing.Color.Red;
            this.btnClearit.Location = new System.Drawing.Point(6, 167);
            this.btnClearit.Name = "btnClearit";
            this.btnClearit.Size = new System.Drawing.Size(130, 49);
            this.btnClearit.TabIndex = 2;
            this.btnClearit.Text = "Clearit!";
            this.btnClearit.UseVisualStyleBackColor = false;
            this.btnClearit.Click += new System.EventHandler(this.btnClearit_Click);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.InitialImage = null;
            this.pictureBox3.Location = new System.Drawing.Point(6, 120);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(127, 43);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 1;
            this.pictureBox3.TabStop = false;
            // 
            // lblClearit
            // 
            this.lblClearit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblClearit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClearit.ForeColor = System.Drawing.Color.Blue;
            this.lblClearit.Location = new System.Drawing.Point(3, 18);
            this.lblClearit.Name = "lblClearit";
            this.lblClearit.Size = new System.Drawing.Size(154, 205);
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
            this.groupChange.Location = new System.Drawing.Point(174, 6);
            this.groupChange.Name = "groupChange";
            this.groupChange.Size = new System.Drawing.Size(463, 230);
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
            this.groupChangeToStatusOr.Location = new System.Drawing.Point(6, 19);
            this.groupChangeToStatusOr.Name = "groupChangeToStatusOr";
            this.groupChangeToStatusOr.Size = new System.Drawing.Size(303, 205);
            this.groupChangeToStatusOr.TabIndex = 0;
            this.groupChangeToStatusOr.TabStop = false;
            this.groupChangeToStatusOr.Text = "Change to Status and/or Lock value";
            // 
            // ckCopySourceToTarget
            // 
            this.ckCopySourceToTarget.AutoSize = true;
            this.ckCopySourceToTarget.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckCopySourceToTarget.Location = new System.Drawing.Point(161, 89);
            this.ckCopySourceToTarget.Name = "ckCopySourceToTarget";
            this.ckCopySourceToTarget.Size = new System.Drawing.Size(127, 17);
            this.ckCopySourceToTarget.TabIndex = 2;
            this.ckCopySourceToTarget.Text = "Copy source to target";
            this.ckCopySourceToTarget.UseVisualStyleBackColor = true;
            // 
            // groupChangeLocked
            // 
            this.groupChangeLocked.Controls.Add(this.ckChangeToUnlocked);
            this.groupChangeLocked.Controls.Add(this.ckChangeToLocked);
            this.groupChangeLocked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupChangeLocked.Location = new System.Drawing.Point(155, 20);
            this.groupChangeLocked.Name = "groupChangeLocked";
            this.groupChangeLocked.Size = new System.Drawing.Size(140, 63);
            this.groupChangeLocked.TabIndex = 1;
            this.groupChangeLocked.TabStop = false;
            this.groupChangeLocked.Text = "Locked / Unlocked";
            // 
            // ckChangeToUnlocked
            // 
            this.ckChangeToUnlocked.AutoSize = true;
            this.ckChangeToUnlocked.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckChangeToUnlocked.Location = new System.Drawing.Point(6, 40);
            this.ckChangeToUnlocked.Name = "ckChangeToUnlocked";
            this.ckChangeToUnlocked.Size = new System.Drawing.Size(71, 17);
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
            this.ckChangeToLocked.Location = new System.Drawing.Point(6, 19);
            this.ckChangeToLocked.Name = "ckChangeToLocked";
            this.ckChangeToLocked.Size = new System.Drawing.Size(61, 17);
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
            this.groupChangeTranslationStatus.Location = new System.Drawing.Point(6, 20);
            this.groupChangeTranslationStatus.Name = "groupChangeTranslationStatus";
            this.groupChangeTranslationStatus.Size = new System.Drawing.Size(143, 179);
            this.groupChangeTranslationStatus.TabIndex = 0;
            this.groupChangeTranslationStatus.TabStop = false;
            this.groupChangeTranslationStatus.Text = "Translation Status";
            // 
            // ckChangeToSignedOff
            // 
            this.ckChangeToSignedOff.AutoSize = true;
            this.ckChangeToSignedOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckChangeToSignedOff.Location = new System.Drawing.Point(6, 133);
            this.ckChangeToSignedOff.Name = "ckChangeToSignedOff";
            this.ckChangeToSignedOff.Size = new System.Drawing.Size(75, 17);
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
            this.ckChangeToSignOffRejected.Location = new System.Drawing.Point(6, 156);
            this.ckChangeToSignOffRejected.Name = "ckChangeToSignOffRejected";
            this.ckChangeToSignOffRejected.Size = new System.Drawing.Size(107, 17);
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
            this.ckChangeToTranslationApproved.Location = new System.Drawing.Point(6, 109);
            this.ckChangeToTranslationApproved.Name = "ckChangeToTranslationApproved";
            this.ckChangeToTranslationApproved.Size = new System.Drawing.Size(126, 17);
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
            this.ckChangeToTranslationRejected.Location = new System.Drawing.Point(6, 86);
            this.ckChangeToTranslationRejected.Name = "ckChangeToTranslationRejected";
            this.ckChangeToTranslationRejected.Size = new System.Drawing.Size(123, 17);
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
            this.ckChangeToTranslated.Location = new System.Drawing.Point(6, 63);
            this.ckChangeToTranslated.Name = "ckChangeToTranslated";
            this.ckChangeToTranslated.Size = new System.Drawing.Size(75, 17);
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
            this.ckChangeToDraft.Location = new System.Drawing.Point(6, 40);
            this.ckChangeToDraft.Name = "ckChangeToDraft";
            this.ckChangeToDraft.Size = new System.Drawing.Size(48, 17);
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
            this.ckChangeToNotTranslated.Location = new System.Drawing.Point(6, 19);
            this.ckChangeToNotTranslated.Name = "ckChangeToNotTranslated";
            this.ckChangeToNotTranslated.Size = new System.Drawing.Size(91, 17);
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
            this.btnChangeit.Location = new System.Drawing.Point(315, 171);
            this.btnChangeit.Name = "btnChangeit";
            this.btnChangeit.Size = new System.Drawing.Size(138, 49);
            this.btnChangeit.TabIndex = 5;
            this.btnChangeit.Text = "Changeit!";
            this.btnChangeit.UseVisualStyleBackColor = false;
            this.btnChangeit.Click += new System.EventHandler(this.btnChangeit_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.InitialImage = null;
            this.pictureBox2.Location = new System.Drawing.Point(315, 124);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(138, 43);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // labelChangeComments
            // 
            this.labelChangeComments.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelChangeComments.ForeColor = System.Drawing.Color.Blue;
            this.labelChangeComments.Location = new System.Drawing.Point(318, 20);
            this.labelChangeComments.Name = "labelChangeComments";
            this.labelChangeComments.Size = new System.Drawing.Size(135, 82);
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
            this.groupSlice.Location = new System.Drawing.Point(4, 7);
            this.groupSlice.Name = "groupSlice";
            this.groupSlice.Size = new System.Drawing.Size(164, 229);
            this.groupSlice.TabIndex = 4;
            this.groupSlice.TabStop = false;
            this.groupSlice.Text = "Slice";
            // 
            // ckMerge
            // 
            this.ckMerge.AutoSize = true;
            this.ckMerge.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckMerge.ForeColor = System.Drawing.Color.Black;
            this.ckMerge.Location = new System.Drawing.Point(9, 103);
            this.ckMerge.Name = "ckMerge";
            this.ckMerge.Size = new System.Drawing.Size(141, 17);
            this.ckMerge.TabIndex = 3;
            this.ckMerge.Text = "Merge result into one file";
            this.ckMerge.UseVisualStyleBackColor = true;
            // 
            // btnSliceit
            // 
            this.btnSliceit.BackColor = System.Drawing.SystemColors.Control;
            this.btnSliceit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSliceit.ForeColor = System.Drawing.Color.Red;
            this.btnSliceit.Location = new System.Drawing.Point(16, 171);
            this.btnSliceit.Name = "btnSliceit";
            this.btnSliceit.Size = new System.Drawing.Size(138, 49);
            this.btnSliceit.TabIndex = 2;
            this.btnSliceit.Text = "Sliceit!";
            this.btnSliceit.UseVisualStyleBackColor = false;
            this.btnSliceit.Click += new System.EventHandler(this.btnSliceit_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(16, 124);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(138, 43);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // labelSliceComments
            // 
            this.labelSliceComments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelSliceComments.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSliceComments.ForeColor = System.Drawing.Color.Blue;
            this.labelSliceComments.Location = new System.Drawing.Point(3, 18);
            this.labelSliceComments.Name = "labelSliceComments";
            this.labelSliceComments.Size = new System.Drawing.Size(158, 208);
            this.labelSliceComments.TabIndex = 0;
            this.labelSliceComments.Text = "Click Sliceit! to create a new SDLXLIFF file based on your selection criteria. Yo" +
    "u can add this file to your Project or share it with others to handle specific s" +
    "egments first.";
            // 
            // browseTableLayoutPanel
            // 
            this.browseTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.browseTableLayoutPanel.ColumnCount = 1;
            this.browseTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.browseTableLayoutPanel.Controls.Add(this.panel1, 0, 1);
            this.browseTableLayoutPanel.Controls.Add(this.browseLabel, 0, 0);
            this.browseTableLayoutPanel.Controls.Add(this.gridXLIFFFiles, 0, 2);
            this.browseTableLayoutPanel.Location = new System.Drawing.Point(6, 3);
            this.browseTableLayoutPanel.MaximumSize = new System.Drawing.Size(804, 274);
            this.browseTableLayoutPanel.Name = "browseTableLayoutPanel";
            this.browseTableLayoutPanel.RowCount = 3;
            this.browseTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.browseTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.browseTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.browseTableLayoutPanel.Size = new System.Drawing.Size(804, 247);
            this.browseTableLayoutPanel.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.cmbLanguages);
            this.panel1.Controls.Add(this.toolStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(798, 31);
            this.panel1.TabIndex = 5;
            // 
            // cmbLanguages
            // 
            this.cmbLanguages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbLanguages.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbLanguages.FormattingEnabled = true;
            this.cmbLanguages.Location = new System.Drawing.Point(646, 5);
            this.cmbLanguages.Name = "cmbLanguages";
            this.cmbLanguages.Size = new System.Drawing.Size(152, 21);
            this.cmbLanguages.TabIndex = 2;
            this.cmbLanguages.SelectedIndexChanged += new System.EventHandler(this.cmbLanguages_SelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.CanOverflow = false;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSelectFolder,
            this.btnSelectSDLXLIFFFiles,
            this.btnSelectProjectFile,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(643, 31);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Image = ((System.Drawing.Image)(resources.GetObject("btnSelectFolder.Image")));
            this.btnSelectFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(104, 28);
            this.btnSelectFolder.Text = "Select folder ...";
            this.btnSelectFolder.Click += new System.EventHandler(this.btnSelectFolder_Click);
            // 
            // btnSelectSDLXLIFFFiles
            // 
            this.btnSelectSDLXLIFFFiles.Image = ((System.Drawing.Image)(resources.GetObject("btnSelectSDLXLIFFFiles.Image")));
            this.btnSelectSDLXLIFFFiles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelectSDLXLIFFFiles.Name = "btnSelectSDLXLIFFFiles";
            this.btnSelectSDLXLIFFFiles.Size = new System.Drawing.Size(131, 28);
            this.btnSelectSDLXLIFFFiles.Text = "Select sdlxliff files ...";
            this.btnSelectSDLXLIFFFiles.Click += new System.EventHandler(this.btnSelectSDLXLIFFFiles_Click);
            // 
            // btnSelectProjectFile
            // 
            this.btnSelectProjectFile.Image = ((System.Drawing.Image)(resources.GetObject("btnSelectProjectFile.Image")));
            this.btnSelectProjectFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSelectProjectFile.Name = "btnSelectProjectFile";
            this.btnSelectProjectFile.Size = new System.Drawing.Size(129, 28);
            this.btnSelectProjectFile.Text = "Select project file ...";
            this.btnSelectProjectFile.Click += new System.EventHandler(this.btnSelectProjectFile_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(136, 28);
            this.toolStripButton1.Text = "Clear selected files ...";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // browseLabel
            // 
            this.browseLabel.AutoSize = true;
            this.browseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.browseLabel.Location = new System.Drawing.Point(3, 0);
            this.browseLabel.Name = "browseLabel";
            this.browseLabel.Size = new System.Drawing.Size(134, 13);
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
            this.gridXLIFFFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPath,
            this.colName,
            this.colSize,
            this.colDate});
            this.gridXLIFFFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridXLIFFFiles.GridColor = System.Drawing.SystemColors.Control;
            this.gridXLIFFFiles.Location = new System.Drawing.Point(3, 53);
            this.gridXLIFFFiles.MinimumSize = new System.Drawing.Size(100, 0);
            this.gridXLIFFFiles.Name = "gridXLIFFFiles";
            this.gridXLIFFFiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridXLIFFFiles.Size = new System.Drawing.Size(798, 191);
            this.gridXLIFFFiles.TabIndex = 2;
            this.gridXLIFFFiles.SelectionChanged += new System.EventHandler(this.gridXLIFFFiles_SelectionChanged);
            this.gridXLIFFFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.gridXLIFFFiles_DragDrop);
            this.gridXLIFFFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.gridXLIFFFiles_DragEnter);
            // 
            // colPath
            // 
            this.colPath.DataPropertyName = "Path";
            this.colPath.HeaderText = "Path";
            this.colPath.Name = "colPath";
            this.colPath.Visible = false;
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colName.DataPropertyName = "Name";
            this.colName.HeaderText = "Name";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // colSize
            // 
            this.colSize.DataPropertyName = "Size";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colSize.DefaultCellStyle = dataGridViewCellStyle2;
            this.colSize.HeaderText = "Size";
            this.colSize.Name = "colSize";
            this.colSize.ReadOnly = true;
            // 
            // colDate
            // 
            this.colDate.DataPropertyName = "Date";
            this.colDate.HeaderText = "Date";
            this.colDate.Name = "colDate";
            this.colDate.Width = 190;
            // 
            // mainTableLayoutPanel
            // 
            this.mainTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainTableLayoutPanel.ColumnCount = 1;
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainTableLayoutPanel.Controls.Add(this.panelCommands, 0, 2);
            this.mainTableLayoutPanel.Controls.Add(this.panelStatus, 0, 3);
            this.mainTableLayoutPanel.Controls.Add(this.tabControl1, 0, 1);
            this.mainTableLayoutPanel.Controls.Add(this.browseTableLayoutPanel, 0, 0);
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.RowCount = 4;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(817, 1056);
            this.mainTableLayoutPanel.TabIndex = 11;
            // 
            // panelStatus
            // 
            this.panelStatus.Controls.Add(this.progressBar);
            this.panelStatus.Location = new System.Drawing.Point(3, 786);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(794, 51);
            this.panelStatus.TabIndex = 10;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(18, 21);
            this.progressBar.MarqueeAnimationSpeed = 10;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(758, 14);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 8;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // SDLXLIFFSliceOrChange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.Controls.Add(this.mainTableLayoutPanel);
            this.Name = "SDLXLIFFSliceOrChange";
            this.Size = new System.Drawing.Size(820, 1059);
            this.Load += new System.EventHandler(this.SDLXLIFFSliceOrChange_Load);
            this.SizeChanged += new System.EventHandler(this.SDLXLIFFSliceOrChange_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.xLIFFFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.filesBindingSource)).EndInit();
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
            ((System.ComponentModel.ISupportInitialize)(this.gridSearchResults)).EndInit();
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
            this.browseTableLayoutPanel.ResumeLayout(false);
            this.browseTableLayoutPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridXLIFFFiles)).EndInit();
            this.mainTableLayoutPanel.ResumeLayout(false);
            this.panelStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
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
        public System.Windows.Forms.Button btnExpandSearchResults;
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
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ErrorProvider errorProvider1;
		public System.Windows.Forms.CheckBox ckNeuralMachineTranslation;
	}
}

