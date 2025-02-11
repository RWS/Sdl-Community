using System.Windows.Forms;

namespace Sdl.Community.NumberVerifier.GUI
{
    partial class NumberVerifierProfile : UserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelNumberVerifierUI;
        private System.Windows.Forms.Label labelHeader;
        private System.Windows.Forms.Label labelDescription;
        private Button buttonImportSettings;
        private Button buttonExportSettings;
        private System.Windows.Forms.Label labelCurrentProfile;
        private System.Windows.Forms.Label labelProfilePath;

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

        private void InitializeComponent()
        {
            this.panelNumberVerifierUI = new System.Windows.Forms.Panel();
            this.labelHeader = new System.Windows.Forms.Label();
            this.labelDescription = new System.Windows.Forms.Label();
            this.labelCurrentProfile = new System.Windows.Forms.Label();
            this.labelProfilePath = new System.Windows.Forms.Label();
            this.buttonImportSettings = new System.Windows.Forms.Button();
            this.buttonExportSettings = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // 
            // labelHeader
            // 
            this.labelHeader.AutoSize = true;
            this.labelHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.labelHeader.Location = new System.Drawing.Point(20, 20);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(220, 18);
            this.labelHeader.TabIndex = 2;
            this.labelHeader.Text = "Number Verifier QA Profile";

            // 
            // labelDescription
            // 
            this.labelDescription.AutoSize = true;
            this.labelDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.labelDescription.Location = new System.Drawing.Point(20, 45);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(500, 13);
            this.labelDescription.TabIndex = 3;
            this.labelDescription.Text = "Number Verifier settings can be imported and exported to allow you to create different configurations and for them to be shared.";

            // 
            // labelCurrentProfile
            // 
            this.labelCurrentProfile.AutoSize = true;
            this.labelCurrentProfile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.labelCurrentProfile.Location = new System.Drawing.Point(20, 70); // Move it above the buttons
            this.labelCurrentProfile.Name = "labelCurrentProfile";
            this.labelCurrentProfile.Size = new System.Drawing.Size(0, 13);
            this.labelCurrentProfile.TabIndex = 6;
            this.labelCurrentProfile.Text = "Current Profile: ";
            this.labelCurrentProfile.Visible = false; // Initially hidden
            //
            // labelProfilePath
            //
            this.labelProfilePath = new System.Windows.Forms.Label();
            this.labelProfilePath.AutoSize = true;
            this.labelProfilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.labelProfilePath.Location = new System.Drawing.Point(
                                            this.labelCurrentProfile.Location.X + this.labelCurrentProfile.Width + 3,
                                            this.labelCurrentProfile.Location.Y
                                        );
            this.labelProfilePath.Name = "labelProfilePath";
            this.labelProfilePath.Size = new System.Drawing.Size(0, 13);
            this.labelProfilePath.TabIndex = 7;
            this.labelProfilePath.Visible = false;

            // 
            // buttonImportSettings
            // 
            this.buttonImportSettings.Location = new System.Drawing.Point(20, 100); // Moved down
            this.buttonImportSettings.Name = "buttonImportSettings";
            this.buttonImportSettings.Size = new System.Drawing.Size(120, 25);
            this.buttonImportSettings.TabIndex = 4;
            this.buttonImportSettings.Text = "Import Settings...";
            this.buttonImportSettings.UseVisualStyleBackColor = true;
            this.buttonImportSettings.Click += new System.EventHandler(this.button1_ImportSettings_Click);

            // 
            // buttonExportSettings
            // 
            this.buttonExportSettings.Location = new System.Drawing.Point(160, 100); // Moved down
            this.buttonExportSettings.Name = "buttonExportSettings";
            this.buttonExportSettings.Size = new System.Drawing.Size(120, 25);
            this.buttonExportSettings.TabIndex = 5;
            this.buttonExportSettings.Text = "Export Settings...";
            this.buttonExportSettings.UseVisualStyleBackColor = true;
            this.buttonExportSettings.Click += new System.EventHandler(this.button2_ExportSettings_Click);

            // 
            // NumberVerifierProfile
            // 
            this.Controls.Add(this.labelHeader);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.labelCurrentProfile); // Add this before buttons
            this.Controls.Add(this.labelProfilePath);
            this.Controls.Add(this.buttonImportSettings);
            this.Controls.Add(this.buttonExportSettings);
            this.Name = "NumberVerifierProfile";
            this.Size = new System.Drawing.Size(782, 560);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}