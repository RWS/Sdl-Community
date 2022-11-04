namespace TMX_TranslationProvider
{
	partial class TmxOptionsForm
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
            this.cancel = new System.Windows.Forms.Button();
            this.ok = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.fileName = new System.Windows.Forms.TextBox();
            this.browse = new System.Windows.Forms.Button();
            this.fileNameIsInvalid = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.Location = new System.Drawing.Point(425, 42);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 0;
            this.cancel.Text = "&Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // ok
            // 
            this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ok.Location = new System.Drawing.Point(344, 42);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 1;
            this.ok.Text = "&OK";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "TMX File";
            // 
            // fileName
            // 
            this.fileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileName.Location = new System.Drawing.Point(57, 12);
            this.fileName.Name = "fileName";
            this.fileName.Size = new System.Drawing.Size(407, 20);
            this.fileName.TabIndex = 3;
            this.fileName.TextChanged += new System.EventHandler(this.fileName_TextChanged);
            // 
            // browse
            // 
            this.browse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browse.Location = new System.Drawing.Point(470, 10);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(30, 23);
            this.browse.TabIndex = 4;
            this.browse.Text = "...";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.browse_Click);
            // 
            // fileNameIsInvalid
            // 
            this.fileNameIsInvalid.AutoSize = true;
            this.fileNameIsInvalid.ForeColor = System.Drawing.Color.Red;
            this.fileNameIsInvalid.Location = new System.Drawing.Point(54, 47);
            this.fileNameIsInvalid.Name = "fileNameIsInvalid";
            this.fileNameIsInvalid.Size = new System.Drawing.Size(95, 13);
            this.fileNameIsInvalid.TabIndex = 5;
            this.fileNameIsInvalid.Text = "File name is invalid";
            this.fileNameIsInvalid.Visible = false;
            // 
            // TmxOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 77);
            this.Controls.Add(this.fileNameIsInvalid);
            this.Controls.Add(this.browse);
            this.Controls.Add(this.fileName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.cancel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TmxOptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TMX Translation Provider Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox fileName;
        private System.Windows.Forms.Button browse;
        private System.Windows.Forms.Label fileNameIsInvalid;
    }
}