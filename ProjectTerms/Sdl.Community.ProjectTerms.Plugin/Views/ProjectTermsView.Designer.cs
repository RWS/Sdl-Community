namespace Sdl.Community.ProjectTerms.Plugin.Views
{
    partial class ProjectTermsView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectTermsView));
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.buttonExtractTerms = new System.Windows.Forms.Button();
            this.buttonIncludeFile = new System.Windows.Forms.Button();
            this.labelErrorRegex = new System.Windows.Forms.Label();
            this.buttonShowWordCloud = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxRegex = new System.Windows.Forms.CheckBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonResetList = new System.Windows.Forms.Button();
            this.numericUpDownTermsLength = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownTermsOccurrences = new System.Windows.Forms.NumericUpDown();
            this.labelTermsLength = new System.Windows.Forms.Label();
            this.labelOccurrences = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelTerm = new System.Windows.Forms.Label();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.textBoxTerm = new System.Windows.Forms.TextBox();
            this.labelBlackList = new System.Windows.Forms.Label();
            this.listViewBlackList = new System.Windows.Forms.ListView();
            this.Terms = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.progressBarExtractTerms = new System.Windows.Forms.ProgressBar();
            this.openFileDialogLoadFile = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogBlacklist = new System.Windows.Forms.SaveFileDialog();
            this.groupBoxSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTermsLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTermsOccurrences)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.buttonExtractTerms);
            this.groupBoxSettings.Controls.Add(this.buttonIncludeFile);
            this.groupBoxSettings.Controls.Add(this.labelErrorRegex);
            this.groupBoxSettings.Controls.Add(this.buttonShowWordCloud);
            this.groupBoxSettings.Controls.Add(this.label2);
            this.groupBoxSettings.Controls.Add(this.checkBoxRegex);
            this.groupBoxSettings.Controls.Add(this.buttonSave);
            this.groupBoxSettings.Controls.Add(this.buttonLoad);
            this.groupBoxSettings.Controls.Add(this.buttonDelete);
            this.groupBoxSettings.Controls.Add(this.buttonResetList);
            this.groupBoxSettings.Controls.Add(this.numericUpDownTermsLength);
            this.groupBoxSettings.Controls.Add(this.numericUpDownTermsOccurrences);
            this.groupBoxSettings.Controls.Add(this.labelTermsLength);
            this.groupBoxSettings.Controls.Add(this.labelOccurrences);
            this.groupBoxSettings.Controls.Add(this.label1);
            this.groupBoxSettings.Controls.Add(this.labelTerm);
            this.groupBoxSettings.Controls.Add(this.buttonAdd);
            this.groupBoxSettings.Controls.Add(this.textBoxTerm);
            this.groupBoxSettings.Controls.Add(this.labelBlackList);
            this.groupBoxSettings.Controls.Add(this.listViewBlackList);
            this.groupBoxSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.groupBoxSettings.Location = new System.Drawing.Point(10, 6);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(581, 417);
            this.groupBoxSettings.TabIndex = 3;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Project terms settings";
            // 
            // buttonExtractTerms
            // 
            this.buttonExtractTerms.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonExtractTerms.Location = new System.Drawing.Point(3, 372);
            this.buttonExtractTerms.Name = "buttonExtractTerms";
            this.buttonExtractTerms.Size = new System.Drawing.Size(176, 23);
            this.buttonExtractTerms.TabIndex = 25;
            this.buttonExtractTerms.UseVisualStyleBackColor = true;
            this.buttonExtractTerms.Click += new System.EventHandler(this.buttonExtractTerms_Click);
            // 
            // buttonIncludeFile
            // 
            this.buttonIncludeFile.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonIncludeFile.Location = new System.Drawing.Point(400, 372);
            this.buttonIncludeFile.Name = "buttonIncludeFile";
            this.buttonIncludeFile.Size = new System.Drawing.Size(176, 23);
            this.buttonIncludeFile.TabIndex = 24;
            this.buttonIncludeFile.Text = "Include terms file to the project";
            this.buttonIncludeFile.UseVisualStyleBackColor = true;
            this.buttonIncludeFile.Click += new System.EventHandler(this.buttonIncludeFile_Click);
            // 
            // labelErrorRegex
            // 
            this.labelErrorRegex.AutoSize = true;
            this.labelErrorRegex.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelErrorRegex.Location = new System.Drawing.Point(201, 81);
            this.labelErrorRegex.Name = "labelErrorRegex";
            this.labelErrorRegex.Size = new System.Drawing.Size(0, 13);
            this.labelErrorRegex.TabIndex = 22;
            // 
            // buttonShowWordCloud
            // 
            this.buttonShowWordCloud.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonShowWordCloud.Location = new System.Drawing.Point(201, 372);
            this.buttonShowWordCloud.Name = "buttonShowWordCloud";
            this.buttonShowWordCloud.Size = new System.Drawing.Size(176, 23);
            this.buttonShowWordCloud.TabIndex = 21;
            this.buttonShowWordCloud.Text = "Preview the extracted terms";
            this.buttonShowWordCloud.UseVisualStyleBackColor = true;
            this.buttonShowWordCloud.Click += new System.EventHandler(this.buttonShowWordCloud_Click);
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(5, 356);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(571, 2);
            this.label2.TabIndex = 20;
            this.label2.Text = "labelSeparator1";
            // 
            // checkBoxRegex
            // 
            this.checkBoxRegex.AutoSize = true;
            this.checkBoxRegex.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.checkBoxRegex.Location = new System.Drawing.Point(358, 55);
            this.checkBoxRegex.Name = "checkBoxRegex";
            this.checkBoxRegex.Size = new System.Drawing.Size(138, 17);
            this.checkBoxRegex.TabIndex = 19;
            this.checkBoxRegex.Text = "Use regular expressions";
            this.checkBoxRegex.UseVisualStyleBackColor = true;
            this.checkBoxRegex.CheckedChanged += new System.EventHandler(this.checkBoxRegex_CheckedChanged);
            // 
            // buttonSave
            // 
            this.buttonSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonSave.Location = new System.Drawing.Point(201, 219);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(130, 23);
            this.buttonSave.TabIndex = 18;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonLoad.Location = new System.Drawing.Point(201, 190);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(130, 23);
            this.buttonLoad.TabIndex = 17;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonDelete.Location = new System.Drawing.Point(201, 130);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(130, 23);
            this.buttonDelete.TabIndex = 16;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonResetList
            // 
            this.buttonResetList.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonResetList.Location = new System.Drawing.Point(201, 160);
            this.buttonResetList.Name = "buttonResetList";
            this.buttonResetList.Size = new System.Drawing.Size(130, 23);
            this.buttonResetList.TabIndex = 15;
            this.buttonResetList.Text = "Reset";
            this.buttonResetList.UseVisualStyleBackColor = true;
            this.buttonResetList.Click += new System.EventHandler(this.buttonResetList_Click);
            // 
            // numericUpDownTermsLength
            // 
            this.numericUpDownTermsLength.Location = new System.Drawing.Point(139, 313);
            this.numericUpDownTermsLength.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownTermsLength.Name = "numericUpDownTermsLength";
            this.numericUpDownTermsLength.Size = new System.Drawing.Size(50, 20);
            this.numericUpDownTermsLength.TabIndex = 11;
            this.numericUpDownTermsLength.ValueChanged += new System.EventHandler(this.numericUpDownTermsLength_ValueChanged);
            // 
            // numericUpDownTermsOccurrences
            // 
            this.numericUpDownTermsOccurrences.Location = new System.Drawing.Point(139, 284);
            this.numericUpDownTermsOccurrences.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownTermsOccurrences.Name = "numericUpDownTermsOccurrences";
            this.numericUpDownTermsOccurrences.Size = new System.Drawing.Size(50, 20);
            this.numericUpDownTermsOccurrences.TabIndex = 10;
            this.numericUpDownTermsOccurrences.ValueChanged += new System.EventHandler(this.numericUpDownTermsOccurrences_ValueChanged);
            // 
            // labelTermsLength
            // 
            this.labelTermsLength.AutoSize = true;
            this.labelTermsLength.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTermsLength.Location = new System.Drawing.Point(10, 315);
            this.labelTermsLength.Name = "labelTermsLength";
            this.labelTermsLength.Size = new System.Drawing.Size(71, 13);
            this.labelTermsLength.TabIndex = 7;
            this.labelTermsLength.Text = "Terms length:";
            // 
            // labelOccurrences
            // 
            this.labelOccurrences.AutoSize = true;
            this.labelOccurrences.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelOccurrences.Location = new System.Drawing.Point(10, 285);
            this.labelOccurrences.Name = "labelOccurrences";
            this.labelOccurrences.Size = new System.Drawing.Size(104, 13);
            this.labelOccurrences.TabIndex = 6;
            this.labelOccurrences.Text = "Terms  occurrences:";
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(6, 261);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(571, 2);
            this.label1.TabIndex = 5;
            this.label1.Text = "labelSeparator1";
            // 
            // labelTerm
            // 
            this.labelTerm.AutoSize = true;
            this.labelTerm.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTerm.Location = new System.Drawing.Point(198, 34);
            this.labelTerm.Name = "labelTerm";
            this.labelTerm.Size = new System.Drawing.Size(111, 13);
            this.labelTerm.TabIndex = 4;
            this.labelTerm.Text = "Enter term to blacklist:";
            // 
            // buttonAdd
            // 
            this.buttonAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonAdd.Location = new System.Drawing.Point(201, 100);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(130, 23);
            this.buttonAdd.TabIndex = 3;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // textBoxTerm
            // 
            this.textBoxTerm.Location = new System.Drawing.Point(201, 55);
            this.textBoxTerm.Name = "textBoxTerm";
            this.textBoxTerm.Size = new System.Drawing.Size(130, 20);
            this.textBoxTerm.TabIndex = 2;
            this.textBoxTerm.TextChanged += new System.EventHandler(this.textBoxTerm_TextChanged);
            // 
            // labelBlackList
            // 
            this.labelBlackList.AutoSize = true;
            this.labelBlackList.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelBlackList.Location = new System.Drawing.Point(7, 18);
            this.labelBlackList.Name = "labelBlackList";
            this.labelBlackList.Size = new System.Drawing.Size(49, 13);
            this.labelBlackList.TabIndex = 1;
            this.labelBlackList.Text = "Blacklist:";
            // 
            // listViewBlackList
            // 
            this.listViewBlackList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Terms});
            this.listViewBlackList.FullRowSelect = true;
            this.listViewBlackList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewBlackList.Location = new System.Drawing.Point(10, 37);
            this.listViewBlackList.Name = "listViewBlackList";
            this.listViewBlackList.Size = new System.Drawing.Size(185, 205);
            this.listViewBlackList.TabIndex = 0;
            this.listViewBlackList.UseCompatibleStateImageBehavior = false;
            this.listViewBlackList.View = System.Windows.Forms.View.Details;
            // 
            // Terms
            // 
            this.Terms.Width = 178;
            // 
            // progressBarExtractTerms
            // 
            this.progressBarExtractTerms.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.progressBarExtractTerms.Location = new System.Drawing.Point(10, 429);
            this.progressBarExtractTerms.Name = "progressBarExtractTerms";
            this.progressBarExtractTerms.Size = new System.Drawing.Size(577, 23);
            this.progressBarExtractTerms.TabIndex = 4;
            // 
            // ProjectTermsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 461);
            this.Controls.Add(this.progressBarExtractTerms);
            this.Controls.Add(this.groupBoxSettings);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ProjectTermsView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ProjectTermExtract";
            this.Load += new System.EventHandler(this.ProjectTermsView_Load);
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTermsLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTermsOccurrences)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.Button buttonIncludeFile;
        private System.Windows.Forms.Label labelErrorRegex;
        private System.Windows.Forms.Button buttonShowWordCloud;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxRegex;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonResetList;
        private System.Windows.Forms.NumericUpDown numericUpDownTermsLength;
        private System.Windows.Forms.NumericUpDown numericUpDownTermsOccurrences;
        private System.Windows.Forms.Label labelTermsLength;
        private System.Windows.Forms.Label labelOccurrences;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTerm;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.TextBox textBoxTerm;
        private System.Windows.Forms.Label labelBlackList;
        private System.Windows.Forms.ListView listViewBlackList;
        private System.Windows.Forms.ProgressBar progressBarExtractTerms;
        private System.Windows.Forms.ColumnHeader Terms;
        private System.Windows.Forms.Button buttonExtractTerms;
        private System.Windows.Forms.OpenFileDialog openFileDialogLoadFile;
        private System.Windows.Forms.SaveFileDialog saveFileDialogBlacklist;
    }
}