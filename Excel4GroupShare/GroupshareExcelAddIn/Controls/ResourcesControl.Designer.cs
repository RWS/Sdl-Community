using GroupshareExcelAddIn.ComboTreeBoxControl;

namespace GroupshareExcelAddIn.Controls
{
    partial class ResourcesControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResourcesControl));
            this._translationMemoriesTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._organization_groupBox = new System.Windows.Forms.GroupBox();
            this._includeSubOrgs_checkBox = new System.Windows.Forms.CheckBox();
            this._orgComboTreeBox = new GroupshareExcelAddIn.ComboTreeBoxControl.CustomTreeView();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this._lblProcessingMessage = new System.Windows.Forms.Label();
            this._getDataButton = new System.Windows.Forms.Button();
            this._dataTypeGroupBox = new System.Windows.Forms.GroupBox();
            this._dataTypeComboBox = new System.Windows.Forms.ComboBox();
            this._translationMemoriesTableLayoutPanel.SuspendLayout();
            this._organization_groupBox.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this._dataTypeGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _translationMemoriesTableLayoutPanel
            // 
            this._translationMemoriesTableLayoutPanel.AutoSize = true;
            this._translationMemoriesTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._translationMemoriesTableLayoutPanel.ColumnCount = 1;
            this._translationMemoriesTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._translationMemoriesTableLayoutPanel.Controls.Add(this._organization_groupBox, 0, 1);
            this._translationMemoriesTableLayoutPanel.Controls.Add(this.tableLayoutPanel2, 0, 3);
            this._translationMemoriesTableLayoutPanel.Controls.Add(this._dataTypeGroupBox, 0, 0);
            this._translationMemoriesTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._translationMemoriesTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._translationMemoriesTableLayoutPanel.Name = "_translationMemoriesTableLayoutPanel";
            this._translationMemoriesTableLayoutPanel.RowCount = 4;
            this._translationMemoriesTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._translationMemoriesTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._translationMemoriesTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._translationMemoriesTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._translationMemoriesTableLayoutPanel.Size = new System.Drawing.Size(385, 172);
            this._translationMemoriesTableLayoutPanel.TabIndex = 28;
            // 
            // _organization_groupBox
            // 
            this._organization_groupBox.AutoSize = true;
            this._organization_groupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._organization_groupBox.Controls.Add(this._includeSubOrgs_checkBox);
            this._organization_groupBox.Controls.Add(this._orgComboTreeBox);
            this._organization_groupBox.Location = new System.Drawing.Point(3, 68);
            this._organization_groupBox.MinimumSize = new System.Drawing.Size(395, 68);
            this._organization_groupBox.Name = "_organization_groupBox";
            this._organization_groupBox.Size = new System.Drawing.Size(395, 68);
            this._organization_groupBox.TabIndex = 27;
            this._organization_groupBox.TabStop = false;
            this._organization_groupBox.Text = "Organization";
            // 
            // _includeSubOrgs_checkBox
            // 
            this._includeSubOrgs_checkBox.AutoSize = true;
            this._includeSubOrgs_checkBox.Location = new System.Drawing.Point(227, 29);
            this._includeSubOrgs_checkBox.Name = "_includeSubOrgs_checkBox";
            this._includeSubOrgs_checkBox.Size = new System.Drawing.Size(143, 17);
            this._includeSubOrgs_checkBox.TabIndex = 29;
            this._includeSubOrgs_checkBox.Text = "Include suborganizations";
            this._includeSubOrgs_checkBox.UseVisualStyleBackColor = true;
            // 
            // _orgComboTreeBox
            // 
            this._orgComboTreeBox.DroppedDown = false;
            this._orgComboTreeBox.Enabled = false;
            this._orgComboTreeBox.Location = new System.Drawing.Point(6, 23);
            this._orgComboTreeBox.Name = "_orgComboTreeBox";
            this._orgComboTreeBox.SelectedNode = null;
            this._orgComboTreeBox.Size = new System.Drawing.Size(194, 23);
            this._orgComboTreeBox.TabIndex = 23;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this._lblProcessingMessage, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this._getDataButton, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 142);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(379, 27);
            this.tableLayoutPanel2.TabIndex = 28;
            // 
            // _lblProcessingMessage
            // 
            this._lblProcessingMessage.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._lblProcessingMessage.AutoSize = true;
            this._lblProcessingMessage.Location = new System.Drawing.Point(3, 7);
            this._lblProcessingMessage.Name = "_lblProcessingMessage";
            this._lblProcessingMessage.Size = new System.Drawing.Size(119, 13);
            this._lblProcessingMessage.TabIndex = 12;
            this._lblProcessingMessage.Text = "Loading organizations...";
            this._lblProcessingMessage.Visible = false;
            // 
            // _getDataButton
            // 
            this._getDataButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._getDataButton.Enabled = false;
            this._getDataButton.Location = new System.Drawing.Point(287, 3);
            this._getDataButton.Name = "_getDataButton";
            this._getDataButton.Size = new System.Drawing.Size(89, 21);
            this._getDataButton.TabIndex = 9;
            this._getDataButton.Text = "Get Data";
            this._getDataButton.UseVisualStyleBackColor = true;
            this._getDataButton.Click += new System.EventHandler(this.GetDataButton_Click);
            // 
            // _dataTypeGroupBox
            // 
            this._dataTypeGroupBox.AutoSize = true;
            this._dataTypeGroupBox.Controls.Add(this._dataTypeComboBox);
            this._dataTypeGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dataTypeGroupBox.Location = new System.Drawing.Point(3, 3);
            this._dataTypeGroupBox.Name = "_dataTypeGroupBox";
            this._dataTypeGroupBox.Size = new System.Drawing.Size(379, 59);
            this._dataTypeGroupBox.TabIndex = 29;
            this._dataTypeGroupBox.TabStop = false;
            this._dataTypeGroupBox.Text = "Data Type";
            // 
            // _dataTypeComboBox
            // 
            this._dataTypeComboBox.FormattingEnabled = true;
            this._dataTypeComboBox.Location = new System.Drawing.Point(6, 19);
            this._dataTypeComboBox.Name = "_dataTypeComboBox";
            this._dataTypeComboBox.Size = new System.Drawing.Size(194, 21);
            this._dataTypeComboBox.TabIndex = 2;
            // 
            // ResourcesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(385, 172);
            this.Controls.Add(this._translationMemoriesTableLayoutPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "ResourcesControl";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Filter GroupShare Resources";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ResourcesControl_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TranslationMemoryControl_KeyDown);
            this._translationMemoriesTableLayoutPanel.ResumeLayout(false);
            this._translationMemoriesTableLayoutPanel.PerformLayout();
            this._organization_groupBox.ResumeLayout(false);
            this._organization_groupBox.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this._dataTypeGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel _translationMemoriesTableLayoutPanel;
        private System.Windows.Forms.Label _lblProcessingMessage;
        private CustomTreeView _orgComboTreeBox;
        private System.Windows.Forms.GroupBox _organization_groupBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button _getDataButton;
        private System.Windows.Forms.GroupBox _dataTypeGroupBox;
        private System.Windows.Forms.ComboBox _dataTypeComboBox;
        private System.Windows.Forms.CheckBox _includeSubOrgs_checkBox;
    }
}