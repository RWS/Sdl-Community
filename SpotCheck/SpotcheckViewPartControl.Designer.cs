namespace Sdl.Studio.SpotCheck
{
    partial class SpotCheckViewPartControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpotCheckViewPartControl));
            this.cmdAddMarkers = new System.Windows.Forms.Button();
            this.cmdRemoveMarkers = new System.Windows.Forms.Button();
            this.numPercentage = new System.Windows.Forms.NumericUpDown();
            this.lblNotes = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numMinWords = new System.Windows.Forms.NumericUpDown();
            this.numMaxWords = new System.Windows.Forms.NumericUpDown();
            this.chkSkipLocked = new System.Windows.Forms.CheckBox();
            this.chkSkipCm = new System.Windows.Forms.CheckBox();
            this.chkSkip100 = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblWordLimit = new System.Windows.Forms.Label();
            this.numTotalWords = new System.Windows.Forms.NumericUpDown();
            this.chkLimitByWords = new System.Windows.Forms.CheckBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblSegmentLimit = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.chkSkipRepetition = new System.Windows.Forms.CheckBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.numPercentage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinWords)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxWords)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTotalWords)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // cmdAddMarkers
            // 
            this.cmdAddMarkers.Location = new System.Drawing.Point(270, 11);
            this.cmdAddMarkers.Name = "cmdAddMarkers";
            this.cmdAddMarkers.Size = new System.Drawing.Size(117, 23);
            this.cmdAddMarkers.TabIndex = 1;
            this.cmdAddMarkers.Text = "Add Markers";
            this.cmdAddMarkers.UseVisualStyleBackColor = true;
            this.cmdAddMarkers.Click += new System.EventHandler(this.cmdAddMarkers_Click);
            // 
            // cmdRemoveMarkers
            // 
            this.cmdRemoveMarkers.Location = new System.Drawing.Point(270, 99);
            this.cmdRemoveMarkers.Name = "cmdRemoveMarkers";
            this.cmdRemoveMarkers.Size = new System.Drawing.Size(117, 23);
            this.cmdRemoveMarkers.TabIndex = 3;
            this.cmdRemoveMarkers.Text = "Remove Markers";
            this.cmdRemoveMarkers.UseVisualStyleBackColor = true;
            this.cmdRemoveMarkers.Click += new System.EventHandler(this.cmdRemoveMarkers_Click);
            // 
            // numPercentage
            // 
            this.numPercentage.Location = new System.Drawing.Point(134, 41);
            this.numPercentage.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPercentage.Name = "numPercentage";
            this.numPercentage.Size = new System.Drawing.Size(47, 20);
            this.numPercentage.TabIndex = 2;
            this.numPercentage.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numPercentage.ValueChanged += new System.EventHandler(this.numPercentage_ValueChanged);
            // 
            // lblNotes
            // 
            this.lblNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNotes.AutoSize = true;
            this.lblNotes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblNotes.Location = new System.Drawing.Point(25, 188);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(0, 13);
            this.lblNotes.TabIndex = 7;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Location = new System.Drawing.Point(1, 175);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(919, 10);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // numMinWords
            // 
            this.numMinWords.Location = new System.Drawing.Point(134, 115);
            this.numMinWords.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMinWords.Name = "numMinWords";
            this.numMinWords.Size = new System.Drawing.Size(47, 20);
            this.numMinWords.TabIndex = 7;
            this.numMinWords.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numMinWords.ValueChanged += new System.EventHandler(this.numMinWords_ValueChanged);
            // 
            // numMaxWords
            // 
            this.numMaxWords.Location = new System.Drawing.Point(134, 137);
            this.numMaxWords.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numMaxWords.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaxWords.Name = "numMaxWords";
            this.numMaxWords.Size = new System.Drawing.Size(47, 20);
            this.numMaxWords.TabIndex = 9;
            this.numMaxWords.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numMaxWords.ValueChanged += new System.EventHandler(this.numMaxWords_ValueChanged);
            // 
            // chkSkipLocked
            // 
            this.chkSkipLocked.AutoSize = true;
            this.chkSkipLocked.Location = new System.Drawing.Point(20, 50);
            this.chkSkipLocked.Name = "chkSkipLocked";
            this.chkSkipLocked.Size = new System.Drawing.Size(130, 17);
            this.chkSkipLocked.TabIndex = 1;
            this.chkSkipLocked.Text = "Skip locked segments";
            this.chkSkipLocked.UseVisualStyleBackColor = true;
            this.chkSkipLocked.CheckStateChanged += new System.EventHandler(this.chkSkipLocked_CheckStateChanged);
            // 
            // chkSkipCm
            // 
            this.chkSkipCm.AutoSize = true;
            this.chkSkipCm.Location = new System.Drawing.Point(20, 94);
            this.chkSkipCm.Name = "chkSkipCm";
            this.chkSkipCm.Size = new System.Drawing.Size(114, 17);
            this.chkSkipCm.TabIndex = 3;
            this.chkSkipCm.Text = "Skip CM segments";
            this.chkSkipCm.UseVisualStyleBackColor = true;
            this.chkSkipCm.CheckStateChanged += new System.EventHandler(this.chkSkipCm_CheckStateChanged);
            // 
            // chkSkip100
            // 
            this.chkSkip100.AutoSize = true;
            this.chkSkip100.Location = new System.Drawing.Point(20, 72);
            this.chkSkip100.Name = "chkSkip100";
            this.chkSkip100.Size = new System.Drawing.Size(119, 17);
            this.chkSkip100.TabIndex = 2;
            this.chkSkip100.Text = "Skip 100% matches";
            this.chkSkip100.UseVisualStyleBackColor = true;
            this.chkSkip100.CheckStateChanged += new System.EventHandler(this.chkSkip100_CheckStateChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Controls.Add(this.lblWordLimit);
            this.panel1.Controls.Add(this.numTotalWords);
            this.panel1.Controls.Add(this.chkLimitByWords);
            this.panel1.Controls.Add(this.pictureBox4);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.lblSegmentLimit);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.numPercentage);
            this.panel1.Controls.Add(this.numMaxWords);
            this.panel1.Controls.Add(this.numMinWords);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 182);
            this.panel1.TabIndex = 0;
            // 
            // lblWordLimit
            // 
            this.lblWordLimit.AutoSize = true;
            this.lblWordLimit.Location = new System.Drawing.Point(15, 87);
            this.lblWordLimit.Name = "lblWordLimit";
            this.lblWordLimit.Size = new System.Drawing.Size(100, 13);
            this.lblWordLimit.TabIndex = 4;
            this.lblWordLimit.Text = "Total words to mark";
            // 
            // numTotalWords
            // 
            this.numTotalWords.Enabled = false;
            this.numTotalWords.Location = new System.Drawing.Point(134, 85);
            this.numTotalWords.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numTotalWords.Name = "numTotalWords";
            this.numTotalWords.Size = new System.Drawing.Size(47, 20);
            this.numTotalWords.TabIndex = 5;
            this.numTotalWords.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTotalWords.ValueChanged += new System.EventHandler(this.numTotalWords_ValueChanged);
            // 
            // chkLimitByWords
            // 
            this.chkLimitByWords.AutoSize = true;
            this.chkLimitByWords.Location = new System.Drawing.Point(18, 66);
            this.chkLimitByWords.Name = "chkLimitByWords";
            this.chkLimitByWords.Size = new System.Drawing.Size(152, 17);
            this.chkLimitByWords.TabIndex = 3;
            this.chkLimitByWords.Text = "Limit by total words instead";
            this.chkLimitByWords.UseVisualStyleBackColor = true;
            this.chkLimitByWords.CheckedChanged += new System.EventHandler(this.chkLimitByWords_CheckedChanged);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(165, 3);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(32, 32);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 29;
            this.pictureBox4.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Max segment length";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Min segment length";
            // 
            // lblSegmentLimit
            // 
            this.lblSegmentLimit.AutoSize = true;
            this.lblSegmentLimit.Location = new System.Drawing.Point(15, 43);
            this.lblSegmentLimit.Name = "lblSegmentLimit";
            this.lblSegmentLimit.Size = new System.Drawing.Size(113, 13);
            this.lblSegmentLimit.TabIndex = 1;
            this.lblSegmentLimit.Text = "% of segments to mark";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label1.Location = new System.Drawing.Point(114, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Settings";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel2.Controls.Add(this.chkSkipRepetition);
            this.panel2.Controls.Add(this.pictureBox3);
            this.panel2.Controls.Add(this.chkSkip100);
            this.panel2.Controls.Add(this.chkSkipLocked);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.chkSkipCm);
            this.panel2.Location = new System.Drawing.Point(717, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 182);
            this.panel2.TabIndex = 5;
            // 
            // chkSkipRepetition
            // 
            this.chkSkipRepetition.AutoSize = true;
            this.chkSkipRepetition.Location = new System.Drawing.Point(20, 116);
            this.chkSkipRepetition.Name = "chkSkipRepetition";
            this.chkSkipRepetition.Size = new System.Drawing.Size(98, 17);
            this.chkSkipRepetition.TabIndex = 4;
            this.chkSkipRepetition.Text = "Skip repetitions";
            this.chkSkipRepetition.UseVisualStyleBackColor = true;
            this.chkSkipRepetition.CheckedChanged += new System.EventHandler(this.chkSkipRepetition_CheckedChanged);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(168, 3);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(32, 32);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox3.TabIndex = 29;
            this.pictureBox3.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label5.Location = new System.Drawing.Point(119, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Options";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label6.Location = new System.Drawing.Point(267, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(444, 53);
            this.label6.TabIndex = 2;
            this.label6.Text = "For selected files: Add comments to mark spotcheck segments. The files are then o" +
    "pened for review.";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label7.Location = new System.Drawing.Point(267, 125);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(444, 45);
            this.label7.TabIndex = 4;
            this.label7.Text = "For selected files: Remove spotcheck comments and and open files for review.";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(229, 11);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 27;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(229, 99);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 32);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 28;
            this.pictureBox2.TabStop = false;
            // 
            // SpotCheckViewPartControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.cmdRemoveMarkers);
            this.Controls.Add(this.cmdAddMarkers);
            this.Name = "SpotCheckViewPartControl";
            this.Size = new System.Drawing.Size(920, 209);
            ((System.ComponentModel.ISupportInitialize)(this.numPercentage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinWords)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxWords)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTotalWords)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdAddMarkers;
        private System.Windows.Forms.Button cmdRemoveMarkers;
        private System.Windows.Forms.NumericUpDown numPercentage;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numMinWords;
        private System.Windows.Forms.NumericUpDown numMaxWords;
        private System.Windows.Forms.CheckBox chkSkipLocked;
        private System.Windows.Forms.CheckBox chkSkipCm;
        private System.Windows.Forms.CheckBox chkSkip100;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblSegmentLimit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Label lblWordLimit;
        private System.Windows.Forms.NumericUpDown numTotalWords;
        private System.Windows.Forms.CheckBox chkLimitByWords;
        private System.Windows.Forms.CheckBox chkSkipRepetition;
    }
}
