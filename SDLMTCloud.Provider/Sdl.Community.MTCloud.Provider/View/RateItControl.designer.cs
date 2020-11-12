namespace Sdl.Community.MTCloud.Provider.View
{
	partial class RateItControl
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
			this.rateItElementHost = new System.Windows.Forms.Integration.ElementHost();
			this.SuspendLayout();
			// 
			// rateItElementHost
			// 
			this.rateItElementHost.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rateItElementHost.Location = new System.Drawing.Point(0, 0);
			this.rateItElementHost.Name = "rateItElementHost";
			this.rateItElementHost.Size = new System.Drawing.Size(150, 150);
			this.rateItElementHost.TabIndex = 0;
			this.rateItElementHost.Text = "rateItElementHost";
			this.rateItElementHost.Child = null;
			// 
			// RateItControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.rateItElementHost);
			this.Name = "RateItControl";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Integration.ElementHost rateItElementHost;
	}
}
