namespace Sdl.Community.XliffCompare
{
    partial class FormSettingsFormatting
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
            this.groupBox_colors = new System.Windows.Forms.GroupBox();
            this.button_backroundColorEdit = new System.Windows.Forms.Button();
            this.button_fontColorEdit = new System.Windows.Forms.Button();
            this.label_backroundColorDisplay = new System.Windows.Forms.Label();
            this.label_fontColorDisplay = new System.Windows.Forms.Label();
            this.checkBox_backroundColor = new System.Windows.Forms.CheckBox();
            this.checkBox_fontColor = new System.Windows.Forms.CheckBox();
            this.groupBox_styles = new System.Windows.Forms.GroupBox();
            this.comboBox_stylePosition = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox_styleUnderline = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox_styleStrikethrough = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_styleItalic = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_styleBold = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox_sample = new System.Windows.Forms.GroupBox();
            this.richTextBox_sample = new System.Windows.Forms.RichTextBox();
            this.button_cancel = new System.Windows.Forms.Button();
            this.button_ok = new System.Windows.Forms.Button();
            this.groupBox_colors.SuspendLayout();
            this.groupBox_styles.SuspendLayout();
            this.groupBox_sample.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_colors
            // 
            this.groupBox_colors.Controls.Add(this.button_backroundColorEdit);
            this.groupBox_colors.Controls.Add(this.button_fontColorEdit);
            this.groupBox_colors.Controls.Add(this.label_backroundColorDisplay);
            this.groupBox_colors.Controls.Add(this.label_fontColorDisplay);
            this.groupBox_colors.Controls.Add(this.checkBox_backroundColor);
            this.groupBox_colors.Controls.Add(this.checkBox_fontColor);
            this.groupBox_colors.Location = new System.Drawing.Point(12, 22);
            this.groupBox_colors.Name = "groupBox_colors";
            this.groupBox_colors.Size = new System.Drawing.Size(244, 191);
            this.groupBox_colors.TabIndex = 0;
            this.groupBox_colors.TabStop = false;
            this.groupBox_colors.Text = "Color";
            // 
            // button_backroundColorEdit
            // 
            this.button_backroundColorEdit.Location = new System.Drawing.Point(102, 138);
            this.button_backroundColorEdit.Name = "button_backroundColorEdit";
            this.button_backroundColorEdit.Size = new System.Drawing.Size(75, 23);
            this.button_backroundColorEdit.TabIndex = 2;
            this.button_backroundColorEdit.Text = "Edit...";
            this.button_backroundColorEdit.UseVisualStyleBackColor = true;
            this.button_backroundColorEdit.Click += new System.EventHandler(this.button_backroundColorEdit_Click);
            // 
            // button_fontColorEdit
            // 
            this.button_fontColorEdit.Location = new System.Drawing.Point(102, 61);
            this.button_fontColorEdit.Name = "button_fontColorEdit";
            this.button_fontColorEdit.Size = new System.Drawing.Size(75, 23);
            this.button_fontColorEdit.TabIndex = 2;
            this.button_fontColorEdit.Text = "Edit...";
            this.button_fontColorEdit.UseVisualStyleBackColor = true;
            this.button_fontColorEdit.Click += new System.EventHandler(this.button_fontColorEdit_Click);
            // 
            // label_backroundColorDisplay
            // 
            this.label_backroundColorDisplay.BackColor = System.Drawing.Color.White;
            this.label_backroundColorDisplay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label_backroundColorDisplay.Location = new System.Drawing.Point(44, 138);
            this.label_backroundColorDisplay.Name = "label_backroundColorDisplay";
            this.label_backroundColorDisplay.Size = new System.Drawing.Size(39, 23);
            this.label_backroundColorDisplay.TabIndex = 1;
            // 
            // label_fontColorDisplay
            // 
            this.label_fontColorDisplay.BackColor = System.Drawing.Color.Black;
            this.label_fontColorDisplay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label_fontColorDisplay.Location = new System.Drawing.Point(44, 61);
            this.label_fontColorDisplay.Name = "label_fontColorDisplay";
            this.label_fontColorDisplay.Size = new System.Drawing.Size(39, 23);
            this.label_fontColorDisplay.TabIndex = 1;
            // 
            // checkBox_backroundColor
            // 
            this.checkBox_backroundColor.AutoSize = true;
            this.checkBox_backroundColor.Location = new System.Drawing.Point(24, 109);
            this.checkBox_backroundColor.Name = "checkBox_backroundColor";
            this.checkBox_backroundColor.Size = new System.Drawing.Size(141, 17);
            this.checkBox_backroundColor.TabIndex = 0;
            this.checkBox_backroundColor.Text = "Specify backround color";
            this.checkBox_backroundColor.UseVisualStyleBackColor = true;
            this.checkBox_backroundColor.CheckedChanged += new System.EventHandler(this.checkBox_backroundColor_CheckedChanged);
            // 
            // checkBox_fontColor
            // 
            this.checkBox_fontColor.AutoSize = true;
            this.checkBox_fontColor.Location = new System.Drawing.Point(24, 32);
            this.checkBox_fontColor.Name = "checkBox_fontColor";
            this.checkBox_fontColor.Size = new System.Drawing.Size(108, 17);
            this.checkBox_fontColor.TabIndex = 0;
            this.checkBox_fontColor.Text = "Specify font color";
            this.checkBox_fontColor.UseVisualStyleBackColor = true;
            this.checkBox_fontColor.CheckedChanged += new System.EventHandler(this.checkBox_fontColor_CheckedChanged);
            // 
            // groupBox_styles
            // 
            this.groupBox_styles.Controls.Add(this.comboBox_stylePosition);
            this.groupBox_styles.Controls.Add(this.label5);
            this.groupBox_styles.Controls.Add(this.comboBox_styleUnderline);
            this.groupBox_styles.Controls.Add(this.label4);
            this.groupBox_styles.Controls.Add(this.comboBox_styleStrikethrough);
            this.groupBox_styles.Controls.Add(this.label3);
            this.groupBox_styles.Controls.Add(this.comboBox_styleItalic);
            this.groupBox_styles.Controls.Add(this.label2);
            this.groupBox_styles.Controls.Add(this.comboBox_styleBold);
            this.groupBox_styles.Controls.Add(this.label1);
            this.groupBox_styles.Location = new System.Drawing.Point(271, 22);
            this.groupBox_styles.Name = "groupBox_styles";
            this.groupBox_styles.Size = new System.Drawing.Size(282, 191);
            this.groupBox_styles.TabIndex = 1;
            this.groupBox_styles.TabStop = false;
            this.groupBox_styles.Text = "Style";
            // 
            // comboBox_stylePosition
            // 
            this.comboBox_stylePosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_stylePosition.FormattingEnabled = true;
            this.comboBox_stylePosition.Items.AddRange(new object[] {
            "Normal",
            "Superscript",
            "Subscript"});
            this.comboBox_stylePosition.Location = new System.Drawing.Point(99, 139);
            this.comboBox_stylePosition.Name = "comboBox_stylePosition";
            this.comboBox_stylePosition.Size = new System.Drawing.Size(166, 21);
            this.comboBox_stylePosition.TabIndex = 1;
            this.comboBox_stylePosition.SelectedIndexChanged += new System.EventHandler(this.comboBox_stylePosition_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(37, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Position:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBox_styleUnderline
            // 
            this.comboBox_styleUnderline.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_styleUnderline.FormattingEnabled = true;
            this.comboBox_styleUnderline.Items.AddRange(new object[] {
            "Activate",
            "Deactivate"});
            this.comboBox_styleUnderline.Location = new System.Drawing.Point(99, 112);
            this.comboBox_styleUnderline.Name = "comboBox_styleUnderline";
            this.comboBox_styleUnderline.Size = new System.Drawing.Size(166, 21);
            this.comboBox_styleUnderline.TabIndex = 1;
            this.comboBox_styleUnderline.SelectedIndexChanged += new System.EventHandler(this.comboBox_styleUnderline_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(29, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Underline:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBox_styleStrikethrough
            // 
            this.comboBox_styleStrikethrough.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_styleStrikethrough.FormattingEnabled = true;
            this.comboBox_styleStrikethrough.Items.AddRange(new object[] {
            "Activate",
            "Deactivate"});
            this.comboBox_styleStrikethrough.Location = new System.Drawing.Point(99, 85);
            this.comboBox_styleStrikethrough.Name = "comboBox_styleStrikethrough";
            this.comboBox_styleStrikethrough.Size = new System.Drawing.Size(166, 21);
            this.comboBox_styleStrikethrough.TabIndex = 1;
            this.comboBox_styleStrikethrough.SelectedIndexChanged += new System.EventHandler(this.comboBox_styleStrikethrough_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Strikethrough:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBox_styleItalic
            // 
            this.comboBox_styleItalic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_styleItalic.FormattingEnabled = true;
            this.comboBox_styleItalic.Items.AddRange(new object[] {
            "Activate",
            "Deactivate"});
            this.comboBox_styleItalic.Location = new System.Drawing.Point(99, 58);
            this.comboBox_styleItalic.Name = "comboBox_styleItalic";
            this.comboBox_styleItalic.Size = new System.Drawing.Size(166, 21);
            this.comboBox_styleItalic.TabIndex = 1;
            this.comboBox_styleItalic.SelectedIndexChanged += new System.EventHandler(this.comboBox_styleItalic_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Italic:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBox_styleBold
            // 
            this.comboBox_styleBold.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_styleBold.FormattingEnabled = true;
            this.comboBox_styleBold.Items.AddRange(new object[] {
            "Activate",
            "Deactivate"});
            this.comboBox_styleBold.Location = new System.Drawing.Point(99, 31);
            this.comboBox_styleBold.Name = "comboBox_styleBold";
            this.comboBox_styleBold.Size = new System.Drawing.Size(166, 21);
            this.comboBox_styleBold.TabIndex = 1;
            this.comboBox_styleBold.SelectedIndexChanged += new System.EventHandler(this.comboBox_styleBold_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Bold:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox_sample
            // 
            this.groupBox_sample.Controls.Add(this.richTextBox_sample);
            this.groupBox_sample.Location = new System.Drawing.Point(12, 219);
            this.groupBox_sample.Name = "groupBox_sample";
            this.groupBox_sample.Size = new System.Drawing.Size(541, 86);
            this.groupBox_sample.TabIndex = 2;
            this.groupBox_sample.TabStop = false;
            this.groupBox_sample.Text = "Sample";
            // 
            // richTextBox_sample
            // 
            this.richTextBox_sample.BackColor = System.Drawing.Color.White;
            this.richTextBox_sample.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_sample.Location = new System.Drawing.Point(19, 19);
            this.richTextBox_sample.Name = "richTextBox_sample";
            this.richTextBox_sample.ReadOnly = true;
            this.richTextBox_sample.Size = new System.Drawing.Size(497, 52);
            this.richTextBox_sample.TabIndex = 0;
            this.richTextBox_sample.Text = "";
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(479, 321);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_cancel.TabIndex = 2;
            this.button_cancel.Text = "&Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // button_ok
            // 
            this.button_ok.Location = new System.Drawing.Point(398, 321);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(75, 23);
            this.button_ok.TabIndex = 2;
            this.button_ok.Text = "&OK";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // FormSettingsFormatting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 352);
            this.Controls.Add(this.groupBox_sample);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.groupBox_styles);
            this.Controls.Add(this.groupBox_colors);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSettingsFormatting";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Font Formatting";
            this.Load += new System.EventHandler(this.FormSettingsFormatting_Load);
            this.groupBox_colors.ResumeLayout(false);
            this.groupBox_colors.PerformLayout();
            this.groupBox_styles.ResumeLayout(false);
            this.groupBox_styles.PerformLayout();
            this.groupBox_sample.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_colors;
        private System.Windows.Forms.Button button_fontColorEdit;
        private System.Windows.Forms.Button button_backroundColorEdit;
        private System.Windows.Forms.GroupBox groupBox_styles;
        private System.Windows.Forms.GroupBox groupBox_sample;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        internal System.Windows.Forms.ComboBox comboBox_styleBold;
        internal System.Windows.Forms.ComboBox comboBox_styleUnderline;
        internal System.Windows.Forms.ComboBox comboBox_styleStrikethrough;
        internal System.Windows.Forms.ComboBox comboBox_styleItalic;
        internal System.Windows.Forms.RichTextBox richTextBox_sample;
        internal System.Windows.Forms.CheckBox checkBox_fontColor;
        internal System.Windows.Forms.Label label_fontColorDisplay;
        internal System.Windows.Forms.Label label_backroundColorDisplay;
        internal System.Windows.Forms.CheckBox checkBox_backroundColor;
        internal System.Windows.Forms.ComboBox comboBox_stylePosition;
        private System.Windows.Forms.Label label5;
    }
}