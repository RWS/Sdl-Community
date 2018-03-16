namespace Sdl.Community.Amgen
{
	partial class AmgenForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AmgenForm));
			this.btn_BrowseFiles = new System.Windows.Forms.Button();
			this.txt_SdlxliffFiles = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btn_BrowseFiles
			// 
			this.btn_BrowseFiles.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btn_BrowseFiles.Location = new System.Drawing.Point(637, 127);
			this.btn_BrowseFiles.Margin = new System.Windows.Forms.Padding(4);
			this.btn_BrowseFiles.Name = "btn_BrowseFiles";
			this.btn_BrowseFiles.Size = new System.Drawing.Size(139, 34);
			this.btn_BrowseFiles.TabIndex = 4;
			this.btn_BrowseFiles.Text = "Browse";
			this.btn_BrowseFiles.UseVisualStyleBackColor = true;
			this.btn_BrowseFiles.Click += new System.EventHandler(this.btn_BrowseFiles_Click);
			// 
			// txt_SdlxliffFiles
			// 
			this.txt_SdlxliffFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txt_SdlxliffFiles.Location = new System.Drawing.Point(13, 60);
			this.txt_SdlxliffFiles.Margin = new System.Windows.Forms.Padding(4);
			this.txt_SdlxliffFiles.Multiline = true;
			this.txt_SdlxliffFiles.Name = "txt_SdlxliffFiles";
			this.txt_SdlxliffFiles.ReadOnly = true;
			this.txt_SdlxliffFiles.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txt_SdlxliffFiles.Size = new System.Drawing.Size(616, 166);
			this.txt_SdlxliffFiles.TabIndex = 5;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 27);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(153, 17);
			this.label1.TabIndex = 30;
			this.label1.Text = "Select SDLXLIFF file(s)";
			// 
			// AmgenForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(789, 342);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txt_SdlxliffFiles);
			this.Controls.Add(this.btn_BrowseFiles);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "AmgenForm";
			this.Text = "Amgen";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btn_BrowseFiles;
		private System.Windows.Forms.TextBox txt_SdlxliffFiles;
		private System.Windows.Forms.Label label1;
	}
}

