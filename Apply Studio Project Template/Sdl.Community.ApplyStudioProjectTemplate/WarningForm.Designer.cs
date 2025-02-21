namespace Sdl.Community.ApplyStudioProjectTemplate
{
    partial class WarningForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WarningForm));
            this.WarningPicture = new System.Windows.Forms.PictureBox();
            this.WarningText = new System.Windows.Forms.Label();
            this.DismissButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.DontShow = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.WarningPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // WarningPicture
            // 
            this.WarningPicture.Image = global::Sdl.Community.ApplyStudioProjectTemplate.PluginResources.question;
            this.WarningPicture.Location = new System.Drawing.Point(12, 12);
            this.WarningPicture.Name = "WarningPicture";
            this.WarningPicture.Size = new System.Drawing.Size(32, 32);
            this.WarningPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.WarningPicture.TabIndex = 0;
            this.WarningPicture.TabStop = false;
            // 
            // WarningText
            // 
            this.WarningText.Location = new System.Drawing.Point(50, 9);
            this.WarningText.Name = "WarningText";
            this.WarningText.Size = new System.Drawing.Size(353, 49);
            this.WarningText.TabIndex = 1;
            this.WarningText.Text = "Selected template has different language directions, or language pairs, from the " +
    "selected project.  Do you still wish to apply the settings from this template?";
            // 
            // DismissButton
            // 
            this.DismissButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.DismissButton.Location = new System.Drawing.Point(335, 71);
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
            this.OkButton.Location = new System.Drawing.Point(247, 71);
            this.OkButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(82, 23);
            this.OkButton.TabIndex = 3;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            // 
            // DontShow
            // 
            this.DontShow.Location = new System.Drawing.Point(12, 72);
            this.DontShow.Name = "DontShow";
            this.DontShow.Size = new System.Drawing.Size(229, 23);
            this.DontShow.TabIndex = 2;
            this.DontShow.Text = "&Don\'t show this warning again";
            this.DontShow.UseVisualStyleBackColor = true;
            // 
            // WarningForm
            // 
            this.AcceptButton = this.OkButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.DismissButton;
            this.ClientSize = new System.Drawing.Size(431, 105);
            this.Controls.Add(this.DontShow);
            this.Controls.Add(this.DismissButton);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.WarningText);
            this.Controls.Add(this.WarningPicture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WarningForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Warning";
            ((System.ComponentModel.ISupportInitialize)(this.WarningPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox WarningPicture;
        private System.Windows.Forms.Label WarningText;
        private System.Windows.Forms.Button DismissButton;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.CheckBox DontShow;
    }
}