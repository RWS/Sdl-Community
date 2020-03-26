namespace Sdl.Community.SDLBatchAnonymize.Ui
{
	partial class BatchAnonymizerHostControl
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
			this.batchAnonymizerHost = new System.Windows.Forms.Integration.ElementHost();
			this.SuspendLayout();
			// 
			// batchAnonymizerHost
			// 
			this.batchAnonymizerHost.Dock = System.Windows.Forms.DockStyle.Fill;
			this.batchAnonymizerHost.Location = new System.Drawing.Point(0, 0);
			this.batchAnonymizerHost.Name = "batchAnonymizerHost";
			this.batchAnonymizerHost.Size = new System.Drawing.Size(150, 150);
			this.batchAnonymizerHost.TabIndex = 0;
			this.batchAnonymizerHost.Text = "batchAnonymizerHost";
			this.batchAnonymizerHost.Child = null;
			// 
			// BatchAnonymizerHostControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.batchAnonymizerHost);
			this.Name = "BatchAnonymizerHostControl";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Integration.ElementHost batchAnonymizerHost;
	}
}
