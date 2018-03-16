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
			this.btn_ConvertFiles = new System.Windows.Forms.Button();
			this.btn_SaveFiles = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btn_BrowseFiles
			// 
			this.btn_BrowseFiles.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.btn_BrowseFiles.Location = new System.Drawing.Point(11, 90);
			this.btn_BrowseFiles.Margin = new System.Windows.Forms.Padding(4);
			this.btn_BrowseFiles.Name = "btn_BrowseFiles";
			this.btn_BrowseFiles.Size = new System.Drawing.Size(139, 34);
			this.btn_BrowseFiles.TabIndex = 4;
			this.btn_BrowseFiles.Text = "&Browse";
			this.btn_BrowseFiles.UseVisualStyleBackColor = true;
			this.btn_BrowseFiles.Click += new System.EventHandler(this.btn_BrowseFiles_Click);
			// 
			// txt_SdlxliffFiles
			// 
			this.txt_SdlxliffFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txt_SdlxliffFiles.Location = new System.Drawing.Point(160, 13);
			this.txt_SdlxliffFiles.Margin = new System.Windows.Forms.Padding(4);
			this.txt_SdlxliffFiles.Multiline = true;
			this.txt_SdlxliffFiles.Name = "txt_SdlxliffFiles";
			this.txt_SdlxliffFiles.ReadOnly = true;
			this.txt_SdlxliffFiles.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txt_SdlxliffFiles.Size = new System.Drawing.Size(631, 222);
			this.txt_SdlxliffFiles.TabIndex = 5;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 143);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(153, 17);
			this.label1.TabIndex = 30;
			this.label1.Text = "Select SDLXLIFF file(s)";
			// 
			// btn_ConvertFiles
			// 
			this.btn_ConvertFiles.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btn_ConvertFiles.Location = new System.Drawing.Point(409, 256);
			this.btn_ConvertFiles.Margin = new System.Windows.Forms.Padding(4);
			this.btn_ConvertFiles.Name = "btn_ConvertFiles";
			this.btn_ConvertFiles.Size = new System.Drawing.Size(181, 28);
			this.btn_ConvertFiles.TabIndex = 31;
			this.btn_ConvertFiles.Text = "&Convert .sdlxliff file(s)";
			this.btn_ConvertFiles.UseVisualStyleBackColor = true;
			this.btn_ConvertFiles.Click += new System.EventHandler(this.btn_ConvertFiles_Click);
			// 
			// btn_SaveFiles
			// 
			this.btn_SaveFiles.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btn_SaveFiles.Location = new System.Drawing.Point(610, 256);
			this.btn_SaveFiles.Margin = new System.Windows.Forms.Padding(4);
			this.btn_SaveFiles.Name = "btn_SaveFiles";
			this.btn_SaveFiles.Size = new System.Drawing.Size(181, 28);
			this.btn_SaveFiles.TabIndex = 32;
			this.btn_SaveFiles.Text = "&Save file(s)";
			this.btn_SaveFiles.UseVisualStyleBackColor = true;
			this.btn_SaveFiles.Click += new System.EventHandler(this.btn_SaveFiles_Click);
			// 
			// AmgenForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(804, 303);
			this.Controls.Add(this.btn_SaveFiles);
			this.Controls.Add(this.btn_ConvertFiles);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txt_SdlxliffFiles);
			this.Controls.Add(this.btn_BrowseFiles);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximumSize = new System.Drawing.Size(2000, 350);
			this.MinimumSize = new System.Drawing.Size(822, 350);
			this.Name = "AmgenForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Amgen";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btn_BrowseFiles;
		private System.Windows.Forms.TextBox txt_SdlxliffFiles;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btn_ConvertFiles;
		private System.Windows.Forms.Button btn_SaveFiles;
	}
}

