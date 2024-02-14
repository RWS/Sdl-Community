namespace TradosPlugin
{
    public partial class ProviderItemControl
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
            this.components = new System.ComponentModel.Container();
            this.lblProviderName = new System.Windows.Forms.Label();
            this.pctDelete = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pctWarning = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pctDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctWarning)).BeginInit();
            this.SuspendLayout();
            // 
            // lblProviderName
            // 
            this.lblProviderName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProviderName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblProviderName.Location = new System.Drawing.Point(2, 4);
            this.lblProviderName.Name = "lblProviderName";
            this.lblProviderName.Size = new System.Drawing.Size(186, 20);
            this.lblProviderName.TabIndex = 1;
            this.lblProviderName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pctDelete
            // 
            this.pctDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pctDelete.BackgroundImage = global::TradosPlugin.Properties.Resources.Archive;
            this.pctDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pctDelete.Location = new System.Drawing.Point(219, 2);
            this.pctDelete.MaximumSize = new System.Drawing.Size(32, 32);
            this.pctDelete.Name = "pctDelete";
            this.pctDelete.Size = new System.Drawing.Size(24, 24);
            this.pctDelete.TabIndex = 6;
            this.pctDelete.TabStop = false;
            this.pctDelete.Visible = false;
            this.pctDelete.Click += new System.EventHandler(this.pctDelete_Click);
            // 
            // pctWarning
            // 
            this.pctWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pctWarning.BackgroundImage = global::TradosPlugin.Properties.Resources.warningIcon16;
            this.pctWarning.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pctWarning.Location = new System.Drawing.Point(194, 2);
            this.pctWarning.MaximumSize = new System.Drawing.Size(32, 32);
            this.pctWarning.Name = "pctWarning";
            this.pctWarning.Size = new System.Drawing.Size(24, 24);
            this.pctWarning.TabIndex = 7;
            this.pctWarning.TabStop = false;
            this.pctWarning.Visible = false;
            // 
            // ProviderItemControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.pctWarning);
            this.Controls.Add(this.pctDelete);
            this.Controls.Add(this.lblProviderName);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Name = "ProviderItemControl";
            this.Size = new System.Drawing.Size(246, 28);
            ((System.ComponentModel.ISupportInitialize)(this.pctDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pctWarning)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblProviderName;
        private System.Windows.Forms.PictureBox pctDelete;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox pctWarning;
    }
}
