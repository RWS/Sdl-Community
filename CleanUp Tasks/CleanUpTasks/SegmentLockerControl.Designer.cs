namespace Sdl.Community.CleanUpTasks
{
    partial class SegmentLockerControl
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
            this.segmentLockerCheckGroupBox = new UIToolbox.CheckGroupBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.contentCheckGroupBox = new UIToolbox.CheckGroupBox();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.structureCheckGroupBox = new UIToolbox.CheckGroupBox();
            this.checkedListBox = new System.Windows.Forms.CheckedListBox();
            this.segmentLockerCheckGroupBox.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.contentCheckGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.structureCheckGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // segmentLockerCheckGroupBox
            // 
            this.segmentLockerCheckGroupBox.Controls.Add(this.tableLayoutPanel);
            this.segmentLockerCheckGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.segmentLockerCheckGroupBox.Location = new System.Drawing.Point(0, 0);
            this.segmentLockerCheckGroupBox.Name = "segmentLockerCheckGroupBox";
            this.segmentLockerCheckGroupBox.Size = new System.Drawing.Size(429, 213);
            this.segmentLockerCheckGroupBox.TabIndex = 0;
            this.segmentLockerCheckGroupBox.TabStop = false;
            this.segmentLockerCheckGroupBox.Text = "Lock Segments";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67.8487F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.1513F));
            this.tableLayoutPanel.Controls.Add(this.contentCheckGroupBox, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.structureCheckGroupBox, 1, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(3, 15);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(423, 195);
            this.tableLayoutPanel.TabIndex = 2;
            // 
            // contentCheckGroupBox
            // 
            this.contentCheckGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contentCheckGroupBox.Controls.Add(this.dataGridView);
            this.contentCheckGroupBox.Location = new System.Drawing.Point(3, 3);
            this.contentCheckGroupBox.Name = "contentCheckGroupBox";
            this.contentCheckGroupBox.Size = new System.Drawing.Size(281, 189);
            this.contentCheckGroupBox.TabIndex = 1;
            this.contentCheckGroupBox.TabStop = false;
            this.contentCheckGroupBox.Text = "Content";
            // 
            // dataGridView
            // 
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(3, 15);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowTemplate.Height = 21;
            this.dataGridView.Size = new System.Drawing.Size(275, 171);
            this.dataGridView.TabIndex = 1;
            // 
            // structureCheckGroupBox
            // 
            this.structureCheckGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.structureCheckGroupBox.Controls.Add(this.checkedListBox);
            this.structureCheckGroupBox.Location = new System.Drawing.Point(290, 3);
            this.structureCheckGroupBox.Name = "structureCheckGroupBox";
            this.structureCheckGroupBox.Size = new System.Drawing.Size(130, 189);
            this.structureCheckGroupBox.TabIndex = 2;
            this.structureCheckGroupBox.TabStop = false;
            this.structureCheckGroupBox.Text = "Structure";
            // 
            // checkedListBox
            // 
            this.checkedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBox.CheckOnClick = true;
            this.checkedListBox.FormattingEnabled = true;
            this.checkedListBox.Location = new System.Drawing.Point(3, 15);
            this.checkedListBox.Name = "checkedListBox";
            this.checkedListBox.Size = new System.Drawing.Size(121, 172);
            this.checkedListBox.TabIndex = 1;
            // 
            // SegmentLockerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.segmentLockerCheckGroupBox);
            this.Name = "SegmentLockerControl";
            this.Size = new System.Drawing.Size(429, 213);
            this.segmentLockerCheckGroupBox.ResumeLayout(false);
            this.segmentLockerCheckGroupBox.PerformLayout();
            this.tableLayoutPanel.ResumeLayout(false);
            this.contentCheckGroupBox.ResumeLayout(false);
            this.contentCheckGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.structureCheckGroupBox.ResumeLayout(false);
            this.structureCheckGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private UIToolbox.CheckGroupBox segmentLockerCheckGroupBox;
        private UIToolbox.CheckGroupBox contentCheckGroupBox;
        private UIToolbox.CheckGroupBox structureCheckGroupBox;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.CheckedListBox checkedListBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    }
}
