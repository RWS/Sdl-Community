namespace Sdl.Community.TranslationMemoryProvider
{
	partial class OptionsPanel
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
			this.lblSelectTask = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblSelectTask
			// 
			this.lblSelectTask.AutoSize = true;
			this.lblSelectTask.Location = new System.Drawing.Point(26, 11);
			this.lblSelectTask.Name = "lblSelectTask";
			this.lblSelectTask.Size = new System.Drawing.Size(142, 13);
			this.lblSelectTask.TabIndex = 1;
			this.lblSelectTask.Text = "Select TM Task to continue.";
			// 
			// pTaskOptions
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblSelectTask);
			this.Name = "pTaskOptions";
			this.Size = new System.Drawing.Size(200, 100);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblSelectTask;
	}
}
