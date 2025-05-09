namespace GroupshareExcelAddIn.Controls.Embedded_controls
{
    partial class ResourceTypeFilterControl
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
            this._resourceTypeGroupBox = new System.Windows.Forms.GroupBox();
            this._projectTypeCheckBox = new System.Windows.Forms.CheckBox();
            this._projectTemplateTypeCheckBox = new System.Windows.Forms.CheckBox();
            this._resourceTypeGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _resourceTypeGroupBox
            // 
            this._resourceTypeGroupBox.AutoSize = true;
            this._resourceTypeGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._resourceTypeGroupBox.Controls.Add(this._projectTemplateTypeCheckBox);
            this._resourceTypeGroupBox.Controls.Add(this._projectTypeCheckBox);
            this._resourceTypeGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resourceTypeGroupBox.Location = new System.Drawing.Point(0, 0);
            this._resourceTypeGroupBox.Name = "_resourceTypeGroupBox";
            this._resourceTypeGroupBox.Size = new System.Drawing.Size(118, 88);
            this._resourceTypeGroupBox.TabIndex = 16;
            this._resourceTypeGroupBox.TabStop = false;
            this._resourceTypeGroupBox.Text = "Resource Type";
            // 
            // _projectTypeCheckBox
            // 
            this._projectTypeCheckBox.AutoSize = true;
            this._projectTypeCheckBox.Location = new System.Drawing.Point(6, 29);
            this._projectTypeCheckBox.Name = "_projectTypeCheckBox";
            this._projectTypeCheckBox.Size = new System.Drawing.Size(59, 17);
            this._projectTypeCheckBox.TabIndex = 0;
            this._projectTypeCheckBox.Text = "Project";
            this._projectTypeCheckBox.UseVisualStyleBackColor = true;
            // 
            // _projectTemplateTypeCheckBox
            // 
            this._projectTemplateTypeCheckBox.AutoSize = true;
            this._projectTemplateTypeCheckBox.Location = new System.Drawing.Point(6, 52);
            this._projectTemplateTypeCheckBox.Name = "_projectTemplateTypeCheckBox";
            this._projectTemplateTypeCheckBox.Size = new System.Drawing.Size(106, 17);
            this._projectTemplateTypeCheckBox.TabIndex = 1;
            this._projectTemplateTypeCheckBox.Text = "Project Template";
            this._projectTemplateTypeCheckBox.UseVisualStyleBackColor = true;
            // 
            // ResourceTypeFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this._resourceTypeGroupBox);
            this.Name = "ResourceTypeFilterControl";
            this.Size = new System.Drawing.Size(118, 88);
            this._resourceTypeGroupBox.ResumeLayout(false);
            this._resourceTypeGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox _resourceTypeGroupBox;
        private System.Windows.Forms.CheckBox _projectTemplateTypeCheckBox;
        private System.Windows.Forms.CheckBox _projectTypeCheckBox;
    }
}
