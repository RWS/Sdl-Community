
using Trados.TargetRenamer.ViewModel;

namespace Trados.TargetRenamer.Control
{
	partial class TargetRenamerSettingsControl
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
            this.targetRenamerHost = new System.Windows.Forms.Integration.ElementHost();
            this.SuspendLayout();
            // 
            // targetRenamerHost
            // 
            this.targetRenamerHost.Location = new System.Drawing.Point(0, 0);
            this.targetRenamerHost.Name = "targetRenamerHost";
            this.targetRenamerHost.Size = new System.Drawing.Size(150, 150);
            this.targetRenamerHost.TabIndex = 0;
            this.targetRenamerHost.Text = "targetRenamerHost";
            this.targetRenamerHost.Child = null;
            // 
            // TargetRenamerSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.targetRenamerHost);
            this.Name = "TargetRenamerSettingsControl";
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Integration.ElementHost targetRenamerHost;
	}
}
