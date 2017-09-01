namespace Sdl.Community.ProjectTerms.Plugin
{
    partial class WordCloudForm
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
            this._cloudControl = new Sdl.Community.ProjectTerms.Controls.CloudControl();
            this.SuspendLayout();
            // 
            // _cloudControl
            // 
            this._cloudControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._cloudControl.LayoutType = Sdl.Community.ProjectTerms.Controls.LayoutType.Spiral;
            this._cloudControl.Location = new System.Drawing.Point(12, 12);
            this._cloudControl.MaxFontSize = 30;
            this._cloudControl.MinFontSize = 10;
            this._cloudControl.Name = "_cloudControl";
            this._cloudControl.Palette = new System.Drawing.Color[] {
        System.Drawing.Color.DarkRed,
        System.Drawing.Color.DarkBlue,
        System.Drawing.Color.DarkGreen,
        System.Drawing.Color.Navy,
        System.Drawing.Color.DarkCyan,
        System.Drawing.Color.DarkOrange,
        System.Drawing.Color.DarkGoldenrod,
        System.Drawing.Color.DarkKhaki,
        System.Drawing.Color.Blue,
        System.Drawing.Color.Red,
        System.Drawing.Color.Green};
            this._cloudControl.Size = new System.Drawing.Size(752, 224);
            this._cloudControl.TabIndex = 0;
            this._cloudControl.WeightedTerms = null;
            // 
            // WordCloudForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 245);
            this.Controls.Add(this._cloudControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "WordCloudForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WordCloudForm";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.CloudControl _cloudControl;
    }
}