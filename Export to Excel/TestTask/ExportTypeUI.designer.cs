namespace ExportToExcel
{
    partial class ExportTypeUI
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
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.tb_Prefix = new System.Windows.Forms.TextBox();
            this.cb_ExtractComments = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.n_ColumnWidth = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.n_ColumnWidth)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox6.Controls.Add(this.tb_Prefix);
            this.groupBox6.Controls.Add(this.cb_ExtractComments);
            this.groupBox6.Controls.Add(this.label10);
            this.groupBox6.Controls.Add(this.n_ColumnWidth);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Location = new System.Drawing.Point(3, 6);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(614, 94);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Excel settings";
            // 
            // tb_Prefix
            // 
            this.tb_Prefix.Location = new System.Drawing.Point(341, 66);
            this.tb_Prefix.Name = "tb_Prefix";
            this.tb_Prefix.Size = new System.Drawing.Size(120, 20);
            this.tb_Prefix.TabIndex = 1;
            this.tb_Prefix.Text = "Generated_";
            // 
            // cb_ExtractComments
            // 
            this.cb_ExtractComments.AutoSize = true;
            this.cb_ExtractComments.Checked = true;
            this.cb_ExtractComments.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_ExtractComments.Location = new System.Drawing.Point(9, 19);
            this.cb_ExtractComments.Name = "cb_ExtractComments";
            this.cb_ExtractComments.Size = new System.Drawing.Size(110, 17);
            this.cb_ExtractComments.TabIndex = 0;
            this.cb_ExtractComments.Text = "Extr&act comments";
            this.cb_ExtractComments.UseVisualStyleBackColor = true;
            this.cb_ExtractComments.CheckedChanged += new System.EventHandler(this.cb_ExtractComments_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 73);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(289, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "&File name prefix (used for export and for AutoMatch feature):";
            // 
            // n_ColumnWidth
            // 
            this.n_ColumnWidth.Location = new System.Drawing.Point(341, 40);
            this.n_ColumnWidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.n_ColumnWidth.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.n_ColumnWidth.Name = "n_ColumnWidth";
            this.n_ColumnWidth.Size = new System.Drawing.Size(120, 20);
            this.n_ColumnWidth.TabIndex = 5;
            this.n_ColumnWidth.Value = new decimal(new int[] {
            75,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(160, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Widt&h of Source/Target column:";
            // 
            // ExportTypeUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.groupBox6);
            this.Name = "ExportTypeUI";
            this.Size = new System.Drawing.Size(620, 103);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.n_ColumnWidth)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.NumericUpDown n_ColumnWidth;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox cb_ExtractComments;
        private System.Windows.Forms.TextBox tb_Prefix;
        private System.Windows.Forms.Label label10;
    }
}
