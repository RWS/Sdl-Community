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
            this.mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.headerLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.titlePanel = new System.Windows.Forms.Panel();
            this.subtitleLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.contentLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.contentInformationLabl = new System.Windows.Forms.Label();
            this.apiKeyLbl = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.descriptionLbl = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.mainTableLayout.SuspendLayout();
            this.headerLayoutPanel.SuspendLayout();
            this.titlePanel.SuspendLayout();
            this.contentLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTableLayout
            // 
            this.mainTableLayout.AutoSize = true;
            this.mainTableLayout.ColumnCount = 1;
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayout.Controls.Add(this.headerLayoutPanel, 0, 0);
            this.mainTableLayout.Controls.Add(this.contentLayoutPanel, 0, 1);
            this.mainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayout.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayout.Name = "mainTableLayout";
            this.mainTableLayout.RowCount = 3;
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainTableLayout.Size = new System.Drawing.Size(284, 261);
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
            this.headerLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.headerLayoutPanel.Name = "headerLayoutPanel";
            this.headerLayoutPanel.RowCount = 1;
            this.headerLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayoutPanel.Size = new System.Drawing.Size(278, 114);
            this.headerLayoutPanel.TabIndex = 0;
            // 
            // titlePanel
            // 
            this.titlePanel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.titlePanel.Controls.Add(this.subtitleLabel);
            this.titlePanel.Controls.Add(this.titleLabel);
            this.titlePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titlePanel.Location = new System.Drawing.Point(3, 3);
            this.titlePanel.Name = "titlePanel";
            this.titlePanel.Size = new System.Drawing.Size(188, 108);
            this.titlePanel.TabIndex = 0;
            // 
            // subtitleLabel
            // 
            this.subtitleLabel.AutoSize = true;
            this.subtitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.subtitleLabel.Location = new System.Drawing.Point(32, 52);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Size = new System.Drawing.Size(290, 15);
            this.subtitleLabel.TabIndex = 1;
            this.subtitleLabel.Text = "Specify yout DeepL Translation API account settings.";
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(7, 19);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(107, 15);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "DeepL Platform";
            // 
            // contentLayoutPanel
            // 
            this.contentLayoutPanel.ColumnCount = 2;
            this.contentLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.contentLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.contentLayoutPanel.Controls.Add(this.contentInformationLabl, 0, 0);
            this.contentLayoutPanel.Controls.Add(this.apiKeyLbl, 0, 1);
            this.contentLayoutPanel.Controls.Add(this.textBox1, 1, 1);
            this.contentLayoutPanel.Controls.Add(this.descriptionLbl, 0, 2);
            this.contentLayoutPanel.Controls.Add(this.linkLabel1, 0, 3);
            this.contentLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentLayoutPanel.Location = new System.Drawing.Point(3, 123);
            this.contentLayoutPanel.Name = "contentLayoutPanel";
            this.contentLayoutPanel.RowCount = 4;
            this.contentLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.contentLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.contentLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.contentLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.contentLayoutPanel.Size = new System.Drawing.Size(278, 114);
            this.contentLayoutPanel.TabIndex = 1;
            // 
            // contentInformationLabl
            // 
            this.contentInformationLabl.AutoSize = true;
            this.contentInformationLabl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contentInformationLabl.Location = new System.Drawing.Point(9, 2);
            this.contentInformationLabl.Name = "contentInformationLabl";
            this.contentInformationLabl.Size = new System.Drawing.Size(51, 20);
            this.contentInformationLabl.TabIndex = 0;
            // 
            // apiKeyLbl
            // 
            this.apiKeyLbl.AutoSize = true;
            this.apiKeyLbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.apiKeyLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.apiKeyLbl.Location = new System.Drawing.Point(3, 26);
            this.apiKeyLbl.Name = "apiKeyLbl";
            this.apiKeyLbl.Size = new System.Drawing.Size(51, 20);
            this.apiKeyLbl.TabIndex = 1;
            this.apiKeyLbl.Text = "API Key*";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(167, 23);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 26);
            this.textBox1.TabIndex = 2;
            // 
            // descriptionLbl
            // 
            this.descriptionLbl.AutoSize = true;
            this.descriptionLbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.descriptionLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.descriptionLbl.Location = new System.Drawing.Point(10, 42);
            this.descriptionLbl.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.descriptionLbl.Name = "descriptionLbl";
            this.descriptionLbl.Size = new System.Drawing.Size(51, 20);
            this.descriptionLbl.TabIndex = 3;
            this.descriptionLbl.Text = "This API Key will be used for all projects that use DeepL Translation API";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.Location = new System.Drawing.Point(13, 82);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(80, 20);
            this.linkLabel1.TabIndex = 4;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Click here to obtain your API Key from DeepL";
            // 
            // DeepLMtDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.mainTableLayout);
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
            this.contentLayoutPanel.ResumeLayout(false);
            this.contentLayoutPanel.PerformLayout();
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
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label descriptionLbl;
		private System.Windows.Forms.LinkLabel linkLabel1;
	}
}