namespace Sdl.Community.TargetWordCount
{
    partial class WordCountBatchTaskSettingsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.includeSpacesCheckBox = new System.Windows.Forms.CheckBox();
            this.charPerLineLabel = new System.Windows.Forms.Label();
            this.charPerLineTextBox = new System.Windows.Forms.TextBox();
            this.lineCountCheckBox = new System.Windows.Forms.CheckBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.cultureLabel = new System.Windows.Forms.Label();
            this.cultureComboBox = new System.Windows.Forms.ComboBox();
            this.reportLockedCheckBox = new System.Windows.Forms.CheckBox();
            this.targetRadioButton = new System.Windows.Forms.RadioButton();
            this.sourceRadioButton = new System.Windows.Forms.RadioButton();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.TypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.includeSpacesCheckBox);
            this.groupBox.Controls.Add(this.charPerLineLabel);
            this.groupBox.Controls.Add(this.charPerLineTextBox);
            this.groupBox.Controls.Add(this.lineCountCheckBox);
            this.groupBox.Controls.Add(this.saveButton);
            this.groupBox.Controls.Add(this.loadButton);
            this.groupBox.Controls.Add(this.cultureLabel);
            this.groupBox.Controls.Add(this.cultureComboBox);
            this.groupBox.Controls.Add(this.reportLockedCheckBox);
            this.groupBox.Controls.Add(this.targetRadioButton);
            this.groupBox.Controls.Add(this.sourceRadioButton);
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox.Location = new System.Drawing.Point(0, 0);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(504, 108);
            this.groupBox.TabIndex = 1;
            this.groupBox.TabStop = false;
            // 
            // includeSpacesCheckBox
            // 
            this.includeSpacesCheckBox.AutoSize = true;
            this.includeSpacesCheckBox.Location = new System.Drawing.Point(395, 49);
            this.includeSpacesCheckBox.Name = "includeSpacesCheckBox";
            this.includeSpacesCheckBox.Size = new System.Drawing.Size(100, 16);
            this.includeSpacesCheckBox.TabIndex = 10;
            this.includeSpacesCheckBox.Text = "Include spaces";
            this.includeSpacesCheckBox.UseVisualStyleBackColor = true;
            // 
            // charPerLineLabel
            // 
            this.charPerLineLabel.AutoSize = true;
            this.charPerLineLabel.Location = new System.Drawing.Point(235, 49);
            this.charPerLineLabel.Name = "charPerLineLabel";
            this.charPerLineLabel.Size = new System.Drawing.Size(105, 12);
            this.charPerLineLabel.TabIndex = 9;
            this.charPerLineLabel.Text = "Characters per line:";
            // 
            // charPerLineTextBox
            // 
            this.charPerLineTextBox.Location = new System.Drawing.Point(346, 46);
            this.charPerLineTextBox.Name = "charPerLineTextBox";
            this.charPerLineTextBox.Size = new System.Drawing.Size(43, 19);
            this.charPerLineTextBox.TabIndex = 8;
            // 
            // lineCountCheckBox
            // 
            this.lineCountCheckBox.AutoSize = true;
            this.lineCountCheckBox.Location = new System.Drawing.Point(349, 17);
            this.lineCountCheckBox.Name = "lineCountCheckBox";
            this.lineCountCheckBox.Size = new System.Drawing.Size(114, 16);
            this.lineCountCheckBox.TabIndex = 7;
            this.lineCountCheckBox.Text = "Calculate per line";
            this.lineCountCheckBox.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(12, 79);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 6;
            this.saveButton.Text = "Save Rates";
            this.saveButton.UseVisualStyleBackColor = true;
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(93, 79);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(75, 23);
            this.loadButton.TabIndex = 5;
            this.loadButton.Text = "Load Rates";
            this.loadButton.UseVisualStyleBackColor = true;
            // 
            // cultureLabel
            // 
            this.cultureLabel.AutoSize = true;
            this.cultureLabel.Location = new System.Drawing.Point(6, 49);
            this.cultureLabel.Name = "cultureLabel";
            this.cultureLabel.Size = new System.Drawing.Size(81, 12);
            this.cultureLabel.TabIndex = 4;
            this.cultureLabel.Text = "Select country:";
            this.toolTip.SetToolTip(this.cultureLabel, "Select country to use for formatting the currency");
            // 
            // cultureComboBox
            // 
            this.cultureComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cultureComboBox.FormattingEnabled = true;
            this.cultureComboBox.Location = new System.Drawing.Point(93, 46);
            this.cultureComboBox.Name = "cultureComboBox";
            this.cultureComboBox.Size = new System.Drawing.Size(136, 20);
            this.cultureComboBox.TabIndex = 3;
            // 
            // reportLockedCheckBox
            // 
            this.reportLockedCheckBox.AutoSize = true;
            this.reportLockedCheckBox.Location = new System.Drawing.Point(132, 18);
            this.reportLockedCheckBox.Name = "reportLockedCheckBox";
            this.reportLockedCheckBox.Size = new System.Drawing.Size(210, 16);
            this.reportLockedCheckBox.TabIndex = 2;
            this.reportLockedCheckBox.Text = "Report Locked Segments Separately";
            this.reportLockedCheckBox.UseVisualStyleBackColor = true;
            // 
            // targetRadioButton
            // 
            this.targetRadioButton.AutoSize = true;
            this.targetRadioButton.Location = new System.Drawing.Point(70, 18);
            this.targetRadioButton.Name = "targetRadioButton";
            this.targetRadioButton.Size = new System.Drawing.Size(56, 16);
            this.targetRadioButton.TabIndex = 1;
            this.targetRadioButton.TabStop = true;
            this.targetRadioButton.Text = "Target";
            this.targetRadioButton.UseVisualStyleBackColor = true;
            // 
            // sourceRadioButton
            // 
            this.sourceRadioButton.AutoSize = true;
            this.sourceRadioButton.Location = new System.Drawing.Point(6, 18);
            this.sourceRadioButton.Name = "sourceRadioButton";
            this.sourceRadioButton.Size = new System.Drawing.Size(58, 16);
            this.sourceRadioButton.TabIndex = 0;
            this.sourceRadioButton.TabStop = true;
            this.sourceRadioButton.Text = "Source";
            this.sourceRadioButton.UseVisualStyleBackColor = true;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TypeColumn,
            this.RateColumn});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 108);
            this.dataGridView.MinimumSize = new System.Drawing.Size(599, 357);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.Size = new System.Drawing.Size(599, 357);
            this.dataGridView.TabIndex = 2;
            // 
            // TypeColumn
            // 
            this.TypeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.TypeColumn.HeaderText = "Type";
            this.TypeColumn.Name = "TypeColumn";
            this.TypeColumn.ReadOnly = true;
            // 
            // RateColumn
            // 
            this.RateColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.RateColumn.HeaderText = "Rate";
            this.RateColumn.Name = "RateColumn";
            // 
            // WordCountBatchTaskSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.groupBox);
            this.MinimumSize = new System.Drawing.Size(504, 436);
            this.Name = "WordCountBatchTaskSettingsControl";
            this.Size = new System.Drawing.Size(504, 436);
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.RadioButton sourceRadioButton;
        private System.Windows.Forms.RadioButton targetRadioButton;
        private System.Windows.Forms.CheckBox reportLockedCheckBox;
        private System.Windows.Forms.Label cultureLabel;
        private System.Windows.Forms.ComboBox cultureComboBox;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn RateColumn;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.CheckBox lineCountCheckBox;
        private System.Windows.Forms.Label charPerLineLabel;
        private System.Windows.Forms.TextBox charPerLineTextBox;
        private System.Windows.Forms.CheckBox includeSpacesCheckBox;
    }
}
