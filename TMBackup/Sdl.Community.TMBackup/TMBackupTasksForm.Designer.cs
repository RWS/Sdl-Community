namespace Sdl.Community.TMBackup
{
	partial class TMBackupTasksForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TMBackupTasksForm));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.TaskNameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TaskRunType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastRunCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NextRunCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IntervalCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolTip_Informative = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton_File = new System.Windows.Forms.ToolStripDropDownButton();
            this.createNewBackupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startManualTasksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startDisabledTasksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowsTaskSchedulerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_CreateTask = new System.Windows.Forms.ToolStripButton();
            this.btn_RunManuallyTasks = new System.Windows.Forms.ToolStripButton();
            this.btn_RunDisabledTasks = new System.Windows.Forms.ToolStripButton();
            this.btn_RefreshView = new System.Windows.Forms.ToolStripButton();
            this.btn_OpenWindowsTaskScheduler = new System.Windows.Forms.ToolStripButton();
            this.btn_Help = new System.Windows.Forms.ToolStripButton();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TaskNameCol,
            this.TaskRunType,
            this.Status,
            this.LastRunCol,
            this.NextRunCol,
            this.IntervalCol});
            this.dataGridView1.Location = new System.Drawing.Point(2, 27);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(885, 372);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            this.dataGridView1.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseUp);
            // 
            // TaskNameCol
            // 
            this.TaskNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.TaskNameCol.DataPropertyName = "TaskName";
            this.TaskNameCol.HeaderText = "Task name";
            this.TaskNameCol.Name = "TaskNameCol";
            this.TaskNameCol.ReadOnly = true;
            this.TaskNameCol.Width = 85;
            // 
            // TaskRunType
            // 
            this.TaskRunType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.TaskRunType.DataPropertyName = "TaskRunType";
            this.TaskRunType.HeaderText = "Task run type";
            this.TaskRunType.Name = "TaskRunType";
            this.TaskRunType.ReadOnly = true;
            this.TaskRunType.Width = 97;
            // 
            // Status
            // 
            this.Status.DataPropertyName = "Status";
            this.Status.HeaderText = "Status";
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            // 
            // LastRunCol
            // 
            this.LastRunCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.LastRunCol.DataPropertyName = "LastRun";
            this.LastRunCol.HeaderText = "Last run";
            this.LastRunCol.Name = "LastRunCol";
            this.LastRunCol.ReadOnly = true;
            this.LastRunCol.Width = 70;
            // 
            // NextRunCol
            // 
            this.NextRunCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.NextRunCol.DataPropertyName = "NextRun";
            this.NextRunCol.HeaderText = "Next run";
            this.NextRunCol.Name = "NextRunCol";
            this.NextRunCol.ReadOnly = true;
            this.NextRunCol.Width = 72;
            // 
            // IntervalCol
            // 
            this.IntervalCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.IntervalCol.DataPropertyName = "Interval";
            this.IntervalCol.HeaderText = "Interval";
            this.IntervalCol.Name = "IntervalCol";
            this.IntervalCol.ReadOnly = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ContextMenuStrip = this.contextMenuStrip1;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton_File,
            this.btn_CreateTask,
            this.btn_RunManuallyTasks,
            this.btn_RunDisabledTasks,
            this.btn_RefreshView,
            this.btn_OpenWindowsTaskScheduler,
            this.btn_Help});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(887, 25);
            this.toolStrip1.TabIndex = 8;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton_File
            // 
            this.toolStripDropDownButton_File.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createNewBackupToolStripMenuItem,
            this.startManualTasksToolStripMenuItem,
            this.startDisabledTasksToolStripMenuItem,
            this.refreshViewToolStripMenuItem,
            this.windowsTaskSchedulerToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.toolStripDropDownButton_File.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripDropDownButton_File.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton_File.Name = "toolStripDropDownButton_File";
            this.toolStripDropDownButton_File.Size = new System.Drawing.Size(39, 22);
            this.toolStripDropDownButton_File.Text = "File";
            // 
            // createNewBackupToolStripMenuItem
            // 
            this.createNewBackupToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.createNewBackupToolStripMenuItem.Name = "createNewBackupToolStripMenuItem";
            this.createNewBackupToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.createNewBackupToolStripMenuItem.Text = "Create New Backup";
            this.createNewBackupToolStripMenuItem.Click += new System.EventHandler(this.btn_CreateTask_Click);
            // 
            // startManualTasksToolStripMenuItem
            // 
            this.startManualTasksToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.startManualTasksToolStripMenuItem.Name = "startManualTasksToolStripMenuItem";
            this.startManualTasksToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.startManualTasksToolStripMenuItem.Text = "Start Manual Tasks";
            this.startManualTasksToolStripMenuItem.Click += new System.EventHandler(this.btn_RunManuallyTasks_Click);
            // 
            // startDisabledTasksToolStripMenuItem
            // 
            this.startDisabledTasksToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.startDisabledTasksToolStripMenuItem.Name = "startDisabledTasksToolStripMenuItem";
            this.startDisabledTasksToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.startDisabledTasksToolStripMenuItem.Text = "Start Disabled Tasks";
            this.startDisabledTasksToolStripMenuItem.Click += new System.EventHandler(this.btn_RunDisabledTasks_Click);
            // 
            // refreshViewToolStripMenuItem
            // 
            this.refreshViewToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.refreshViewToolStripMenuItem.Name = "refreshViewToolStripMenuItem";
            this.refreshViewToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.refreshViewToolStripMenuItem.Text = "Refresh View";
            this.refreshViewToolStripMenuItem.Click += new System.EventHandler(this.btn_RefreshView_Click);
            // 
            // windowsTaskSchedulerToolStripMenuItem
            // 
            this.windowsTaskSchedulerToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.windowsTaskSchedulerToolStripMenuItem.Name = "windowsTaskSchedulerToolStripMenuItem";
            this.windowsTaskSchedulerToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.windowsTaskSchedulerToolStripMenuItem.Text = "Windows Task Scheduler";
            this.windowsTaskSchedulerToolStripMenuItem.Click += new System.EventHandler(this.btn_OpenWindowsTaskScheduler_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.helpToolStripMenuItem.Text = "Help";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.btn_Help_Click);
            // 
            // btn_CreateTask
            // 
            this.btn_CreateTask.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_CreateTask.Image = ((System.Drawing.Image)(resources.GetObject("btn_CreateTask.Image")));
            this.btn_CreateTask.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_CreateTask.Name = "btn_CreateTask";
            this.btn_CreateTask.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.btn_CreateTask.Size = new System.Drawing.Size(23, 22);
            this.btn_CreateTask.Text = "Create New Task";
            this.btn_CreateTask.Click += new System.EventHandler(this.btn_CreateTask_Click);
            // 
            // btn_RunManuallyTasks
            // 
            this.btn_RunManuallyTasks.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_RunManuallyTasks.Image = ((System.Drawing.Image)(resources.GetObject("btn_RunManuallyTasks.Image")));
            this.btn_RunManuallyTasks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_RunManuallyTasks.Name = "btn_RunManuallyTasks";
            this.btn_RunManuallyTasks.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.btn_RunManuallyTasks.Size = new System.Drawing.Size(32, 22);
            this.btn_RunManuallyTasks.Text = "Run Manual Tasks";
            this.btn_RunManuallyTasks.Click += new System.EventHandler(this.btn_RunManuallyTasks_Click);
            // 
            // btn_RunDisabledTasks
            // 
            this.btn_RunDisabledTasks.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_RunDisabledTasks.Image = ((System.Drawing.Image)(resources.GetObject("btn_RunDisabledTasks.Image")));
            this.btn_RunDisabledTasks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_RunDisabledTasks.Name = "btn_RunDisabledTasks";
            this.btn_RunDisabledTasks.Padding = new System.Windows.Forms.Padding(0, 0, 9, 0);
            this.btn_RunDisabledTasks.Size = new System.Drawing.Size(29, 22);
            this.btn_RunDisabledTasks.Text = "Run Disabled Tasks";
            this.btn_RunDisabledTasks.Click += new System.EventHandler(this.btn_RunDisabledTasks_Click);
            // 
            // btn_RefreshView
            // 
            this.btn_RefreshView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_RefreshView.Image = ((System.Drawing.Image)(resources.GetObject("btn_RefreshView.Image")));
            this.btn_RefreshView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_RefreshView.Name = "btn_RefreshView";
            this.btn_RefreshView.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.btn_RefreshView.Size = new System.Drawing.Size(30, 22);
            this.btn_RefreshView.Text = "Refresh View";
            this.btn_RefreshView.Click += new System.EventHandler(this.btn_RefreshView_Click);
            // 
            // btn_OpenWindowsTaskScheduler
            // 
            this.btn_OpenWindowsTaskScheduler.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_OpenWindowsTaskScheduler.Image = ((System.Drawing.Image)(resources.GetObject("btn_OpenWindowsTaskScheduler.Image")));
            this.btn_OpenWindowsTaskScheduler.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_OpenWindowsTaskScheduler.Name = "btn_OpenWindowsTaskScheduler";
            this.btn_OpenWindowsTaskScheduler.Padding = new System.Windows.Forms.Padding(0, 0, 11, 0);
            this.btn_OpenWindowsTaskScheduler.Size = new System.Drawing.Size(31, 22);
            this.btn_OpenWindowsTaskScheduler.Text = "Open Windows Task Scheduler";
            this.btn_OpenWindowsTaskScheduler.Click += new System.EventHandler(this.btn_OpenWindowsTaskScheduler_Click);
            // 
            // btn_Help
            // 
            this.btn_Help.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn_Help.Image = ((System.Drawing.Image)(resources.GetObject("btn_Help.Image")));
            this.btn_Help.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn_Help.Name = "btn_Help";
            this.btn_Help.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.btn_Help.Size = new System.Drawing.Size(32, 22);
            this.btn_Help.Text = "Help";
            this.btn_Help.Click += new System.EventHandler(this.btn_Help_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 48);
            this.contextMenuStrip1.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // TMBackupTasksForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(887, 400);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TMBackupTasksForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TM Backup Tasks";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.ToolTip toolTip_Informative;
		private System.Windows.Forms.DataGridViewTextBoxColumn TaskNameCol;
		private System.Windows.Forms.DataGridViewTextBoxColumn TaskRunType;
		private System.Windows.Forms.DataGridViewTextBoxColumn Status;
		private System.Windows.Forms.DataGridViewTextBoxColumn LastRunCol;
		private System.Windows.Forms.DataGridViewTextBoxColumn NextRunCol;
		private System.Windows.Forms.DataGridViewTextBoxColumn IntervalCol;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton btn_CreateTask;
		private System.Windows.Forms.ToolStripButton btn_RunManuallyTasks;
		private System.Windows.Forms.ToolStripButton btn_RunDisabledTasks;
		private System.Windows.Forms.ToolStripButton btn_RefreshView;
		private System.Windows.Forms.ToolStripButton btn_OpenWindowsTaskScheduler;
		private System.Windows.Forms.ToolStripButton btn_Help;
		private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton_File;
		private System.Windows.Forms.ToolStripMenuItem createNewBackupToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem startManualTasksToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem startDisabledTasksToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem refreshViewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem windowsTaskSchedulerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
	}
}