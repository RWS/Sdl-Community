namespace GroupshareExcelAddIn.Controls
{
    partial class ResourcesForm
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
            this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._dataTypeGroupBox = new System.Windows.Forms.GroupBox();
            this._dataTypeComboBox = new System.Windows.Forms.ComboBox();
            this._tableLayoutPanel.SuspendLayout();
            this._dataTypeGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tableLayoutPanel
            // 
            this._tableLayoutPanel.AutoSize = true;
            this._tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._tableLayoutPanel.ColumnCount = 1;
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanel.Controls.Add(this._dataTypeGroupBox, 0, 0);
            this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._tableLayoutPanel.Name = "_tableLayoutPanel";
            this._tableLayoutPanel.RowCount = 2;
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanel.Size = new System.Drawing.Size(365, 207);
            this._tableLayoutPanel.TabIndex = 0;
            // 
            // _dataTypeGroupBox
            // 
            this._dataTypeGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._dataTypeGroupBox.Controls.Add(this._dataTypeComboBox);
            this._dataTypeGroupBox.Location = new System.Drawing.Point(3, 3);
            this._dataTypeGroupBox.Name = "_dataTypeGroupBox";
            this._dataTypeGroupBox.Size = new System.Drawing.Size(359, 59);
            this._dataTypeGroupBox.TabIndex = 4;
            this._dataTypeGroupBox.TabStop = false;
            this._dataTypeGroupBox.Text = "Data type";
            // 
            // _dataTypeComboBox
            // 
            this._dataTypeComboBox.FormattingEnabled = true;
            this._dataTypeComboBox.Location = new System.Drawing.Point(6, 19);
            this._dataTypeComboBox.Name = "_dataTypeComboBox";
            this._dataTypeComboBox.Size = new System.Drawing.Size(194, 21);
            this._dataTypeComboBox.TabIndex = 1;
            this._dataTypeComboBox.SelectedIndexChanged += new System.EventHandler(this._dataTypeComboBox_SelectedIndexChanged);
            // 
            // ResourcesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(365, 207);
            this.Controls.Add(this._tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(381, 246);
            this.Name = "ResourcesForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "ResourcesForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ResourcesForm_FormClosing);
            this._tableLayoutPanel.ResumeLayout(false);
            this._dataTypeGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
        private System.Windows.Forms.GroupBox _dataTypeGroupBox;
        private System.Windows.Forms.ComboBox _dataTypeComboBox;
    }
}