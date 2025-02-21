namespace Sdl.Community.NumberVerifier.GUI
{
	partial class TellMeSettingsActionWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TellMeSettingsActionWindow));
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.Ui = new Sdl.Community.NumberVerifier.NumberVerifierUI();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(470, 18);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 1;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(551, 18);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // Ui
            // 
            this.Ui.AddedNumbersErrorType = "";
            this.Ui.AllowLocalizations = true;
            this.Ui.AutoScroll = true;
            this.Ui.AutoSize = true;
            this.Ui.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Ui.CheckInOrder = true;
            this.Ui.CustomsSeparatorsAlphanumerics = true;
            this.Ui.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Ui.Exclude100Percents = false;
            this.Ui.ExcludeDraftSegments = false;
            this.Ui.ExcludeLockedSegments = false;
            this.Ui.ExcludeTagText = false;
            this.Ui.ExcludeUntranslatedSegments = false;
            this.Ui.GetAlphanumericsCustomSeparator = "";
            this.Ui.GetSourceDecimalCustomSeparator = "";
            this.Ui.GetSourceThousandsCustomSeparator = "";
            this.Ui.GetTargetDecimalCustomSeparator = "";
            this.Ui.GetTargetThousandsCustomSeparator = "";
            this.Ui.Location = new System.Drawing.Point(0, 0);
            this.Ui.ModifiedAlphanumericsErrorType = "";
            this.Ui.ModifiedNumbersErrorType = "";
            this.Ui.Name = "Ui";
            this.Ui.NumberFormatErrorType = "";
            this.Ui.PreventLocalizations = false;
            this.Ui.RegexExclusionList = null;
            this.Ui.RemovedNumbersErrorType = "";
            this.Ui.ReportAddedNumbers = true;
            this.Ui.ReportBriefMessages = true;
            this.Ui.ReportExtendedMessages = false;
            this.Ui.ReportModifiedAlphanumerics = true;
            this.Ui.ReportModifiedNumbers = true;
            this.Ui.ReportNumberFormatErrors = true;
            this.Ui.ReportRemovedNumbers = true;
            this.Ui.RequireLocalizations = false;
            this.Ui.Size = new System.Drawing.Size(629, 829);
            this.Ui.SourceDecimalComma = true;
            this.Ui.SourceDecimalCustomSeparator = false;
            this.Ui.SourceDecimalPeriod = true;
            this.Ui.SourceNoSeparator = true;
            this.Ui.SourceOmitLeadingZero = false;
            this.Ui.SourceThousandsComma = true;
            this.Ui.SourceThousandsCustomSeparator = false;
            this.Ui.SourceThousandsNobreakSpace = true;
            this.Ui.SourceThousandsNobreakThinSpace = true;
            this.Ui.SourceThousandsPeriod = true;
            this.Ui.SourceThousandsSpace = true;
            this.Ui.SourceThousandsThinSpace = true;
            this.Ui.TabIndex = 0;
            this.Ui.TargetDecimalComma = true;
            this.Ui.TargetDecimalCustomSeparator = false;
            this.Ui.TargetDecimalPeriod = true;
            this.Ui.TargetNoSeparator = true;
            this.Ui.TargetOmitLeadingZero = false;
            this.Ui.TargetThousandsComma = true;
            this.Ui.TargetThousandsCustomSeparator = false;
            this.Ui.TargetThousandsNobreakSpace = true;
            this.Ui.TargetThousandsNobreakThinSpace = true;
            this.Ui.TargetThousandsPeriod = true;
            this.Ui.TargetThousandsSpace = true;
            this.Ui.TargetThousandsThinSpace = true;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.OKButton);
            this.panel1.Controls.Add(this.CancelButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 785);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(629, 44);
            this.panel1.TabIndex = 3;
            // 
            // TellMeSettingsActionWindow
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(629, 829);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Ui);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "TellMeSettingsActionWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Trados Number Verifier Settings";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private NumberVerifierUI Ui;
		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.Button CancelButton;
		private System.Windows.Forms.Panel panel1;
	}
}