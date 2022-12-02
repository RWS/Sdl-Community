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
            this.fileName = new System.Windows.Forms.TextBox();
            this.browse = new System.Windows.Forms.Button();
            this.error = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.importStatus = new System.Windows.Forms.Label();
            this.importProgress = new System.Windows.Forms.ProgressBar();
            this.tryConnect = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.dbName = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.dbPasswordTip = new System.Windows.Forms.Label();
            this.dbPassword = new System.Windows.Forms.TextBox();
            this.dbPasswordLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.downloadCommunityServer = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dbConnection = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.timerImportProgress = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.Location = new System.Drawing.Point(649, 285);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(90, 23);
            this.cancel.TabIndex = 0;
            this.cancel.Text = "&Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // ok
            // 
            this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ok.Location = new System.Drawing.Point(553, 285);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(90, 23);
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
            // fileName
            // 
            this.fileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileName.Location = new System.Drawing.Point(96, 12);
            this.fileName.Name = "fileName";
            this.fileName.Size = new System.Drawing.Size(607, 20);
            this.fileName.TabIndex = 3;
            this.fileName.TextChanged += new System.EventHandler(this.fileName_TextChanged);
            // 
            // browse
            // 
            this.browse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browse.Location = new System.Drawing.Point(709, 10);
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
            this.error.Location = new System.Drawing.Point(93, 290);
            this.error.Name = "error";
            this.error.Size = new System.Drawing.Size(95, 13);
            this.error.TabIndex = 5;
            this.error.Text = "File name is invalid";
            this.error.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.importStatus);
            this.groupBox1.Controls.Add(this.importProgress);
            this.groupBox1.Controls.Add(this.tryConnect);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.dbName);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.dbPasswordTip);
            this.groupBox1.Controls.Add(this.dbPassword);
            this.groupBox1.Controls.Add(this.dbPasswordLabel);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.downloadCommunityServer);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.dbConnection);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(5, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(734, 221);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MongoDb Database";
            // 
            // importStatus
            // 
            this.importStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.importStatus.AutoSize = true;
            this.importStatus.Location = new System.Drawing.Point(91, 196);
            this.importStatus.Name = "importStatus";
            this.importStatus.Size = new System.Drawing.Size(41, 13);
            this.importStatus.TabIndex = 17;
            this.importStatus.Text = "label13";
            // 
            // importProgress
            // 
            this.importProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.importProgress.Location = new System.Drawing.Point(90, 190);
            this.importProgress.Name = "importProgress";
            this.importProgress.Size = new System.Drawing.Size(548, 23);
            this.importProgress.TabIndex = 16;
            this.importProgress.Visible = false;
            // 
            // tryConnect
            // 
            this.tryConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tryConnect.Location = new System.Drawing.Point(644, 190);
            this.tryConnect.Name = "tryConnect";
            this.tryConnect.Size = new System.Drawing.Size(84, 25);
            this.tryConnect.TabIndex = 15;
            this.tryConnect.Text = "&Try Connect";
            this.tryConnect.UseVisualStyleBackColor = true;
            this.tryConnect.Click += new System.EventHandler(this.tryConnect_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label12.Location = new System.Drawing.Point(288, 161);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(277, 13);
            this.label12.TabIndex = 14;
            this.label12.Text = "If you leave it empty, we\'ll name it based on the TMX File.";
            // 
            // dbName
            // 
            this.dbName.Location = new System.Drawing.Point(91, 158);
            this.dbName.Name = "dbName";
            this.dbName.Size = new System.Drawing.Size(191, 20);
            this.dbName.TabIndex = 13;
            this.dbName.TextChanged += new System.EventHandler(this.dbName_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 162);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(82, 13);
            this.label11.TabIndex = 12;
            this.label11.Text = "Database name";
            // 
            // dbPasswordTip
            // 
            this.dbPasswordTip.AutoSize = true;
            this.dbPasswordTip.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.dbPasswordTip.Location = new System.Drawing.Point(288, 123);
            this.dbPasswordTip.Name = "dbPasswordTip";
            this.dbPasswordTip.Size = new System.Drawing.Size(183, 13);
            this.dbPasswordTip.TabIndex = 11;
            this.dbPasswordTip.Text = "required only for AtlasDb connections";
            // 
            // dbPassword
            // 
            this.dbPassword.Location = new System.Drawing.Point(151, 120);
            this.dbPassword.Name = "dbPassword";
            this.dbPassword.PasswordChar = '*';
            this.dbPassword.Size = new System.Drawing.Size(131, 20);
            this.dbPassword.TabIndex = 10;
            this.dbPassword.TextChanged += new System.EventHandler(this.dbPassword_TextChanged);
            // 
            // dbPasswordLabel
            // 
            this.dbPasswordLabel.AutoSize = true;
            this.dbPasswordLabel.Location = new System.Drawing.Point(91, 123);
            this.dbPasswordLabel.Name = "dbPasswordLabel";
            this.dbPasswordLabel.Size = new System.Drawing.Size(53, 13);
            this.dbPasswordLabel.TabIndex = 9;
            this.dbPasswordLabel.Text = "Password";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label8.Location = new System.Drawing.Point(203, 98);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(466, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "mongodb+srv://user:<password>@cluster0.mbqowvc.mongodb.net/?retryWrites=true&&w=m" +
    "ajority";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label7.Location = new System.Drawing.Point(91, 98);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(103, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "AtlasDb connection:";
            // 
            // downloadCommunityServer
            // 
            this.downloadCommunityServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadCommunityServer.AutoSize = true;
            this.downloadCommunityServer.Location = new System.Drawing.Point(555, 81);
            this.downloadCommunityServer.Name = "downloadCommunityServer";
            this.downloadCommunityServer.Size = new System.Drawing.Size(143, 13);
            this.downloadCommunityServer.TabIndex = 6;
            this.downloadCommunityServer.TabStop = true;
            this.downloadCommunityServer.Tag = "https://www.mongodb.com/try/download/community";
            this.downloadCommunityServer.Text = "Download Community Server";
            this.downloadCommunityServer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.downloadCommunityServer_LinkClicked);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label6.Location = new System.Drawing.Point(91, 81);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Local connection:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label5.Location = new System.Drawing.Point(200, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "localhost:27017";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label4.Location = new System.Drawing.Point(27, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Examples:";
            // 
            // dbConnection
            // 
            this.dbConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dbConnection.Location = new System.Drawing.Point(91, 53);
            this.dbConnection.Name = "dbConnection";
            this.dbConnection.Size = new System.Drawing.Size(607, 20);
            this.dbConnection.TabIndex = 2;
            this.dbConnection.TextChanged += new System.EventHandler(this.dbConnection_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Connection";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label2.Location = new System.Drawing.Point(7, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(325, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "We\'re importing the TMX Texts, for insanely fast search capabilities.";
            // 
            // timerImportProgress
            // 
            this.timerImportProgress.Interval = 500;
            this.timerImportProgress.Tick += new System.EventHandler(this.timerImportProgress_Tick);
            // 
            // TmxOptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(745, 315);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.error);
            this.Controls.Add(this.browse);
            this.Controls.Add(this.fileName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.cancel);
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
        private System.Windows.Forms.TextBox fileName;
        private System.Windows.Forms.Button browse;
        private System.Windows.Forms.Label error;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox dbConnection;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.LinkLabel downloadCommunityServer;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox dbName;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label dbPasswordTip;
        private System.Windows.Forms.TextBox dbPassword;
        private System.Windows.Forms.Label dbPasswordLabel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label importStatus;
        private System.Windows.Forms.ProgressBar importProgress;
        private System.Windows.Forms.Button tryConnect;
        private System.Windows.Forms.Timer timerImportProgress;
    }
}