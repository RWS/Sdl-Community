using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Community.Qualitivity.Progress
{
    partial class ProgressDialog
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressDialog));
            this.current_progress_percentage = new System.Windows.Forms.Label();
            this.progressBar_current = new System.Windows.Forms.ProgressBar();
            this.current_progress_value_message = new System.Windows.Forms.Label();
            this.current_processing_message = new System.Windows.Forms.Label();
            this.button_close = new System.Windows.Forms.Button();
            this.total_progress_value_message = new System.Windows.Forms.Label();
            this.progressBar_total = new System.Windows.Forms.ProgressBar();
            this.total_progress_percentage = new System.Windows.Forms.Label();
            this.total_progress_title = new System.Windows.Forms.Label();
            this.current_progress_title = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // current_progress_percentage
            // 
            this.current_progress_percentage.AutoSize = true;
            this.current_progress_percentage.Location = new System.Drawing.Point(459, 168);
            this.current_progress_percentage.Name = "current_progress_percentage";
            this.current_progress_percentage.Size = new System.Drawing.Size(21, 13);
            this.current_progress_percentage.TabIndex = 14;
            this.current_progress_percentage.Text = "0%";
            // 
            // progressBar_current
            // 
            this.progressBar_current.Location = new System.Drawing.Point(27, 161);
            this.progressBar_current.Name = "progressBar_current";
            this.progressBar_current.Size = new System.Drawing.Size(424, 23);
            this.progressBar_current.TabIndex = 13;
            // 
            // current_progress_value_message
            // 
            this.current_progress_value_message.AutoSize = true;
            this.current_progress_value_message.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.current_progress_value_message.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.current_progress_value_message.Location = new System.Drawing.Point(27, 187);
            this.current_progress_value_message.Name = "current_progress_value_message";
            this.current_progress_value_message.Size = new System.Drawing.Size(119, 13);
            this.current_progress_value_message.TabIndex = 12;
            this.current_progress_value_message.Text = "Updated 0 of 0 records.";
            // 
            // current_processing_message
            // 
            this.current_processing_message.Location = new System.Drawing.Point(27, 19);
            this.current_processing_message.Name = "current_processing_message";
            this.current_processing_message.Size = new System.Drawing.Size(459, 46);
            this.current_processing_message.TabIndex = 15;
            this.current_processing_message.Text = "Creating a new project activity; the activity records are being saved to the data" +
    "base.\r\nPlease wait until processing has completed.";
            // 
            // button_close
            // 
            this.button_close.Enabled = false;
            this.button_close.Location = new System.Drawing.Point(405, 209);
            this.button_close.Name = "button_close";
            this.button_close.Size = new System.Drawing.Size(75, 23);
            this.button_close.TabIndex = 16;
            this.button_close.Text = "Close";
            this.button_close.UseVisualStyleBackColor = true;
            // 
            // total_progress_value_message
            // 
            this.total_progress_value_message.AutoSize = true;
            this.total_progress_value_message.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.total_progress_value_message.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.total_progress_value_message.Location = new System.Drawing.Point(27, 110);
            this.total_progress_value_message.Name = "total_progress_value_message";
            this.total_progress_value_message.Size = new System.Drawing.Size(133, 13);
            this.total_progress_value_message.TabIndex = 12;
            this.total_progress_value_message.Text = "Updated 0 of 0 documents";
            // 
            // progressBar_total
            // 
            this.progressBar_total.Location = new System.Drawing.Point(27, 84);
            this.progressBar_total.Name = "progressBar_total";
            this.progressBar_total.Size = new System.Drawing.Size(424, 23);
            this.progressBar_total.TabIndex = 13;
            // 
            // total_progress_percentage
            // 
            this.total_progress_percentage.AutoSize = true;
            this.total_progress_percentage.Location = new System.Drawing.Point(459, 91);
            this.total_progress_percentage.Name = "total_progress_percentage";
            this.total_progress_percentage.Size = new System.Drawing.Size(21, 13);
            this.total_progress_percentage.TabIndex = 14;
            this.total_progress_percentage.Text = "0%";
            // 
            // total_progress_title
            // 
            this.total_progress_title.AutoSize = true;
            this.total_progress_title.Location = new System.Drawing.Point(27, 67);
            this.total_progress_title.Name = "total_progress_title";
            this.total_progress_title.Size = new System.Drawing.Size(78, 13);
            this.total_progress_title.TabIndex = 12;
            this.total_progress_title.Text = "Total Progress:";
            // 
            // current_progress_title
            // 
            this.current_progress_title.AutoSize = true;
            this.current_progress_title.Location = new System.Drawing.Point(27, 144);
            this.current_progress_title.Name = "current_progress_title";
            this.current_progress_title.Size = new System.Drawing.Size(89, 13);
            this.current_progress_title.TabIndex = 12;
            this.current_progress_title.Text = "Record Progress:";
            // 
            // ProgressDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 244);
            this.Controls.Add(this.button_close);
            this.Controls.Add(this.current_processing_message);
            this.Controls.Add(this.total_progress_percentage);
            this.Controls.Add(this.current_progress_percentage);
            this.Controls.Add(this.progressBar_total);
            this.Controls.Add(this.total_progress_title);
            this.Controls.Add(this.total_progress_value_message);
            this.Controls.Add(this.progressBar_current);
            this.Controls.Add(this.current_progress_title);
            this.Controls.Add(this.current_progress_value_message);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Progress...";
            this.Load += new System.EventHandler(this.ProgressDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public Label current_progress_percentage;
        public ProgressBar progressBar_current;
        public Label current_progress_value_message;
        public Label current_processing_message;
        private Button button_close;
        public Label total_progress_value_message;
        public ProgressBar progressBar_total;
        public Label total_progress_percentage;
        public Label total_progress_title;
        public Label current_progress_title;
    }
}