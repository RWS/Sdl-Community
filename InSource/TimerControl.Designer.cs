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
            this.timeLbl = new System.Windows.Forms.Label();
            this.timeTextBox = new System.Windows.Forms.TextBox();
            this.timerLbl = new System.Windows.Forms.Label();
            this.remainingTimeLbl = new System.Windows.Forms.Label();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.White;
            this.mainPanel.ColumnCount = 2;
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainPanel.Controls.Add(this.timerCheckBox, 0, 0);
            this.mainPanel.Controls.Add(this.timeLbl, 0, 1);
            this.mainPanel.Controls.Add(this.timeTextBox, 1, 1);
            this.mainPanel.Controls.Add(this.timerLbl, 0, 2);
            this.mainPanel.Controls.Add(this.remainingTimeLbl, 1, 2);
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
            this.timerCheckBox.Size = new System.Drawing.Size(84, 17);
            this.timerCheckBox.TabIndex = 0;
            this.timerCheckBox.Text = "Enable timer";
            this.timerCheckBox.UseVisualStyleBackColor = true;
            this.timerCheckBox.CheckedChanged += new System.EventHandler(this.timerCheckBox_CheckedChanged);
            // 
            // timeLbl
            // 
            this.timeLbl.AutoSize = true;
            this.timeLbl.Location = new System.Drawing.Point(3, 50);
            this.timeLbl.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.timeLbl.Name = "timeLbl";
            this.timeLbl.Size = new System.Drawing.Size(55, 13);
            this.timeLbl.TabIndex = 1;
            this.timeLbl.Text = "Time (min)";
            // 
            // timeTextBox
            // 
            this.timeTextBox.Location = new System.Drawing.Point(103, 47);
            this.timeTextBox.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.timeTextBox.Name = "timeTextBox";
            this.timeTextBox.Size = new System.Drawing.Size(100, 20);
            this.timeTextBox.TabIndex = 2;
            // 
            // timerLbl
            // 
            this.timerLbl.AutoSize = true;
            this.timerLbl.Location = new System.Drawing.Point(3, 90);
            this.timerLbl.Name = "timerLbl";
            this.timerLbl.Size = new System.Drawing.Size(79, 13);
            this.timerLbl.TabIndex = 3;
            this.timerLbl.Text = "Remaining time";
            // 
            // remainingTimeLbl
            // 
            this.remainingTimeLbl.AutoSize = true;
            this.remainingTimeLbl.Location = new System.Drawing.Point(103, 90);
            this.remainingTimeLbl.Name = "remainingTimeLbl";
            this.remainingTimeLbl.Size = new System.Drawing.Size(35, 13);
            this.remainingTimeLbl.TabIndex = 4;
            this.remainingTimeLbl.Text = "label1";
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
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel mainPanel;
        private CheckBox timerCheckBox;
        private Label timeLbl;
        private TextBox timeTextBox;
        private Label timerLbl;
        private Label remainingTimeLbl;
    }
}
