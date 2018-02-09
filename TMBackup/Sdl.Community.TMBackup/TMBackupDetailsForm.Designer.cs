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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TMBackupDetailsForm));
            this.btn_Ok = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Add = new System.Windows.Forms.Button();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.lbl_Line = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.lbl_InformativeMessage = new System.Windows.Forms.Label();
            this.backupActionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.backupTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.backupPatternDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.backupDetailsModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.backupDetailsModelBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Ok
            // 
            this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_Ok.Location = new System.Drawing.Point(344, 130);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(75, 23);
            this.btn_Ok.TabIndex = 7;
            this.btn_Ok.Text = "Ok";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_Cancel.Location = new System.Drawing.Point(425, 130);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 8;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Add
            // 
            this.btn_Add.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_Add.Location = new System.Drawing.Point(425, 13);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new System.Drawing.Size(75, 23);
            this.btn_Add.TabIndex = 11;
            this.btn_Add.Text = "Add...";
            this.btn_Add.UseVisualStyleBackColor = true;
            this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
            // 
            // btn_Delete
            // 
            this.btn_Delete.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_Delete.Location = new System.Drawing.Point(425, 42);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(75, 23);
            this.btn_Delete.TabIndex = 12;
            this.btn_Delete.Text = "Delete";
            this.btn_Delete.UseVisualStyleBackColor = true;
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // lbl_Line
            // 
            this.lbl_Line.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_Line.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_Line.Location = new System.Drawing.Point(-10, 123);
            this.lbl_Line.Name = "lbl_Line";
            this.lbl_Line.Size = new System.Drawing.Size(535, 2);
            this.lbl_Line.TabIndex = 16;
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.backupActionDataGridViewTextBoxColumn,
            this.backupTypeDataGridViewTextBoxColumn,
            this.backupPatternDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.backupDetailsModelBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(10, 13);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(409, 97);
            this.dataGridView1.TabIndex = 17;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick_1);
            this.dataGridView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyUp);
            // 
            // lbl_InformativeMessage
            // 
            this.lbl_InformativeMessage.AutoSize = true;
            this.lbl_InformativeMessage.ForeColor = System.Drawing.Color.Firebrick;
            this.lbl_InformativeMessage.Location = new System.Drawing.Point(7, 135);
            this.lbl_InformativeMessage.Name = "lbl_InformativeMessage";
            this.lbl_InformativeMessage.Size = new System.Drawing.Size(304, 13);
            this.lbl_InformativeMessage.TabIndex = 18;
            this.lbl_InformativeMessage.Text = "Note: To backup all types of files, no action needs to be added";
            // 
            // backupActionDataGridViewTextBoxColumn
            // 
            this.backupActionDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.backupActionDataGridViewTextBoxColumn.DataPropertyName = "BackupAction";
            this.backupActionDataGridViewTextBoxColumn.HeaderText = "Action name";
            this.backupActionDataGridViewTextBoxColumn.Name = "backupActionDataGridViewTextBoxColumn";
            this.backupActionDataGridViewTextBoxColumn.Width = 91;
            // 
            // backupTypeDataGridViewTextBoxColumn
            // 
            this.backupTypeDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.backupTypeDataGridViewTextBoxColumn.DataPropertyName = "BackupType";
            this.backupTypeDataGridViewTextBoxColumn.HeaderText = "Type of file";
            this.backupTypeDataGridViewTextBoxColumn.Name = "backupTypeDataGridViewTextBoxColumn";
            this.backupTypeDataGridViewTextBoxColumn.Width = 84;
            // 
            // backupPatternDataGridViewTextBoxColumn
            // 
            this.backupPatternDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.backupPatternDataGridViewTextBoxColumn.DataPropertyName = "BackupPattern";
            this.backupPatternDataGridViewTextBoxColumn.HeaderText = "Pattern (.sdltm or .sdltb)";
            this.backupPatternDataGridViewTextBoxColumn.Name = "backupPatternDataGridViewTextBoxColumn";
            // 
            // backupDetailsModelBindingSource
            // 
            this.backupDetailsModelBindingSource.DataSource = typeof(Sdl.Community.BackupService.Models.BackupDetailsModel);
            // 
            // TMBackupDetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 160);
            this.Controls.Add(this.lbl_InformativeMessage);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.lbl_Line);
            this.Controls.Add(this.btn_Add);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Ok);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(528, 199);
            this.Name = "TMBackupDetailsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "What to backup";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.backupDetailsModelBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button btn_Ok;
		private System.Windows.Forms.Button btn_Cancel;
		private System.Windows.Forms.Button btn_Add;
		private System.Windows.Forms.Button btn_Delete;
		private System.Windows.Forms.Label lbl_Line;
		private System.Windows.Forms.BindingSource backupDetailsModelBindingSource;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.DataGridViewTextBoxColumn backupActionDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn backupTypeDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn backupPatternDataGridViewTextBoxColumn;
		private System.Windows.Forms.Label lbl_InformativeMessage;
	}
}