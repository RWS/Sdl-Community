using System.ComponentModel;

namespace Sdl.Community.TMLifting
{
    partial class TMLiftingForm
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
            this.tabPageFileBasedTM = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rtbStatus = new System.Windows.Forms.RichTextBox();
            this.lstTms = new System.Windows.Forms.ListBox();
            this.tabControlTMLifting = new System.Windows.Forms.TabControl();
            this.tabPageServerBasedTM = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxTM = new System.Windows.Forms.GroupBox();
            this.gridServerBasedTMs = new System.Windows.Forms.DataGridView();
            this.connectToServerBtn = new System.Windows.Forms.Button();
            this.server = new System.Windows.Forms.Label();
            this.comboBoxServerBasedTM = new System.Windows.Forms.ComboBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.chkLoadStudioTMs = new System.Windows.Forms.CheckBox();
            this.reIndexCheckBox = new System.Windows.Forms.CheckBox();
            this.upLiftCheckBox = new System.Windows.Forms.CheckBox();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.cleanBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnReindex = new System.Windows.Forms.Button();
            this.tabPageFileBasedTM.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControlTMLifting.SuspendLayout();
            this.tabPageServerBasedTM.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxTM.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridServerBasedTMs)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPageFileBasedTM
            // 
            this.tabPageFileBasedTM.Controls.Add(this.panel2);
            this.tabPageFileBasedTM.Location = new System.Drawing.Point(4, 22);
            this.tabPageFileBasedTM.Name = "tabPageFileBasedTM";
            this.tabPageFileBasedTM.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFileBasedTM.Size = new System.Drawing.Size(1132, 583);
            this.tabPageFileBasedTM.TabIndex = 0;
            this.tabPageFileBasedTM.Text = "File-Based Translation Memories";
            this.tabPageFileBasedTM.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rtbStatus);
            this.panel2.Controls.Add(this.lstTms);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1126, 577);
            this.panel2.TabIndex = 3;
            // 
            // rtbStatus
            // 
            this.rtbStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbStatus.Location = new System.Drawing.Point(0, 186);
            this.rtbStatus.Name = "rtbStatus";
            this.rtbStatus.Size = new System.Drawing.Size(1126, 391);
            this.rtbStatus.TabIndex = 1;
            this.rtbStatus.Text = "";
            // 
            // lstTms
            // 
            this.lstTms.AllowDrop = true;
            this.lstTms.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstTms.FormattingEnabled = true;
            this.lstTms.Location = new System.Drawing.Point(0, 0);
            this.lstTms.Name = "lstTms";
            this.lstTms.Size = new System.Drawing.Size(1126, 186);
            this.lstTms.TabIndex = 0;
            this.lstTms.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstTms_DragDrop);
            this.lstTms.DragOver += new System.Windows.Forms.DragEventHandler(this.lstTms_DragOver);
            // 
            // tabControlTMLifting
            // 
            this.tabControlTMLifting.Controls.Add(this.tabPageFileBasedTM);
            this.tabControlTMLifting.Controls.Add(this.tabPageServerBasedTM);
            this.tabControlTMLifting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlTMLifting.Location = new System.Drawing.Point(0, 0);
            this.tabControlTMLifting.Name = "tabControlTMLifting";
            this.tabControlTMLifting.SelectedIndex = 0;
            this.tabControlTMLifting.Size = new System.Drawing.Size(1140, 609);
            this.tabControlTMLifting.TabIndex = 0;
            // 
            // tabPageServerBasedTM
            // 
            this.tabPageServerBasedTM.BackColor = System.Drawing.Color.White;
            this.tabPageServerBasedTM.Controls.Add(this.tableLayoutPanel1);
            this.tabPageServerBasedTM.Location = new System.Drawing.Point(4, 22);
            this.tabPageServerBasedTM.Name = "tabPageServerBasedTM";
            this.tabPageServerBasedTM.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageServerBasedTM.Size = new System.Drawing.Size(1132, 583);
            this.tabPageServerBasedTM.TabIndex = 1;
            this.tabPageServerBasedTM.Text = "Server-Based Translation Memories";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.groupBoxTM, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.connectToServerBtn, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.server, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxServerBasedTM, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.1194F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 83.8806F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1126, 577);
            this.tableLayoutPanel1.TabIndex = 13;
            // 
            // groupBoxTM
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBoxTM, 3);
            this.groupBoxTM.Controls.Add(this.gridServerBasedTMs);
            this.groupBoxTM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxTM.Location = new System.Drawing.Point(3, 96);
            this.groupBoxTM.Name = "groupBoxTM";
            this.groupBoxTM.Size = new System.Drawing.Size(1120, 478);
            this.groupBoxTM.TabIndex = 13;
            this.groupBoxTM.TabStop = false;
            this.groupBoxTM.Text = "Translation Memories";
            // 
            // gridServerBasedTMs
            // 
            this.gridServerBasedTMs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridServerBasedTMs.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridServerBasedTMs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridServerBasedTMs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridServerBasedTMs.Location = new System.Drawing.Point(3, 16);
            this.gridServerBasedTMs.Name = "gridServerBasedTMs";
            this.gridServerBasedTMs.Size = new System.Drawing.Size(1114, 459);
            this.gridServerBasedTMs.TabIndex = 6;
            this.gridServerBasedTMs.Visible = false;
            this.gridServerBasedTMs.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridServerBasedTMs_CellContentClick);
            this.gridServerBasedTMs.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.gridServerBasedTMs_RowStateChanged);
            // 
            // connectToServerBtn
            // 
            this.connectToServerBtn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.connectToServerBtn.Location = new System.Drawing.Point(564, 35);
            this.connectToServerBtn.Name = "connectToServerBtn";
            this.connectToServerBtn.Size = new System.Drawing.Size(75, 23);
            this.connectToServerBtn.TabIndex = 10;
            this.connectToServerBtn.Text = "Connect";
            this.connectToServerBtn.UseVisualStyleBackColor = true;
            this.connectToServerBtn.Click += new System.EventHandler(this.connectToServer_Click);
            // 
            // server
            // 
            this.server.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.server.AutoSize = true;
            this.server.Location = new System.Drawing.Point(3, 40);
            this.server.Name = "server";
            this.server.Size = new System.Drawing.Size(41, 13);
            this.server.TabIndex = 12;
            this.server.Text = "Server:";
            // 
            // comboBoxServerBasedTM
            // 
            this.comboBoxServerBasedTM.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.comboBoxServerBasedTM.BackColor = System.Drawing.SystemColors.Window;
            this.comboBoxServerBasedTM.Location = new System.Drawing.Point(50, 36);
            this.comboBoxServerBasedTM.Name = "comboBoxServerBasedTM";
            this.comboBoxServerBasedTM.Size = new System.Drawing.Size(508, 21);
            this.comboBoxServerBasedTM.TabIndex = 11;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(12, 6);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse ...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // chkLoadStudioTMs
            // 
            this.chkLoadStudioTMs.AutoSize = true;
            this.chkLoadStudioTMs.Location = new System.Drawing.Point(94, 11);
            this.chkLoadStudioTMs.Name = "chkLoadStudioTMs";
            this.chkLoadStudioTMs.Size = new System.Drawing.Size(107, 17);
            this.chkLoadStudioTMs.TabIndex = 2;
            this.chkLoadStudioTMs.Text = "Load Studio TMs";
            this.chkLoadStudioTMs.UseVisualStyleBackColor = true;
            this.chkLoadStudioTMs.CheckedChanged += new System.EventHandler(this.chkLoadStudioTMs_CheckedChanged);
            // 
            // reIndexCheckBox
            // 
            this.reIndexCheckBox.AutoSize = true;
            this.reIndexCheckBox.Location = new System.Drawing.Point(213, 10);
            this.reIndexCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.reIndexCheckBox.Name = "reIndexCheckBox";
            this.reIndexCheckBox.Size = new System.Drawing.Size(69, 17);
            this.reIndexCheckBox.TabIndex = 3;
            this.reIndexCheckBox.Text = "Re-Index";
            this.reIndexCheckBox.UseVisualStyleBackColor = true;
            // 
            // upLiftCheckBox
            // 
            this.upLiftCheckBox.AutoSize = true;
            this.upLiftCheckBox.Location = new System.Drawing.Point(299, 10);
            this.upLiftCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.upLiftCheckBox.Name = "upLiftCheckBox";
            this.upLiftCheckBox.Size = new System.Drawing.Size(50, 17);
            this.upLiftCheckBox.TabIndex = 4;
            this.upLiftCheckBox.Text = "Uplift";
            this.upLiftCheckBox.UseVisualStyleBackColor = true;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.Enabled = false;
            this.cancelBtn.Location = new System.Drawing.Point(1007, 16);
            this.cancelBtn.Margin = new System.Windows.Forms.Padding(2);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(60, 23);
            this.cancelBtn.TabIndex = 5;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // cleanBtn
            // 
            this.cleanBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cleanBtn.Location = new System.Drawing.Point(947, 16);
            this.cleanBtn.Margin = new System.Windows.Forms.Padding(2);
            this.cleanBtn.Name = "cleanBtn";
            this.cleanBtn.Size = new System.Drawing.Size(60, 23);
            this.cleanBtn.TabIndex = 6;
            this.cleanBtn.Text = "Clean";
            this.cleanBtn.UseVisualStyleBackColor = true;
            this.cleanBtn.Click += new System.EventHandler(this.cleanBtn_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cleanBtn);
            this.panel1.Controls.Add(this.cancelBtn);
            this.panel1.Controls.Add(this.upLiftCheckBox);
            this.panel1.Controls.Add(this.reIndexCheckBox);
            this.panel1.Controls.Add(this.chkLoadStudioTMs);
            this.panel1.Controls.Add(this.btnBrowse);
            this.panel1.Controls.Add(this.btnReindex);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 609);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1140, 46);
            this.panel1.TabIndex = 3;
            // 
            // btnReindex
            // 
            this.btnReindex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReindex.Enabled = false;
            this.btnReindex.Location = new System.Drawing.Point(1067, 16);
            this.btnReindex.Name = "btnReindex";
            this.btnReindex.Size = new System.Drawing.Size(60, 23);
            this.btnReindex.TabIndex = 0;
            this.btnReindex.Text = "Process";
            this.btnReindex.UseVisualStyleBackColor = true;
            this.btnReindex.Click += new System.EventHandler(this.btnReindex_Click);
            // 
            // TMLiftingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControlTMLifting);
            this.Controls.Add(this.panel1);
            this.Name = "TMLiftingForm";
            this.Size = new System.Drawing.Size(1140, 655);
            this.tabPageFileBasedTM.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabControlTMLifting.ResumeLayout(false);
            this.tabPageServerBasedTM.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBoxTM.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridServerBasedTMs)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

		#endregion
		private System.Windows.Forms.TabPage tabPageFileBasedTM;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.RichTextBox rtbStatus;
		private System.Windows.Forms.ListBox lstTms;
		private System.Windows.Forms.TabControl tabControlTMLifting;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.CheckBox chkLoadStudioTMs;
		private System.Windows.Forms.CheckBox reIndexCheckBox;
		private System.Windows.Forms.CheckBox upLiftCheckBox;
		private System.Windows.Forms.Button cancelBtn;
		private System.Windows.Forms.Button cleanBtn;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TabPage tabPageServerBasedTM;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox groupBoxTM;
		private System.Windows.Forms.DataGridView gridServerBasedTMs;
		private System.Windows.Forms.Button connectToServerBtn;
		private System.Windows.Forms.Label server;
		private System.Windows.Forms.ComboBox comboBoxServerBasedTM;
		private System.Windows.Forms.Button btnReindex;
	}
}

