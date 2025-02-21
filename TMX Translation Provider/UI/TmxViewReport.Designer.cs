namespace TMX_TranslationProvider.UI
{
	partial class TmxViewReport
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
            this.timerUpdateReport = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tusRead = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.close = new System.Windows.Forms.Button();
            this.tusImportedSuccessfully = new System.Windows.Forms.Label();
            this.tusWithSyntaxErrors = new System.Windows.Forms.Label();
            this.tusWithInvalidChars = new System.Windows.Forms.Label();
            this.importedLanguages = new System.Windows.Forms.Label();
            this.importStartTime = new System.Windows.Forms.Label();
            this.importTimeSecs = new System.Windows.Forms.Label();
            this.error = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // timerUpdateReport
            // 
            this.timerUpdateReport.Enabled = true;
            this.timerUpdateReport.Interval = 1000;
            this.timerUpdateReport.Tick += new System.EventHandler(this.timerUpdateReport_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "TUs Read";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "TUs Imported Successfully";
            // 
            // tusRead
            // 
            this.tusRead.AutoSize = true;
            this.tusRead.Location = new System.Drawing.Point(181, 15);
            this.tusRead.Name = "tusRead";
            this.tusRead.Size = new System.Drawing.Size(35, 13);
            this.tusRead.TabIndex = 2;
            this.tusRead.Text = "label3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "TUs With Syntax Errors";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 90);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(116, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "TUs With Invalid Chars";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(348, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Imported Languages";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(348, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(86, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Import Started At";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(348, 90);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(95, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Import Time (Secs)";
            // 
            // close
            // 
            this.close.Location = new System.Drawing.Point(501, 131);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(75, 23);
            this.close.TabIndex = 8;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // tusImportedSuccessfully
            // 
            this.tusImportedSuccessfully.AutoSize = true;
            this.tusImportedSuccessfully.Location = new System.Drawing.Point(181, 40);
            this.tusImportedSuccessfully.Name = "tusImportedSuccessfully";
            this.tusImportedSuccessfully.Size = new System.Drawing.Size(35, 13);
            this.tusImportedSuccessfully.TabIndex = 9;
            this.tusImportedSuccessfully.Text = "label3";
            // 
            // tusWithSyntaxErrors
            // 
            this.tusWithSyntaxErrors.AutoSize = true;
            this.tusWithSyntaxErrors.Location = new System.Drawing.Point(181, 65);
            this.tusWithSyntaxErrors.Name = "tusWithSyntaxErrors";
            this.tusWithSyntaxErrors.Size = new System.Drawing.Size(35, 13);
            this.tusWithSyntaxErrors.TabIndex = 10;
            this.tusWithSyntaxErrors.Text = "label3";
            // 
            // tusWithInvalidChars
            // 
            this.tusWithInvalidChars.AutoSize = true;
            this.tusWithInvalidChars.Location = new System.Drawing.Point(181, 90);
            this.tusWithInvalidChars.Name = "tusWithInvalidChars";
            this.tusWithInvalidChars.Size = new System.Drawing.Size(35, 13);
            this.tusWithInvalidChars.TabIndex = 11;
            this.tusWithInvalidChars.Text = "label3";
            // 
            // importedLanguages
            // 
            this.importedLanguages.AutoSize = true;
            this.importedLanguages.Location = new System.Drawing.Point(498, 15);
            this.importedLanguages.Name = "importedLanguages";
            this.importedLanguages.Size = new System.Drawing.Size(35, 13);
            this.importedLanguages.TabIndex = 12;
            this.importedLanguages.Text = "label3";
            // 
            // importStartTime
            // 
            this.importStartTime.AutoSize = true;
            this.importStartTime.Location = new System.Drawing.Point(498, 65);
            this.importStartTime.Name = "importStartTime";
            this.importStartTime.Size = new System.Drawing.Size(35, 13);
            this.importStartTime.TabIndex = 13;
            this.importStartTime.Text = "label3";
            // 
            // importTimeSecs
            // 
            this.importTimeSecs.AutoSize = true;
            this.importTimeSecs.Location = new System.Drawing.Point(498, 90);
            this.importTimeSecs.Name = "importTimeSecs";
            this.importTimeSecs.Size = new System.Drawing.Size(35, 13);
            this.importTimeSecs.TabIndex = 14;
            this.importTimeSecs.Text = "label3";
            // 
            // error
            // 
            this.error.AutoSize = true;
            this.error.ForeColor = System.Drawing.Color.Red;
            this.error.Location = new System.Drawing.Point(12, 136);
            this.error.Name = "error";
            this.error.Size = new System.Drawing.Size(0, 13);
            this.error.TabIndex = 15;
            // 
            // TmxViewReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 166);
            this.Controls.Add(this.error);
            this.Controls.Add(this.importTimeSecs);
            this.Controls.Add(this.importStartTime);
            this.Controls.Add(this.importedLanguages);
            this.Controls.Add(this.tusWithInvalidChars);
            this.Controls.Add(this.tusWithSyntaxErrors);
            this.Controls.Add(this.tusImportedSuccessfully);
            this.Controls.Add(this.close);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tusRead);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "TmxViewReport";
            this.Text = "Import Report";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Timer timerUpdateReport;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label tusRead;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button close;
        private System.Windows.Forms.Label tusImportedSuccessfully;
        private System.Windows.Forms.Label tusWithSyntaxErrors;
        private System.Windows.Forms.Label tusWithInvalidChars;
        private System.Windows.Forms.Label importedLanguages;
        private System.Windows.Forms.Label importStartTime;
        private System.Windows.Forms.Label importTimeSecs;
        private System.Windows.Forms.Label error;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}