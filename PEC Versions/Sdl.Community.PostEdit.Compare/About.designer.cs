namespace PostEdit.Compare
{
    partial class About
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.labelCompanyName = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.linkLabelpostEditCompareNameAndVersion = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
            this.logoPictureBox.Location = new System.Drawing.Point(8, 8);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new System.Drawing.Size(248, 236);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.logoPictureBox.TabIndex = 13;
            this.logoPictureBox.TabStop = false;
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(271, 110);
            this.textBoxDescription.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.textBoxDescription.Multiline = true;
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxDescription.Size = new System.Drawing.Size(422, 134);
            this.textBoxDescription.TabIndex = 24;
            this.textBoxDescription.TabStop = false;
            this.textBoxDescription.Text = "Description";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Location = new System.Drawing.Point(618, 250);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 25);
            this.okButton.TabIndex = 25;
            this.okButton.Text = "&OK";
            // 
            // labelVersion
            // 
            this.labelVersion.Location = new System.Drawing.Point(271, 41);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(422, 18);
            this.labelVersion.TabIndex = 27;
            this.labelVersion.Text = "labelVersion";
            // 
            // labelCopyright
            // 
            this.labelCopyright.Location = new System.Drawing.Point(271, 64);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(422, 18);
            this.labelCopyright.TabIndex = 28;
            this.labelCopyright.Text = "labelCopyright";
            // 
            // labelCompanyName
            // 
            this.labelCompanyName.Location = new System.Drawing.Point(271, 89);
            this.labelCompanyName.Name = "labelCompanyName";
            this.labelCompanyName.Size = new System.Drawing.Size(422, 18);
            this.labelCompanyName.TabIndex = 29;
            this.labelCompanyName.Text = "labelCompanyName";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "PostEdit.Compare.ico");
            // 
            // linkLabelpostEditCompareNameAndVersion
            // 
            this.linkLabelpostEditCompareNameAndVersion.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabelpostEditCompareNameAndVersion.ImageIndex = 0;
            this.linkLabelpostEditCompareNameAndVersion.ImageList = this.imageList1;
            this.linkLabelpostEditCompareNameAndVersion.Location = new System.Drawing.Point(271, 10);
            this.linkLabelpostEditCompareNameAndVersion.Name = "linkLabelpostEditCompareNameAndVersion";
            this.linkLabelpostEditCompareNameAndVersion.Padding = new System.Windows.Forms.Padding(25, 0, 0, 0);
            this.linkLabelpostEditCompareNameAndVersion.Size = new System.Drawing.Size(422, 23);
            this.linkLabelpostEditCompareNameAndVersion.TabIndex = 31;
            this.linkLabelpostEditCompareNameAndVersion.TabStop = true;
            this.linkLabelpostEditCompareNameAndVersion.Text = "Post-Edit Compare";
            this.linkLabelpostEditCompareNameAndVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabelpostEditCompareNameAndVersion.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelpostEditCompareNameAndVersion_LinkClicked);
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 282);
            this.Controls.Add(this.linkLabelpostEditCompareNameAndVersion);
            this.Controls.Add(this.labelCompanyName);
            this.Controls.Add(this.labelCopyright);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.logoPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About Post-Edit Compare for SDL Trados Studio 2015";
            this.Load += new System.EventHandler(this.About_Load);
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.Label labelCompanyName;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.LinkLabel linkLabelpostEditCompareNameAndVersion;
    }
}