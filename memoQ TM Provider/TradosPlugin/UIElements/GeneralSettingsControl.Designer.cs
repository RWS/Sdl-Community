namespace TradosPlugin
{
    partial class GeneralSettingsControl
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
            this.chkConcNumericEquiv = new System.Windows.Forms.CheckBox();
            this.chkConcCaseSens = new System.Windows.Forms.CheckBox();
            this.pnlHeader = new TradosPlugin.GradientPanel();
            this.lblGeneralSettings = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkConcNumericEquiv
            // 
            this.chkConcNumericEquiv.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkConcNumericEquiv.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkConcNumericEquiv.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkConcNumericEquiv.Location = new System.Drawing.Point(9, 76);
            this.chkConcNumericEquiv.Name = "chkConcNumericEquiv";
            this.chkConcNumericEquiv.Size = new System.Drawing.Size(232, 43);
            this.chkConcNumericEquiv.TabIndex = 41;
            this.chkConcNumericEquiv.Text = "Numeric equivalence in concordance searches";
            this.chkConcNumericEquiv.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkConcNumericEquiv.UseVisualStyleBackColor = true;
            // 
            // chkConcCaseSens
            // 
            this.chkConcCaseSens.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkConcCaseSens.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkConcCaseSens.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkConcCaseSens.Location = new System.Drawing.Point(9, 34);
            this.chkConcCaseSens.Name = "chkConcCaseSens";
            this.chkConcCaseSens.Size = new System.Drawing.Size(232, 40);
            this.chkConcCaseSens.TabIndex = 42;
            this.chkConcCaseSens.Text = "Concordance searches are case sensitive";
            this.chkConcCaseSens.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkConcCaseSens.UseVisualStyleBackColor = true;
            // 
            // pnlHeader
            // 
            this.pnlHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(99)))), ((int)(((byte)(130)))));
            this.pnlHeader.BackEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(99)))), ((int)(((byte)(130)))));
            this.pnlHeader.BackStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(99)))), ((int)(((byte)(130)))));
            this.pnlHeader.Controls.Add(this.lblGeneralSettings);
            this.pnlHeader.GradientMode = TradosPlugin.GradientPanel.GradientModes.TwoPoint;
            this.pnlHeader.GradientOrientation = System.Windows.Forms.Orientation.Horizontal;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(249, 28);
            this.pnlHeader.TabIndex = 43;
            this.pnlHeader.Click += new System.EventHandler(this.gradientPanel4_Click);
            // 
            // lblGeneralSettings
            // 
            this.lblGeneralSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGeneralSettings.BackColor = System.Drawing.Color.Transparent;
            this.lblGeneralSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.lblGeneralSettings.ForeColor = System.Drawing.Color.White;
            this.lblGeneralSettings.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblGeneralSettings.Location = new System.Drawing.Point(2, 6);
            this.lblGeneralSettings.Name = "lblGeneralSettings";
            this.lblGeneralSettings.Size = new System.Drawing.Size(244, 17);
            this.lblGeneralSettings.TabIndex = 0;
            this.lblGeneralSettings.Text = "general tm settings";
            this.lblGeneralSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblGeneralSettings.Click += new System.EventHandler(this.lblGeneralSettings_Click);
            // 
            // GeneralSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.chkConcNumericEquiv);
            this.Controls.Add(this.chkConcCaseSens);
            this.Name = "GeneralSettingsControl";
            this.Size = new System.Drawing.Size(249, 130);
            this.pnlHeader.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GradientPanel pnlHeader;
        private System.Windows.Forms.Label lblGeneralSettings;
        private System.Windows.Forms.CheckBox chkConcNumericEquiv;
        private System.Windows.Forms.CheckBox chkConcCaseSens;
    }
}
