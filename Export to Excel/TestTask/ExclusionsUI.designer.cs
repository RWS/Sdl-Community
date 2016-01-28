namespace ExportToExcel
{
    partial class ExclusionsUI
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
            this.cb_DontExportNoMatch = new System.Windows.Forms.CheckBox();
            this.cb_DontExportFuzzy = new System.Windows.Forms.CheckBox();
            this.cb_DontExportExact = new System.Windows.Forms.CheckBox();
            this.cb_DontExportContext = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbl_ExcludedStatuses = new System.Windows.Forms.CheckedListBox();
            this.rb_ExcludeStatus = new System.Windows.Forms.RadioButton();
            this.rb_ExcludeCategory = new System.Windows.Forms.RadioButton();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cb_DontExportNoMatch
            // 
            this.cb_DontExportNoMatch.AutoSize = true;
            this.cb_DontExportNoMatch.Location = new System.Drawing.Point(27, 109);
            this.cb_DontExportNoMatch.Name = "cb_DontExportNoMatch";
            this.cb_DontExportNoMatch.Size = new System.Drawing.Size(83, 17);
            this.cb_DontExportNoMatch.TabIndex = 5;
            this.cb_DontExportNoMatch.Text = "&No matches";
            this.cb_DontExportNoMatch.UseVisualStyleBackColor = true;
            this.cb_DontExportNoMatch.CheckedChanged += new System.EventHandler(this.cb_DontExportNoMatch_CheckedChanged);
            // 
            // cb_DontExportFuzzy
            // 
            this.cb_DontExportFuzzy.AutoSize = true;
            this.cb_DontExportFuzzy.Location = new System.Drawing.Point(27, 87);
            this.cb_DontExportFuzzy.Name = "cb_DontExportFuzzy";
            this.cb_DontExportFuzzy.Size = new System.Drawing.Size(96, 17);
            this.cb_DontExportFuzzy.TabIndex = 4;
            this.cb_DontExportFuzzy.Text = "&Fuzzy matches";
            this.cb_DontExportFuzzy.UseVisualStyleBackColor = true;
            this.cb_DontExportFuzzy.CheckedChanged += new System.EventHandler(this.cb_DontExportFuzzy_CheckedChanged);
            // 
            // cb_DontExportExact
            // 
            this.cb_DontExportExact.AutoSize = true;
            this.cb_DontExportExact.Location = new System.Drawing.Point(27, 66);
            this.cb_DontExportExact.Name = "cb_DontExportExact";
            this.cb_DontExportExact.Size = new System.Drawing.Size(96, 17);
            this.cb_DontExportExact.TabIndex = 3;
            this.cb_DontExportExact.Text = "Ex&act matches";
            this.cb_DontExportExact.UseVisualStyleBackColor = true;
            this.cb_DontExportExact.CheckedChanged += new System.EventHandler(this.cb_DontExportExact_CheckedChanged);
            // 
            // cb_DontExportContext
            // 
            this.cb_DontExportContext.AutoSize = true;
            this.cb_DontExportContext.Location = new System.Drawing.Point(27, 43);
            this.cb_DontExportContext.Name = "cb_DontExportContext";
            this.cb_DontExportContext.Size = new System.Drawing.Size(144, 17);
            this.cb_DontExportContext.TabIndex = 2;
            this.cb_DontExportContext.Text = "&Context/Perfect matches";
            this.cb_DontExportContext.UseVisualStyleBackColor = true;
            this.cb_DontExportContext.CheckedChanged += new System.EventHandler(this.cb_DontExportContext_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.cbl_ExcludedStatuses);
            this.groupBox2.Controls.Add(this.rb_ExcludeStatus);
            this.groupBox2.Controls.Add(this.rb_ExcludeCategory);
            this.groupBox2.Controls.Add(this.cb_DontExportNoMatch);
            this.groupBox2.Controls.Add(this.cb_DontExportContext);
            this.groupBox2.Controls.Add(this.cb_DontExportExact);
            this.groupBox2.Controls.Add(this.cb_DontExportFuzzy);
            this.groupBox2.Location = new System.Drawing.Point(3, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(417, 286);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Segment exclusion";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // cbl_ExcludedStatuses
            // 
            this.cbl_ExcludedStatuses.CheckOnClick = true;
            this.cbl_ExcludedStatuses.Enabled = false;
            this.cbl_ExcludedStatuses.FormattingEnabled = true;
            this.cbl_ExcludedStatuses.Location = new System.Drawing.Point(26, 155);
            this.cbl_ExcludedStatuses.Name = "cbl_ExcludedStatuses";
            this.cbl_ExcludedStatuses.Size = new System.Drawing.Size(251, 124);
            this.cbl_ExcludedStatuses.TabIndex = 7;
            // 
            // rb_ExcludeStatus
            // 
            this.rb_ExcludeStatus.AutoSize = true;
            this.rb_ExcludeStatus.Location = new System.Drawing.Point(7, 132);
            this.rb_ExcludeStatus.Name = "rb_ExcludeStatus";
            this.rb_ExcludeStatus.Size = new System.Drawing.Size(151, 17);
            this.rb_ExcludeStatus.TabIndex = 6;
            this.rb_ExcludeStatus.TabStop = true;
            this.rb_ExcludeStatus.Text = "E&xclude segment statuses:";
            this.rb_ExcludeStatus.UseVisualStyleBackColor = true;
            this.rb_ExcludeStatus.CheckedChanged += new System.EventHandler(this.rb_ExcludeStatus_CheckedChanged);
            // 
            // rb_ExcludeCategory
            // 
            this.rb_ExcludeCategory.AutoSize = true;
            this.rb_ExcludeCategory.Checked = true;
            this.rb_ExcludeCategory.Location = new System.Drawing.Point(7, 20);
            this.rb_ExcludeCategory.Name = "rb_ExcludeCategory";
            this.rb_ExcludeCategory.Size = new System.Drawing.Size(161, 17);
            this.rb_ExcludeCategory.TabIndex = 0;
            this.rb_ExcludeCategory.TabStop = true;
            this.rb_ExcludeCategory.Text = "&Exclude segment categories:";
            this.rb_ExcludeCategory.UseVisualStyleBackColor = true;
            // 
            // ExclusionsUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.groupBox2);
            this.Name = "ExclusionsUI";
            this.Size = new System.Drawing.Size(423, 291);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox cb_DontExportNoMatch;
        private System.Windows.Forms.CheckBox cb_DontExportFuzzy;
        private System.Windows.Forms.CheckBox cb_DontExportExact;
        private System.Windows.Forms.CheckBox cb_DontExportContext;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckedListBox cbl_ExcludedStatuses;
        private System.Windows.Forms.RadioButton rb_ExcludeStatus;
        private System.Windows.Forms.RadioButton rb_ExcludeCategory;
    }
}
