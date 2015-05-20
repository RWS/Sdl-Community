namespace Sdl.Community.Jobs.UI
{
    partial class SearchCriteriaControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.cbFields = new System.Windows.Forms.ComboBox();
            this.cbTargetLanguages = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbSourceLanguages = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkInterpreting = new System.Windows.Forms.CheckBox();
            this.chkPotential = new System.Windows.Forms.CheckBox();
            this.chkTranslation = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lnkNext = new System.Windows.Forms.LinkLabel();
            this.lnkPrevious = new System.Windows.Forms.LinkLabel();
            this.lblCurrentPage = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.cbFields, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.cbTargetLanguages, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.cbSourceLanguages, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtSearch, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnSearch, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 6);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 10);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(426, 446);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 120);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Types:";
            // 
            // cbFields
            // 
            this.cbFields.DisplayMember = "DiscSpecName";
            this.cbFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbFields.FormattingEnabled = true;
            this.cbFields.Location = new System.Drawing.Point(173, 63);
            this.cbFields.Name = "cbFields";
            this.cbFields.Size = new System.Drawing.Size(250, 21);
            this.cbFields.TabIndex = 7;
            this.cbFields.ValueMember = "DiscSpecId";
            // 
            // cbTargetLanguages
            // 
            this.cbTargetLanguages.DisplayMember = "LanguageName";
            this.cbTargetLanguages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbTargetLanguages.FormattingEnabled = true;
            this.cbTargetLanguages.Location = new System.Drawing.Point(173, 33);
            this.cbTargetLanguages.Name = "cbTargetLanguages";
            this.cbTargetLanguages.Size = new System.Drawing.Size(250, 21);
            this.cbTargetLanguages.TabIndex = 6;
            this.cbTargetLanguages.ValueMember = "LanguageCode";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source language:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Target language:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Fields:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Search term (optional):";
            // 
            // cbSourceLanguages
            // 
            this.cbSourceLanguages.DisplayMember = "LanguageName";
            this.cbSourceLanguages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbSourceLanguages.FormattingEnabled = true;
            this.cbSourceLanguages.Location = new System.Drawing.Point(173, 3);
            this.cbSourceLanguages.Name = "cbSourceLanguages";
            this.cbSourceLanguages.Size = new System.Drawing.Size(250, 21);
            this.cbSourceLanguages.TabIndex = 5;
            this.cbSourceLanguages.ValueMember = "LanguageCode";
            // 
            // txtSearch
            // 
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Location = new System.Drawing.Point(173, 93);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(250, 20);
            this.txtSearch.TabIndex = 8;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(348, 203);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "Search jobs";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkInterpreting);
            this.panel1.Controls.Add(this.chkPotential);
            this.panel1.Controls.Add(this.chkTranslation);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(173, 123);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(250, 74);
            this.panel1.TabIndex = 10;
            // 
            // chkInterpreting
            // 
            this.chkInterpreting.AutoSize = true;
            this.chkInterpreting.Checked = true;
            this.chkInterpreting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkInterpreting.Location = new System.Drawing.Point(3, 28);
            this.chkInterpreting.Name = "chkInterpreting";
            this.chkInterpreting.Size = new System.Drawing.Size(79, 17);
            this.chkInterpreting.TabIndex = 2;
            this.chkInterpreting.Text = "Interpreting";
            this.chkInterpreting.UseVisualStyleBackColor = true;
            // 
            // chkPotential
            // 
            this.chkPotential.AutoSize = true;
            this.chkPotential.Checked = true;
            this.chkPotential.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPotential.Location = new System.Drawing.Point(3, 51);
            this.chkPotential.Name = "chkPotential";
            this.chkPotential.Size = new System.Drawing.Size(67, 17);
            this.chkPotential.TabIndex = 1;
            this.chkPotential.Text = "Potential";
            this.chkPotential.UseVisualStyleBackColor = true;
            // 
            // chkTranslation
            // 
            this.chkTranslation.AutoSize = true;
            this.chkTranslation.Checked = true;
            this.chkTranslation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTranslation.Location = new System.Drawing.Point(3, 5);
            this.chkTranslation.Name = "chkTranslation";
            this.chkTranslation.Size = new System.Drawing.Size(78, 17);
            this.chkTranslation.TabIndex = 0;
            this.chkTranslation.Text = "Translation";
            this.chkTranslation.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.lblCurrentPage);
            this.panel2.Controls.Add(this.lnkPrevious);
            this.panel2.Controls.Add(this.lnkNext);
            this.panel2.Location = new System.Drawing.Point(223, 343);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 100);
            this.panel2.TabIndex = 11;
            // 
            // lnkNext
            // 
            this.lnkNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkNext.AutoSize = true;
            this.lnkNext.Location = new System.Drawing.Point(168, 77);
            this.lnkNext.Name = "lnkNext";
            this.lnkNext.Size = new System.Drawing.Size(29, 13);
            this.lnkNext.TabIndex = 0;
            this.lnkNext.TabStop = true;
            this.lnkNext.Text = "Next";
            this.lnkNext.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkNext_LinkClicked);
            // 
            // lnkPrevious
            // 
            this.lnkPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkPrevious.AutoSize = true;
            this.lnkPrevious.Enabled = false;
            this.lnkPrevious.Location = new System.Drawing.Point(95, 77);
            this.lnkPrevious.Name = "lnkPrevious";
            this.lnkPrevious.Size = new System.Drawing.Size(48, 13);
            this.lnkPrevious.TabIndex = 1;
            this.lnkPrevious.TabStop = true;
            this.lnkPrevious.Text = "Previous";
            this.lnkPrevious.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPrevious_LinkClicked);
            // 
            // lblCurrentPage
            // 
            this.lblCurrentPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentPage.AutoSize = true;
            this.lblCurrentPage.Location = new System.Drawing.Point(149, 77);
            this.lblCurrentPage.Name = "lblCurrentPage";
            this.lblCurrentPage.Size = new System.Drawing.Size(13, 13);
            this.lblCurrentPage.TabIndex = 2;
            this.lblCurrentPage.Text = "1";
            // 
            // SearchCriteriaControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SearchCriteriaControl";
            this.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.Size = new System.Drawing.Size(426, 456);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox cbFields;
        private System.Windows.Forms.ComboBox cbTargetLanguages;
        private System.Windows.Forms.ComboBox cbSourceLanguages;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chkInterpreting;
        private System.Windows.Forms.CheckBox chkPotential;
        private System.Windows.Forms.CheckBox chkTranslation;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblCurrentPage;
        private System.Windows.Forms.LinkLabel lnkPrevious;
        private System.Windows.Forms.LinkLabel lnkNext;
    }
}
