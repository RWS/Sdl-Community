namespace Sdl.Community.TMRepair
{
    partial class SDLTMRepair
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SDLTMRepair));
            this.txtFile = new System.Windows.Forms.TextBox();
            this.btnSelectTM = new System.Windows.Forms.Button();
            this.btnIntegrityCheck = new System.Windows.Forms.Button();
            this.btnRepair = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtFile
            // 
            this.txtFile.Location = new System.Drawing.Point(12, 12);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(317, 20);
            this.txtFile.TabIndex = 0;
            // 
            // btnSelectTM
            // 
            this.btnSelectTM.Location = new System.Drawing.Point(335, 12);
            this.btnSelectTM.Name = "btnSelectTM";
            this.btnSelectTM.Size = new System.Drawing.Size(75, 23);
            this.btnSelectTM.TabIndex = 1;
            this.btnSelectTM.Text = "Select TM";
            this.btnSelectTM.UseVisualStyleBackColor = true;
            this.btnSelectTM.Click += new System.EventHandler(this.btnSelectTM_Click);
            // 
            // btnIntegrityCheck
            // 
            this.btnIntegrityCheck.Location = new System.Drawing.Point(12, 207);
            this.btnIntegrityCheck.Name = "btnIntegrityCheck";
            this.btnIntegrityCheck.Size = new System.Drawing.Size(131, 23);
            this.btnIntegrityCheck.TabIndex = 2;
            this.btnIntegrityCheck.Text = "Integrity Check";
            this.btnIntegrityCheck.UseVisualStyleBackColor = true;
            this.btnIntegrityCheck.Click += new System.EventHandler(this.btnIntegrityCheck_Click);
            // 
            // btnRepair
            // 
            this.btnRepair.Location = new System.Drawing.Point(12, 236);
            this.btnRepair.Name = "btnRepair";
            this.btnRepair.Size = new System.Drawing.Size(131, 23);
            this.btnRepair.TabIndex = 3;
            this.btnRepair.Text = "Repair";
            this.btnRepair.UseVisualStyleBackColor = true;
            this.btnRepair.Click += new System.EventHandler(this.btnRepair_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(149, 129);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Repair";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(149, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(261, 85);
            this.label2.TabIndex = 5;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(149, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(261, 47);
            this.label3.TabIndex = 6;
            this.label3.Text = "Select your SDLTM and click on \"Integrity Check\" to simply generate a simple repo" +
    "rt identify wheater there is a problem or not.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(149, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "Integrity Check";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(149, 244);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(165, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Note: Success is not guaranteed!";
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(12, 265);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(394, 187);
            this.txtLog.TabIndex = 9;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(24, 74);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(107, 94);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // SDLTMRepair
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 464);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRepair);
            this.Controls.Add(this.btnIntegrityCheck);
            this.Controls.Add(this.btnSelectTM);
            this.Controls.Add(this.txtFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SDLTMRepair";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SDLTM Repair";
            this.Load += new System.EventHandler(this.SDLTMRepair_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.Button btnSelectTM;
        private System.Windows.Forms.Button btnIntegrityCheck;
        private System.Windows.Forms.Button btnRepair;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

