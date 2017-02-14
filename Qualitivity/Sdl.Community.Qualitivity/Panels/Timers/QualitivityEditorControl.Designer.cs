using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Community.Qualitivity.Panels.Timers
{
    partial class QualitivityEditorControl
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
            this.components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(QualitivityEditorControl));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.imageList3 = new System.Windows.Forms.ImageList(this.components);
            this.imageList4 = new System.Windows.Forms.ImageList(this.components);
            this.toolStripButton_start = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_pause = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_stop = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_delete = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel12 = new System.Windows.Forms.Panel();
            this.panel15 = new System.Windows.Forms.Panel();
            this.panel16 = new System.Windows.Forms.Panel();
            this.panel23 = new System.Windows.Forms.Panel();
            this.label_elapsed_idle_time = new System.Windows.Forms.Label();
            this.panel14 = new System.Windows.Forms.Panel();
            this.label_elapsed_time = new System.Windows.Forms.Label();
            this.panel13 = new System.Windows.Forms.Panel();
            this.label_elapsed_time_label = new System.Windows.Forms.Label();
            this.panel17 = new System.Windows.Forms.Panel();
            this.panel18 = new System.Windows.Forms.Panel();
            this.label_type = new System.Windows.Forms.Label();
            this.panel19 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.label_elapsed_time_document = new System.Windows.Forms.Label();
            this.panel11 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label_document = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel12.SuspendLayout();
            this.panel15.SuspendLayout();
            this.panel16.SuspendLayout();
            this.panel23.SuspendLayout();
            this.panel14.SuspendLayout();
            this.panel13.SuspendLayout();
            this.panel17.SuspendLayout();
            this.panel18.SuspendLayout();
            this.panel19.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel6.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Project.png");
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "remove");
            this.imageList2.Images.SetKeyName(1, "started");
            this.imageList2.Images.SetKeyName(2, "pause");
            this.imageList2.Images.SetKeyName(3, "stop");
            this.imageList2.Images.SetKeyName(4, "unpause");
            // 
            // imageList3
            // 
            this.imageList3.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList3.ImageStream")));
            this.imageList3.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList3.Images.SetKeyName(0, "Project.png");
            // 
            // imageList4
            // 
            this.imageList4.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList4.ImageStream")));
            this.imageList4.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList4.Images.SetKeyName(0, "remove");
            this.imageList4.Images.SetKeyName(1, "started");
            this.imageList4.Images.SetKeyName(2, "pause");
            this.imageList4.Images.SetKeyName(3, "stop");
            this.imageList4.Images.SetKeyName(4, "unpause");
            // 
            // toolStripButton_start
            // 
            this.toolStripButton_start.Enabled = false;
            this.toolStripButton_start.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_start.Image")));
            this.toolStripButton_start.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_start.Name = "toolStripButton_start";
            this.toolStripButton_start.Size = new System.Drawing.Size(36, 51);
            this.toolStripButton_start.Text = "Start";
            this.toolStripButton_start.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton_start.ToolTipText = "Start Activity Tracking";
            this.toolStripButton_start.Click += new System.EventHandler(this.toolStripButton_start_Click);
            // 
            // toolStripButton_pause
            // 
            this.toolStripButton_pause.Enabled = false;
            this.toolStripButton_pause.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_pause.Image")));
            this.toolStripButton_pause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_pause.Name = "toolStripButton_pause";
            this.toolStripButton_pause.Size = new System.Drawing.Size(42, 51);
            this.toolStripButton_pause.Text = "Pause";
            this.toolStripButton_pause.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton_pause.ToolTipText = "Pause Activity Tracking";
            this.toolStripButton_pause.Click += new System.EventHandler(this.toolStripButton_pause_Click);
            // 
            // toolStripButton_stop
            // 
            this.toolStripButton_stop.Enabled = false;
            this.toolStripButton_stop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_stop.Image")));
            this.toolStripButton_stop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_stop.Name = "toolStripButton_stop";
            this.toolStripButton_stop.Size = new System.Drawing.Size(36, 51);
            this.toolStripButton_stop.Text = "Stop";
            this.toolStripButton_stop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton_stop.ToolTipText = "Stop Activity Tracking";
            this.toolStripButton_stop.Click += new System.EventHandler(this.toolStripButton_stop_Click);
            // 
            // toolStripButton_delete
            // 
            this.toolStripButton_delete.Enabled = false;
            this.toolStripButton_delete.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_delete.Image")));
            this.toolStripButton_delete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_delete.Name = "toolStripButton_delete";
            this.toolStripButton_delete.Size = new System.Drawing.Size(47, 51);
            this.toolStripButton_delete.Text = "Cancel";
            this.toolStripButton_delete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton_delete.ToolTipText = "Cancel Activity Tracking";
            this.toolStripButton_delete.Click += new System.EventHandler(this.toolStripButton_delete_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_start,
            this.toolStripButton_pause,
            this.toolStripButton_stop,
            this.toolStripButton_delete});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(336, 54);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 54);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(2, 5, 2, 2);
            this.panel1.Size = new System.Drawing.Size(336, 219);
            this.panel1.TabIndex = 10;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Location = new System.Drawing.Point(2, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(332, 212);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tracking";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel12);
            this.panel2.Controls.Add(this.panel17);
            this.panel2.Controls.Add(this.panel9);
            this.panel2.Controls.Add(this.panel6);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 16);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(2);
            this.panel2.Size = new System.Drawing.Size(326, 193);
            this.panel2.TabIndex = 0;
            // 
            // panel12
            // 
            this.panel12.Controls.Add(this.panel15);
            this.panel12.Controls.Add(this.panel14);
            this.panel12.Controls.Add(this.panel13);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel12.Location = new System.Drawing.Point(2, 58);
            this.panel12.Name = "panel12";
            this.panel12.Padding = new System.Windows.Forms.Padding(0, 2, 4, 0);
            this.panel12.Size = new System.Drawing.Size(322, 133);
            this.panel12.TabIndex = 13;
            // 
            // panel15
            // 
            this.panel15.Controls.Add(this.panel16);
            this.panel15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel15.Location = new System.Drawing.Point(0, 72);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(318, 61);
            this.panel15.TabIndex = 2;
            // 
            // panel16
            // 
            this.panel16.Controls.Add(this.panel23);
            this.panel16.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel16.Location = new System.Drawing.Point(0, 42);
            this.panel16.Name = "panel16";
            this.panel16.Size = new System.Drawing.Size(318, 19);
            this.panel16.TabIndex = 0;
            // 
            // panel23
            // 
            this.panel23.Controls.Add(this.label_elapsed_idle_time);
            this.panel23.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel23.Location = new System.Drawing.Point(0, 0);
            this.panel23.Name = "panel23";
            this.panel23.Size = new System.Drawing.Size(318, 19);
            this.panel23.TabIndex = 3;
            // 
            // label_elapsed_idle_time
            // 
            this.label_elapsed_idle_time.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_elapsed_idle_time.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_elapsed_idle_time.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.label_elapsed_idle_time.Location = new System.Drawing.Point(0, 0);
            this.label_elapsed_idle_time.Margin = new System.Windows.Forms.Padding(0);
            this.label_elapsed_idle_time.Name = "label_elapsed_idle_time";
            this.label_elapsed_idle_time.Size = new System.Drawing.Size(318, 19);
            this.label_elapsed_idle_time.TabIndex = 3;
            this.label_elapsed_idle_time.Text = "Idle Time: 00:00:00";
            this.label_elapsed_idle_time.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_elapsed_idle_time.Visible = false;
            // 
            // panel14
            // 
            this.panel14.Controls.Add(this.label_elapsed_time);
            this.panel14.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel14.Location = new System.Drawing.Point(0, 43);
            this.panel14.Name = "panel14";
            this.panel14.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.panel14.Size = new System.Drawing.Size(318, 29);
            this.panel14.TabIndex = 1;
            // 
            // label_elapsed_time
            // 
            this.label_elapsed_time.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_elapsed_time.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_elapsed_time.Location = new System.Drawing.Point(0, 5);
            this.label_elapsed_time.Name = "label_elapsed_time";
            this.label_elapsed_time.Size = new System.Drawing.Size(318, 24);
            this.label_elapsed_time.TabIndex = 1;
            this.label_elapsed_time.Text = "00:00:00";
            this.label_elapsed_time.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel13
            // 
            this.panel13.Controls.Add(this.label_elapsed_time_label);
            this.panel13.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel13.Location = new System.Drawing.Point(0, 2);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(318, 41);
            this.panel13.TabIndex = 0;
            // 
            // label_elapsed_time_label
            // 
            this.label_elapsed_time_label.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_elapsed_time_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_elapsed_time_label.Location = new System.Drawing.Point(0, 0);
            this.label_elapsed_time_label.Name = "label_elapsed_time_label";
            this.label_elapsed_time_label.Size = new System.Drawing.Size(318, 41);
            this.label_elapsed_time_label.TabIndex = 0;
            this.label_elapsed_time_label.Text = "Total Elapsed Time";
            this.label_elapsed_time_label.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // panel17
            // 
            this.panel17.Controls.Add(this.panel18);
            this.panel17.Controls.Add(this.panel19);
            this.panel17.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel17.Location = new System.Drawing.Point(2, 39);
            this.panel17.Name = "panel17";
            this.panel17.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.panel17.Size = new System.Drawing.Size(322, 19);
            this.panel17.TabIndex = 12;
            // 
            // panel18
            // 
            this.panel18.Controls.Add(this.label_type);
            this.panel18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel18.Location = new System.Drawing.Point(66, 0);
            this.panel18.Name = "panel18";
            this.panel18.Size = new System.Drawing.Size(252, 19);
            this.panel18.TabIndex = 1;
            // 
            // label_type
            // 
            this.label_type.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_type.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_type.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_type.Location = new System.Drawing.Point(0, 0);
            this.label_type.Name = "label_type";
            this.label_type.Padding = new System.Windows.Forms.Padding(0, 3, 0, 2);
            this.label_type.Size = new System.Drawing.Size(252, 19);
            this.label_type.TabIndex = 1;
            this.label_type.Text = "...";
            this.label_type.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel19
            // 
            this.panel19.Controls.Add(this.label3);
            this.panel19.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel19.Location = new System.Drawing.Point(0, 0);
            this.panel19.Name = "panel19";
            this.panel19.Size = new System.Drawing.Size(66, 19);
            this.panel19.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 19);
            this.label3.TabIndex = 2;
            this.label3.Text = "Type:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.panel10);
            this.panel9.Controls.Add(this.panel11);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel9.Location = new System.Drawing.Point(2, 20);
            this.panel9.Name = "panel9";
            this.panel9.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.panel9.Size = new System.Drawing.Size(322, 19);
            this.panel9.TabIndex = 10;
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.label_elapsed_time_document);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel10.Location = new System.Drawing.Point(66, 0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(252, 19);
            this.panel10.TabIndex = 1;
            // 
            // label_elapsed_time_document
            // 
            this.label_elapsed_time_document.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_elapsed_time_document.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_elapsed_time_document.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label_elapsed_time_document.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_elapsed_time_document.Location = new System.Drawing.Point(0, 0);
            this.label_elapsed_time_document.Name = "label_elapsed_time_document";
            this.label_elapsed_time_document.Padding = new System.Windows.Forms.Padding(0, 3, 0, 1);
            this.label_elapsed_time_document.Size = new System.Drawing.Size(252, 19);
            this.label_elapsed_time_document.TabIndex = 1;
            this.label_elapsed_time_document.Text = "...";
            this.label_elapsed_time_document.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.pictureBox1);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel11.Location = new System.Drawing.Point(0, 0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(66, 19);
            this.panel11.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(41, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(20, 19);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.panel7);
            this.panel6.Controls.Add(this.panel8);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(2, 2);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.panel6.Size = new System.Drawing.Size(322, 18);
            this.panel6.TabIndex = 9;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.label_document);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(66, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(252, 18);
            this.panel7.TabIndex = 1;
            // 
            // label_document
            // 
            this.label_document.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_document.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_document.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_document.Location = new System.Drawing.Point(0, 0);
            this.label_document.Name = "label_document";
            this.label_document.Padding = new System.Windows.Forms.Padding(0, 3, 0, 2);
            this.label_document.Size = new System.Drawing.Size(252, 18);
            this.label_document.TabIndex = 2;
            this.label_document.Text = "...";
            this.label_document.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.label4);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(66, 18);
            this.panel8.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 18);
            this.label4.TabIndex = 2;
            this.label4.Text = "Document:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // StudioTimeTrackerEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "StudioTimeTrackerEditorControl";
            this.Size = new System.Drawing.Size(336, 273);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.panel15.ResumeLayout(false);
            this.panel16.ResumeLayout(false);
            this.panel23.ResumeLayout(false);
            this.panel14.ResumeLayout(false);
            this.panel13.ResumeLayout(false);
            this.panel17.ResumeLayout(false);
            this.panel18.ResumeLayout(false);
            this.panel19.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel6.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal ImageList imageList1;
        private ImageList imageList2;
        internal ImageList imageList3;
        private ImageList imageList4;
        private ToolStripButton toolStripButton_start;
        private ToolStripButton toolStripButton_pause;
        private ToolStripButton toolStripButton_stop;
        private ToolStripButton toolStripButton_delete;
        private ToolStrip toolStrip1;
        private Panel panel1;
        private GroupBox groupBox1;
        private Panel panel2;
        private Panel panel12;
        private Panel panel15;
        private Panel panel16;
        private Panel panel23;
        private Label label_elapsed_idle_time;
        private Panel panel14;
        private Label label_elapsed_time;
        private Panel panel13;
        private Label label_elapsed_time_label;
        private Panel panel17;
        private Panel panel18;
        private Label label_type;
        private Panel panel19;
        private Label label3;
        private Panel panel9;
        private Panel panel10;
        private Label label_elapsed_time_document;
        private Panel panel11;
        private PictureBox pictureBox1;
        private Panel panel6;
        private Panel panel7;
        private Label label_document;
        private Panel panel8;
        private Label label4;



    }
}
