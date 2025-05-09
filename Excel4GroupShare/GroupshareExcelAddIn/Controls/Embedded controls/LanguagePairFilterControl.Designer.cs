namespace GroupshareExcelAddIn.Controls.Embedded_controls
{
    partial class LanguagePairFilterControl
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
            this._languagePairsGroupBox = new System.Windows.Forms.GroupBox();
            this._targetLangComboBox = new System.Windows.Forms.ComboBox();
            this._sourceLangComboBox = new System.Windows.Forms.ComboBox();
            this._label2 = new System.Windows.Forms.Label();
            this._label1 = new System.Windows.Forms.Label();
            this._languagePairsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _languagePairsGroupBox
            // 
            this._languagePairsGroupBox.AutoSize = true;
            this._languagePairsGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._languagePairsGroupBox.Controls.Add(this._targetLangComboBox);
            this._languagePairsGroupBox.Controls.Add(this._sourceLangComboBox);
            this._languagePairsGroupBox.Controls.Add(this._label2);
            this._languagePairsGroupBox.Controls.Add(this._label1);
            this._languagePairsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._languagePairsGroupBox.Location = new System.Drawing.Point(0, 0);
            this._languagePairsGroupBox.Name = "_languagePairsGroupBox";
            this._languagePairsGroupBox.Size = new System.Drawing.Size(379, 112);
            this._languagePairsGroupBox.TabIndex = 15;
            this._languagePairsGroupBox.TabStop = false;
            this._languagePairsGroupBox.Text = "Language Pair";
            // 
            // _targetLangComboBox
            // 
            this._targetLangComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._targetLangComboBox.FormattingEnabled = true;
            this._targetLangComboBox.Items.AddRange(new object[] {
            "Any"});
            this._targetLangComboBox.Location = new System.Drawing.Point(157, 72);
            this._targetLangComboBox.Name = "_targetLangComboBox";
            this._targetLangComboBox.Size = new System.Drawing.Size(216, 21);
            this._targetLangComboBox.TabIndex = 17;
            // 
            // _sourceLangComboBox
            // 
            this._sourceLangComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._sourceLangComboBox.FormattingEnabled = true;
            this._sourceLangComboBox.Items.AddRange(new object[] {
            "Any"});
            this._sourceLangComboBox.Location = new System.Drawing.Point(157, 34);
            this._sourceLangComboBox.Name = "_sourceLangComboBox";
            this._sourceLangComboBox.Size = new System.Drawing.Size(216, 21);
            this._sourceLangComboBox.TabIndex = 16;
            // 
            // _label2
            // 
            this._label2.AutoSize = true;
            this._label2.Location = new System.Drawing.Point(6, 75);
            this._label2.Name = "_label2";
            this._label2.Size = new System.Drawing.Size(92, 13);
            this._label2.TabIndex = 15;
            this._label2.Text = "Target Language:";
            // 
            // _label1
            // 
            this._label1.AutoSize = true;
            this._label1.Location = new System.Drawing.Point(6, 34);
            this._label1.Name = "_label1";
            this._label1.Size = new System.Drawing.Size(95, 13);
            this._label1.TabIndex = 14;
            this._label1.Text = "Source Language:";
            // 
            // LanguagePairFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this._languagePairsGroupBox);
            this.Name = "LanguagePairFilterControl";
            this.Size = new System.Drawing.Size(379, 112);
            this.Load += new System.EventHandler(this.LanguagePairFilterControl_Load);
            this._languagePairsGroupBox.ResumeLayout(false);
            this._languagePairsGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox _languagePairsGroupBox;
        private System.Windows.Forms.ComboBox _targetLangComboBox;
        private System.Windows.Forms.ComboBox _sourceLangComboBox;
        private System.Windows.Forms.Label _label2;
        private System.Windows.Forms.Label _label1;
    }
}
