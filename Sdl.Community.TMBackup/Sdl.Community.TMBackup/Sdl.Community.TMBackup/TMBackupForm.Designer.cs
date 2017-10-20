namespace Sdl.Community.TMBackup
{
	partial class TMBackupForm
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
            this.txt_BackupFrom = new System.Windows.Forms.TextBox();
            this.btn_BackupFrom = new System.Windows.Forms.Button();
            this.btn_BackupTo = new System.Windows.Forms.Button();
            this.txt_BackupTo = new System.Windows.Forms.TextBox();
            this.lbl_BackupFrom = new System.Windows.Forms.Label();
            this.lbl_BackupTo = new System.Windows.Forms.Label();
            this.txt_Description = new System.Windows.Forms.TextBox();
            this.lbl_Description = new System.Windows.Forms.Label();
            this.btn_Create = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Change = new System.Windows.Forms.Button();
            this.btn_Details = new System.Windows.Forms.Button();
            this.txt_BackupFileDesc = new System.Windows.Forms.TextBox();
            this.txt_BackupTime = new System.Windows.Forms.TextBox();
            this.lbl_BackupTime = new System.Windows.Forms.Label();
            this.lbl_BackupFileDesc = new System.Windows.Forms.Label();
            this.lbl_Line2 = new System.Windows.Forms.Label();
            this.lbl_Line1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txt_BackupFrom
            // 
            this.txt_BackupFrom.Location = new System.Drawing.Point(137, 41);
            this.txt_BackupFrom.Multiline = true;
            this.txt_BackupFrom.Name = "txt_BackupFrom";
            this.txt_BackupFrom.ReadOnly = true;
            this.txt_BackupFrom.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_BackupFrom.Size = new System.Drawing.Size(400, 87);
            this.txt_BackupFrom.TabIndex = 2;
            // 
            // btn_BackupFrom
            // 
            this.btn_BackupFrom.Location = new System.Drawing.Point(543, 66);
            this.btn_BackupFrom.Name = "btn_BackupFrom";
            this.btn_BackupFrom.Size = new System.Drawing.Size(104, 28);
            this.btn_BackupFrom.TabIndex = 3;
            this.btn_BackupFrom.Text = "Browse";
            this.btn_BackupFrom.UseVisualStyleBackColor = true;
            this.btn_BackupFrom.Click += new System.EventHandler(this.btn_BackupFrom_Click);
            // 
            // btn_BackupTo
            // 
            this.btn_BackupTo.Location = new System.Drawing.Point(543, 138);
            this.btn_BackupTo.Name = "btn_BackupTo";
            this.btn_BackupTo.Size = new System.Drawing.Size(104, 27);
            this.btn_BackupTo.TabIndex = 4;
            this.btn_BackupTo.Text = "Browse";
            this.btn_BackupTo.UseVisualStyleBackColor = true;
            this.btn_BackupTo.Click += new System.EventHandler(this.btn_BackupTo_Click);
            // 
            // txt_BackupTo
            // 
            this.txt_BackupTo.Location = new System.Drawing.Point(137, 134);
            this.txt_BackupTo.Multiline = true;
            this.txt_BackupTo.Name = "txt_BackupTo";
            this.txt_BackupTo.ReadOnly = true;
            this.txt_BackupTo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_BackupTo.Size = new System.Drawing.Size(400, 38);
            this.txt_BackupTo.TabIndex = 5;
            // 
            // lbl_BackupFrom
            // 
            this.lbl_BackupFrom.AutoSize = true;
            this.lbl_BackupFrom.Location = new System.Drawing.Point(21, 49);
            this.lbl_BackupFrom.Name = "lbl_BackupFrom";
            this.lbl_BackupFrom.Size = new System.Drawing.Size(67, 13);
            this.lbl_BackupFrom.TabIndex = 11;
            this.lbl_BackupFrom.Text = "Backup from";
            // 
            // lbl_BackupTo
            // 
            this.lbl_BackupTo.AutoSize = true;
            this.lbl_BackupTo.Location = new System.Drawing.Point(21, 145);
            this.lbl_BackupTo.Name = "lbl_BackupTo";
            this.lbl_BackupTo.Size = new System.Drawing.Size(56, 13);
            this.lbl_BackupTo.TabIndex = 12;
            this.lbl_BackupTo.Text = "Backup to";
            // 
            // txt_Description
            // 
            this.txt_Description.Location = new System.Drawing.Point(137, 185);
            this.txt_Description.Multiline = true;
            this.txt_Description.Name = "txt_Description";
            this.txt_Description.Size = new System.Drawing.Size(400, 27);
            this.txt_Description.TabIndex = 14;
            // 
            // lbl_Description
            // 
            this.lbl_Description.AutoSize = true;
            this.lbl_Description.Location = new System.Drawing.Point(21, 188);
            this.lbl_Description.Name = "lbl_Description";
            this.lbl_Description.Size = new System.Drawing.Size(60, 13);
            this.lbl_Description.TabIndex = 13;
            this.lbl_Description.Text = "Description";
            // 
            // btn_Create
            // 
            this.btn_Create.Location = new System.Drawing.Point(433, 357);
            this.btn_Create.Name = "btn_Create";
            this.btn_Create.Size = new System.Drawing.Size(104, 27);
            this.btn_Create.TabIndex = 18;
            this.btn_Create.Text = "Create";
            this.btn_Create.UseVisualStyleBackColor = true;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(543, 357);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(104, 27);
            this.btn_Cancel.TabIndex = 19;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // btn_Change
            // 
            this.btn_Change.Location = new System.Drawing.Point(543, 288);
            this.btn_Change.Name = "btn_Change";
            this.btn_Change.Size = new System.Drawing.Size(104, 27);
            this.btn_Change.TabIndex = 25;
            this.btn_Change.Text = "Change ↓";
            this.btn_Change.UseVisualStyleBackColor = true;
            // 
            // btn_Details
            // 
            this.btn_Details.Location = new System.Drawing.Point(543, 255);
            this.btn_Details.Name = "btn_Details";
            this.btn_Details.Size = new System.Drawing.Size(104, 27);
            this.btn_Details.TabIndex = 24;
            this.btn_Details.Text = "Details";
            this.btn_Details.UseVisualStyleBackColor = true;
            // 
            // txt_BackupFileDesc
            // 
            this.txt_BackupFileDesc.Location = new System.Drawing.Point(137, 255);
            this.txt_BackupFileDesc.Multiline = true;
            this.txt_BackupFileDesc.Name = "txt_BackupFileDesc";
            this.txt_BackupFileDesc.ReadOnly = true;
            this.txt_BackupFileDesc.Size = new System.Drawing.Size(400, 27);
            this.txt_BackupFileDesc.TabIndex = 23;
            // 
            // txt_BackupTime
            // 
            this.txt_BackupTime.Location = new System.Drawing.Point(137, 288);
            this.txt_BackupTime.Multiline = true;
            this.txt_BackupTime.Name = "txt_BackupTime";
            this.txt_BackupTime.ReadOnly = true;
            this.txt_BackupTime.Size = new System.Drawing.Size(400, 27);
            this.txt_BackupTime.TabIndex = 22;
            // 
            // lbl_BackupTime
            // 
            this.lbl_BackupTime.AutoSize = true;
            this.lbl_BackupTime.Location = new System.Drawing.Point(21, 291);
            this.lbl_BackupTime.Name = "lbl_BackupTime";
            this.lbl_BackupTime.Size = new System.Drawing.Size(87, 13);
            this.lbl_BackupTime.TabIndex = 21;
            this.lbl_BackupTime.Text = "When to backup";
            // 
            // lbl_BackupFileDesc
            // 
            this.lbl_BackupFileDesc.AutoSize = true;
            this.lbl_BackupFileDesc.Location = new System.Drawing.Point(21, 258);
            this.lbl_BackupFileDesc.Name = "lbl_BackupFileDesc";
            this.lbl_BackupFileDesc.Size = new System.Drawing.Size(84, 13);
            this.lbl_BackupFileDesc.TabIndex = 20;
            this.lbl_BackupFileDesc.Text = "What to backup";
            // 
            // lbl_Line2
            // 
            this.lbl_Line2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_Line2.Location = new System.Drawing.Point(-7, 341);
            this.lbl_Line2.Name = "lbl_Line2";
            this.lbl_Line2.Size = new System.Drawing.Size(700, 2);
            this.lbl_Line2.TabIndex = 26;
            // 
            // lbl_Line1
            // 
            this.lbl_Line1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_Line1.Location = new System.Drawing.Point(-7, 228);
            this.lbl_Line1.Name = "lbl_Line1";
            this.lbl_Line1.Size = new System.Drawing.Size(701, 2);
            this.lbl_Line1.TabIndex = 27;
            // 
            // TMBackupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 401);
            this.Controls.Add(this.lbl_Line1);
            this.Controls.Add(this.lbl_Line2);
            this.Controls.Add(this.txt_Description);
            this.Controls.Add(this.btn_Change);
            this.Controls.Add(this.lbl_Description);
            this.Controls.Add(this.btn_Details);
            this.Controls.Add(this.lbl_BackupFrom);
            this.Controls.Add(this.txt_BackupFileDesc);
            this.Controls.Add(this.lbl_BackupTo);
            this.Controls.Add(this.txt_BackupTime);
            this.Controls.Add(this.txt_BackupFrom);
            this.Controls.Add(this.btn_BackupFrom);
            this.Controls.Add(this.lbl_BackupTime);
            this.Controls.Add(this.btn_BackupTo);
            this.Controls.Add(this.lbl_BackupFileDesc);
            this.Controls.Add(this.txt_BackupTo);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Create);
            this.Name = "TMBackupForm";
            this.Text = "New backup";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox txt_BackupFrom;
		private System.Windows.Forms.Button btn_BackupFrom;
		private System.Windows.Forms.Button btn_BackupTo;
		private System.Windows.Forms.TextBox txt_BackupTo;
		private System.Windows.Forms.Label lbl_BackupFrom;
		private System.Windows.Forms.Label lbl_BackupTo;
		private System.Windows.Forms.Label lbl_Description;
		private System.Windows.Forms.TextBox txt_Description;
		private System.Windows.Forms.Button btn_Create;
		private System.Windows.Forms.Button btn_Cancel;
		private System.Windows.Forms.Button btn_Change;
		private System.Windows.Forms.Button btn_Details;
		private System.Windows.Forms.TextBox txt_BackupFileDesc;
		private System.Windows.Forms.TextBox txt_BackupTime;
		private System.Windows.Forms.Label lbl_BackupTime;
		private System.Windows.Forms.Label lbl_BackupFileDesc;
		private System.Windows.Forms.Label lbl_Line2;
		private System.Windows.Forms.Label lbl_Line1;
	}
}