namespace Sdl.Community.TermInjector
{
    partial class TermInjectorTranslationProviderConfDialog
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
            this.glosFile = new System.Windows.Forms.TextBox();
            this.btn_Browse = new System.Windows.Forms.Button();
            this.dlg_OpenFile = new System.Windows.Forms.OpenFileDialog();
            this.bnt_OK = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.EditExact = new System.Windows.Forms.Button();
            this.btn_Create = new System.Windows.Forms.Button();
            this.matchCase = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tmFile = new System.Windows.Forms.TextBox();
            this.btn_browseTM = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tokenBoundaryCharacters = new System.Windows.Forms.TextBox();
            this.termAdditionSeparator = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.useBoundaryLabel = new System.Windows.Forms.Label();
            this.useBoundaryChars = new System.Windows.Forms.CheckBox();
            this.addnewtermscheckbox = new System.Windows.Forms.CheckBox();
            this.NewSegmentPercentageBox = new System.Windows.Forms.MaskedTextBox();
            this.newmatchpercentageLabel = new System.Windows.Forms.Label();
            this.InjectIntoFullMatchesCheckBox = new System.Windows.Forms.CheckBox();
            this.delimiterCharacter = new System.Windows.Forms.ComboBox();
            this.btn_create_regex = new System.Windows.Forms.Button();
            this.btn_browse_regex = new System.Windows.Forms.Button();
            this.regexFile = new System.Windows.Forms.TextBox();
            this.dlg_CreateFile = new System.Windows.Forms.SaveFileDialog();
            this.btn_Help = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_reload = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.EditRegex = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // glosFile
            // 
            this.glosFile.Location = new System.Drawing.Point(6, 28);
            this.glosFile.Name = "glosFile";
            this.glosFile.Size = new System.Drawing.Size(282, 20);
            this.glosFile.TabIndex = 0;
            // 
            // btn_Browse
            // 
            this.btn_Browse.Location = new System.Drawing.Point(344, 25);
            this.btn_Browse.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_Browse.Name = "btn_Browse";
            this.btn_Browse.Size = new System.Drawing.Size(52, 23);
            this.btn_Browse.TabIndex = 1;
            this.btn_Browse.Text = "&Browse";
            this.btn_Browse.UseVisualStyleBackColor = true;
            this.btn_Browse.Click += new System.EventHandler(this.btn_Browse_Click);
            // 
            // bnt_OK
            // 
            this.bnt_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bnt_OK.Location = new System.Drawing.Point(12, 462);
            this.bnt_OK.Name = "bnt_OK";
            this.bnt_OK.Size = new System.Drawing.Size(75, 23);
            this.bnt_OK.TabIndex = 3;
            this.bnt_OK.Text = "&OK";
            this.bnt_OK.UseVisualStyleBackColor = true;
            this.bnt_OK.Click += new System.EventHandler(this.bnt_OK_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Location = new System.Drawing.Point(93, 462);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 4;
            this.btn_Cancel.Text = "&Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.EditExact);
            this.groupBox1.Controls.Add(this.btn_Create);
            this.groupBox1.Controls.Add(this.glosFile);
            this.groupBox1.Controls.Add(this.btn_Browse);
            this.groupBox1.Controls.Add(this.matchCase);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(454, 76);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Exact match rule file";
            // 
            // EditExact
            // 
            this.EditExact.Location = new System.Drawing.Point(295, 25);
            this.EditExact.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.EditExact.Name = "EditExact";
            this.EditExact.Size = new System.Drawing.Size(44, 23);
            this.EditExact.TabIndex = 10;
            this.EditExact.Text = "E&dit";
            this.EditExact.UseVisualStyleBackColor = true;
            this.EditExact.Click += new System.EventHandler(this.EditExact_Click);
            // 
            // btn_Create
            // 
            this.btn_Create.Location = new System.Drawing.Point(399, 25);
            this.btn_Create.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_Create.Name = "btn_Create";
            this.btn_Create.Size = new System.Drawing.Size(52, 23);
            this.btn_Create.TabIndex = 2;
            this.btn_Create.Text = "&Create";
            this.btn_Create.UseVisualStyleBackColor = true;
            this.btn_Create.Click += new System.EventHandler(this.btn_Create_Click);
            // 
            // matchCase
            // 
            this.matchCase.AutoSize = true;
            this.matchCase.Location = new System.Drawing.Point(6, 51);
            this.matchCase.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.matchCase.Name = "matchCase";
            this.matchCase.Size = new System.Drawing.Size(82, 17);
            this.matchCase.TabIndex = 9;
            this.matchCase.Text = "&Match case";
            this.matchCase.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(300, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "&Delimiter character";
            this.label1.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tmFile);
            this.groupBox2.Controls.Add(this.btn_browseTM);
            this.groupBox2.Location = new System.Drawing.Point(12, 169);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(454, 69);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Translation memory";
            // 
            // tmFile
            // 
            this.tmFile.Location = new System.Drawing.Point(6, 28);
            this.tmFile.Name = "tmFile";
            this.tmFile.Size = new System.Drawing.Size(385, 20);
            this.tmFile.TabIndex = 0;
            // 
            // btn_browseTM
            // 
            this.btn_browseTM.Location = new System.Drawing.Point(395, 25);
            this.btn_browseTM.Name = "btn_browseTM";
            this.btn_browseTM.Size = new System.Drawing.Size(52, 23);
            this.btn_browseTM.TabIndex = 1;
            this.btn_browseTM.Text = "B&rowse";
            this.btn_browseTM.UseVisualStyleBackColor = true;
            this.btn_browseTM.Click += new System.EventHandler(this.btn_browseTM_Click_1);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(9, 7);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(114, 17);
            this.checkBox2.TabIndex = 9;
            this.checkBox2.Text = "&Advanced settings";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(243, 34);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "&Term addition separator";
            this.label3.Visible = false;
            // 
            // tokenBoundaryCharacters
            // 
            this.tokenBoundaryCharacters.Location = new System.Drawing.Point(347, 110);
            this.tokenBoundaryCharacters.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tokenBoundaryCharacters.Name = "tokenBoundaryCharacters";
            this.tokenBoundaryCharacters.Size = new System.Drawing.Size(103, 20);
            this.tokenBoundaryCharacters.TabIndex = 12;
            this.tokenBoundaryCharacters.Visible = false;
            // 
            // termAdditionSeparator
            // 
            this.termAdditionSeparator.Location = new System.Drawing.Point(385, 32);
            this.termAdditionSeparator.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.termAdditionSeparator.Name = "termAdditionSeparator";
            this.termAdditionSeparator.Size = new System.Drawing.Size(63, 20);
            this.termAdditionSeparator.TabIndex = 13;
            this.termAdditionSeparator.Visible = false;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.useBoundaryLabel);
            this.panel1.Controls.Add(this.useBoundaryChars);
            this.panel1.Controls.Add(this.addnewtermscheckbox);
            this.panel1.Controls.Add(this.NewSegmentPercentageBox);
            this.panel1.Controls.Add(this.newmatchpercentageLabel);
            this.panel1.Controls.Add(this.InjectIntoFullMatchesCheckBox);
            this.panel1.Controls.Add(this.delimiterCharacter);
            this.panel1.Controls.Add(this.termAdditionSeparator);
            this.panel1.Controls.Add(this.tokenBoundaryCharacters);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.checkBox2);
            this.panel1.Location = new System.Drawing.Point(12, 253);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(455, 154);
            this.panel1.TabIndex = 14;
            // 
            // useBoundaryLabel
            // 
            this.useBoundaryLabel.AutoSize = true;
            this.useBoundaryLabel.Location = new System.Drawing.Point(7, 132);
            this.useBoundaryLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.useBoundaryLabel.Name = "useBoundaryLabel";
            this.useBoundaryLabel.Size = new System.Drawing.Size(430, 13);
            this.useBoundaryLabel.TabIndex = 24;
            this.useBoundaryLabel.Text = "(If the box is unchecked, rules are applied to any matching string, including par" +
    "ts of words)";
            this.useBoundaryLabel.Visible = false;
            // 
            // useBoundaryChars
            // 
            this.useBoundaryChars.AutoSize = true;
            this.useBoundaryChars.Checked = true;
            this.useBoundaryChars.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useBoundaryChars.Location = new System.Drawing.Point(9, 110);
            this.useBoundaryChars.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.useBoundaryChars.Name = "useBoundaryChars";
            this.useBoundaryChars.Size = new System.Drawing.Size(337, 17);
            this.useBoundaryChars.TabIndex = 23;
            this.useBoundaryChars.Text = "Use following word boundary characters to split the text into words";
            this.useBoundaryChars.UseVisualStyleBackColor = true;
            this.useBoundaryChars.Visible = false;
            // 
            // addnewtermscheckbox
            // 
            this.addnewtermscheckbox.AutoSize = true;
            this.addnewtermscheckbox.Location = new System.Drawing.Point(9, 55);
            this.addnewtermscheckbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.addnewtermscheckbox.Name = "addnewtermscheckbox";
            this.addnewtermscheckbox.Size = new System.Drawing.Size(220, 17);
            this.addnewtermscheckbox.TabIndex = 22;
            this.addnewtermscheckbox.Text = "Add new terms to the start of &fuzzy match";
            this.addnewtermscheckbox.UseVisualStyleBackColor = true;
            this.addnewtermscheckbox.Visible = false;
            // 
            // NewSegmentPercentageBox
            // 
            this.NewSegmentPercentageBox.Location = new System.Drawing.Point(206, 82);
            this.NewSegmentPercentageBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.NewSegmentPercentageBox.Mask = "990";
            this.NewSegmentPercentageBox.Name = "NewSegmentPercentageBox";
            this.NewSegmentPercentageBox.Size = new System.Drawing.Size(21, 20);
            this.NewSegmentPercentageBox.TabIndex = 21;
            this.NewSegmentPercentageBox.Text = "0";
            this.NewSegmentPercentageBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NewSegmentPercentageBox.Visible = false;
            // 
            // newmatchpercentageLabel
            // 
            this.newmatchpercentageLabel.AutoSize = true;
            this.newmatchpercentageLabel.Location = new System.Drawing.Point(7, 84);
            this.newmatchpercentageLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.newmatchpercentageLabel.Name = "newmatchpercentageLabel";
            this.newmatchpercentageLabel.Size = new System.Drawing.Size(196, 13);
            this.newmatchpercentageLabel.TabIndex = 20;
            this.newmatchpercentageLabel.Text = "Constructed segment match &percentage";
            this.newmatchpercentageLabel.Visible = false;
            // 
            // InjectIntoFullMatchesCheckBox
            // 
            this.InjectIntoFullMatchesCheckBox.AutoSize = true;
            this.InjectIntoFullMatchesCheckBox.Location = new System.Drawing.Point(9, 33);
            this.InjectIntoFullMatchesCheckBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.InjectIntoFullMatchesCheckBox.Name = "InjectIntoFullMatchesCheckBox";
            this.InjectIntoFullMatchesCheckBox.Size = new System.Drawing.Size(203, 17);
            this.InjectIntoFullMatchesCheckBox.TabIndex = 19;
            this.InjectIntoFullMatchesCheckBox.Text = "I&nject terms into 100 percent matches";
            this.InjectIntoFullMatchesCheckBox.UseVisualStyleBackColor = true;
            this.InjectIntoFullMatchesCheckBox.Visible = false;
            // 
            // delimiterCharacter
            // 
            this.delimiterCharacter.DisplayMember = ",:=";
            this.delimiterCharacter.FormattingEnabled = true;
            this.delimiterCharacter.Items.AddRange(new object[] {
            ";",
            "Tab",
            ":",
            ",",
            "="});
            this.delimiterCharacter.Location = new System.Drawing.Point(400, 82);
            this.delimiterCharacter.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.delimiterCharacter.Name = "delimiterCharacter";
            this.delimiterCharacter.Size = new System.Drawing.Size(50, 21);
            this.delimiterCharacter.TabIndex = 14;
            this.delimiterCharacter.Visible = false;
            // 
            // btn_create_regex
            // 
            this.btn_create_regex.Location = new System.Drawing.Point(398, 25);
            this.btn_create_regex.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_create_regex.Name = "btn_create_regex";
            this.btn_create_regex.Size = new System.Drawing.Size(51, 23);
            this.btn_create_regex.TabIndex = 18;
            this.btn_create_regex.Text = "Cr&eate";
            this.btn_create_regex.UseVisualStyleBackColor = true;
            this.btn_create_regex.Click += new System.EventHandler(this.btn_create_regex_Click);
            // 
            // btn_browse_regex
            // 
            this.btn_browse_regex.Location = new System.Drawing.Point(343, 25);
            this.btn_browse_regex.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_browse_regex.Name = "btn_browse_regex";
            this.btn_browse_regex.Size = new System.Drawing.Size(52, 23);
            this.btn_browse_regex.TabIndex = 17;
            this.btn_browse_regex.Text = "Br&owse";
            this.btn_browse_regex.UseVisualStyleBackColor = true;
            this.btn_browse_regex.Click += new System.EventHandler(this.btn_browse_regex_Click);
            // 
            // regexFile
            // 
            this.regexFile.Location = new System.Drawing.Point(5, 27);
            this.regexFile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.regexFile.Name = "regexFile";
            this.regexFile.Size = new System.Drawing.Size(282, 20);
            this.regexFile.TabIndex = 16;
            // 
            // dlg_CreateFile
            // 
            this.dlg_CreateFile.DefaultExt = "txt";
            // 
            // btn_Help
            // 
            this.btn_Help.Location = new System.Drawing.Point(391, 462);
            this.btn_Help.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_Help.Name = "btn_Help";
            this.btn_Help.Size = new System.Drawing.Size(75, 23);
            this.btn_Help.TabIndex = 15;
            this.btn_Help.Text = "&Help";
            this.btn_Help.UseVisualStyleBackColor = true;
            this.btn_Help.Click += new System.EventHandler(this.btn_Help_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 434);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "TermInjector v2.3";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(296, 434);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 13);
            this.label5.TabIndex = 17;
            // 
            // btn_reload
            // 
            this.btn_reload.Location = new System.Drawing.Point(286, 463);
            this.btn_reload.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btn_reload.Name = "btn_reload";
            this.btn_reload.Size = new System.Drawing.Size(100, 22);
            this.btn_reload.TabIndex = 18;
            this.btn_reload.Text = "Reload files";
            this.btn_reload.UseVisualStyleBackColor = true;
            this.btn_reload.Click += new System.EventHandler(this.btn_reload_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.EditRegex);
            this.groupBox3.Controls.Add(this.btn_create_regex);
            this.groupBox3.Controls.Add(this.regexFile);
            this.groupBox3.Controls.Add(this.btn_browse_regex);
            this.groupBox3.Location = new System.Drawing.Point(13, 94);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(454, 68);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Regular expression rule file";
            // 
            // EditRegex
            // 
            this.EditRegex.Location = new System.Drawing.Point(293, 25);
            this.EditRegex.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.EditRegex.Name = "EditRegex";
            this.EditRegex.Size = new System.Drawing.Size(43, 23);
            this.EditRegex.TabIndex = 19;
            this.EditRegex.Text = "Ed&it";
            this.EditRegex.UseVisualStyleBackColor = true;
            this.EditRegex.Click += new System.EventHandler(this.EditRegex_Click);
            // 
            // TermInjectorTranslationProviderConfDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(475, 496);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btn_reload);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btn_Help);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.bnt_OK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "TermInjectorTranslationProviderConfDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TermInjector Options";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Browse;
        private System.Windows.Forms.OpenFileDialog dlg_OpenFile;
        private System.Windows.Forms.Button bnt_OK;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox glosFile;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tmFile;
        private System.Windows.Forms.Button btn_browseTM;
        private System.Windows.Forms.CheckBox matchCase;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tokenBoundaryCharacters;
        private System.Windows.Forms.TextBox termAdditionSeparator;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_Create;
        private System.Windows.Forms.SaveFileDialog dlg_CreateFile;
        private System.Windows.Forms.Button btn_Help;
        private System.Windows.Forms.ComboBox delimiterCharacter;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn_create_regex;
        private System.Windows.Forms.Button btn_browse_regex;
        private System.Windows.Forms.TextBox regexFile;
        private System.Windows.Forms.Button btn_reload;
        private System.Windows.Forms.CheckBox InjectIntoFullMatchesCheckBox;
        private System.Windows.Forms.MaskedTextBox NewSegmentPercentageBox;
        private System.Windows.Forms.Label newmatchpercentageLabel;
        private System.Windows.Forms.CheckBox addnewtermscheckbox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button EditExact;
        private System.Windows.Forms.Button EditRegex;
        private System.Windows.Forms.Label useBoundaryLabel;
        private System.Windows.Forms.CheckBox useBoundaryChars;
    }
}