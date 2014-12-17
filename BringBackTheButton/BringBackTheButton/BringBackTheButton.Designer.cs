namespace Sdl.Community.BringBackTheButton
{
    partial class BringBackTheButton
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BringBackTheButton));
            this.lnkDont = new System.Windows.Forms.LinkLabel();
            this.btnOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lnkDont
            // 
            this.lnkDont.AutoSize = true;
            this.lnkDont.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkDont.Location = new System.Drawing.Point(101, 9);
            this.lnkDont.Name = "lnkDont";
            this.lnkDont.Size = new System.Drawing.Size(158, 17);
            this.lnkDont.TabIndex = 0;
            this.lnkDont.TabStop = true;
            this.lnkDont.Text = "Do not push this button!";
            this.lnkDont.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkDont_LinkClicked);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Location = new System.Drawing.Point(138, 50);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // BringBackTheButton
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(360, 85);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lnkDont);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(376, 124);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(376, 124);
            this.Name = "BringBackTheButton";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel lnkDont;
        private System.Windows.Forms.Button btnOk;
    }
}