namespace PostEdit.Compare
{
    partial class ProgressDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressDialog));
            this.panel6 = new System.Windows.Forms.Panel();
            this.textBox_header_title = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button_cancel = new System.Windows.Forms.Button();
            this.progressBar_total_progress = new System.Windows.Forms.ProgressBar();
            this.progressBar_progress = new System.Windows.Forms.ProgressBar();
            this.label_processing_message_3 = new System.Windows.Forms.Label();
            this.label_processing_message_2 = new System.Windows.Forms.Label();
            this.label_total_progress_percentage = new System.Windows.Forms.Label();
            this.label_processing_message_1 = new System.Windows.Forms.Label();
            this.labelName_total_progress = new System.Windows.Forms.Label();
            this.label_progress_percentage = new System.Windows.Forms.Label();
            this.label_processing = new System.Windows.Forms.Label();
            this.labelName_process = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.pictureBox_progressBar_progress = new System.Windows.Forms.PictureBox();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_progressBar_progress)).BeginInit();
            this.SuspendLayout();
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.White;
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.textBox_header_title);
            this.panel6.Controls.Add(this.pictureBox1);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(490, 56);
            this.panel6.TabIndex = 54;
            // 
            // textBox_header_title
            // 
            this.textBox_header_title.BackColor = System.Drawing.Color.White;
            this.textBox_header_title.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_header_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_header_title.Location = new System.Drawing.Point(72, 15);
            this.textBox_header_title.Name = "textBox_header_title";
            this.textBox_header_title.ReadOnly = true;
            this.textBox_header_title.Size = new System.Drawing.Size(493, 25);
            this.textBox_header_title.TabIndex = 4;
            this.textBox_header_title.TabStop = false;
            this.textBox_header_title.Text = "...";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(21, 11);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(43, 38);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(385, 203);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_cancel.TabIndex = 55;
            this.button_cancel.Text = "&Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // progressBar_total_progress
            // 
            this.progressBar_total_progress.Location = new System.Drawing.Point(105, 152);
            this.progressBar_total_progress.MarqueeAnimationSpeed = 10;
            this.progressBar_total_progress.Name = "progressBar_total_progress";
            this.progressBar_total_progress.Size = new System.Drawing.Size(331, 18);
            this.progressBar_total_progress.TabIndex = 65;
            // 
            // progressBar_progress
            // 
            this.progressBar_progress.Location = new System.Drawing.Point(105, 105);
            this.progressBar_progress.MarqueeAnimationSpeed = 10;
            this.progressBar_progress.Name = "progressBar_progress";
            this.progressBar_progress.Size = new System.Drawing.Size(331, 18);
            this.progressBar_progress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar_progress.TabIndex = 66;
            // 
            // label_processing_message_3
            // 
            this.label_processing_message_3.Location = new System.Drawing.Point(105, 173);
            this.label_processing_message_3.Name = "label_processing_message_3";
            this.label_processing_message_3.Size = new System.Drawing.Size(355, 18);
            this.label_processing_message_3.TabIndex = 64;
            this.label_processing_message_3.Text = "initializing...";
            // 
            // label_processing_message_2
            // 
            this.label_processing_message_2.Location = new System.Drawing.Point(105, 127);
            this.label_processing_message_2.Name = "label_processing_message_2";
            this.label_processing_message_2.Size = new System.Drawing.Size(355, 18);
            this.label_processing_message_2.TabIndex = 63;
            this.label_processing_message_2.Text = "loading...";
            // 
            // label_total_progress_percentage
            // 
            this.label_total_progress_percentage.AutoSize = true;
            this.label_total_progress_percentage.Location = new System.Drawing.Point(439, 155);
            this.label_total_progress_percentage.Name = "label_total_progress_percentage";
            this.label_total_progress_percentage.Size = new System.Drawing.Size(21, 13);
            this.label_total_progress_percentage.TabIndex = 58;
            this.label_total_progress_percentage.Text = "0%";
            // 
            // label_processing_message_1
            // 
            this.label_processing_message_1.Location = new System.Drawing.Point(105, 79);
            this.label_processing_message_1.Name = "label_processing_message_1";
            this.label_processing_message_1.Size = new System.Drawing.Size(355, 18);
            this.label_processing_message_1.TabIndex = 57;
            this.label_processing_message_1.Text = "loading files for comparison...";
            this.label_processing_message_1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelName_total_progress
            // 
            this.labelName_total_progress.Location = new System.Drawing.Point(13, 152);
            this.labelName_total_progress.Name = "labelName_total_progress";
            this.labelName_total_progress.Size = new System.Drawing.Size(85, 18);
            this.labelName_total_progress.TabIndex = 62;
            this.labelName_total_progress.Text = "Total Progress:";
            this.labelName_total_progress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_progress_percentage
            // 
            this.label_progress_percentage.AutoSize = true;
            this.label_progress_percentage.Location = new System.Drawing.Point(439, 108);
            this.label_progress_percentage.Name = "label_progress_percentage";
            this.label_progress_percentage.Size = new System.Drawing.Size(21, 13);
            this.label_progress_percentage.TabIndex = 61;
            this.label_progress_percentage.Text = "0%";
            // 
            // label_processing
            // 
            this.label_processing.Location = new System.Drawing.Point(13, 105);
            this.label_processing.Name = "label_processing";
            this.label_processing.Size = new System.Drawing.Size(85, 18);
            this.label_processing.TabIndex = 60;
            this.label_processing.Text = "Progress:";
            this.label_processing.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelName_process
            // 
            this.labelName_process.Location = new System.Drawing.Point(13, 79);
            this.labelName_process.Name = "labelName_process";
            this.labelName_process.Size = new System.Drawing.Size(85, 18);
            this.labelName_process.TabIndex = 59;
            this.labelName_process.Text = "Process:";
            this.labelName_process.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 239);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(490, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 67;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(137, 17);
            this.toolStripStatusLabel1.Text = "Processing.... please wait";
            // 
            // pictureBox_progressBar_progress
            // 
            this.pictureBox_progressBar_progress.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox_progressBar_progress.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_progressBar_progress.Image")));
            this.pictureBox_progressBar_progress.Location = new System.Drawing.Point(12, 203);
            this.pictureBox_progressBar_progress.Name = "pictureBox_progressBar_progress";
            this.pictureBox_progressBar_progress.Size = new System.Drawing.Size(118, 17);
            this.pictureBox_progressBar_progress.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_progressBar_progress.TabIndex = 0;
            this.pictureBox_progressBar_progress.TabStop = false;
            this.pictureBox_progressBar_progress.Visible = false;
            // 
            // ProgressDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 261);
            this.ControlBox = false;
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.pictureBox_progressBar_progress);
            this.Controls.Add(this.progressBar_total_progress);
            this.Controls.Add(this.progressBar_progress);
            this.Controls.Add(this.label_processing_message_3);
            this.Controls.Add(this.label_processing_message_2);
            this.Controls.Add(this.label_total_progress_percentage);
            this.Controls.Add(this.label_processing_message_1);
            this.Controls.Add(this.labelName_total_progress);
            this.Controls.Add(this.label_progress_percentage);
            this.Controls.Add(this.label_processing);
            this.Controls.Add(this.labelName_process);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.panel6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Progress";
            this.Load += new System.EventHandler(this.ProgressDialogLoadProject_Load);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_progressBar_progress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button_cancel;
        internal System.Windows.Forms.Label labelName_total_progress;
        internal System.Windows.Forms.Label label_processing;
        internal System.Windows.Forms.Label labelName_process;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        public System.Windows.Forms.ProgressBar progressBar_total_progress;
        public System.Windows.Forms.ProgressBar progressBar_progress;
        public System.Windows.Forms.Label label_processing_message_3;
        public System.Windows.Forms.Label label_processing_message_2;
        public System.Windows.Forms.Label label_total_progress_percentage;
        public System.Windows.Forms.Label label_processing_message_1;
        public System.Windows.Forms.Label label_progress_percentage;
        public System.Windows.Forms.TextBox textBox_header_title;
        internal System.Windows.Forms.PictureBox pictureBox_progressBar_progress;
    }
}