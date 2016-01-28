namespace ExportToExcel
{
    partial class ExportToExcelSettingsControl
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
            this.btnLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.btnReset = new System.Windows.Forms.Button();
            this.exclusionsUI = new ExportToExcel.ExclusionsUI();
            this.exportTypeUI = new ExportToExcel.ExportTypeUI();
            this.btnLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLayoutPanel
            // 
            this.btnLayoutPanel.ColumnCount = 1;
            this.btnLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.btnLayoutPanel.Controls.Add(this.exclusionsUI, 0, 0);
            this.btnLayoutPanel.Controls.Add(this.exportTypeUI, 0, 1);
            this.btnLayoutPanel.Controls.Add(this.btnReset, 0, 2);
            this.btnLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.btnLayoutPanel.Name = "btnLayoutPanel";
            this.btnLayoutPanel.RowCount = 3;
            this.btnLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.btnLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.btnLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.btnLayoutPanel.Size = new System.Drawing.Size(656, 436);
            this.btnLayoutPanel.TabIndex = 1;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(3, 409);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(109, 23);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "Reset to default";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // exclusionsUI
            // 
            this.exclusionsUI.AutoSize = true;
            this.exclusionsUI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exclusionsUI.Location = new System.Drawing.Point(3, 3);
            this.exclusionsUI.Name = "exclusionsUI";
            this.exclusionsUI.Size = new System.Drawing.Size(650, 291);
            this.exclusionsUI.TabIndex = 0;
            // 
            // exportTypeUI
            // 
            this.exportTypeUI.AutoSize = true;
            this.exportTypeUI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exportTypeUI.Location = new System.Drawing.Point(3, 300);
            this.exportTypeUI.Name = "exportTypeUI";
            this.exportTypeUI.Size = new System.Drawing.Size(650, 103);
            this.exportTypeUI.TabIndex = 1;
            // 
            // ExportToExcelSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnLayoutPanel);
            this.Name = "ExportToExcelSettingsControl";
            this.Size = new System.Drawing.Size(656, 436);
            this.btnLayoutPanel.ResumeLayout(false);
            this.btnLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ExclusionsUI exclusionsUI;
        private System.Windows.Forms.TableLayoutPanel btnLayoutPanel;
        private ExportTypeUI exportTypeUI;
        private System.Windows.Forms.Button btnReset;
    }
}
