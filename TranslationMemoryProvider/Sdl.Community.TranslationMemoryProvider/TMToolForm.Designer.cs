namespace Sdl.Community.TranslationMemoryProvider
{
	partial class TMToolForm
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Remove Duplicates");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Revert Index");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("TM Task  ", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TMToolForm));
            this.tabsPages = new System.Windows.Forms.TabControl();
            this.tabFiles = new System.Windows.Forms.TabPage();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSplitRemoveAll = new System.Windows.Forms.Button();
            this.btnSplitInFileRemove = new System.Windows.Forms.Button();
            this.btnSplitInFileAdd = new System.Windows.Forms.Button();
            this.lvFiles = new System.Windows.Forms.ListView();
            this.chFiles = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblFiles = new System.Windows.Forms.Label();
            this.tabTasks = new System.Windows.Forms.TabPage();
            this.btnPerform = new System.Windows.Forms.Button();
            this.btnClose2 = new System.Windows.Forms.Button();
            this.scTasks = new System.Windows.Forms.SplitContainer();
            this.tvTasks = new System.Windows.Forms.TreeView();
            this.tabsPages.SuspendLayout();
            this.tabFiles.SuspendLayout();
            this.tabTasks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scTasks)).BeginInit();
            this.scTasks.Panel1.SuspendLayout();
            this.scTasks.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabsPages
            // 
            this.tabsPages.Controls.Add(this.tabFiles);
            this.tabsPages.Controls.Add(this.tabTasks);
            this.tabsPages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabsPages.Location = new System.Drawing.Point(0, 0);
            this.tabsPages.Name = "tabsPages";
            this.tabsPages.SelectedIndex = 0;
            this.tabsPages.Size = new System.Drawing.Size(642, 416);
            this.tabsPages.TabIndex = 1;
            // 
            // tabFiles
            // 
            this.tabFiles.BackColor = System.Drawing.SystemColors.Control;
            this.tabFiles.Controls.Add(this.btnClose);
            this.tabFiles.Controls.Add(this.btnSplitRemoveAll);
            this.tabFiles.Controls.Add(this.btnSplitInFileRemove);
            this.tabFiles.Controls.Add(this.btnSplitInFileAdd);
            this.tabFiles.Controls.Add(this.lvFiles);
            this.tabFiles.Controls.Add(this.lblFiles);
            this.tabFiles.Location = new System.Drawing.Point(4, 22);
            this.tabFiles.Name = "tabFiles";
            this.tabFiles.Padding = new System.Windows.Forms.Padding(3);
            this.tabFiles.Size = new System.Drawing.Size(634, 390);
            this.tabFiles.TabIndex = 0;
            this.tabFiles.Text = "Files";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(551, 359);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSplitRemoveAll
            // 
            this.btnSplitRemoveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSplitRemoveAll.Location = new System.Drawing.Point(470, 359);
            this.btnSplitRemoveAll.Name = "btnSplitRemoveAll";
            this.btnSplitRemoveAll.Size = new System.Drawing.Size(75, 23);
            this.btnSplitRemoveAll.TabIndex = 5;
            this.btnSplitRemoveAll.Text = "&Remove All";
            this.btnSplitRemoveAll.UseVisualStyleBackColor = true;
            this.btnSplitRemoveAll.Click += new System.EventHandler(this.btnSplitRemoveAll_Click);
            // 
            // btnSplitInFileRemove
            // 
            this.btnSplitInFileRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSplitInFileRemove.Location = new System.Drawing.Point(357, 359);
            this.btnSplitInFileRemove.Name = "btnSplitInFileRemove";
            this.btnSplitInFileRemove.Size = new System.Drawing.Size(107, 23);
            this.btnSplitInFileRemove.TabIndex = 4;
            this.btnSplitInFileRemove.Text = "Remove &Selected";
            this.btnSplitInFileRemove.UseVisualStyleBackColor = true;
            this.btnSplitInFileRemove.Click += new System.EventHandler(this.btnSplitInFileRemove_Click);
            // 
            // btnSplitInFileAdd
            // 
            this.btnSplitInFileAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSplitInFileAdd.Location = new System.Drawing.Point(8, 359);
            this.btnSplitInFileAdd.Name = "btnSplitInFileAdd";
            this.btnSplitInFileAdd.Size = new System.Drawing.Size(65, 23);
            this.btnSplitInFileAdd.TabIndex = 3;
            this.btnSplitInFileAdd.Text = "&Add...";
            this.btnSplitInFileAdd.UseVisualStyleBackColor = true;
            this.btnSplitInFileAdd.Click += new System.EventHandler(this.btnSplitInFileAdd_Click);
            this.btnSplitInFileAdd.Resize += new System.EventHandler(this.TMToolForm_Resize);
            // 
            // lvFiles
            // 
            this.lvFiles.AllowDrop = true;
            this.lvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFiles});
            this.lvFiles.FullRowSelect = true;
            this.lvFiles.GridLines = true;
            this.lvFiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvFiles.HideSelection = false;
            this.lvFiles.Location = new System.Drawing.Point(3, 24);
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(628, 329);
            this.lvFiles.TabIndex = 2;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            this.lvFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvFiles_DragDrop);
            this.lvFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvFiles_DragEnter);
            this.lvFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvFiles_KeyDown);
            // 
            // chFiles
            // 
            this.chFiles.Text = "Translation Memory files";
            this.chFiles.Width = 622;
            // 
            // lblFiles
            // 
            this.lblFiles.AutoSize = true;
            this.lblFiles.Location = new System.Drawing.Point(5, 8);
            this.lblFiles.Name = "lblFiles";
            this.lblFiles.Size = new System.Drawing.Size(197, 13);
            this.lblFiles.TabIndex = 1;
            this.lblFiles.Text = "Add Translation Memory files to &process:";
            // 
            // tabTasks
            // 
            this.tabTasks.BackColor = System.Drawing.SystemColors.Control;
            this.tabTasks.Controls.Add(this.btnPerform);
            this.tabTasks.Controls.Add(this.btnClose2);
            this.tabTasks.Controls.Add(this.scTasks);
            this.tabTasks.Location = new System.Drawing.Point(4, 22);
            this.tabTasks.Name = "tabTasks";
            this.tabTasks.Padding = new System.Windows.Forms.Padding(3);
            this.tabTasks.Size = new System.Drawing.Size(634, 390);
            this.tabTasks.TabIndex = 1;
            this.tabTasks.Text = "TM Tasks";
            // 
            // btnPerform
            // 
            this.btnPerform.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPerform.Location = new System.Drawing.Point(454, 359);
            this.btnPerform.Name = "btnPerform";
            this.btnPerform.Size = new System.Drawing.Size(91, 23);
            this.btnPerform.TabIndex = 9;
            this.btnPerform.Text = "&Perform Task";
            this.btnPerform.UseVisualStyleBackColor = true;
            this.btnPerform.Click += new System.EventHandler(this.btnPerform_Click);
            // 
            // btnClose2
            // 
            this.btnClose2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose2.Location = new System.Drawing.Point(551, 359);
            this.btnClose2.Name = "btnClose2";
            this.btnClose2.Size = new System.Drawing.Size(75, 23);
            this.btnClose2.TabIndex = 10;
            this.btnClose2.Text = "Close";
            this.btnClose2.UseVisualStyleBackColor = true;
            this.btnClose2.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // scTasks
            // 
            this.scTasks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scTasks.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.scTasks.ForeColor = System.Drawing.SystemColors.ControlText;
            this.scTasks.Location = new System.Drawing.Point(3, 3);
            this.scTasks.Name = "scTasks";
            // 
            // scTasks.Panel1
            // 
            this.scTasks.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.scTasks.Panel1.Controls.Add(this.tvTasks);
            this.scTasks.Panel1MinSize = 180;
            // 
            // scTasks.Panel2
            // 
            this.scTasks.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.scTasks.Panel2MinSize = 270;
            this.scTasks.Size = new System.Drawing.Size(628, 350);
            this.scTasks.SplitterDistance = 180;
            this.scTasks.TabIndex = 8;
            this.scTasks.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.scTasks_SplitterMoved);
            // 
            // tvTasks
            // 
            this.tvTasks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvTasks.HideSelection = false;
            this.tvTasks.Location = new System.Drawing.Point(0, 0);
            this.tvTasks.Name = "tvTasks";
            treeNode1.Name = "Node2";
            treeNode1.Text = "Remove Duplicates";
            treeNode2.Name = "Node1";
            treeNode2.Text = "Revert Index";
            treeNode3.Checked = true;
            treeNode3.Name = "Node0";
            treeNode3.NodeFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            treeNode3.Text = "TM Task  ";
            this.tvTasks.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3});
            this.tvTasks.ShowNodeToolTips = true;
            this.tvTasks.Size = new System.Drawing.Size(180, 350);
            this.tvTasks.TabIndex = 7;
            // 
            // TMToolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 416);
            this.Controls.Add(this.tabsPages);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(450, 350);
            this.Name = "TMToolForm";
            this.Text = "SDL Translation Memory Management 2018";
            this.Load += new System.EventHandler(this.TMToolForm_Load);
            this.Resize += new System.EventHandler(this.TMToolForm_Resize);
            this.tabsPages.ResumeLayout(false);
            this.tabFiles.ResumeLayout(false);
            this.tabFiles.PerformLayout();
            this.tabTasks.ResumeLayout(false);
            this.scTasks.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scTasks)).EndInit();
            this.scTasks.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabsPages;
		private System.Windows.Forms.TabPage tabFiles;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btnSplitRemoveAll;
		private System.Windows.Forms.Button btnSplitInFileRemove;
		private System.Windows.Forms.Button btnSplitInFileAdd;
		private System.Windows.Forms.ListView lvFiles;
		private System.Windows.Forms.ColumnHeader chFiles;
		private System.Windows.Forms.Label lblFiles;
		private System.Windows.Forms.TabPage tabTasks;
		private System.Windows.Forms.Button btnPerform;
		private System.Windows.Forms.Button btnClose2;
		private System.Windows.Forms.SplitContainer scTasks;
		private System.Windows.Forms.TreeView tvTasks;
	}
}