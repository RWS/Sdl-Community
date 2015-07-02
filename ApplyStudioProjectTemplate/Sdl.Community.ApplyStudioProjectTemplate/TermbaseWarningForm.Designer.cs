namespace Sdl.Community.ApplyStudioProjectTemplate
{
    partial class TermbaseWarningForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TermbaseWarningForm));
            this.WarningPicture = new System.Windows.Forms.PictureBox();
            this.WarningHeading = new System.Windows.Forms.Label();
            this.WarningText = new System.Windows.Forms.Label();
            this.DismissButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.DontShow = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.WarningPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // WarningPicture
            // 
            this.WarningPicture.Image = ((System.Drawing.Image)(resources.GetObject("WarningPicture.Image")));
            this.WarningPicture.Location = new System.Drawing.Point(12, 12);
            this.WarningPicture.Name = "WarningPicture";
            this.WarningPicture.Size = new System.Drawing.Size(32, 32);
            this.WarningPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.WarningPicture.TabIndex = 0;
            this.WarningPicture.TabStop = false;
            // 
            // WarningHeading
            // 
            this.WarningHeading.BackColor = System.Drawing.Color.Yellow;
            this.WarningHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WarningHeading.Location = new System.Drawing.Point(50, 12);
            this.WarningHeading.Name = "WarningHeading";
            this.WarningHeading.Size = new System.Drawing.Size(367, 32);
            this.WarningHeading.TabIndex = 0;
            this.WarningHeading.Text = "Warning";
            this.WarningHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // WarningText
            // 
            this.WarningText.Location = new System.Drawing.Point(12, 56);
            this.WarningText.Name = "WarningText";
            this.WarningText.Size = new System.Drawing.Size(405, 49);
            this.WarningText.TabIndex = 1;
            this.WarningText.Text = resources.GetString("WarningText.Text");
            // 
            // DismissButton
            // 
            this.DismissButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.DismissButton.Location = new System.Drawing.Point(335, 116);
            this.DismissButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.DismissButton.Name = "DismissButton";
            this.DismissButton.Size = new System.Drawing.Size(82, 23);
            this.DismissButton.TabIndex = 4;
            this.DismissButton.Text = "Cancel";
            this.DismissButton.UseVisualStyleBackColor = true;
            // 
            // OkButton
            // 
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Location = new System.Drawing.Point(247, 116);
            this.OkButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(82, 23);
            this.OkButton.TabIndex = 3;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            // 
            // DontShow
            // 
            this.DontShow.Location = new System.Drawing.Point(12, 117);
            this.DontShow.Name = "DontShow";
            this.DontShow.Size = new System.Drawing.Size(229, 23);
            this.DontShow.TabIndex = 2;
            this.DontShow.Text = "&Don\'t show this warning again";
            this.DontShow.UseVisualStyleBackColor = true;
            // 
            // TermbaseWarningForm
            // 
            this.AcceptButton = this.OkButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.DismissButton;
            this.ClientSize = new System.Drawing.Size(429, 151);
            this.Controls.Add(this.DontShow);
            this.Controls.Add(this.DismissButton);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.WarningText);
            this.Controls.Add(this.WarningHeading);
            this.Controls.Add(this.WarningPicture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TermbaseWarningForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Changing Termbases";
            ((System.ComponentModel.ISupportInitialize)(this.WarningPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox WarningPicture;
        private System.Windows.Forms.Label WarningHeading;
        private System.Windows.Forms.Label WarningText;
        private System.Windows.Forms.Button DismissButton;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.CheckBox DontShow;
    }
}