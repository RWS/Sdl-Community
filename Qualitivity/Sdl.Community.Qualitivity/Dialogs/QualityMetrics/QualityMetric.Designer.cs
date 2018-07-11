using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Community.Qualitivity.Dialogs.QualityMetrics
{
    partial class QualityMetric
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QualityMetric));
			this.button_save = new System.Windows.Forms.Button();
			this.button_cancel = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel4 = new System.Windows.Forms.Panel();
			this.label_status_text = new System.Windows.Forms.Label();
			this.panel3 = new System.Windows.Forms.Panel();
			this.panel_main = new System.Windows.Forms.Panel();
			this.panel9 = new System.Windows.Forms.Panel();
			this.panel11 = new System.Windows.Forms.Panel();
			this.textBox_comment = new System.Windows.Forms.TextBox();
			this.panel10 = new System.Windows.Forms.Panel();
			this.label4 = new System.Windows.Forms.Label();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.panel6 = new System.Windows.Forms.Panel();
			this.panel8 = new System.Windows.Forms.Panel();
			this.textBox_content = new System.Windows.Forms.TextBox();
			this.panel7 = new System.Windows.Forms.Panel();
			this.label3 = new System.Windows.Forms.Label();
			this.panel5 = new System.Windows.Forms.Panel();
			this.panel18 = new System.Windows.Forms.Panel();
			this.panel19 = new System.Windows.Forms.Panel();
			this.comboBox_status = new System.Windows.Forms.ComboBox();
			this.panel20 = new System.Windows.Forms.Panel();
			this.label5 = new System.Windows.Forms.Label();
			this.panel15 = new System.Windows.Forms.Panel();
			this.panel16 = new System.Windows.Forms.Panel();
			this.comboBox_severity = new System.Windows.Forms.ComboBox();
			this.panel17 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.panel12 = new System.Windows.Forms.Panel();
			this.panel13 = new System.Windows.Forms.Panel();
			this.comboBox_qm = new System.Windows.Forms.ComboBox();
			this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.panel14 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.panel4.SuspendLayout();
			this.panel3.SuspendLayout();
			this.panel_main.SuspendLayout();
			this.panel9.SuspendLayout();
			this.panel11.SuspendLayout();
			this.panel10.SuspendLayout();
			this.panel6.SuspendLayout();
			this.panel8.SuspendLayout();
			this.panel7.SuspendLayout();
			this.panel5.SuspendLayout();
			this.panel18.SuspendLayout();
			this.panel19.SuspendLayout();
			this.panel20.SuspendLayout();
			this.panel15.SuspendLayout();
			this.panel16.SuspendLayout();
			this.panel17.SuspendLayout();
			this.panel12.SuspendLayout();
			this.panel13.SuspendLayout();
			this.panel14.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_save
			// 
			this.button_save.Location = new System.Drawing.Point(30, 6);
			this.button_save.Name = "button_save";
			this.button_save.Size = new System.Drawing.Size(75, 23);
			this.button_save.TabIndex = 9;
			this.button_save.Text = "&OK";
			this.button_save.UseVisualStyleBackColor = true;
			this.button_save.Click += new System.EventHandler(this.button_save_Click);
			// 
			// button_cancel
			// 
			this.button_cancel.Location = new System.Drawing.Point(111, 6);
			this.button_cancel.Name = "button_cancel";
			this.button_cancel.Size = new System.Drawing.Size(75, 23);
			this.button_cancel.TabIndex = 8;
			this.button_cancel.Text = "&Cancel";
			this.button_cancel.UseVisualStyleBackColor = true;
			this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.panel4);
			this.panel1.Controls.Add(this.panel3);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 400);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(746, 36);
			this.panel1.TabIndex = 59;
			// 
			// panel4
			// 
			this.panel4.Controls.Add(this.label_status_text);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel4.Location = new System.Drawing.Point(0, 0);
			this.panel4.Name = "panel4";
			this.panel4.Padding = new System.Windows.Forms.Padding(3, 3, 5, 5);
			this.panel4.Size = new System.Drawing.Size(552, 36);
			this.panel4.TabIndex = 1;
			// 
			// label_status_text
			// 
			this.label_status_text.AutoSize = true;
			this.label_status_text.Dock = System.Windows.Forms.DockStyle.Top;
			this.label_status_text.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_status_text.ForeColor = System.Drawing.Color.Gray;
			this.label_status_text.Location = new System.Drawing.Point(3, 3);
			this.label_status_text.Name = "label_status_text";
			this.label_status_text.Size = new System.Drawing.Size(110, 26);
			this.label_status_text.TabIndex = 2;
			this.label_status_text.Text = "Modified: 2015-04-24 \r\nBy: Somebody";
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.button_cancel);
			this.panel3.Controls.Add(this.button_save);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel3.Location = new System.Drawing.Point(552, 0);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(194, 36);
			this.panel3.TabIndex = 0;
			// 
			// panel_main
			// 
			this.panel_main.Controls.Add(this.panel9);
			this.panel_main.Controls.Add(this.splitter1);
			this.panel_main.Controls.Add(this.panel6);
			this.panel_main.Controls.Add(this.panel5);
			this.panel_main.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_main.Location = new System.Drawing.Point(0, 0);
			this.panel_main.Name = "panel_main";
			this.panel_main.Padding = new System.Windows.Forms.Padding(5);
			this.panel_main.Size = new System.Drawing.Size(746, 400);
			this.panel_main.TabIndex = 60;
			// 
			// panel9
			// 
			this.panel9.Controls.Add(this.panel11);
			this.panel9.Controls.Add(this.panel10);
			this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel9.Location = new System.Drawing.Point(5, 192);
			this.panel9.Name = "panel9";
			this.panel9.Size = new System.Drawing.Size(736, 203);
			this.panel9.TabIndex = 11;
			// 
			// panel11
			// 
			this.panel11.Controls.Add(this.textBox_comment);
			this.panel11.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel11.Location = new System.Drawing.Point(65, 0);
			this.panel11.Name = "panel11";
			this.panel11.Padding = new System.Windows.Forms.Padding(2);
			this.panel11.Size = new System.Drawing.Size(671, 203);
			this.panel11.TabIndex = 9;
			// 
			// textBox_comment
			// 
			this.textBox_comment.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox_comment.Location = new System.Drawing.Point(2, 2);
			this.textBox_comment.Multiline = true;
			this.textBox_comment.Name = "textBox_comment";
			this.textBox_comment.Size = new System.Drawing.Size(667, 199);
			this.textBox_comment.TabIndex = 8;
			// 
			// panel10
			// 
			this.panel10.Controls.Add(this.label4);
			this.panel10.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel10.Location = new System.Drawing.Point(0, 0);
			this.panel10.Name = "panel10";
			this.panel10.Size = new System.Drawing.Size(65, 203);
			this.panel10.TabIndex = 8;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(10, 3);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(54, 13);
			this.label4.TabIndex = 6;
			this.label4.Text = "Comment:";
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(5, 188);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(736, 4);
			this.splitter1.TabIndex = 10;
			this.splitter1.TabStop = false;
			// 
			// panel6
			// 
			this.panel6.Controls.Add(this.panel8);
			this.panel6.Controls.Add(this.panel7);
			this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel6.Location = new System.Drawing.Point(5, 99);
			this.panel6.Name = "panel6";
			this.panel6.Padding = new System.Windows.Forms.Padding(2);
			this.panel6.Size = new System.Drawing.Size(736, 89);
			this.panel6.TabIndex = 9;
			// 
			// panel8
			// 
			this.panel8.Controls.Add(this.textBox_content);
			this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel8.Location = new System.Drawing.Point(65, 2);
			this.panel8.Name = "panel8";
			this.panel8.Size = new System.Drawing.Size(669, 85);
			this.panel8.TabIndex = 1;
			// 
			// textBox_content
			// 
			this.textBox_content.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox_content.Location = new System.Drawing.Point(0, 0);
			this.textBox_content.Multiline = true;
			this.textBox_content.Name = "textBox_content";
			this.textBox_content.Size = new System.Drawing.Size(669, 85);
			this.textBox_content.TabIndex = 5;
			this.textBox_content.TextChanged += new System.EventHandler(this.textBox_content_TextChanged);
			// 
			// panel7
			// 
			this.panel7.Controls.Add(this.label3);
			this.panel7.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel7.Location = new System.Drawing.Point(2, 2);
			this.panel7.Name = "panel7";
			this.panel7.Size = new System.Drawing.Size(63, 85);
			this.panel7.TabIndex = 0;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(17, 3);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(47, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Content:";
			// 
			// panel5
			// 
			this.panel5.Controls.Add(this.panel18);
			this.panel5.Controls.Add(this.panel15);
			this.panel5.Controls.Add(this.panel12);
			this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel5.Location = new System.Drawing.Point(5, 5);
			this.panel5.Name = "panel5";
			this.panel5.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
			this.panel5.Size = new System.Drawing.Size(736, 94);
			this.panel5.TabIndex = 8;
			// 
			// panel18
			// 
			this.panel18.Controls.Add(this.panel19);
			this.panel18.Controls.Add(this.panel20);
			this.panel18.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel18.Location = new System.Drawing.Point(0, 68);
			this.panel18.Name = "panel18";
			this.panel18.Padding = new System.Windows.Forms.Padding(2);
			this.panel18.Size = new System.Drawing.Size(736, 29);
			this.panel18.TabIndex = 12;
			// 
			// panel19
			// 
			this.panel19.Controls.Add(this.comboBox_status);
			this.panel19.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel19.Location = new System.Drawing.Point(65, 2);
			this.panel19.Name = "panel19";
			this.panel19.Size = new System.Drawing.Size(669, 25);
			this.panel19.TabIndex = 1;
			// 
			// comboBox_status
			// 
			this.comboBox_status.Dock = System.Windows.Forms.DockStyle.Fill;
			this.comboBox_status.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_status.FormattingEnabled = true;
			this.comboBox_status.Items.AddRange(new object[] {
            "Open",
            "Resolved",
            "Ignore"});
			this.comboBox_status.Location = new System.Drawing.Point(0, 0);
			this.comboBox_status.Name = "comboBox_status";
			this.comboBox_status.Size = new System.Drawing.Size(669, 21);
			this.comboBox_status.TabIndex = 3;
			this.comboBox_status.SelectedIndexChanged += new System.EventHandler(this.comboBox_severity_SelectedIndexChanged);
			// 
			// panel20
			// 
			this.panel20.Controls.Add(this.label5);
			this.panel20.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel20.Location = new System.Drawing.Point(2, 2);
			this.panel20.Name = "panel20";
			this.panel20.Size = new System.Drawing.Size(63, 25);
			this.panel20.TabIndex = 0;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(24, 1);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(40, 13);
			this.label5.TabIndex = 2;
			this.label5.Text = "Status:";
			// 
			// panel15
			// 
			this.panel15.Controls.Add(this.panel16);
			this.panel15.Controls.Add(this.panel17);
			this.panel15.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel15.Location = new System.Drawing.Point(0, 39);
			this.panel15.Name = "panel15";
			this.panel15.Padding = new System.Windows.Forms.Padding(2);
			this.panel15.Size = new System.Drawing.Size(736, 29);
			this.panel15.TabIndex = 11;
			// 
			// panel16
			// 
			this.panel16.Controls.Add(this.comboBox_severity);
			this.panel16.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel16.Location = new System.Drawing.Point(65, 2);
			this.panel16.Name = "panel16";
			this.panel16.Size = new System.Drawing.Size(669, 25);
			this.panel16.TabIndex = 1;
			// 
			// comboBox_severity
			// 
			this.comboBox_severity.Dock = System.Windows.Forms.DockStyle.Fill;
			this.comboBox_severity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_severity.FormattingEnabled = true;
			this.comboBox_severity.Location = new System.Drawing.Point(0, 0);
			this.comboBox_severity.Name = "comboBox_severity";
			this.comboBox_severity.Size = new System.Drawing.Size(669, 21);
			this.comboBox_severity.TabIndex = 3;
			this.comboBox_severity.SelectedIndexChanged += new System.EventHandler(this.comboBox_severity_SelectedIndexChanged);
			// 
			// panel17
			// 
			this.panel17.Controls.Add(this.label2);
			this.panel17.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel17.Location = new System.Drawing.Point(2, 2);
			this.panel17.Name = "panel17";
			this.panel17.Size = new System.Drawing.Size(63, 25);
			this.panel17.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(16, 1);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Severity:";
			// 
			// panel12
			// 
			this.panel12.Controls.Add(this.panel13);
			this.panel12.Controls.Add(this.panel14);
			this.panel12.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel12.Location = new System.Drawing.Point(0, 10);
			this.panel12.Name = "panel12";
			this.panel12.Padding = new System.Windows.Forms.Padding(2);
			this.panel12.Size = new System.Drawing.Size(736, 29);
			this.panel12.TabIndex = 10;
			// 
			// panel13
			// 
			this.panel13.Controls.Add(this.comboBox_qm);
			this.panel13.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel13.Location = new System.Drawing.Point(65, 2);
			this.panel13.Name = "panel13";
			this.panel13.Size = new System.Drawing.Size(669, 25);
			this.panel13.TabIndex = 1;
			// 
			// comboBox_qm
			// 
			this.comboBox_qm.ContextMenuStrip = this.contextMenuStrip2;
			this.comboBox_qm.Dock = System.Windows.Forms.DockStyle.Fill;
			this.comboBox_qm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_qm.FormattingEnabled = true;
			this.comboBox_qm.Location = new System.Drawing.Point(0, 0);
			this.comboBox_qm.Name = "comboBox_qm";
			this.comboBox_qm.Size = new System.Drawing.Size(669, 21);
			this.comboBox_qm.TabIndex = 1;
			this.comboBox_qm.SelectedIndexChanged += new System.EventHandler(this.comboBox_qm_SelectedIndexChanged);
			this.comboBox_qm.Click += new System.EventHandler(this.comboBox_qm_Click);
			// 
			// contextMenuStrip2
			// 
			this.contextMenuStrip2.Name = "contextMenuStrip2";
			this.contextMenuStrip2.Size = new System.Drawing.Size(61, 4);
			this.contextMenuStrip2.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip2_ItemClicked);
			// 
			// panel14
			// 
			this.panel14.Controls.Add(this.label1);
			this.panel14.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel14.Location = new System.Drawing.Point(2, 2);
			this.panel14.Name = "panel14";
			this.panel14.Size = new System.Drawing.Size(63, 25);
			this.panel14.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 2);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "QM Type:";
			// 
			// QualityMetric
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(746, 436);
			this.Controls.Add(this.panel_main);
			this.Controls.Add(this.panel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(401, 261);
			this.Name = "QualityMetric";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Quality Metric";
			this.Load += new System.EventHandler(this.QualityMetric_Load);
			this.panel1.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.panel4.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.panel_main.ResumeLayout(false);
			this.panel9.ResumeLayout(false);
			this.panel11.ResumeLayout(false);
			this.panel11.PerformLayout();
			this.panel10.ResumeLayout(false);
			this.panel10.PerformLayout();
			this.panel6.ResumeLayout(false);
			this.panel8.ResumeLayout(false);
			this.panel8.PerformLayout();
			this.panel7.ResumeLayout(false);
			this.panel7.PerformLayout();
			this.panel5.ResumeLayout(false);
			this.panel18.ResumeLayout(false);
			this.panel19.ResumeLayout(false);
			this.panel20.ResumeLayout(false);
			this.panel20.PerformLayout();
			this.panel15.ResumeLayout(false);
			this.panel16.ResumeLayout(false);
			this.panel17.ResumeLayout(false);
			this.panel17.PerformLayout();
			this.panel12.ResumeLayout(false);
			this.panel13.ResumeLayout(false);
			this.panel14.ResumeLayout(false);
			this.panel14.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private Button button_save;
        private Button button_cancel;
        private Panel panel1;
        private Panel panel_main;
        private Label label1;
        private Label label2;
        internal ComboBox comboBox_severity;
        private Panel panel3;
        private TextBox textBox_content;
        internal ComboBox comboBox_qm;
        private Label label4;
        private Label label3;
        private Panel panel4;
        private Label label_status_text;
        internal ComboBox comboBox_status;
        private Label label5;
        private Splitter splitter1;
        private Panel panel6;
        private Panel panel8;
        private Panel panel7;
        private Panel panel5;
        private Panel panel9;
        private Panel panel11;
        private TextBox textBox_comment;
        private Panel panel10;
        private ContextMenuStrip contextMenuStrip2;
        private Panel panel18;
        private Panel panel19;
        private Panel panel20;
        private Panel panel15;
        private Panel panel16;
        private Panel panel17;
        private Panel panel12;
        private Panel panel13;
        private Panel panel14;
    }
}