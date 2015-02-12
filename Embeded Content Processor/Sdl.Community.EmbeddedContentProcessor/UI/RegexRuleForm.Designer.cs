namespace Sdl.Community.EmbeddedContentProcessor.UI
{
    partial class RegexRuleForm
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this._ruleTypeComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this._ruleGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.l_Opening = new System.Windows.Forms.Label();
            this._endTagTextBox = new System.Windows.Forms.TextBox();
            this.l_Closing = new System.Windows.Forms.Label();
            this._ignoreCaseCheckbox = new System.Windows.Forms.CheckBox();
            this._startTagTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this._translateComboBox = new System.Windows.Forms.ComboBox();
            this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this._ruleGroupBox.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._ruleTypeComboBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._ruleGroupBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 63F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 37F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(536, 247);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox2, 2);
            this.groupBox2.Controls.Add(this.btnCancel);
            this.groupBox2.Controls.Add(this.btnOk);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 193);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(530, 51);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(449, 19);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(368, 19);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // _ruleTypeComboBox
            // 
            this._ruleTypeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._ruleTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._ruleTypeComboBox.FormattingEnabled = true;
            this._ruleTypeComboBox.Location = new System.Drawing.Point(65, 3);
            this._ruleTypeComboBox.Name = "_ruleTypeComboBox";
            this._ruleTypeComboBox.Size = new System.Drawing.Size(468, 21);
            this._ruleTypeComboBox.TabIndex = 2;
            this._ruleTypeComboBox.SelectedIndexChanged += new System.EventHandler(this._ruleTypeComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "&Tag Type:";
            // 
            // _ruleGroupBox
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._ruleGroupBox, 2);
            this._ruleGroupBox.Controls.Add(this.tableLayoutPanel3);
            this._ruleGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._ruleGroupBox.Location = new System.Drawing.Point(3, 30);
            this._ruleGroupBox.Name = "_ruleGroupBox";
            this._ruleGroupBox.Size = new System.Drawing.Size(530, 97);
            this._ruleGroupBox.TabIndex = 3;
            this._ruleGroupBox.TabStop = false;
            this._ruleGroupBox.Text = "Regular Expression";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.l_Opening, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this._endTagTextBox, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.l_Closing, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this._ignoreCaseCheckbox, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this._startTagTextBox, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(524, 78);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // l_Opening
            // 
            this.l_Opening.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.l_Opening.AutoSize = true;
            this.l_Opening.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.l_Opening.Location = new System.Drawing.Point(3, 6);
            this.l_Opening.Name = "l_Opening";
            this.l_Opening.Size = new System.Drawing.Size(54, 13);
            this.l_Opening.TabIndex = 0;
            this.l_Opening.Text = "&Start Tag:";
            // 
            // _endTagTextBox
            // 
            this._endTagTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._endTagTextBox.Location = new System.Drawing.Point(63, 29);
            this._endTagTextBox.Name = "_endTagTextBox";
            this._endTagTextBox.Size = new System.Drawing.Size(458, 20);
            this._endTagTextBox.TabIndex = 3;
            this._endTagTextBox.TextChanged += new System.EventHandler(this.tb_Closing_TextChanged);
            // 
            // l_Closing
            // 
            this.l_Closing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.l_Closing.AutoSize = true;
            this.l_Closing.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.l_Closing.Location = new System.Drawing.Point(3, 32);
            this.l_Closing.Name = "l_Closing";
            this.l_Closing.Size = new System.Drawing.Size(54, 13);
            this.l_Closing.TabIndex = 2;
            this.l_Closing.Text = "&End Tag:";
            // 
            // _ignoreCaseCheckbox
            // 
            this._ignoreCaseCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._ignoreCaseCheckbox.AutoSize = true;
            this.tableLayoutPanel3.SetColumnSpan(this._ignoreCaseCheckbox, 2);
            this._ignoreCaseCheckbox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._ignoreCaseCheckbox.Location = new System.Drawing.Point(3, 56);
            this._ignoreCaseCheckbox.Name = "_ignoreCaseCheckbox";
            this._ignoreCaseCheckbox.Size = new System.Drawing.Size(518, 17);
            this._ignoreCaseCheckbox.TabIndex = 4;
            this._ignoreCaseCheckbox.Text = "&Ignore case";
            this._ignoreCaseCheckbox.UseVisualStyleBackColor = true;
            // 
            // _startTagTextBox
            // 
            this._startTagTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._startTagTextBox.Location = new System.Drawing.Point(63, 3);
            this._startTagTextBox.Name = "_startTagTextBox";
            this._startTagTextBox.Size = new System.Drawing.Size(458, 20);
            this._startTagTextBox.TabIndex = 1;
            this._startTagTextBox.TextChanged += new System.EventHandler(this.tb_Opening_TextChanged);
            // 
            // groupBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 2);
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 133);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(530, 54);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tag Properties";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this._translateComboBox, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(524, 35);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(3, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "T&ranslate:";
            // 
            // _translateComboBox
            // 
            this._translateComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._translateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._translateComboBox.FormattingEnabled = true;
            this._translateComboBox.Location = new System.Drawing.Point(63, 7);
            this._translateComboBox.Name = "_translateComboBox";
            this._translateComboBox.Size = new System.Drawing.Size(458, 21);
            this._translateComboBox.TabIndex = 2;
            // 
            // _errorProvider
            // 
            this._errorProvider.ContainerControl = this;
            // 
            // RegexRuleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 247);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RegexRuleForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add/Edit Embedded Content Rule";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this._ruleGroupBox.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox _ruleTypeComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox _ruleGroupBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label l_Opening;
        private System.Windows.Forms.TextBox _endTagTextBox;
        private System.Windows.Forms.Label l_Closing;
        private System.Windows.Forms.CheckBox _ignoreCaseCheckbox;
        private System.Windows.Forms.TextBox _startTagTextBox;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox _translateComboBox;
        private System.Windows.Forms.ErrorProvider _errorProvider;
    }
}