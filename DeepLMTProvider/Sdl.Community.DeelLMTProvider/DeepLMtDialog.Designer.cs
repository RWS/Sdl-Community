namespace Sdl.Community.DeepLMTProvider
{
    partial class DeepLMtDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeepLMtDialog));
			this.mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
			this.headerLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.titlePanel = new System.Windows.Forms.Panel();
			this.subtitleLabel = new System.Windows.Forms.Label();
			this.titleLabel = new System.Windows.Forms.Label();
			this.logoPicture = new System.Windows.Forms.PictureBox();
			this.contentLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.contentInformationLabl = new System.Windows.Forms.Label();
			this.apiKeyLbl = new System.Windows.Forms.Label();
			this.apiKey = new System.Windows.Forms.TextBox();
			this.descriptionLbl = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.buttonsPanel = new System.Windows.Forms.Panel();
			this.cancelBtn = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.mainTableLayout.SuspendLayout();
			this.headerLayoutPanel.SuspendLayout();
			this.titlePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.logoPicture)).BeginInit();
			this.contentLayoutPanel.SuspendLayout();
			this.buttonsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainTableLayout
			// 
			this.mainTableLayout.AutoSize = true;
			this.mainTableLayout.ColumnCount = 1;
			this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.mainTableLayout.Controls.Add(this.headerLayoutPanel, 0, 0);
			this.mainTableLayout.Controls.Add(this.contentLayoutPanel, 0, 1);
			this.mainTableLayout.Controls.Add(this.buttonsPanel, 0, 2);
			this.mainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainTableLayout.Location = new System.Drawing.Point(0, 0);
			this.mainTableLayout.Margin = new System.Windows.Forms.Padding(2);
			this.mainTableLayout.Name = "mainTableLayout";
			this.mainTableLayout.RowCount = 3;
			this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
			this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
			this.mainTableLayout.Size = new System.Drawing.Size(522, 265);
			this.mainTableLayout.TabIndex = 0;
			// 
			// headerLayoutPanel
			// 
			this.headerLayoutPanel.AutoSize = true;
			this.headerLayoutPanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.headerLayoutPanel.ColumnCount = 2;
			this.headerLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
			this.headerLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
			this.headerLayoutPanel.Controls.Add(this.titlePanel, 0, 0);
			this.headerLayoutPanel.Controls.Add(this.logoPicture, 1, 0);
			this.headerLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.headerLayoutPanel.Location = new System.Drawing.Point(2, 2);
			this.headerLayoutPanel.Margin = new System.Windows.Forms.Padding(2);
			this.headerLayoutPanel.Name = "headerLayoutPanel";
			this.headerLayoutPanel.RowCount = 1;
			this.headerLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.headerLayoutPanel.Size = new System.Drawing.Size(518, 49);
			this.headerLayoutPanel.TabIndex = 0;
			// 
			// titlePanel
			// 
			this.titlePanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.titlePanel.Controls.Add(this.subtitleLabel);
			this.titlePanel.Controls.Add(this.titleLabel);
			this.titlePanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.titlePanel.Location = new System.Drawing.Point(2, 2);
			this.titlePanel.Margin = new System.Windows.Forms.Padding(2);
			this.titlePanel.Name = "titlePanel";
			this.titlePanel.Size = new System.Drawing.Size(358, 45);
			this.titlePanel.TabIndex = 0;
			// 
			// subtitleLabel
			// 
			this.subtitleLabel.AutoSize = true;
			this.subtitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.subtitleLabel.Location = new System.Drawing.Point(7, 16);
			this.subtitleLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.subtitleLabel.Name = "subtitleLabel";
			this.subtitleLabel.Size = new System.Drawing.Size(291, 15);
			this.subtitleLabel.TabIndex = 1;
			this.subtitleLabel.Text = "Specify your DeepL Translation API account settings.";
			// 
			// titleLabel
			// 
			this.titleLabel.AutoSize = true;
			this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.titleLabel.Location = new System.Drawing.Point(3, 1);
			this.titleLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Size = new System.Drawing.Size(107, 15);
			this.titleLabel.TabIndex = 0;
			this.titleLabel.Text = "DeepL Platform";
			// 
			// logoPicture
			// 
			this.logoPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.logoPicture.InitialImage = ((System.Drawing.Image)(resources.GetObject("logoPicture.InitialImage")));
			this.logoPicture.Location = new System.Drawing.Point(364, 2);
			this.logoPicture.Margin = new System.Windows.Forms.Padding(2);
			this.logoPicture.Name = "logoPicture";
			this.logoPicture.Size = new System.Drawing.Size(152, 45);
			this.logoPicture.TabIndex = 1;
			this.logoPicture.TabStop = false;
			// 
			// contentLayoutPanel
			// 
			this.contentLayoutPanel.ColumnCount = 2;
			this.contentLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.contentLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
			this.contentLayoutPanel.Controls.Add(this.contentInformationLabl, 0, 0);
			this.contentLayoutPanel.Controls.Add(this.apiKeyLbl, 0, 1);
			this.contentLayoutPanel.Controls.Add(this.apiKey, 1, 1);
			this.contentLayoutPanel.Controls.Add(this.descriptionLbl, 0, 2);
			this.contentLayoutPanel.Controls.Add(this.linkLabel1, 0, 3);
			this.contentLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.contentLayoutPanel.Location = new System.Drawing.Point(2, 55);
			this.contentLayoutPanel.Margin = new System.Windows.Forms.Padding(2);
			this.contentLayoutPanel.Name = "contentLayoutPanel";
			this.contentLayoutPanel.RowCount = 4;
			this.contentLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.contentLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 26.66667F));
			this.contentLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 26.66667F));
			this.contentLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 26.66667F));
			this.contentLayoutPanel.Size = new System.Drawing.Size(518, 168);
			this.contentLayoutPanel.TabIndex = 1;
			// 
			// contentInformationLabl
			// 
			this.contentInformationLabl.AutoSize = true;
			this.contentInformationLabl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.contentInformationLabl.Location = new System.Drawing.Point(2, 0);
			this.contentInformationLabl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.contentInformationLabl.Name = "contentInformationLabl";
			this.contentInformationLabl.Size = new System.Drawing.Size(0, 15);
			this.contentInformationLabl.TabIndex = 0;
			// 
			// apiKeyLbl
			// 
			this.apiKeyLbl.AutoSize = true;
			this.apiKeyLbl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.apiKeyLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.apiKeyLbl.Location = new System.Drawing.Point(2, 33);
			this.apiKeyLbl.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.apiKeyLbl.Name = "apiKeyLbl";
			this.apiKeyLbl.Size = new System.Drawing.Size(99, 44);
			this.apiKeyLbl.TabIndex = 1;
			this.apiKeyLbl.Text = "API Key *";
			// 
			// apiKey
			// 
			this.apiKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.apiKey.Location = new System.Drawing.Point(105, 35);
			this.apiKey.Margin = new System.Windows.Forms.Padding(2);
			this.apiKey.Name = "apiKey";
			this.apiKey.PasswordChar = '*';
			this.apiKey.Size = new System.Drawing.Size(411, 20);
			this.apiKey.TabIndex = 2;
			// 
			// descriptionLbl
			// 
			this.descriptionLbl.AutoSize = true;
			this.descriptionLbl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.descriptionLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.descriptionLbl.Location = new System.Drawing.Point(2, 83);
			this.descriptionLbl.Margin = new System.Windows.Forms.Padding(2, 6, 2, 0);
			this.descriptionLbl.Name = "descriptionLbl";
			this.descriptionLbl.Size = new System.Drawing.Size(99, 38);
			this.descriptionLbl.TabIndex = 3;
			this.descriptionLbl.Text = "This API Key will be used for all projects that use the DeepL Translation API";
			this.descriptionLbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
			this.linkLabel1.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(103)))), ((int)(((byte)(153)))), ((int)(((byte)(216)))));
			this.linkLabel1.Location = new System.Drawing.Point(2, 127);
			this.linkLabel1.Margin = new System.Windows.Forms.Padding(2, 6, 2, 0);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(99, 41);
			this.linkLabel1.TabIndex = 4;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "Click here to obtain your API Key from DeepL";
			this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// buttonsPanel
			// 
			this.buttonsPanel.Controls.Add(this.cancelBtn);
			this.buttonsPanel.Controls.Add(this.okButton);
			this.buttonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonsPanel.Location = new System.Drawing.Point(2, 227);
			this.buttonsPanel.Margin = new System.Windows.Forms.Padding(2);
			this.buttonsPanel.Name = "buttonsPanel";
			this.buttonsPanel.Size = new System.Drawing.Size(518, 36);
			this.buttonsPanel.TabIndex = 2;
			// 
			// cancelBtn
			// 
			this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cancelBtn.Location = new System.Drawing.Point(460, 8);
			this.cancelBtn.Margin = new System.Windows.Forms.Padding(2);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.Size = new System.Drawing.Size(53, 19);
			this.cancelBtn.TabIndex = 1;
			this.cancelBtn.Text = "Cancel";
			this.cancelBtn.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.okButton.Location = new System.Drawing.Point(401, 8);
			this.okButton.Margin = new System.Windows.Forms.Padding(2);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(55, 19);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// DeepLMtDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(522, 265);
			this.Controls.Add(this.mainTableLayout);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "DeepLMtDialog";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Add Connection to DeepL Translation API";
			this.TransparencyKey = System.Drawing.Color.Gray;
			this.mainTableLayout.ResumeLayout(false);
			this.mainTableLayout.PerformLayout();
			this.headerLayoutPanel.ResumeLayout(false);
			this.titlePanel.ResumeLayout(false);
			this.titlePanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.logoPicture)).EndInit();
			this.contentLayoutPanel.ResumeLayout(false);
			this.contentLayoutPanel.PerformLayout();
			this.buttonsPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

		#endregion

		private System.Windows.Forms.TableLayoutPanel mainTableLayout;
		private System.Windows.Forms.TableLayoutPanel headerLayoutPanel;
		private System.Windows.Forms.Panel titlePanel;
		private System.Windows.Forms.Label titleLabel;
		private System.Windows.Forms.Label subtitleLabel;
		private System.Windows.Forms.TableLayoutPanel contentLayoutPanel;
		private System.Windows.Forms.Label contentInformationLabl;
		private System.Windows.Forms.Label apiKeyLbl;
		private System.Windows.Forms.TextBox apiKey;
		private System.Windows.Forms.Label descriptionLbl;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Panel buttonsPanel;
		private System.Windows.Forms.Button cancelBtn;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.PictureBox logoPicture;
	}
}