namespace Sdl.Community.InSource
{
    partial class InSourceViewControl
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
            this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this._projectsListBox = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this._filesListView = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonsDialogPanel = new System.Windows.Forms.TableLayoutPanel();
            this.folderMainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.foldersListView = new BrightIdeasSoftware.ObjectListView();
            this.pathColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.deleteColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.templateColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.btnsLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.btn_ClearMessages = new System.Windows.Forms.Button();
            this.saveBtn = new System.Windows.Forms.Button();
            this.addBtn = new System.Windows.Forms.Button();
            this._resultsTextBox = new System.Windows.Forms.TextBox();
            this._tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.folderMainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.foldersListView)).BeginInit();
            this.btnsLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tableLayoutPanel
            // 
            this._tableLayoutPanel.ColumnCount = 2;
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanel.Controls.Add(this._progressBar, 0, 4);
            this._tableLayoutPanel.Controls.Add(this.splitContainer1, 0, 1);
            this._tableLayoutPanel.Controls.Add(this.buttonsDialogPanel, 1, 3);
            this._tableLayoutPanel.Controls.Add(this.folderMainPanel, 1, 5);
            this._tableLayoutPanel.Controls.Add(this._resultsTextBox, 0, 5);
            this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._tableLayoutPanel.Name = "_tableLayoutPanel";
            this._tableLayoutPanel.RowCount = 6;
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 53.20513F));
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 46.79487F));
            this._tableLayoutPanel.Size = new System.Drawing.Size(736, 413);
            this._tableLayoutPanel.TabIndex = 0;
            // 
            // _progressBar
            // 
            this._progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tableLayoutPanel.SetColumnSpan(this._progressBar, 2);
            this._progressBar.Location = new System.Drawing.Point(3, 208);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(730, 20);
            this._progressBar.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tableLayoutPanel.SetColumnSpan(this.splitContainer1, 2);
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel2);
            this.splitContainer1.Size = new System.Drawing.Size(730, 199);
            this.splitContainer1.SplitterDistance = 242;
            this.splitContainer1.TabIndex = 8;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._projectsListBox, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(242, 199);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Project Requests:";
            // 
            // _projectsListBox
            // 
            this._projectsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._projectsListBox.FormattingEnabled = true;
            this._projectsListBox.IntegralHeight = false;
            this._projectsListBox.Location = new System.Drawing.Point(0, 13);
            this._projectsListBox.Margin = new System.Windows.Forms.Padding(0);
            this._projectsListBox.Name = "_projectsListBox";
            this._projectsListBox.Size = new System.Drawing.Size(242, 186);
            this._projectsListBox.TabIndex = 0;
            this._projectsListBox.Click += new System.EventHandler(this._projectsListBox_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this._filesListView, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(484, 199);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // _filesListView
            // 
            this._filesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._filesListView.HideSelection = false;
            this._filesListView.Location = new System.Drawing.Point(0, 13);
            this._filesListView.Margin = new System.Windows.Forms.Padding(0);
            this._filesListView.Name = "_filesListView";
            this._filesListView.Size = new System.Drawing.Size(484, 186);
            this._filesListView.TabIndex = 0;
            this._filesListView.UseCompatibleStateImageBehavior = false;
            this._filesListView.View = System.Windows.Forms.View.List;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Files:";
            // 
            // buttonsDialogPanel
            // 
            this.buttonsDialogPanel.AutoSize = true;
            this.buttonsDialogPanel.ColumnCount = 2;
            this.buttonsDialogPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.buttonsDialogPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.buttonsDialogPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonsDialogPanel.Location = new System.Drawing.Point(435, 205);
            this.buttonsDialogPanel.Margin = new System.Windows.Forms.Padding(0);
            this.buttonsDialogPanel.Name = "buttonsDialogPanel";
            this.buttonsDialogPanel.RowCount = 1;
            this.buttonsDialogPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.buttonsDialogPanel.Size = new System.Drawing.Size(301, 1);
            this.buttonsDialogPanel.TabIndex = 9;
            // 
            // folderMainPanel
            // 
            this.folderMainPanel.ColumnCount = 1;
            this.folderMainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.folderMainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.folderMainPanel.Controls.Add(this.label3, 0, 0);
            this.folderMainPanel.Controls.Add(this.foldersListView, 0, 1);
            this.folderMainPanel.Controls.Add(this.btnsLayoutPanel, 0, 2);
            this.folderMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.folderMainPanel.Location = new System.Drawing.Point(438, 234);
            this.folderMainPanel.Name = "folderMainPanel";
            this.folderMainPanel.RowCount = 3;
            this.folderMainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.folderMainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.folderMainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.folderMainPanel.Size = new System.Drawing.Size(295, 176);
            this.folderMainPanel.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Watch folders";
            // 
            // foldersListView
            // 
            this.foldersListView.AllColumns.Add(this.pathColumn);
            this.foldersListView.AllColumns.Add(this.deleteColumn);
            this.foldersListView.AllColumns.Add(this.templateColumn);
            this.foldersListView.CellEditUseWholeCell = false;
            this.foldersListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.pathColumn,
            this.deleteColumn,
            this.templateColumn});
            this.foldersListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.foldersListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.foldersListView.HideSelection = false;
            this.foldersListView.HighlightBackgroundColor = System.Drawing.Color.Empty;
            this.foldersListView.HighlightForegroundColor = System.Drawing.Color.Empty;
            this.foldersListView.Location = new System.Drawing.Point(3, 16);
            this.foldersListView.Name = "foldersListView";
            this.foldersListView.Size = new System.Drawing.Size(289, 117);
            this.foldersListView.TabIndex = 2;
            this.foldersListView.UseCompatibleStateImageBehavior = false;
            this.foldersListView.View = System.Windows.Forms.View.Details;
            // 
            // pathColumn
            // 
            this.pathColumn.IsEditable = false;
            this.pathColumn.Text = "Path";
            this.pathColumn.ToolTipText = "";
            this.pathColumn.Width = 300;
            // 
            // deleteColumn
            // 
            this.deleteColumn.Text = "Delete";
            this.deleteColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.deleteColumn.Width = 50;
            // 
            // templateColumn
            // 
            this.templateColumn.Text = "Template";
            this.templateColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.templateColumn.Width = 120;
            // 
            // btnsLayoutPanel
            // 
            this.btnsLayoutPanel.ColumnCount = 4;
            this.btnsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42.68191F));
            this.btnsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.39166F));
            this.btnsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.92642F));
            this.btnsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.btnsLayoutPanel.Controls.Add(this.btn_ClearMessages, 0, 0);
            this.btnsLayoutPanel.Controls.Add(this.saveBtn, 2, 0);
            this.btnsLayoutPanel.Controls.Add(this.addBtn, 1, 0);
            this.btnsLayoutPanel.Location = new System.Drawing.Point(3, 139);
            this.btnsLayoutPanel.Name = "btnsLayoutPanel";
            this.btnsLayoutPanel.RowCount = 1;
            this.btnsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.btnsLayoutPanel.Size = new System.Drawing.Size(289, 34);
            this.btnsLayoutPanel.TabIndex = 3;
            // 
            // btn_ClearMessages
            // 
            this.btn_ClearMessages.Location = new System.Drawing.Point(3, 3);
            this.btn_ClearMessages.Name = "btn_ClearMessages";
            this.btn_ClearMessages.Size = new System.Drawing.Size(102, 23);
            this.btn_ClearMessages.TabIndex = 0;
            this.btn_ClearMessages.Text = "Clear messages";
            this.btn_ClearMessages.UseVisualStyleBackColor = true;
            this.btn_ClearMessages.Click += new System.EventHandler(this.btn_ClearMessages_Click);
            // 
            // saveBtn
            // 
            this.saveBtn.Location = new System.Drawing.Point(209, 3);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(65, 23);
            this.saveBtn.TabIndex = 2;
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // addBtn
            // 
            this.addBtn.Location = new System.Drawing.Point(119, 3);
            this.addBtn.Name = "addBtn";
            this.addBtn.Size = new System.Drawing.Size(64, 23);
            this.addBtn.TabIndex = 1;
            this.addBtn.Text = "Add folder";
            this.addBtn.UseVisualStyleBackColor = true;
            this.addBtn.Click += new System.EventHandler(this.addBtn_Click);
            // 
            // _resultsTextBox
            // 
            this._resultsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._resultsTextBox.Location = new System.Drawing.Point(3, 234);
            this._resultsTextBox.Multiline = true;
            this._resultsTextBox.Name = "_resultsTextBox";
            this._resultsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._resultsTextBox.Size = new System.Drawing.Size(429, 176);
            this._resultsTextBox.TabIndex = 5;
            // 
            // InSourceViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this._tableLayoutPanel);
            this.Name = "InSourceViewControl";
            this.Size = new System.Drawing.Size(736, 413);
            this._tableLayoutPanel.ResumeLayout(false);
            this._tableLayoutPanel.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.folderMainPanel.ResumeLayout(false);
            this.folderMainPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.foldersListView)).EndInit();
            this.btnsLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
        private System.Windows.Forms.ProgressBar _progressBar;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView _filesListView;
        private System.Windows.Forms.ListBox _projectsListBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel buttonsDialogPanel;
        private System.Windows.Forms.TextBox _resultsTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel folderMainPanel;
        private BrightIdeasSoftware.ObjectListView foldersListView;
        private BrightIdeasSoftware.OLVColumn pathColumn;
        private BrightIdeasSoftware.OLVColumn deleteColumn;
        private BrightIdeasSoftware.OLVColumn templateColumn;
        private System.Windows.Forms.TableLayoutPanel btnsLayoutPanel;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Button addBtn;
		private System.Windows.Forms.Button btn_ClearMessages;
	}
}
