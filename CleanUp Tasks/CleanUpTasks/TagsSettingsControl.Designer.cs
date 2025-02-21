namespace Sdl.Community.CleanUpTasks
{
    partial class TagsSettingsControl
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
            this.tagCheckGroupBox = new UIToolbox.CheckGroupBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.fmtCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.phCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.tagCheckGroupBox.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tagCheckGroupBox
            // 
            this.tagCheckGroupBox.Controls.Add(this.tableLayoutPanel);
            this.tagCheckGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tagCheckGroupBox.Location = new System.Drawing.Point(0, 0);
            this.tagCheckGroupBox.Name = "tagCheckGroupBox";
            this.tagCheckGroupBox.Size = new System.Drawing.Size(399, 68);
            this.tagCheckGroupBox.TabIndex = 0;
            this.tagCheckGroupBox.TabStop = false;
            this.tagCheckGroupBox.Text = "Remove Tags";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.fmtCheckedListBox, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.phCheckedListBox, 1, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(3, 15);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(393, 50);
            this.tableLayoutPanel.TabIndex = 1;
            // 
            // fmtCheckedListBox
            // 
            this.fmtCheckedListBox.CheckOnClick = true;
            this.fmtCheckedListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fmtCheckedListBox.FormattingEnabled = true;
            this.fmtCheckedListBox.Location = new System.Drawing.Point(3, 3);
            this.fmtCheckedListBox.Name = "fmtCheckedListBox";
            this.fmtCheckedListBox.Size = new System.Drawing.Size(190, 44);
            this.fmtCheckedListBox.TabIndex = 0;
            // 
            // phCheckedListBox
            // 
            this.phCheckedListBox.CheckOnClick = true;
            this.phCheckedListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.phCheckedListBox.FormattingEnabled = true;
            this.phCheckedListBox.Location = new System.Drawing.Point(199, 3);
            this.phCheckedListBox.Name = "phCheckedListBox";
            this.phCheckedListBox.Size = new System.Drawing.Size(191, 44);
            this.phCheckedListBox.TabIndex = 1;
            // 
            // TagsSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tagCheckGroupBox);
            this.Name = "TagsSettingsControl";
            this.Size = new System.Drawing.Size(399, 68);
            this.tagCheckGroupBox.ResumeLayout(false);
            this.tagCheckGroupBox.PerformLayout();
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private UIToolbox.CheckGroupBox tagCheckGroupBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.CheckedListBox fmtCheckedListBox;
        private System.Windows.Forms.CheckedListBox phCheckedListBox;
    }
}
