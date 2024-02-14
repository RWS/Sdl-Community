namespace TradosPlugin
{
    partial class StartingPanelControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblStartText = new System.Windows.Forms.Label();
            this.pbLT = new System.Windows.Forms.PictureBox();
            this.pbServer = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbLT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbServer)).BeginInit();
            this.SuspendLayout();
            // 
            // lblStartText
            // 
            this.lblStartText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStartText.BackColor = System.Drawing.SystemColors.Window;
            this.lblStartText.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblStartText.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblStartText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblStartText.Location = new System.Drawing.Point(43, 28);
            this.lblStartText.Name = "lblStartText";
            this.lblStartText.Size = new System.Drawing.Size(414, 88);
            this.lblStartText.TabIndex = 43;
            this.lblStartText.Text = "click here to...";
            this.lblStartText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbLT
            // 
            this.pbLT.Image = global::TradosPlugin.Properties.Resources.Language_Terminal_100px_width;
            this.pbLT.Location = new System.Drawing.Point(308, 156);
            this.pbLT.Name = "pbLT";
            this.pbLT.Size = new System.Drawing.Size(100, 24);
            this.pbLT.TabIndex = 45;
            this.pbLT.TabStop = false;
            this.pbLT.Click += new System.EventHandler(this.pbLT_Click);
            // 
            // pbServer
            // 
            this.pbServer.Image = global::TradosPlugin.Properties.Resources.memoQ_Server_100px_width;
            this.pbServer.Location = new System.Drawing.Point(92, 148);
            this.pbServer.Name = "pbServer";
            this.pbServer.Size = new System.Drawing.Size(100, 32);
            this.pbServer.TabIndex = 44;
            this.pbServer.TabStop = false;
            this.pbServer.Click += new System.EventHandler(this.pbServer_Click);
            // 
            // StartingPanelControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pbLT);
            this.Controls.Add(this.pbServer);
            this.Controls.Add(this.lblStartText);
            this.Name = "StartingPanelControl";
            this.Size = new System.Drawing.Size(500, 240);
            ((System.ComponentModel.ISupportInitialize)(this.pbLT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbServer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblStartText;
        private System.Windows.Forms.PictureBox pbServer;
        private System.Windows.Forms.PictureBox pbLT;

    }
}
