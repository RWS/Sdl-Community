namespace Sdl.Community.ApplyStudioProjectTemplate
{
    partial class EditApplyTemplates
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditApplyTemplates));
            this.AddTemplate = new System.Windows.Forms.Button();
            this.RemoveTemplate = new System.Windows.Forms.Button();
            this.ClearTemplates = new System.Windows.Forms.Button();
            this.ProjectTemplateName = new System.Windows.Forms.TextBox();
            this.OkButton = new System.Windows.Forms.Button();
            this.DismissButton = new System.Windows.Forms.Button();
            this.ProjectTemplates = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // AddTemplate
            // 
            this.AddTemplate.Location = new System.Drawing.Point(304, 12);
            this.AddTemplate.Name = "AddTemplate";
            this.AddTemplate.Size = new System.Drawing.Size(102, 23);
            this.AddTemplate.TabIndex = 2;
            this.AddTemplate.Text = "&Add...";
            this.AddTemplate.UseVisualStyleBackColor = true;
            this.AddTemplate.Click += new System.EventHandler(this.AddTemplate_Click);
            // 
            // RemoveTemplate
            // 
            this.RemoveTemplate.Location = new System.Drawing.Point(304, 41);
            this.RemoveTemplate.Name = "RemoveTemplate";
            this.RemoveTemplate.Size = new System.Drawing.Size(102, 23);
            this.RemoveTemplate.TabIndex = 3;
            this.RemoveTemplate.Text = "&Remove";
            this.RemoveTemplate.UseVisualStyleBackColor = true;
            this.RemoveTemplate.Click += new System.EventHandler(this.RemoveTemplate_Click);
            // 
            // ClearTemplates
            // 
            this.ClearTemplates.Location = new System.Drawing.Point(304, 70);
            this.ClearTemplates.Name = "ClearTemplates";
            this.ClearTemplates.Size = new System.Drawing.Size(102, 23);
            this.ClearTemplates.TabIndex = 4;
            this.ClearTemplates.Text = "&Clear";
            this.ClearTemplates.UseVisualStyleBackColor = true;
            this.ClearTemplates.Click += new System.EventHandler(this.ClearTemplates_Click);
            // 
            // ProjectTemplateName
            // 
            this.ProjectTemplateName.Location = new System.Drawing.Point(12, 191);
            this.ProjectTemplateName.Name = "ProjectTemplateName";
            this.ProjectTemplateName.Size = new System.Drawing.Size(286, 22);
            this.ProjectTemplateName.TabIndex = 1;
            this.ProjectTemplateName.TextChanged += new System.EventHandler(this.ProjectTemplateName_TextChanged);
            // 
            // OkButton
            // 
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Location = new System.Drawing.Point(304, 162);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(102, 23);
            this.OkButton.TabIndex = 5;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            // 
            // DismissButton
            // 
            this.DismissButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.DismissButton.Location = new System.Drawing.Point(304, 190);
            this.DismissButton.Name = "DismissButton";
            this.DismissButton.Size = new System.Drawing.Size(102, 23);
            this.DismissButton.TabIndex = 6;
            this.DismissButton.Text = "Cancel";
            this.DismissButton.UseVisualStyleBackColor = true;
            // 
            // ProjectTemplates
            // 
            this.ProjectTemplates.FormattingEnabled = true;
            this.ProjectTemplates.Location = new System.Drawing.Point(12, 12);
            this.ProjectTemplates.Name = "ProjectTemplates";
            this.ProjectTemplates.Size = new System.Drawing.Size(286, 173);
            this.ProjectTemplates.TabIndex = 0;
            this.ProjectTemplates.SelectedIndexChanged += new System.EventHandler(this.ProjectTemplates_SelectedIndexChanged);
            // 
            // EditApplyTemplates
            // 
            this.AcceptButton = this.OkButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.DismissButton;
            this.ClientSize = new System.Drawing.Size(417, 224);
            this.Controls.Add(this.ProjectTemplates);
            this.Controls.Add(this.ProjectTemplateName);
            this.Controls.Add(this.DismissButton);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.ClearTemplates);
            this.Controls.Add(this.RemoveTemplate);
            this.Controls.Add(this.AddTemplate);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EditApplyTemplates";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Templates";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button AddTemplate;
        private System.Windows.Forms.Button RemoveTemplate;
        private System.Windows.Forms.Button ClearTemplates;
        private System.Windows.Forms.TextBox ProjectTemplateName;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Button DismissButton;
        private System.Windows.Forms.ListBox ProjectTemplates;
    }
}