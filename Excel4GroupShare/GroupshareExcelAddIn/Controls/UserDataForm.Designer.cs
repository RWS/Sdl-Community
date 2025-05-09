using GroupshareExcelAddIn.ComboTreeBoxControl;

namespace GroupshareExcelAddIn.Controls
{
    partial class UserDataForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserDataForm));
            this._organization_groupBox = new System.Windows.Forms.GroupBox();
            this._orgComboTreeBox = new GroupshareExcelAddIn.ComboTreeBoxControl.CustomTreeView();
            this._lblProcessingMessage = new System.Windows.Forms.Label();
            this._getDataButton = new System.Windows.Forms.Button();
            this._organization_groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _organization_groupBox
            // 
            this._organization_groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._organization_groupBox.Controls.Add(this._orgComboTreeBox);
            this._organization_groupBox.Location = new System.Drawing.Point(12, 24);
            this._organization_groupBox.Name = "_organization_groupBox";
            this._organization_groupBox.Size = new System.Drawing.Size(355, 55);
            this._organization_groupBox.TabIndex = 23;
            this._organization_groupBox.TabStop = false;
            this._organization_groupBox.Text = "Organization";
            // 
            // _orgComboTreeBox
            // 
            this._orgComboTreeBox.DroppedDown = false;
            this._orgComboTreeBox.Enabled = false;
            this._orgComboTreeBox.Location = new System.Drawing.Point(6, 19);
            this._orgComboTreeBox.Name = "_orgComboTreeBox";
            this._orgComboTreeBox.SelectedNode = null;
            this._orgComboTreeBox.Size = new System.Drawing.Size(178, 23);
            this._orgComboTreeBox.TabIndex = 23;
            // 
            // _lblProcessingMessage
            // 
            this._lblProcessingMessage.AutoSize = true;
            this._lblProcessingMessage.Location = new System.Drawing.Point(9, 114);
            this._lblProcessingMessage.Name = "_lblProcessingMessage";
            this._lblProcessingMessage.Size = new System.Drawing.Size(119, 13);
            this._lblProcessingMessage.TabIndex = 12;
            this._lblProcessingMessage.Text = "Loading organizations...";
            this._lblProcessingMessage.Visible = false;
            // 
            // _getDataButton
            // 
            this._getDataButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._getDataButton.Location = new System.Drawing.Point(278, 104);
            this._getDataButton.Name = "_getDataButton";
            this._getDataButton.Size = new System.Drawing.Size(89, 23);
            this._getDataButton.TabIndex = 24;
            this._getDataButton.Text = "Get Data";
            this._getDataButton.UseVisualStyleBackColor = true;
            this._getDataButton.Click += new System.EventHandler(this._getDataButton_Click);
            // 
            // UserDataForm
            // 
            this.AcceptButton = this._getDataButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.ClientSize = new System.Drawing.Size(379, 136);
            this.Controls.Add(this._getDataButton);
            this.Controls.Add(this._organization_groupBox);
            this.Controls.Add(this._lblProcessingMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "UserDataForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Filter User Data";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UserDataForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UserDataForm_KeyDown);
            this._organization_groupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox _organization_groupBox;
        private System.Windows.Forms.Label _lblProcessingMessage;
        public System.Windows.Forms.Button _getDataButton;
        private CustomTreeView _orgComboTreeBox;
    }
}