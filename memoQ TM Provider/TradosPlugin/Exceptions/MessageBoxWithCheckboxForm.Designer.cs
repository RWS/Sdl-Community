namespace TradosPlugin
{
    partial class MessageBoxWithCheckboxForm
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
            this.btnOK = new System.Windows.Forms.Button();
            this.chkDontShow = new System.Windows.Forms.CheckBox();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lnkLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(160, 124);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // chkDontShow
            // 
            this.chkDontShow.Location = new System.Drawing.Point(12, 94);
            this.chkDontShow.Name = "chkDontShow";
            this.chkDontShow.Size = new System.Drawing.Size(367, 24);
            this.chkDontShow.TabIndex = 1;
            this.chkDontShow.Text = "checkBox1";
            this.chkDontShow.UseVisualStyleBackColor = true;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(180, 19);
            this.lblMessage.MaximumSize = new System.Drawing.Size(400, 0);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(35, 13);
            this.lblMessage.TabIndex = 2;
            this.lblMessage.Text = "label1";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lnkLink
            // 
            this.lnkLink.BackColor = System.Drawing.Color.Transparent;
            this.lnkLink.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lnkLink.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lnkLink.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lnkLink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lnkLink.Location = new System.Drawing.Point(12, 68);
            this.lnkLink.Name = "lnkLink";
            this.lnkLink.Size = new System.Drawing.Size(367, 23);
            this.lnkLink.TabIndex = 19;
            this.lnkLink.TabStop = true;
            this.lnkLink.Text = "Log in";
            this.lnkLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkLink_LinkClicked);
            // 
            // MessageBoxWithCheckboxForm
            // 
            this.AcceptButton = this.btnOK;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 160);
            this.ControlBox = false;
            this.Controls.Add(this.lnkLink);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.chkDontShow);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MessageBoxWithCheckboxForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "MessageBoxWithCheckboxForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox chkDontShow;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.LinkLabel lnkLink;
    }
}