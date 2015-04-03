namespace Sdl.Community.Productivity.UI
{
    partial class ProductivityForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProductivityForm));
            this.productivityControl = new Sdl.Community.Productivity.UI.ProductivityControl();
            this.SuspendLayout();
            // 
            // gamificationControl1
            // 
            this.productivityControl.Controller = null;
            this.productivityControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productivityControl.Location = new System.Drawing.Point(0, 0);
            this.productivityControl.Name = "productivityControl1";
            this.productivityControl.Size = new System.Drawing.Size(696, 458);
            this.productivityControl.TabIndex = 0;
            // 
            // ProductivityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 458);
            this.Controls.Add(this.productivityControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProductivityForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Productivity Score";
            this.ResumeLayout(false);

        }

        #endregion

        private ProductivityControl productivityControl;
    }
}