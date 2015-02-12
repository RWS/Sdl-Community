namespace Sdl.Community.EmbeddedContentProcessor.UI
{
    partial class EmbeddedContentSettingsControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._enableProcessingCheckbox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this._tagDefinitionLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._rulesListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._addRuleButton = new System.Windows.Forms.Button();
            this._removeAllButton = new System.Windows.Forms.Button();
            this._removeRuleButton = new System.Windows.Forms.Button();
            this._editRuleButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this._tagDefinitionLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(570, 525);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Embedded content";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this._enableProcessingCheckbox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(564, 506);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // _enableProcessingCheckbox
            // 
            this._enableProcessingCheckbox.AutoSize = true;
            this._enableProcessingCheckbox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._enableProcessingCheckbox.Location = new System.Drawing.Point(3, 29);
            this._enableProcessingCheckbox.Name = "_enableProcessingCheckbox";
            this._enableProcessingCheckbox.Size = new System.Drawing.Size(205, 17);
            this._enableProcessingCheckbox.TabIndex = 6;
            this._enableProcessingCheckbox.Text = "&Enable embedded content processing";
            this._enableProcessingCheckbox.UseVisualStyleBackColor = true;
            this._enableProcessingCheckbox.CheckedChanged += new System.EventHandler(this._enableProcessingCheckbox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(550, 26);
            this.label1.TabIndex = 5;
            this.label1.Text = "Allows processing of content embedded in a document into tagged markup according " +
    "to a set of regular expression rules. Embedded content is identified using docum" +
    "ent structure information.";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this._tagDefinitionLayoutPanel);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 52);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(558, 451);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tag definition rules";
            // 
            // _tagDefinitionLayoutPanel
            // 
            this._tagDefinitionLayoutPanel.ColumnCount = 2;
            this._tagDefinitionLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tagDefinitionLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tagDefinitionLayoutPanel.Controls.Add(this._rulesListView, 0, 0);
            this._tagDefinitionLayoutPanel.Controls.Add(this._addRuleButton, 1, 0);
            this._tagDefinitionLayoutPanel.Controls.Add(this._removeAllButton, 1, 3);
            this._tagDefinitionLayoutPanel.Controls.Add(this._removeRuleButton, 1, 2);
            this._tagDefinitionLayoutPanel.Controls.Add(this._editRuleButton, 1, 1);
            this._tagDefinitionLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tagDefinitionLayoutPanel.Location = new System.Drawing.Point(3, 16);
            this._tagDefinitionLayoutPanel.Name = "_tagDefinitionLayoutPanel";
            this._tagDefinitionLayoutPanel.RowCount = 5;
            this._tagDefinitionLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tagDefinitionLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tagDefinitionLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tagDefinitionLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tagDefinitionLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tagDefinitionLayoutPanel.Size = new System.Drawing.Size(552, 432);
            this._tagDefinitionLayoutPanel.TabIndex = 0;
            // 
            // _rulesListView
            // 
            this._rulesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this._rulesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rulesListView.FullRowSelect = true;
            this._rulesListView.HideSelection = false;
            this._rulesListView.Location = new System.Drawing.Point(3, 3);
            this._rulesListView.MultiSelect = false;
            this._rulesListView.Name = "_rulesListView";
            this._tagDefinitionLayoutPanel.SetRowSpan(this._rulesListView, 5);
            this._rulesListView.Size = new System.Drawing.Size(439, 426);
            this._rulesListView.TabIndex = 0;
            this._rulesListView.UseCompatibleStateImageBehavior = false;
            this._rulesListView.View = System.Windows.Forms.View.Details;
            this._rulesListView.SelectedIndexChanged += new System.EventHandler(this._rulesListView_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Start Tag Expression";
            this.columnHeader1.Width = 117;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "End Tag Expression";
            this.columnHeader2.Width = 114;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Tag Type";
            this.columnHeader3.Width = 89;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Translate";
            this.columnHeader4.Width = 72;
            // 
            // _addRuleButton
            // 
            this._addRuleButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._addRuleButton.Location = new System.Drawing.Point(448, 3);
            this._addRuleButton.Name = "_addRuleButton";
            this._addRuleButton.Size = new System.Drawing.Size(101, 23);
            this._addRuleButton.TabIndex = 1;
            this._addRuleButton.Text = "A&dd...";
            this._addRuleButton.UseVisualStyleBackColor = true;
            this._addRuleButton.Click += new System.EventHandler(this._addRuleButton_Click);
            // 
            // _removeAllButton
            // 
            this._removeAllButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._removeAllButton.Location = new System.Drawing.Point(448, 90);
            this._removeAllButton.Name = "_removeAllButton";
            this._removeAllButton.Size = new System.Drawing.Size(101, 23);
            this._removeAllButton.TabIndex = 4;
            this._removeAllButton.Text = "Remo&ve all";
            this._removeAllButton.UseVisualStyleBackColor = true;
            this._removeAllButton.Click += new System.EventHandler(this._removeAllButton_Click);
            // 
            // _removeRuleButton
            // 
            this._removeRuleButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._removeRuleButton.Location = new System.Drawing.Point(448, 61);
            this._removeRuleButton.Name = "_removeRuleButton";
            this._removeRuleButton.Size = new System.Drawing.Size(101, 23);
            this._removeRuleButton.TabIndex = 3;
            this._removeRuleButton.Text = "Rem&ove";
            this._removeRuleButton.UseVisualStyleBackColor = true;
            this._removeRuleButton.Click += new System.EventHandler(this._removeRuleButton_Click);
            // 
            // _editRuleButton
            // 
            this._editRuleButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._editRuleButton.Location = new System.Drawing.Point(448, 32);
            this._editRuleButton.Name = "_editRuleButton";
            this._editRuleButton.Size = new System.Drawing.Size(101, 23);
            this._editRuleButton.TabIndex = 2;
            this._editRuleButton.Text = "Edi&t...";
            this._editRuleButton.UseVisualStyleBackColor = true;
            this._editRuleButton.Click += new System.EventHandler(this._editRuleButton_Click);
            // 
            // EmbeddedContentSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "EmbeddedContentSettingsControl";
            this.Size = new System.Drawing.Size(570, 525);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this._tagDefinitionLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox _enableProcessingCheckbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TableLayoutPanel _tagDefinitionLayoutPanel;
        private System.Windows.Forms.ListView _rulesListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button _addRuleButton;
        private System.Windows.Forms.Button _removeAllButton;
        private System.Windows.Forms.Button _removeRuleButton;
        private System.Windows.Forms.Button _editRuleButton;
    }
}
