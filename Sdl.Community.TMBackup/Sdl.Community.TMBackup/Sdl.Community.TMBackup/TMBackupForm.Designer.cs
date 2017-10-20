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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lbl_Description = new System.Windows.Forms.Label();
            this.txt_Description = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbl_BackupFileDesc = new System.Windows.Forms.Label();
            this.lbl_BackupTime = new System.Windows.Forms.Label();
            this.txt_BackupTime = new System.Windows.Forms.TextBox();
            this.txt_BackupFileDesc = new System.Windows.Forms.TextBox();
            this.btn_Details = new System.Windows.Forms.Button();
            this.btn_Change = new System.Windows.Forms.Button();
            this.btn_Create = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_BackupFrom
            // 
            this.txt_BackupFrom.Location = new System.Drawing.Point(136, 10);
            this.txt_BackupFrom.Multiline = true;
            this.txt_BackupFrom.Name = "txt_BackupFrom";
            this.txt_BackupFrom.Size = new System.Drawing.Size(400, 28);
            this.txt_BackupFrom.TabIndex = 2;
            // 
            // btn_BackupFrom
            // 
            this.btn_BackupFrom.Location = new System.Drawing.Point(542, 10);
            this.btn_BackupFrom.Name = "btn_BackupFrom";
            this.btn_BackupFrom.Size = new System.Drawing.Size(104, 28);
            this.btn_BackupFrom.TabIndex = 3;
            this.btn_BackupFrom.Text = "Browse";
            this.btn_BackupFrom.UseVisualStyleBackColor = true;
            // 
            // btn_BackupTo
            // 
            this.btn_BackupTo.Location = new System.Drawing.Point(542, 44);
            this.btn_BackupTo.Name = "btn_BackupTo";
            this.btn_BackupTo.Size = new System.Drawing.Size(104, 27);
            this.btn_BackupTo.TabIndex = 4;
            this.btn_BackupTo.Text = "Browse";
            this.btn_BackupTo.UseVisualStyleBackColor = true;
            // 
            // txt_BackupTo
            // 
            this.txt_BackupTo.Location = new System.Drawing.Point(136, 44);
            this.txt_BackupTo.Multiline = true;
            this.txt_BackupTo.Name = "txt_BackupTo";
            this.txt_BackupTo.Size = new System.Drawing.Size(400, 27);
            this.txt_BackupTo.TabIndex = 5;
            // 
            // lbl_BackupFrom
            // 
            this.lbl_BackupFrom.AutoSize = true;
            this.lbl_BackupFrom.Location = new System.Drawing.Point(20, 18);
            this.lbl_BackupFrom.Name = "lbl_BackupFrom";
            this.lbl_BackupFrom.Size = new System.Drawing.Size(67, 13);
            this.lbl_BackupFrom.TabIndex = 11;
            this.lbl_BackupFrom.Text = "Backup from";
            // 
            // lbl_BackupTo
            // 
            this.lbl_BackupTo.AutoSize = true;
            this.lbl_BackupTo.Location = new System.Drawing.Point(20, 51);
            this.lbl_BackupTo.Name = "lbl_BackupTo";
            this.lbl_BackupTo.Size = new System.Drawing.Size(56, 13);
            this.lbl_BackupTo.TabIndex = 12;
            this.lbl_BackupTo.Text = "Backup to";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txt_Description);
            this.panel1.Controls.Add(this.lbl_Description);
            this.panel1.Controls.Add(this.lbl_BackupFrom);
            this.panel1.Controls.Add(this.lbl_BackupTo);
            this.panel1.Controls.Add(this.txt_BackupFrom);
            this.panel1.Controls.Add(this.btn_BackupFrom);
            this.panel1.Controls.Add(this.btn_BackupTo);
            this.panel1.Controls.Add(this.txt_BackupTo);
            this.panel1.Location = new System.Drawing.Point(-9, 35);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(697, 136);
            this.panel1.TabIndex = 13;
            // 
            // lbl_Description
            // 
            this.lbl_Description.AutoSize = true;
            this.lbl_Description.Location = new System.Drawing.Point(20, 80);
            this.lbl_Description.Name = "lbl_Description";
            this.lbl_Description.Size = new System.Drawing.Size(60, 13);
            this.lbl_Description.TabIndex = 13;
            this.lbl_Description.Text = "Description";
            // 
            // txt_Description
            // 
            this.txt_Description.Location = new System.Drawing.Point(136, 77);
            this.txt_Description.Multiline = true;
            this.txt_Description.Name = "txt_Description";
            this.txt_Description.Size = new System.Drawing.Size(400, 27);
            this.txt_Description.TabIndex = 14;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btn_Change);
            this.panel2.Controls.Add(this.btn_Details);
            this.panel2.Controls.Add(this.txt_BackupFileDesc);
            this.panel2.Controls.Add(this.txt_BackupTime);
            this.panel2.Controls.Add(this.lbl_BackupTime);
            this.panel2.Controls.Add(this.lbl_BackupFileDesc);
            this.panel2.Location = new System.Drawing.Point(-9, 159);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(697, 103);
            this.panel2.TabIndex = 14;
            // 
            // lbl_BackupFileDesc
            // 
            this.lbl_BackupFileDesc.AutoSize = true;
            this.lbl_BackupFileDesc.Location = new System.Drawing.Point(20, 25);
            this.lbl_BackupFileDesc.Name = "lbl_BackupFileDesc";
            this.lbl_BackupFileDesc.Size = new System.Drawing.Size(84, 13);
            this.lbl_BackupFileDesc.TabIndex = 0;
            this.lbl_BackupFileDesc.Text = "What to backup";
            // 
            // lbl_BackupTime
            // 
            this.lbl_BackupTime.AutoSize = true;
            this.lbl_BackupTime.Location = new System.Drawing.Point(20, 58);
            this.lbl_BackupTime.Name = "lbl_BackupTime";
            this.lbl_BackupTime.Size = new System.Drawing.Size(87, 13);
            this.lbl_BackupTime.TabIndex = 1;
            this.lbl_BackupTime.Text = "When to backup";
            // 
            // txt_BackupTime
            // 
            this.txt_BackupTime.Location = new System.Drawing.Point(136, 55);
            this.txt_BackupTime.Multiline = true;
            this.txt_BackupTime.Name = "txt_BackupTime";
            this.txt_BackupTime.ReadOnly = true;
            this.txt_BackupTime.Size = new System.Drawing.Size(400, 27);
            this.txt_BackupTime.TabIndex = 15;
            // 
            // txt_BackupFileDesc
            // 
            this.txt_BackupFileDesc.Location = new System.Drawing.Point(136, 22);
            this.txt_BackupFileDesc.Multiline = true;
            this.txt_BackupFileDesc.Name = "txt_BackupFileDesc";
            this.txt_BackupFileDesc.ReadOnly = true;
            this.txt_BackupFileDesc.Size = new System.Drawing.Size(400, 27);
            this.txt_BackupFileDesc.TabIndex = 16;
            // 
            // btn_Details
            // 
            this.btn_Details.Location = new System.Drawing.Point(542, 22);
            this.btn_Details.Name = "btn_Details";
            this.btn_Details.Size = new System.Drawing.Size(104, 27);
            this.btn_Details.TabIndex = 17;
            this.btn_Details.Text = "Details";
            this.btn_Details.UseVisualStyleBackColor = true;
            // 
            // btn_Change
            // 
            this.btn_Change.Location = new System.Drawing.Point(542, 55);
            this.btn_Change.Name = "btn_Change";
            this.btn_Change.Size = new System.Drawing.Size(104, 27);
            this.btn_Change.TabIndex = 18;
            this.btn_Change.Text = "Change ↓";
            this.btn_Change.UseVisualStyleBackColor = true;
            // 
            // btn_Create
            // 
            this.btn_Create.Location = new System.Drawing.Point(416, 268);
            this.btn_Create.Name = "btn_Create";
            this.btn_Create.Size = new System.Drawing.Size(104, 27);
            this.btn_Create.TabIndex = 18;
            this.btn_Create.Text = "Create";
            this.btn_Create.UseVisualStyleBackColor = true;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(526, 268);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(104, 27);
            this.btn_Cancel.TabIndex = 19;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // TMBackupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 308);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Create);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "TMBackupForm";
            this.Text = "New backup";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.TextBox txt_BackupFrom;
		private System.Windows.Forms.Button btn_BackupFrom;
		private System.Windows.Forms.Button btn_BackupTo;
		private System.Windows.Forms.TextBox txt_BackupTo;
		private System.Windows.Forms.Label lbl_BackupFrom;
		private System.Windows.Forms.Label lbl_BackupTo;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label lbl_Description;
		private System.Windows.Forms.TextBox txt_Description;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label lbl_BackupTime;
		private System.Windows.Forms.Label lbl_BackupFileDesc;
		private System.Windows.Forms.TextBox txt_BackupFileDesc;
		private System.Windows.Forms.TextBox txt_BackupTime;
		private System.Windows.Forms.Button btn_Change;
		private System.Windows.Forms.Button btn_Details;
		private System.Windows.Forms.Button btn_Create;
		private System.Windows.Forms.Button btn_Cancel;
	}
}