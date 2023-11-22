namespace TMX_TranslationProvider
{
	partial class TmxOptionsForm
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
            this.cancel = new System.Windows.Forms.Button();
            this.ok = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.browse = new System.Windows.Forms.Button();
            this.error = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.viewReport = new System.Windows.Forms.Button();
            this.quickImport = new System.Windows.Forms.CheckBox();
            this.importStatus = new System.Windows.Forms.Label();
            this.importProgress = new System.Windows.Forms.ProgressBar();
            this.tryConnect = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.dbConnection = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.downloadCommunityServer = new System.Windows.Forms.LinkLabel();
            this.timerImportProgress = new System.Windows.Forms.Timer(this.components);
            this.viewLog = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.exportToTmx = new System.Windows.Forms.Button();
            this.dbNames = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.Location = new System.Drawing.Point(640, 185);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(90, 28);
            this.cancel.TabIndex = 0;
            this.cancel.Text = "&Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // ok
            // 
            this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ok.Location = new System.Drawing.Point(544, 185);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(90, 28);
            this.ok.TabIndex = 1;
            this.ok.Text = "&OK";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "TMX File";
            // 
            // browse
            // 
            this.browse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browse.Location = new System.Drawing.Point(700, 10);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(30, 23);
            this.browse.TabIndex = 4;
            this.browse.Text = "...";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.browse_Click);
            // 
            // error
            // 
            this.error.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.error.AutoSize = true;
            this.error.ForeColor = System.Drawing.Color.Red;
            this.error.Location = new System.Drawing.Point(93, 196);
            this.error.Name = "error";
            this.error.Size = new System.Drawing.Size(95, 13);
            this.error.TabIndex = 5;
            this.error.Text = "File name is invalid";
            this.error.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.viewReport);
            this.groupBox1.Controls.Add(this.quickImport);
            this.groupBox1.Controls.Add(this.importStatus);
            this.groupBox1.Controls.Add(this.importProgress);
            this.groupBox1.Controls.Add(this.tryConnect);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dbConnection);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(5, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(725, 120);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MongoDb Database";
            // 
            // viewReport
            // 
            this.viewReport.Enabled = false;
            this.viewReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.viewReport.Location = new System.Drawing.Point(546, 83);
            this.viewReport.Name = "viewReport";
            this.viewReport.Size = new System.Drawing.Size(84, 25);
            this.viewReport.TabIndex = 19;
            this.viewReport.Text = "&View Report";
            this.toolTip1.SetToolTip(this.viewReport, "Try to connect to this database.  Assuming the connection works, we\'ll automatica" +
        "lly import your TMX file");
            this.viewReport.UseVisualStyleBackColor = true;
            this.viewReport.Click += new System.EventHandler(this.viewReport_Click);
            // 
            // quickImport
            // 
            this.quickImport.AutoSize = true;
            this.quickImport.Location = new System.Drawing.Point(603, 61);
            this.quickImport.Name = "quickImport";
            this.quickImport.Size = new System.Drawing.Size(86, 17);
            this.quickImport.TabIndex = 18;
            this.quickImport.Text = "Quick Import";
            this.toolTip1.SetToolTip(this.quickImport, "If selected, we\'ll only be importing a small portion of your file, so you can qui" +
        "ckly test it");
            this.quickImport.UseVisualStyleBackColor = true;
            // 
            // importStatus
            // 
            this.importStatus.AutoSize = true;
            this.importStatus.Location = new System.Drawing.Point(92, 89);
            this.importStatus.Name = "importStatus";
            this.importStatus.Size = new System.Drawing.Size(41, 13);
            this.importStatus.TabIndex = 17;
            this.importStatus.Text = "label13";
            // 
            // importProgress
            // 
            this.importProgress.Location = new System.Drawing.Point(91, 83);
            this.importProgress.Name = "importProgress";
            this.importProgress.Size = new System.Drawing.Size(449, 23);
            this.importProgress.TabIndex = 16;
            this.importProgress.Visible = false;
            // 
            // tryConnect
            // 
            this.tryConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tryConnect.Location = new System.Drawing.Point(636, 83);
            this.tryConnect.Name = "tryConnect";
            this.tryConnect.Size = new System.Drawing.Size(84, 25);
            this.tryConnect.TabIndex = 15;
            this.tryConnect.Text = "&Try Connect";
            this.toolTip1.SetToolTip(this.tryConnect, "Try to connect to this database.  Assuming the connection works, we\'ll automatica" +
        "lly import your TMX file");
            this.tryConnect.UseVisualStyleBackColor = true;
            this.tryConnect.Click += new System.EventHandler(this.tryConnect_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label2.Location = new System.Drawing.Point(7, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(325, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "We\'re importing the TMX Texts, for insanely fast search capabilities.";
            // 
            // dbConnection
            // 
            this.dbConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dbConnection.Enabled = false;
            this.dbConnection.Location = new System.Drawing.Point(91, 35);
            this.dbConnection.Name = "dbConnection";
            this.dbConnection.Size = new System.Drawing.Size(598, 20);
            this.dbConnection.TabIndex = 2;
            this.dbConnection.Text = "localhost:27017";
            this.dbConnection.TextChanged += new System.EventHandler(this.dbConnection_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Connection";
            // 
            // downloadCommunityServer
            // 
            this.downloadCommunityServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadCommunityServer.AutoSize = true;
            this.downloadCommunityServer.Location = new System.Drawing.Point(551, 37);
            this.downloadCommunityServer.Name = "downloadCommunityServer";
            this.downloadCommunityServer.Size = new System.Drawing.Size(143, 13);
            this.downloadCommunityServer.TabIndex = 6;
            this.downloadCommunityServer.TabStop = true;
            this.downloadCommunityServer.Tag = "https://www.mongodb.com/try/download/community";
            this.downloadCommunityServer.Text = "Download Community Server";
            this.toolTip1.SetToolTip(this.downloadCommunityServer, "Click to Download the kit to install MongoDB locally");
            this.downloadCommunityServer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.downloadCommunityServer_LinkClicked);
            // 
            // timerImportProgress
            // 
            this.timerImportProgress.Interval = 500;
            this.timerImportProgress.Tick += new System.EventHandler(this.timerImportProgress_Tick);
            // 
            // viewLog
            // 
            this.viewLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.viewLog.Image = global::TMX_TranslationProvider.PluginResources.wrench;
            this.viewLog.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.viewLog.Location = new System.Drawing.Point(5, 185);
            this.viewLog.Margin = new System.Windows.Forms.Padding(0);
            this.viewLog.Name = "viewLog";
            this.viewLog.Size = new System.Drawing.Size(28, 28);
            this.viewLog.TabIndex = 7;
            this.toolTip1.SetToolTip(this.viewLog, "View Log");
            this.viewLog.UseVisualStyleBackColor = true;
            this.viewLog.Click += new System.EventHandler(this.viewLog_Click);
            // 
            // exportToTmx
            // 
            this.exportToTmx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exportToTmx.Location = new System.Drawing.Point(448, 185);
            this.exportToTmx.Name = "exportToTmx";
            this.exportToTmx.Size = new System.Drawing.Size(90, 28);
            this.exportToTmx.TabIndex = 8;
            this.exportToTmx.Text = "&Export To TMX";
            this.toolTip1.SetToolTip(this.exportToTmx, "Exports this database back to a .tmx file");
            this.exportToTmx.UseVisualStyleBackColor = true;
            this.exportToTmx.Click += new System.EventHandler(this.exportToTmx_Click);
            // 
            // dbNames
            // 
            this.dbNames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dbNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dbNames.FormattingEnabled = true;
            this.dbNames.Location = new System.Drawing.Point(67, 10);
            this.dbNames.Name = "dbNames";
            this.dbNames.Size = new System.Drawing.Size(627, 21);
            this.dbNames.TabIndex = 9;
            this.dbNames.SelectedIndexChanged += new System.EventHandler(this.dbNames_SelectedIndexChanged);
            // 
            // TmxOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 221);
            this.Controls.Add(this.dbNames);
            this.Controls.Add(this.exportToTmx);
            this.Controls.Add(this.viewLog);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.error);
            this.Controls.Add(this.browse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.downloadCommunityServer);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TmxOptionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TMX Translation Provider Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button browse;
        private System.Windows.Forms.Label error;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox dbConnection;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel downloadCommunityServer;
        private System.Windows.Forms.Label importStatus;
        private System.Windows.Forms.ProgressBar importProgress;
        private System.Windows.Forms.Button tryConnect;
        private System.Windows.Forms.Timer timerImportProgress;
        private System.Windows.Forms.Button viewLog;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox quickImport;
        private System.Windows.Forms.Button viewReport;
        private System.Windows.Forms.Button exportToTmx;
        private System.Windows.Forms.ComboBox dbNames;
    }
}