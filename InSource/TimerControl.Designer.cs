using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Sdl.Community.InSource
{
    partial class TimerControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;


        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.timerCheckBox = new System.Windows.Forms.CheckBox();
            this.remainingTimeLbl = new System.Windows.Forms.Label();
            this.refreshIntervalTable = new System.Windows.Forms.TableLayoutPanel();
            this.intervalTextBox = new System.Windows.Forms.TextBox();
            this.refreshIntervalLbl = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.deleteBtn = new System.Windows.Forms.RadioButton();
            this.archiveBtn = new System.Windows.Forms.RadioButton();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.mainPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.refreshIntervalTable.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.White;
            this.mainPanel.ColumnCount = 1;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400F));
            this.mainPanel.Controls.Add(this.groupBox2, 0, 0);
            this.mainPanel.Controls.Add(this.groupBox1, 0, 1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.RowCount = 3;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainPanel.Size = new System.Drawing.Size(400, 480);
            this.mainPanel.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tableLayoutPanel1);
            this.groupBox2.Location = new System.Drawing.Point(10, 3);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(10, 3, 10, 10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(285, 100);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Timer management";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.timerCheckBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.remainingTimeLbl, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.refreshIntervalTable, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(279, 81);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // timerCheckBox
            // 
            this.timerCheckBox.AutoSize = true;
            this.timerCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.timerCheckBox.Location = new System.Drawing.Point(3, 10);
            this.timerCheckBox.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.timerCheckBox.Name = "timerCheckBox";
            this.timerCheckBox.Size = new System.Drawing.Size(273, 17);
            this.timerCheckBox.TabIndex = 0;
            this.timerCheckBox.Text = "Enable timer for checking project request ";
            this.timerCheckBox.UseVisualStyleBackColor = true;
            this.timerCheckBox.CheckedChanged += new System.EventHandler(this.timerCheckBox_CheckedChanged);
            // 
            // remainingTimeLbl
            // 
            this.remainingTimeLbl.AutoSize = true;
            this.remainingTimeLbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.remainingTimeLbl.Location = new System.Drawing.Point(3, 60);
            this.remainingTimeLbl.Name = "remainingTimeLbl";
            this.remainingTimeLbl.Size = new System.Drawing.Size(273, 21);
            this.remainingTimeLbl.TabIndex = 3;
            this.remainingTimeLbl.Text = "remainingTimeLabel";
            // 
            // refreshIntervalTable
            // 
            this.refreshIntervalTable.ColumnCount = 2;
            this.refreshIntervalTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.refreshIntervalTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.refreshIntervalTable.Controls.Add(this.intervalTextBox, 1, 0);
            this.refreshIntervalTable.Controls.Add(this.refreshIntervalLbl, 0, 0);
            this.refreshIntervalTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.refreshIntervalTable.Location = new System.Drawing.Point(3, 33);
            this.refreshIntervalTable.Name = "refreshIntervalTable";
            this.refreshIntervalTable.RowCount = 1;
            this.refreshIntervalTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.refreshIntervalTable.Size = new System.Drawing.Size(273, 24);
            this.refreshIntervalTable.TabIndex = 4;
            // 
            // intervalTextBox
            // 
            this.intervalTextBox.Location = new System.Drawing.Point(112, 0);
            this.intervalTextBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.intervalTextBox.Name = "intervalTextBox";
            this.intervalTextBox.Size = new System.Drawing.Size(100, 20);
            this.intervalTextBox.TabIndex = 1;
            // 
            // refreshIntervalLbl
            // 
            this.refreshIntervalLbl.AutoSize = true;
            this.refreshIntervalLbl.Location = new System.Drawing.Point(0, 2);
            this.refreshIntervalLbl.Margin = new System.Windows.Forms.Padding(0, 2, 3, 0);
            this.refreshIntervalLbl.Name = "refreshIntervalLbl";
            this.refreshIntervalLbl.Size = new System.Drawing.Size(106, 13);
            this.refreshIntervalLbl.TabIndex = 0;
            this.refreshIntervalLbl.Text = "Refresh interval (min)";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.deleteBtn);
            this.groupBox1.Controls.Add(this.archiveBtn);
            this.groupBox1.Location = new System.Drawing.Point(10, 116);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(282, 100);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Folders management";
            // 
            // deleteBtn
            // 
            this.deleteBtn.AutoSize = true;
            this.deleteBtn.Location = new System.Drawing.Point(3, 54);
            this.deleteBtn.Name = "deleteBtn";
            this.deleteBtn.Size = new System.Drawing.Size(90, 17);
            this.deleteBtn.TabIndex = 1;
            this.deleteBtn.TabStop = true;
            this.deleteBtn.Text = "Delete folders";
            this.deleteBtn.UseVisualStyleBackColor = true;
            // 
            // archiveBtn
            // 
            this.archiveBtn.AutoSize = true;
            this.archiveBtn.Location = new System.Drawing.Point(3, 31);
            this.archiveBtn.Name = "archiveBtn";
            this.archiveBtn.Size = new System.Drawing.Size(95, 17);
            this.archiveBtn.TabIndex = 0;
            this.archiveBtn.TabStop = true;
            this.archiveBtn.Text = "Archive folders";
            this.archiveBtn.UseVisualStyleBackColor = true;
            // 
            // TimerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainPanel);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 3);
            this.Name = "TimerControl";
            this.Size = new System.Drawing.Size(400, 480);
            this.mainPanel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.refreshIntervalTable.ResumeLayout(false);
            this.refreshIntervalTable.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel mainPanel;
        private CheckBox timerCheckBox;
        private Label remainingTimeLbl;
        private TableLayoutPanel refreshIntervalTable;
        private Label refreshIntervalLbl;
        private TextBox intervalTextBox;
        private RadioButton deleteBtn;
        private RadioButton archiveBtn;
        private GroupBox groupBox1;
        private BackgroundWorker backgroundWorker1;
        private GroupBox groupBox2;
        private TableLayoutPanel tableLayoutPanel1;
	}
}
