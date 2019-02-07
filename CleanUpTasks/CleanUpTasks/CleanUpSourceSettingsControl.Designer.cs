

namespace Sdl.Community.CleanUpTasks
{
    partial class CleanUpSourceSettingsControl 
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
            this.segmentLockerControl = new SegmentLockerControl();
            this.conversionsSettingsControl = new ConversionsSettingsControl();
            this.tagsSettingsControl = new TagsSettingsControl();
            this.SuspendLayout();
            // 
            // segmentLockerControl
            // 
            this.segmentLockerControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.segmentLockerControl.Location = new System.Drawing.Point(0, 0);
            this.segmentLockerControl.Name = "segmentLockerControl";
            this.segmentLockerControl.Size = new System.Drawing.Size(442, 149);
            this.segmentLockerControl.TabIndex = 2;
            // 
            // conversionsSettingsControl
            // 
            this.conversionsSettingsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.conversionsSettingsControl.Location = new System.Drawing.Point(0, 284);
            this.conversionsSettingsControl.Name = "conversionsSettingsControl";
            this.conversionsSettingsControl.Size = new System.Drawing.Size(442, 159);
            this.conversionsSettingsControl.TabIndex = 1;
            // 
            // tagsSettingsControl
            // 
            this.tagsSettingsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tagsSettingsControl.Location = new System.Drawing.Point(0, 155);
            this.tagsSettingsControl.Name = "tagsSettingsControl";
            this.tagsSettingsControl.Size = new System.Drawing.Size(442, 123);
            this.tagsSettingsControl.TabIndex = 0;
            // 
            // CleanUpSourceSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.segmentLockerControl);
            this.Controls.Add(this.conversionsSettingsControl);
            this.Controls.Add(this.tagsSettingsControl);
            this.Name = "CleanUpSourceSettingsControl";
            this.Size = new System.Drawing.Size(442, 443);
            this.ResumeLayout(false);

        }

        #endregion

        private TagsSettingsControl tagsSettingsControl;
        private ConversionsSettingsControl conversionsSettingsControl;
        private SegmentLockerControl segmentLockerControl;
    }
}
