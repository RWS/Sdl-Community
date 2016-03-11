namespace Sdl.Community.ContentConnector
{
    partial class AddFoldersForm
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
            this.mainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.foldersPathListView = new BrightIdeasSoftware.ObjectListView();
            this.pathColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.deleteColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.buttonsLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.addButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.mainLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.foldersPathListView)).BeginInit();
            this.buttonsLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainLayoutPanel
            // 
            this.mainLayoutPanel.ColumnCount = 1;
            this.mainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayoutPanel.Controls.Add(this.foldersPathListView, 0, 1);
            this.mainLayoutPanel.Controls.Add(this.buttonsLayoutPanel, 0, 2);
            this.mainLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mainLayoutPanel.Name = "mainLayoutPanel";
            this.mainLayoutPanel.RowCount = 3;
            this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.mainLayoutPanel.Size = new System.Drawing.Size(519, 322);
            this.mainLayoutPanel.TabIndex = 0;
            // 
            // foldersPathListView
            // 
            this.foldersPathListView.AllColumns.Add(this.pathColumn);
            this.foldersPathListView.AllColumns.Add(this.deleteColumn);
            this.foldersPathListView.CellEditUseWholeCell = false;
            this.foldersPathListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.pathColumn,
            this.deleteColumn});
            this.foldersPathListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.foldersPathListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.foldersPathListView.HighlightBackgroundColor = System.Drawing.Color.Empty;
            this.foldersPathListView.HighlightForegroundColor = System.Drawing.Color.Empty;
            this.foldersPathListView.Location = new System.Drawing.Point(3, 23);
            this.foldersPathListView.Name = "foldersPathListView";
            this.foldersPathListView.Size = new System.Drawing.Size(513, 246);
            this.foldersPathListView.TabIndex = 0;
            this.foldersPathListView.UseCompatibleStateImageBehavior = false;
            this.foldersPathListView.View = System.Windows.Forms.View.Details;
            // 
            // pathColumn
            // 
            this.pathColumn.Text = "Path";
            this.pathColumn.Width = 403;
            // 
            // deleteColumn
            // 
            this.deleteColumn.Text = "Delete";
            this.deleteColumn.Width = 106;
            // 
            // buttonsLayoutPanel
            // 
            this.buttonsLayoutPanel.ColumnCount = 3;
            this.buttonsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.buttonsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.buttonsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.buttonsLayoutPanel.Controls.Add(this.addButton, 0, 0);
            this.buttonsLayoutPanel.Controls.Add(this.saveButton, 1, 0);
            this.buttonsLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonsLayoutPanel.Location = new System.Drawing.Point(3, 275);
            this.buttonsLayoutPanel.Name = "buttonsLayoutPanel";
            this.buttonsLayoutPanel.RowCount = 1;
            this.buttonsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.buttonsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.buttonsLayoutPanel.Size = new System.Drawing.Size(513, 44);
            this.buttonsLayoutPanel.TabIndex = 1;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(3, 3);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(126, 23);
            this.addButton.TabIndex = 0;
            this.addButton.Text = "Add folder path";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addBtn_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(174, 3);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(93, 23);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // AddFoldersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 322);
            this.Controls.Add(this.mainLayoutPanel);
            this.Name = "AddFoldersForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select folders";
            this.mainLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.foldersPathListView)).EndInit();
            this.buttonsLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainLayoutPanel;
        private BrightIdeasSoftware.ObjectListView foldersPathListView;
        private System.Windows.Forms.TableLayoutPanel buttonsLayoutPanel;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button saveButton;
        private BrightIdeasSoftware.OLVColumn pathColumn;
        private BrightIdeasSoftware.OLVColumn deleteColumn;
    }
}