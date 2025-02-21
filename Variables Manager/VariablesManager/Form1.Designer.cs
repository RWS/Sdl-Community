namespace VariablesManager
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtLRT = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.btnBrowseLRT = new System.Windows.Forms.Button();
			this.btnBrowseTM = new System.Windows.Forms.Button();
			this.txtTM = new System.Windows.Forms.TextBox();
			this.txtVariables = new System.Windows.Forms.TextBox();
			this.btnImportFromFile = new System.Windows.Forms.Button();
			this.btnExportToFile = new System.Windows.Forms.Button();
			this.btnFetchList = new System.Windows.Forms.Button();
			this.btnAddToTMorLRT = new System.Windows.Forms.Button();
			this.btnReplateToTMorLRT = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.btnClear = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(293, 330);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(234, 99);
			this.label1.TabIndex = 0;
			this.label1.Text = "To edit an existing list in your Language Resources click on \"fetch\" and then edi" +
    "t as required before replacing it.  Alternatively use \"export to file\", edit in " +
    "a text editor and then import as above.";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(293, 244);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(234, 81);
			this.label2.TabIndex = 1;
			this.label2.Text = "To use simply copy and paste your variable list into the window above, or import " +
    "from file, and then \"add/replace\" to the list in your language resources as need" +
    "ed.";
			// 
			// txtLRT
			// 
			this.txtLRT.Location = new System.Drawing.Point(15, 64);
			this.txtLRT.Name = "txtLRT";
			this.txtLRT.Size = new System.Drawing.Size(234, 20);
			this.txtLRT.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(151, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Language Resource Template";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 9);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(124, 13);
			this.label4.TabIndex = 4;
			this.label4.Text = "Translation Memory (TM)";
			// 
			// btnBrowseLRT
			// 
			this.btnBrowseLRT.Location = new System.Drawing.Point(255, 62);
			this.btnBrowseLRT.Name = "btnBrowseLRT";
			this.btnBrowseLRT.Size = new System.Drawing.Size(26, 22);
			this.btnBrowseLRT.TabIndex = 5;
			this.btnBrowseLRT.Text = "...";
			this.btnBrowseLRT.UseVisualStyleBackColor = true;
			this.btnBrowseLRT.Click += new System.EventHandler(this.btnBrowseLRT_Click);
			// 
			// btnBrowseTM
			// 
			this.btnBrowseTM.Location = new System.Drawing.Point(255, 25);
			this.btnBrowseTM.Name = "btnBrowseTM";
			this.btnBrowseTM.Size = new System.Drawing.Size(26, 22);
			this.btnBrowseTM.TabIndex = 7;
			this.btnBrowseTM.Text = "...";
			this.btnBrowseTM.UseVisualStyleBackColor = true;
			this.btnBrowseTM.Click += new System.EventHandler(this.btnBrowseTM_Click);
			// 
			// txtTM
			// 
			this.txtTM.Location = new System.Drawing.Point(15, 25);
			this.txtTM.Name = "txtTM";
			this.txtTM.Size = new System.Drawing.Size(234, 20);
			this.txtTM.TabIndex = 6;
			// 
			// txtVariables
			// 
			this.txtVariables.Location = new System.Drawing.Point(296, 12);
			this.txtVariables.MaxLength = 2147483646;
			this.txtVariables.Multiline = true;
			this.txtVariables.Name = "txtVariables";
			this.txtVariables.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtVariables.Size = new System.Drawing.Size(230, 228);
			this.txtVariables.TabIndex = 8;
			// 
			// btnImportFromFile
			// 
			this.btnImportFromFile.Location = new System.Drawing.Point(15, 250);
			this.btnImportFromFile.Name = "btnImportFromFile";
			this.btnImportFromFile.Size = new System.Drawing.Size(266, 30);
			this.btnImportFromFile.TabIndex = 9;
			this.btnImportFromFile.Text = "Import from file";
			this.btnImportFromFile.UseVisualStyleBackColor = true;
			this.btnImportFromFile.Click += new System.EventHandler(this.btnImportFromFile_Click);
			// 
			// btnExportToFile
			// 
			this.btnExportToFile.Location = new System.Drawing.Point(15, 286);
			this.btnExportToFile.Name = "btnExportToFile";
			this.btnExportToFile.Size = new System.Drawing.Size(266, 30);
			this.btnExportToFile.TabIndex = 10;
			this.btnExportToFile.Text = "Export to file";
			this.btnExportToFile.UseVisualStyleBackColor = true;
			this.btnExportToFile.Click += new System.EventHandler(this.btnExportToFile_Click);
			// 
			// btnFetchList
			// 
			this.btnFetchList.Location = new System.Drawing.Point(15, 322);
			this.btnFetchList.Name = "btnFetchList";
			this.btnFetchList.Size = new System.Drawing.Size(266, 30);
			this.btnFetchList.TabIndex = 11;
			this.btnFetchList.Text = "Fetch variables list";
			this.btnFetchList.UseVisualStyleBackColor = true;
			this.btnFetchList.Click += new System.EventHandler(this.btnFetchList_Click);
			// 
			// btnAddToTMorLRT
			// 
			this.btnAddToTMorLRT.Location = new System.Drawing.Point(15, 358);
			this.btnAddToTMorLRT.Name = "btnAddToTMorLRT";
			this.btnAddToTMorLRT.Size = new System.Drawing.Size(266, 30);
			this.btnAddToTMorLRT.TabIndex = 12;
			this.btnAddToTMorLRT.Text = "Add to variables list";
			this.btnAddToTMorLRT.UseVisualStyleBackColor = true;
			this.btnAddToTMorLRT.Click += new System.EventHandler(this.btnAddToTMorLRT_Click);
			// 
			// btnReplateToTMorLRT
			// 
			this.btnReplateToTMorLRT.Location = new System.Drawing.Point(15, 394);
			this.btnReplateToTMorLRT.Name = "btnReplateToTMorLRT";
			this.btnReplateToTMorLRT.Size = new System.Drawing.Size(266, 30);
			this.btnReplateToTMorLRT.TabIndex = 13;
			this.btnReplateToTMorLRT.Text = "Replace variables list";
			this.btnReplateToTMorLRT.UseVisualStyleBackColor = true;
			this.btnReplateToTMorLRT.Click += new System.EventHandler(this.btnReplateToTMorLRT_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(70, 103);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(161, 94);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBox1.TabIndex = 14;
			this.pictureBox1.TabStop = false;
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(15, 214);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(266, 30);
			this.btnClear.TabIndex = 15;
			this.btnClear.Text = "Clear variables list";
			this.btnClear.UseVisualStyleBackColor = true;
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(538, 434);
			this.Controls.Add(this.btnClear);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.btnReplateToTMorLRT);
			this.Controls.Add(this.btnAddToTMorLRT);
			this.Controls.Add(this.btnFetchList);
			this.Controls.Add(this.btnExportToFile);
			this.Controls.Add(this.btnImportFromFile);
			this.Controls.Add(this.txtVariables);
			this.Controls.Add(this.btnBrowseTM);
			this.Controls.Add(this.txtTM);
			this.Controls.Add(this.btnBrowseLRT);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtLRT);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Variables Manager for Trados Studio";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLRT;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnBrowseLRT;
        private System.Windows.Forms.Button btnBrowseTM;
        private System.Windows.Forms.TextBox txtTM;
        private System.Windows.Forms.TextBox txtVariables;
        private System.Windows.Forms.Button btnImportFromFile;
        private System.Windows.Forms.Button btnExportToFile;
        private System.Windows.Forms.Button btnFetchList;
        private System.Windows.Forms.Button btnAddToTMorLRT;
        private System.Windows.Forms.Button btnReplateToTMorLRT;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Button btnClear;
    }
}

