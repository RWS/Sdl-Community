namespace Sdl.Community.TMBackup
{
	partial class TMBackupDetailsForm
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
            this.btn_Ok = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.lbl_RulesDescription = new System.Windows.Forms.Label();
            this.btn_Add = new System.Windows.Forms.Button();
            this.btn_UpArrow = new System.Windows.Forms.Button();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.btn_DownArrow = new System.Windows.Forms.Button();
            this.btn_Reset = new System.Windows.Forms.Button();
            this.lbl_Line = new System.Windows.Forms.Label();
            this.col_Action = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Pattern = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Ok
            // 
            this.btn_Ok.Location = new System.Drawing.Point(358, 167);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(75, 23);
            this.btn_Ok.TabIndex = 7;
            this.btn_Ok.Text = "Ok";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(450, 167);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 8;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_Action,
            this.col_Type,
            this.col_Pattern});
            this.dataGridView1.Location = new System.Drawing.Point(12, 23);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView1.Size = new System.Drawing.Size(432, 43);
            this.dataGridView1.TabIndex = 9;
            // 
            // lbl_RulesDescription
            // 
            this.lbl_RulesDescription.AutoSize = true;
            this.lbl_RulesDescription.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lbl_RulesDescription.Location = new System.Drawing.Point(17, 131);
            this.lbl_RulesDescription.Name = "lbl_RulesDescription";
            this.lbl_RulesDescription.Size = new System.Drawing.Size(281, 13);
            this.lbl_RulesDescription.TabIndex = 10;
            this.lbl_RulesDescription.Text = "Rules are scanned from the top. First matching rule is final.";
            // 
            // btn_Add
            // 
            this.btn_Add.Location = new System.Drawing.Point(450, 23);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(75, 23);
            this.btn_Add.TabIndex = 11;
            this.btn_Add.Text = "Add...";
            this.btn_Add.UseVisualStyleBackColor = true;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // btn_UpArrow
            // 
            this.btn_UpArrow.Location = new System.Drawing.Point(492, 81);
            this.btn_UpArrow.Name = "btn_UpArrow";
            this.btn_UpArrow.Size = new System.Drawing.Size(33, 23);
            this.btn_UpArrow.TabIndex = 15;
            this.btn_UpArrow.Text = "↑";
            this.btn_UpArrow.UseVisualStyleBackColor = true;
            this.btn_UpArrow.Click += new System.EventHandler(this.btn_UpArrow_Click);
            // 
            // btn_Delete
            // 
            this.btn_Delete.Location = new System.Drawing.Point(450, 52);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(75, 23);
            this.btn_Delete.TabIndex = 12;
            this.btn_Delete.Text = "Delete";
            this.btn_Delete.UseVisualStyleBackColor = true;
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // btn_DownArrow
            // 
            this.btn_DownArrow.Location = new System.Drawing.Point(450, 81);
            this.btn_DownArrow.Name = "btn_DownArrow";
            this.btn_DownArrow.Size = new System.Drawing.Size(35, 23);
            this.btn_DownArrow.TabIndex = 14;
            this.btn_DownArrow.Text = "↓";
            this.btn_DownArrow.UseVisualStyleBackColor = true;
            this.btn_DownArrow.Click += new System.EventHandler(this.btn_DownArrow_Click);
            // 
            // btn_Reset
            // 
            this.btn_Reset.Location = new System.Drawing.Point(450, 110);
            this.btn_Reset.Name = "btn_Reset";
            this.btn_Reset.Size = new System.Drawing.Size(75, 23);
            this.btn_Reset.TabIndex = 13;
            this.btn_Reset.Text = "Reset";
            this.btn_Reset.UseVisualStyleBackColor = true;
            this.btn_Reset.Click += new System.EventHandler(this.btn_Reset_Click);
            // 
            // lbl_Line
            // 
            this.lbl_Line.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_Line.Location = new System.Drawing.Point(-7, 154);
            this.lbl_Line.Name = "lbl_Line";
            this.lbl_Line.Size = new System.Drawing.Size(567, 2);
            this.lbl_Line.TabIndex = 16;
            // 
            // col_Action
            // 
            this.col_Action.FillWeight = 45.68528F;
            this.col_Action.HeaderText = "Action";
            this.col_Action.Name = "col_Action";
            // 
            // col_Type
            // 
            this.col_Type.FillWeight = 127.1574F;
            this.col_Type.HeaderText = "Type";
            this.col_Type.Name = "col_Type";
            // 
            // col_Pattern
            // 
            this.col_Pattern.FillWeight = 127.1574F;
            this.col_Pattern.HeaderText = "Pattern";
            this.col_Pattern.Name = "col_Pattern";
            // 
            // TMBackupDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 206);
            this.Controls.Add(this.lbl_Line);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.lbl_RulesDescription);
            this.Controls.Add(this.btn_Add);
            this.Controls.Add(this.btn_UpArrow);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.btn_DownArrow);
            this.Controls.Add(this.btn_Reset);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Ok);
            this.Name = "TMBackupDetailsForm";
            this.Text = "What to backup";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button btn_Ok;
		private System.Windows.Forms.Button btn_Cancel;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.Label lbl_RulesDescription;
		private System.Windows.Forms.Button btn_Add;
		private System.Windows.Forms.Button btn_UpArrow;
		private System.Windows.Forms.Button btn_Delete;
		private System.Windows.Forms.Button btn_DownArrow;
		private System.Windows.Forms.Button btn_Reset;
		private System.Windows.Forms.Label lbl_Line;
		private System.Windows.Forms.DataGridViewTextBoxColumn col_Action;
		private System.Windows.Forms.DataGridViewTextBoxColumn col_Type;
		private System.Windows.Forms.DataGridViewTextBoxColumn col_Pattern;
	}
}