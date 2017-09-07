using Sdl.Community.ProjectTerms.Controls;

namespace Sdl.Community.ProjectTerms.Plugin.ViewPart
{
    partial class ProjectTermsViewPartControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._cloudControl = new Sdl.Community.ProjectTerms.Controls.CloudControl();
            this._tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tableLayoutPanel
            // 
            this._tableLayoutPanel.ColumnCount = 3;
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanel.Controls.Add(this._cloudControl, 0, 1);
            this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._tableLayoutPanel.Name = "_tableLayoutPanel";
            this._tableLayoutPanel.RowCount = 2;
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanel.Size = new System.Drawing.Size(510, 313);
            this._tableLayoutPanel.TabIndex = 0;
            // 
            // _cloudControl
            // 
            this._cloudControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._cloudControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._tableLayoutPanel.SetColumnSpan(this._cloudControl, 3);
            this._cloudControl.LayoutType = Sdl.Community.ProjectTerms.Controls.LayoutType.Spiral;
            this._cloudControl.Location = new System.Drawing.Point(3, 3);
            this._cloudControl.MaxFontSize = 50;
            this._cloudControl.MinFontSize = 6;
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
            this._cloudControl.Size = new System.Drawing.Size(504, 307);
            this._cloudControl.TabIndex = 2;
            this._cloudControl.WeightedTerms = null;
            // 
            // ProjectTermsViewPartControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._tableLayoutPanel);
            this.Name = "ProjectTermsViewPartControl";
            this.Size = new System.Drawing.Size(510, 313);
            this._tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private CloudControl _cloudControl;
        private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
    }
}
