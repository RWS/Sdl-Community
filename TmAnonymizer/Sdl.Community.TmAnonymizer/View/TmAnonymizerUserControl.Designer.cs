namespace Sdl.Community.SdlTmAnonymizer.View
{
	partial class TmAnonymizerUserControl
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
			this.elementHost = new System.Windows.Forms.Integration.ElementHost();
			this.SuspendLayout();
			// 
			// elementHost
			// 
			this.elementHost.Dock = System.Windows.Forms.DockStyle.Fill;
			this.elementHost.Location = new System.Drawing.Point(0, 0);
			this.elementHost.Name = "elementHost";
			this.elementHost.Size = new System.Drawing.Size(512, 353);
			this.elementHost.TabIndex = 0;
			this.elementHost.Text = "elementHost1";
			this.elementHost.Child = null;
			// 
			// TmAnonymizerUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.elementHost);
			this.Name = "TmAnonymizerUserControl";
			this.Size = new System.Drawing.Size(512, 353);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Integration.ElementHost elementHost;
	}
}
