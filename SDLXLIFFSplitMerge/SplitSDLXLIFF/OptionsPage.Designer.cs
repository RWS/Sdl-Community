using Sdl.Utilities.SplitSDLXLIFF.Wizard;
namespace Sdl.Utilities.SplitSDLXLIFF
{
    partial class OptionsPage
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
            this.gbSplitOptions = new System.Windows.Forms.GroupBox();
            this.clbSegments = new System.Windows.Forms.CheckedListBox();
            this.chSplitCheckPercent = new System.Windows.Forms.CheckBox();
            this.lblSplitWordCount2 = new System.Windows.Forms.Label();
            this.lblSplitWordCount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudWords = new System.Windows.Forms.NumericUpDown();
            this.nudPercent = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblSplitSegNumbers2 = new System.Windows.Forms.Label();
            this.tbSegments = new System.Windows.Forms.TextBox();
            this.lblSplitSegNumbers = new System.Windows.Forms.Label();
            this.rbSegNumbers = new System.Windows.Forms.RadioButton();
            this.lblSplitEqualParts = new System.Windows.Forms.Label();
            this.nudParts = new System.Windows.Forms.NumericUpDown();
            this.rbEqualParts = new System.Windows.Forms.RadioButton();
            this.rbWordCount = new System.Windows.Forms.RadioButton();
            this.pWordCountOptions = new System.Windows.Forms.Panel();
            this.gbSplitOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudWords)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPercent)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudParts)).BeginInit();
            this.pWordCountOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // Banner
            // 
            this.Banner.Size = new System.Drawing.Size(610, 64);
            // 
            // gbSplitOptions
            // 
            this.gbSplitOptions.Controls.Add(this.clbSegments);
            this.gbSplitOptions.Location = new System.Drawing.Point(3, 46);
            this.gbSplitOptions.Name = "gbSplitOptions";
            this.gbSplitOptions.Size = new System.Drawing.Size(318, 117);
            this.gbSplitOptions.TabIndex = 17;
            this.gbSplitOptions.TabStop = false;
            this.gbSplitOptions.Text = "&Don\'t include the following segment statuses in word count:";
            // 
            // clbSegments
            // 
            this.clbSegments.BackColor = System.Drawing.SystemColors.Control;
            this.clbSegments.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbSegments.CheckOnClick = true;
            this.clbSegments.FormattingEnabled = true;
            this.clbSegments.Location = new System.Drawing.Point(11, 19);
            this.clbSegments.Name = "clbSegments";
            this.clbSegments.Size = new System.Drawing.Size(223, 90);
            this.clbSegments.TabIndex = 18;
            // 
            // chSplitCheckPercent
            // 
            this.chSplitCheckPercent.AutoSize = true;
            this.chSplitCheckPercent.Location = new System.Drawing.Point(15, 6);
            this.chSplitCheckPercent.Name = "chSplitCheckPercent";
            this.chSplitCheckPercent.Size = new System.Drawing.Size(262, 17);
            this.chSplitCheckPercent.TabIndex = 13;
            this.chSplitCheckPercent.Text = "C&heck translation correspondence score (percent)";
            this.chSplitCheckPercent.UseVisualStyleBackColor = true;
            this.chSplitCheckPercent.CheckedChanged += new System.EventHandler(this.chSplitCheckPercent_CheckedChanged);
            // 
            // lblSplitWordCount2
            // 
            this.lblSplitWordCount2.AutoSize = true;
            this.lblSplitWordCount2.Location = new System.Drawing.Point(258, 39);
            this.lblSplitWordCount2.Name = "lblSplitWordCount2";
            this.lblSplitWordCount2.Size = new System.Drawing.Size(35, 13);
            this.lblSplitWordCount2.TabIndex = 5;
            this.lblSplitWordCount2.Text = "words";
            // 
            // lblSplitWordCount
            // 
            this.lblSplitWordCount.AutoSize = true;
            this.lblSplitWordCount.Location = new System.Drawing.Point(44, 39);
            this.lblSplitWordCount.Name = "lblSplitWordCount";
            this.lblSplitWordCount.Size = new System.Drawing.Size(134, 13);
            this.lblSplitWordCount.TabIndex = 3;
            this.lblSplitWordCount.Text = "&Max word count in split file:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(228, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "%";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(47, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Max score (&percent):";
            // 
            // nudWords
            // 
            this.nudWords.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWords.Location = new System.Drawing.Point(183, 36);
            this.nudWords.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudWords.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudWords.Name = "nudWords";
            this.nudWords.Size = new System.Drawing.Size(74, 20);
            this.nudWords.TabIndex = 4;
            this.nudWords.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // nudPercent
            // 
            this.nudPercent.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudPercent.Location = new System.Drawing.Point(151, 23);
            this.nudPercent.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudPercent.Name = "nudPercent";
            this.nudPercent.Size = new System.Drawing.Size(74, 20);
            this.nudPercent.TabIndex = 15;
            this.nudPercent.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblSplitSegNumbers2);
            this.groupBox1.Controls.Add(this.tbSegments);
            this.groupBox1.Controls.Add(this.lblSplitSegNumbers);
            this.groupBox1.Controls.Add(this.rbSegNumbers);
            this.groupBox1.Controls.Add(this.lblSplitEqualParts);
            this.groupBox1.Controls.Add(this.nudParts);
            this.groupBox1.Controls.Add(this.rbEqualParts);
            this.groupBox1.Controls.Add(this.rbWordCount);
            this.groupBox1.Controls.Add(this.lblSplitWordCount);
            this.groupBox1.Controls.Add(this.nudWords);
            this.groupBox1.Controls.Add(this.lblSplitWordCount2);
            this.groupBox1.Location = new System.Drawing.Point(11, 69);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(381, 141);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Split Criterion";
            // 
            // lblSplitSegNumbers2
            // 
            this.lblSplitSegNumbers2.AutoSize = true;
            this.lblSplitSegNumbers2.Location = new System.Drawing.Point(257, 111);
            this.lblSplitSegNumbers2.Name = "lblSplitSegNumbers2";
            this.lblSplitSegNumbers2.Size = new System.Drawing.Size(112, 13);
            this.lblSplitSegNumbers2.TabIndex = 12;
            this.lblSplitSegNumbers2.Text = "(comma-separated list)";
            // 
            // tbSegments
            // 
            this.tbSegments.Location = new System.Drawing.Point(183, 108);
            this.tbSegments.Name = "tbSegments";
            this.tbSegments.Size = new System.Drawing.Size(74, 20);
            this.tbSegments.TabIndex = 11;
            // 
            // lblSplitSegNumbers
            // 
            this.lblSplitSegNumbers.AutoSize = true;
            this.lblSplitSegNumbers.Location = new System.Drawing.Point(31, 111);
            this.lblSplitSegNumbers.Name = "lblSplitSegNumbers";
            this.lblSplitSegNumbers.Size = new System.Drawing.Size(152, 13);
            this.lblSplitSegNumbers.TabIndex = 10;
            this.lblSplitSegNumbers.Text = "Segment numbers to split &after:";
            // 
            // rbSegNumbers
            // 
            this.rbSegNumbers.AutoSize = true;
            this.rbSegNumbers.Location = new System.Drawing.Point(12, 91);
            this.rbSegNumbers.Name = "rbSegNumbers";
            this.rbSegNumbers.Size = new System.Drawing.Size(145, 17);
            this.rbSegNumbers.TabIndex = 9;
            this.rbSegNumbers.Text = "Split by &segment numbers";
            this.rbSegNumbers.UseVisualStyleBackColor = true;
            this.rbSegNumbers.CheckedChanged += new System.EventHandler(this.rbSegNumbers_CheckedChanged);
            // 
            // lblSplitEqualParts
            // 
            this.lblSplitEqualParts.AutoSize = true;
            this.lblSplitEqualParts.Location = new System.Drawing.Point(53, 75);
            this.lblSplitEqualParts.Name = "lblSplitEqualParts";
            this.lblSplitEqualParts.Size = new System.Drawing.Size(130, 13);
            this.lblSplitEqualParts.TabIndex = 7;
            this.lblSplitEqualParts.Text = "&Number of parts to split to:";
            // 
            // nudParts
            // 
            this.nudParts.Location = new System.Drawing.Point(183, 72);
            this.nudParts.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudParts.Name = "nudParts";
            this.nudParts.Size = new System.Drawing.Size(74, 20);
            this.nudParts.TabIndex = 8;
            this.nudParts.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // rbEqualParts
            // 
            this.rbEqualParts.AutoSize = true;
            this.rbEqualParts.Location = new System.Drawing.Point(12, 55);
            this.rbEqualParts.Name = "rbEqualParts";
            this.rbEqualParts.Size = new System.Drawing.Size(120, 17);
            this.rbEqualParts.TabIndex = 6;
            this.rbEqualParts.Text = "Split into &equal parts";
            this.rbEqualParts.UseVisualStyleBackColor = true;
            this.rbEqualParts.CheckedChanged += new System.EventHandler(this.rbEqualParts_CheckedChanged);
            // 
            // rbWordCount
            // 
            this.rbWordCount.AutoSize = true;
            this.rbWordCount.Checked = true;
            this.rbWordCount.Location = new System.Drawing.Point(12, 19);
            this.rbWordCount.Name = "rbWordCount";
            this.rbWordCount.Size = new System.Drawing.Size(115, 17);
            this.rbWordCount.TabIndex = 2;
            this.rbWordCount.TabStop = true;
            this.rbWordCount.Text = "Split by &word count";
            this.rbWordCount.UseVisualStyleBackColor = true;
            this.rbWordCount.CheckedChanged += new System.EventHandler(this.rbWordCount_CheckedChanged);
            // 
            // pWordCountOptions
            // 
            this.pWordCountOptions.Controls.Add(this.gbSplitOptions);
            this.pWordCountOptions.Controls.Add(this.chSplitCheckPercent);
            this.pWordCountOptions.Controls.Add(this.nudPercent);
            this.pWordCountOptions.Controls.Add(this.label2);
            this.pWordCountOptions.Controls.Add(this.label1);
            this.pWordCountOptions.Location = new System.Drawing.Point(8, 212);
            this.pWordCountOptions.Name = "pWordCountOptions";
            this.pWordCountOptions.Size = new System.Drawing.Size(384, 166);
            this.pWordCountOptions.TabIndex = 12;
            // 
            // OptionsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pWordCountOptions);
            this.Controls.Add(this.groupBox1);
            this.Name = "OptionsPage";
            this.Size = new System.Drawing.Size(610, 385);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.OptionsPage_SetActive);
            this.WizardNext += new Sdl.Utilities.SplitSDLXLIFF.Wizard.WizardPageEventHandler(this.OptionsPage_WizardNext);
            this.WizardPreNext += new Sdl.Utilities.SplitSDLXLIFF.Wizard.WizardPageEventHandler(this.OptionsPage_WizardPreNext);
            this.WizardBack += new Sdl.Utilities.SplitSDLXLIFF.Wizard.WizardPageEventHandler(this.OptionsPage_WizardBack);
            this.Controls.SetChildIndex(this.Banner, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.pWordCountOptions, 0);
            this.gbSplitOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudWords)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPercent)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudParts)).EndInit();
            this.pWordCountOptions.ResumeLayout(false);
            this.pWordCountOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbSplitOptions;
        private System.Windows.Forms.CheckedListBox clbSegments;
        private System.Windows.Forms.CheckBox chSplitCheckPercent;
        private System.Windows.Forms.Label lblSplitWordCount2;
        private System.Windows.Forms.Label lblSplitWordCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudWords;
        private System.Windows.Forms.NumericUpDown nudPercent;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblSplitEqualParts;
        private System.Windows.Forms.NumericUpDown nudParts;
        private System.Windows.Forms.RadioButton rbEqualParts;
        private System.Windows.Forms.RadioButton rbWordCount;
        private System.Windows.Forms.Label lblSplitSegNumbers2;
        private System.Windows.Forms.TextBox tbSegments;
        private System.Windows.Forms.Label lblSplitSegNumbers;
        private System.Windows.Forms.RadioButton rbSegNumbers;
        private System.Windows.Forms.Panel pWordCountOptions;
    }
}
