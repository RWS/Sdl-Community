namespace Sdl.Community.TMBackup
{
	partial class RealTimeParametersForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_Line = new System.Windows.Forms.Label();
            this.txtBox_Interval = new System.Windows.Forms.TextBox();
            this.cmbBox_Interval = new System.Windows.Forms.ComboBox();
            this.btn_Set = new System.Windows.Forms.Button();
            this.btn_Close = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(173, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Minimum interval between backups";
            // 
            // lbl_Line
            // 
            this.lbl_Line.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_Line.Location = new System.Drawing.Point(-10, 50);
            this.lbl_Line.Name = "lbl_Line";
            this.lbl_Line.Size = new System.Drawing.Size(440, 2);
            this.lbl_Line.TabIndex = 1;
            // 
            // txtBox_Interval
            // 
            this.txtBox_Interval.Location = new System.Drawing.Point(233, 18);
            this.txtBox_Interval.Name = "txtBox_Interval";
            this.txtBox_Interval.Size = new System.Drawing.Size(38, 20);
            this.txtBox_Interval.TabIndex = 2;
            // 
            // cmbBox_Interval
            // 
            this.cmbBox_Interval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBox_Interval.FormattingEnabled = true;
            this.cmbBox_Interval.Location = new System.Drawing.Point(289, 17);
            this.cmbBox_Interval.Name = "cmbBox_Interval";
            this.cmbBox_Interval.Size = new System.Drawing.Size(121, 21);
            this.cmbBox_Interval.TabIndex = 3;
            // 
            // btn_Set
            // 
            this.btn_Set.Location = new System.Drawing.Point(221, 64);
            this.btn_Set.Name = "btn_Set";
            this.btn_Set.Size = new System.Drawing.Size(96, 23);
            this.btn_Set.TabIndex = 4;
            this.btn_Set.Text = "Set";
            this.btn_Set.UseVisualStyleBackColor = true;
            this.btn_Set.Click += new System.EventHandler(this.btn_Set_Click);
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(323, 64);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(87, 23);
            this.btn_Close.TabIndex = 5;
            this.btn_Close.Text = "Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // RealTimeParametersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(427, 102);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.btn_Set);
            this.Controls.Add(this.cmbBox_Interval);
            this.Controls.Add(this.txtBox_Interval);
            this.Controls.Add(this.lbl_Line);
            this.Controls.Add(this.label1);
            this.Name = "RealTimeParametersForm";
            this.Text = "Real-time parameters";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lbl_Line;
		private System.Windows.Forms.TextBox txtBox_Interval;
		private System.Windows.Forms.ComboBox cmbBox_Interval;
		private System.Windows.Forms.Button btn_Set;
		private System.Windows.Forms.Button btn_Close;
	}
}