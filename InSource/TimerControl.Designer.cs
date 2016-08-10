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
            this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
            this.timerCheckBox = new System.Windows.Forms.CheckBox();
            this.remainingTime = new System.Windows.Forms.Label();
            this.refreshIntervalTable = new System.Windows.Forms.TableLayoutPanel();
            this.refreshIntervalLbl = new System.Windows.Forms.Label();
            this.intervalTextBox = new System.Windows.Forms.TextBox();
            this.mainPanel.SuspendLayout();
            this.refreshIntervalTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.White;
            this.mainPanel.ColumnCount = 1;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 400F));
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainPanel.Controls.Add(this.timerCheckBox, 0, 0);
            this.mainPanel.Controls.Add(this.remainingTime, 0, 2);
            this.mainPanel.Controls.Add(this.refreshIntervalTable, 0, 1);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.RowCount = 3;
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.Size = new System.Drawing.Size(400, 480);
            this.mainPanel.TabIndex = 0;
            // 
            // timerCheckBox
            // 
            this.timerCheckBox.AutoSize = true;
            this.timerCheckBox.Location = new System.Drawing.Point(3, 10);
            this.timerCheckBox.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.timerCheckBox.Name = "timerCheckBox";
            this.timerCheckBox.Size = new System.Drawing.Size(222, 17);
            this.timerCheckBox.TabIndex = 0;
            this.timerCheckBox.Text = "Enable timer for checking project request ";
            this.timerCheckBox.UseVisualStyleBackColor = true;
            this.timerCheckBox.CheckedChanged += new System.EventHandler(this.timerCheckBox_CheckedChanged);
            // 
            // remainingTime
            // 
            this.remainingTime.AutoSize = true;
            this.remainingTime.Location = new System.Drawing.Point(3, 90);
            this.remainingTime.Name = "remainingTime";
            this.remainingTime.Size = new System.Drawing.Size(35, 13);
            this.remainingTime.TabIndex = 3;
            this.remainingTime.Text = "label1";
            // 
            // refreshIntervalTable
            // 
            this.refreshIntervalTable.ColumnCount = 2;
            this.refreshIntervalTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.refreshIntervalTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.refreshIntervalTable.Controls.Add(this.refreshIntervalLbl, 0, 0);
            this.refreshIntervalTable.Controls.Add(this.intervalTextBox, 1, 0);
            this.refreshIntervalTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.refreshIntervalTable.Location = new System.Drawing.Point(3, 43);
            this.refreshIntervalTable.Name = "refreshIntervalTable";
            this.refreshIntervalTable.RowCount = 1;
            this.refreshIntervalTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.refreshIntervalTable.Size = new System.Drawing.Size(394, 44);
            this.refreshIntervalTable.TabIndex = 4;
            // 
            // refreshIntervalLbl
            // 
            this.refreshIntervalLbl.AutoSize = true;
            this.refreshIntervalLbl.Location = new System.Drawing.Point(0, 0);
            this.refreshIntervalLbl.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.refreshIntervalLbl.Name = "refreshIntervalLbl";
            this.refreshIntervalLbl.Size = new System.Drawing.Size(106, 13);
            this.refreshIntervalLbl.TabIndex = 0;
            this.refreshIntervalLbl.Text = "Refresh interval (min)";
            // 
            // intervalTextBox
            // 
            this.intervalTextBox.Location = new System.Drawing.Point(112, 0);
            this.intervalTextBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.intervalTextBox.Name = "intervalTextBox";
            this.intervalTextBox.Size = new System.Drawing.Size(100, 20);
            this.intervalTextBox.TabIndex = 1;
            // 
            // TimerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainPanel);
            this.Name = "TimerControl";
            this.Size = new System.Drawing.Size(400, 480);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.refreshIntervalTable.ResumeLayout(false);
            this.refreshIntervalTable.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel mainPanel;
        private CheckBox timerCheckBox;
        private Label remainingTime;
        private TableLayoutPanel refreshIntervalTable;
        private Label refreshIntervalLbl;
        private TextBox intervalTextBox;
    }
}
