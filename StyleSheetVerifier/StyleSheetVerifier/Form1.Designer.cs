namespace Sdl.Community.StyleSheetVerifier
{
    partial class Form1
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.webBrowser1 = new System.Windows.Forms.WebBrowser();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.txtSheet = new System.Windows.Forms.TextBox();
			this.btnBrowseSheet = new System.Windows.Forms.Button();
			this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
			this.txtXml = new System.Windows.Forms.TextBox();
			this.btnBrowseXml = new System.Windows.Forms.Button();
			this.btnPreview = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.flowLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75F));
			this.tableLayoutPanel1.Controls.Add(this.webBrowser1, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.btnPreview, 1, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(845, 599);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// webBrowser1
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.webBrowser1, 2);
			this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowser1.Location = new System.Drawing.Point(3, 108);
			this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser1.Name = "webBrowser1";
			this.webBrowser1.Size = new System.Drawing.Size(839, 488);
			this.webBrowser1.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(5, 11);
			this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 3, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Stylesheet";
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(5, 46);
			this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 3, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Xml file";
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this.txtSheet);
			this.flowLayoutPanel1.Controls.Add(this.btnBrowseSheet);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(214, 3);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(628, 29);
			this.flowLayoutPanel1.TabIndex = 3;
			// 
			// txtSheet
			// 
			this.txtSheet.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtSheet.Location = new System.Drawing.Point(3, 3);
			this.txtSheet.Name = "txtSheet";
			this.txtSheet.Size = new System.Drawing.Size(552, 20);
			this.txtSheet.TabIndex = 0;
			// 
			// btnBrowseSheet
			// 
			this.btnBrowseSheet.Location = new System.Drawing.Point(561, 3);
			this.btnBrowseSheet.Name = "btnBrowseSheet";
			this.btnBrowseSheet.Size = new System.Drawing.Size(56, 23);
			this.btnBrowseSheet.TabIndex = 1;
			this.btnBrowseSheet.Text = "...";
			this.btnBrowseSheet.UseVisualStyleBackColor = true;
			this.btnBrowseSheet.Click += new System.EventHandler(this.btnBrowseSheet_Click);
			// 
			// flowLayoutPanel2
			// 
			this.flowLayoutPanel2.Controls.Add(this.txtXml);
			this.flowLayoutPanel2.Controls.Add(this.btnBrowseXml);
			this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel2.Location = new System.Drawing.Point(214, 38);
			this.flowLayoutPanel2.Name = "flowLayoutPanel2";
			this.flowLayoutPanel2.Size = new System.Drawing.Size(628, 29);
			this.flowLayoutPanel2.TabIndex = 4;
			// 
			// txtXml
			// 
			this.txtXml.Location = new System.Drawing.Point(3, 3);
			this.txtXml.Name = "txtXml";
			this.txtXml.Size = new System.Drawing.Size(552, 20);
			this.txtXml.TabIndex = 0;
			// 
			// btnBrowseXml
			// 
			this.btnBrowseXml.Location = new System.Drawing.Point(561, 3);
			this.btnBrowseXml.Name = "btnBrowseXml";
			this.btnBrowseXml.Size = new System.Drawing.Size(56, 23);
			this.btnBrowseXml.TabIndex = 2;
			this.btnBrowseXml.Text = "...";
			this.btnBrowseXml.UseVisualStyleBackColor = true;
			this.btnBrowseXml.Click += new System.EventHandler(this.btnBrowseXml_Click);
			// 
			// btnPreview
			// 
			this.btnPreview.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnPreview.Location = new System.Drawing.Point(755, 76);
			this.btnPreview.Margin = new System.Windows.Forms.Padding(3, 3, 15, 3);
			this.btnPreview.Name = "btnPreview";
			this.btnPreview.Size = new System.Drawing.Size(75, 23);
			this.btnPreview.TabIndex = 5;
			this.btnPreview.Text = "Preview";
			this.btnPreview.UseVisualStyleBackColor = true;
			this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(845, 599);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Stylesheet preview";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.flowLayoutPanel2.ResumeLayout(false);
			this.flowLayoutPanel2.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TextBox txtSheet;
        private System.Windows.Forms.Button btnBrowseSheet;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.TextBox txtXml;
        private System.Windows.Forms.Button btnBrowseXml;
        private System.Windows.Forms.Button btnPreview;
    }
}

