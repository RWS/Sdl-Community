namespace Sdl.Verification.Sdk.EditAndApplyChanges.MessageUI
{
    partial class CustomMessageControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomMessageControl));
            this._sourceSegmentPanel = new System.Windows.Forms.Panel();
            this._sourceSegmentLabel = new System.Windows.Forms.Label();
            this._problemDescriptionLabel = new System.Windows.Forms.Label();
            this._messageTextBox = new System.Windows.Forms.TextBox();
            this._messageLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._suggestionsList = new System.Windows.Forms.ListBox();
            this._problemDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.l_Severity = new System.Windows.Forms.Label();
            this._targetSegmentPanel = new System.Windows.Forms.Panel();
            this._targetSegmentLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _sourceSegmentPanel
            // 
            resources.ApplyResources(this._sourceSegmentPanel, "_sourceSegmentPanel");
            this._sourceSegmentPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tableLayoutPanel1.SetColumnSpan(this._sourceSegmentPanel, 3);
            this._sourceSegmentPanel.Name = "_sourceSegmentPanel";
            // 
            // _sourceSegmentLabel
            // 
            resources.ApplyResources(this._sourceSegmentLabel, "_sourceSegmentLabel");
            this._sourceSegmentLabel.Name = "_sourceSegmentLabel";
            // 
            // _problemDescriptionLabel
            // 
            resources.ApplyResources(this._problemDescriptionLabel, "_problemDescriptionLabel");
            this.tableLayoutPanel1.SetColumnSpan(this._problemDescriptionLabel, 2);
            this._problemDescriptionLabel.Name = "_problemDescriptionLabel";
            // 
            // _messageTextBox
            // 
            resources.ApplyResources(this._messageTextBox, "_messageTextBox");
            this.tableLayoutPanel1.SetColumnSpan(this._messageTextBox, 2);
            this._messageTextBox.Name = "_messageTextBox";
            this._messageTextBox.ReadOnly = true;
            // 
            // _messageLabel
            // 
            resources.ApplyResources(this._messageLabel, "_messageLabel");
            this._messageLabel.Name = "_messageLabel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // _suggestionsList
            // 
            resources.ApplyResources(this._suggestionsList, "_suggestionsList");
            this._suggestionsList.FormattingEnabled = true;
            this._suggestionsList.Name = "_suggestionsList";
            this.tableLayoutPanel1.SetRowSpan(this._suggestionsList, 3);
            // 
            // _problemDescriptionTextBox
            // 
            this._problemDescriptionTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.SetColumnSpan(this._problemDescriptionTextBox, 2);
            resources.ApplyResources(this._problemDescriptionTextBox, "_problemDescriptionTextBox");
            this._problemDescriptionTextBox.Name = "_problemDescriptionTextBox";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this._problemDescriptionTextBox, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this._suggestionsList, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this._sourceSegmentPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._messageLabel, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this._sourceSegmentLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._problemDescriptionLabel, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this._messageTextBox, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.l_Severity, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this._targetSegmentPanel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._targetSegmentLabel, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // l_Severity
            // 
            resources.ApplyResources(this.l_Severity, "l_Severity");
            this.l_Severity.Image = null;
            this.l_Severity.Name = "l_Severity";
            // 
            // _targetSegmentPanel
            // 
            resources.ApplyResources(this._targetSegmentPanel, "_targetSegmentPanel");
            this._targetSegmentPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tableLayoutPanel1.SetColumnSpan(this._targetSegmentPanel, 3);
            this._targetSegmentPanel.Name = "_targetSegmentPanel";
            // 
            // _targetSegmentLabel
            // 
            resources.ApplyResources(this._targetSegmentLabel, "_targetSegmentLabel");
            this._targetSegmentLabel.Name = "_targetSegmentLabel";
            // 
            // CustomMessageControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(392, 275);
            this.Name = "CustomMessageControl";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _sourceSegmentPanel;
        private System.Windows.Forms.Label _sourceSegmentLabel;
        private System.Windows.Forms.Label _problemDescriptionLabel;
        private System.Windows.Forms.TextBox _messageTextBox;
        private System.Windows.Forms.Label _messageLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox _suggestionsList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox _problemDescriptionTextBox;
        private System.Windows.Forms.Label l_Severity;
        private System.Windows.Forms.Panel _targetSegmentPanel;
        private System.Windows.Forms.Label _targetSegmentLabel;
    }
}
