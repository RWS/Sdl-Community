using GroupshareExcelAddIn.ComboTreeBoxControl;

namespace GroupshareExcelAddIn.Controls
{
    partial class ProjectForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectForm));
            this._getProjectDataButton = new System.Windows.Forms.Button();
            this._projectStatus_groupBox = new System.Windows.Forms.GroupBox();
            this._projectStatus_checkBox = new System.Windows.Forms.CheckBox();
            this.checkBoxInProgress = new System.Windows.Forms.CheckBox();
            this.checkBoxCompleted = new System.Windows.Forms.CheckBox();
            this.checkBoxArchived = new System.Windows.Forms.CheckBox();
            this.pendingCheckBox = new System.Windows.Forms.CheckBox();
            this.detachedCheckBox = new System.Windows.Forms.CheckBox();
            this.deliveryDate_groupBox = new System.Windows.Forms.GroupBox();
            this._projectDelDate_checkBox = new System.Windows.Forms.CheckBox();
            this._startDeliveryDate = new System.Windows.Forms.DateTimePicker();
            this._endDeliveryDate = new System.Windows.Forms.DateTimePicker();
            this.label_to_deliveryDate = new System.Windows.Forms.Label();
            this.publishingDate_groupBox = new System.Windows.Forms.GroupBox();
            this._projectPubDate_checkBox = new System.Windows.Forms.CheckBox();
            this._startPublishingDate = new System.Windows.Forms.DateTimePicker();
            this._endPublishingDate = new System.Windows.Forms.DateTimePicker();
            this.label_to_publishDate = new System.Windows.Forms.Label();
            this._organization_groupBox = new System.Windows.Forms.GroupBox();
            this._orgComboTreeBox = new GroupshareExcelAddIn.ComboTreeBoxControl.CustomTreeView();
            this._includeSubOrgs_checkBox = new System.Windows.Forms.CheckBox();
            this._lblProcessingMessage = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._includePhasesCheckBox = new System.Windows.Forms.CheckBox();
            this._phasesInclusionToolTip = new System.Windows.Forms.ToolTip(this.components);
            this._projectStatus_groupBox.SuspendLayout();
            this.deliveryDate_groupBox.SuspendLayout();
            this.publishingDate_groupBox.SuspendLayout();
            this._organization_groupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _getProjectDataButton
            // 
            this._getProjectDataButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._getProjectDataButton.Location = new System.Drawing.Point(303, 469);
            this._getProjectDataButton.Name = "_getProjectDataButton";
            this._getProjectDataButton.Size = new System.Drawing.Size(89, 23);
            this._getProjectDataButton.TabIndex = 10;
            this._getProjectDataButton.Text = "Get Data";
            this._getProjectDataButton.UseVisualStyleBackColor = true;
            this._getProjectDataButton.Click += new System.EventHandler(this.GetProjectDataButton_Click);
            // 
            // _projectStatus_groupBox
            // 
            this._projectStatus_groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._projectStatus_groupBox.Controls.Add(this._projectStatus_checkBox);
            this._projectStatus_groupBox.Controls.Add(this.checkBoxInProgress);
            this._projectStatus_groupBox.Controls.Add(this.checkBoxCompleted);
            this._projectStatus_groupBox.Controls.Add(this.checkBoxArchived);
            this._projectStatus_groupBox.Controls.Add(this.pendingCheckBox);
            this._projectStatus_groupBox.Controls.Add(this.detachedCheckBox);
            this._projectStatus_groupBox.Location = new System.Drawing.Point(15, 303);
            this._projectStatus_groupBox.Name = "_projectStatus_groupBox";
            this._projectStatus_groupBox.Size = new System.Drawing.Size(377, 137);
            this._projectStatus_groupBox.TabIndex = 29;
            this._projectStatus_groupBox.TabStop = false;
            // 
            // _projectStatus_checkBox
            // 
            this._projectStatus_checkBox.AutoSize = true;
            this._projectStatus_checkBox.Checked = true;
            this._projectStatus_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this._projectStatus_checkBox.Location = new System.Drawing.Point(5, 0);
            this._projectStatus_checkBox.Name = "_projectStatus_checkBox";
            this._projectStatus_checkBox.Size = new System.Drawing.Size(92, 17);
            this._projectStatus_checkBox.TabIndex = 22;
            this._projectStatus_checkBox.Text = "Project Status";
            this._projectStatus_checkBox.UseVisualStyleBackColor = true;
            // 
            // checkBoxInProgress
            // 
            this.checkBoxInProgress.AutoSize = true;
            this.checkBoxInProgress.Checked = true;
            this.checkBoxInProgress.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxInProgress.Location = new System.Drawing.Point(10, 45);
            this.checkBoxInProgress.Name = "checkBoxInProgress";
            this.checkBoxInProgress.Size = new System.Drawing.Size(78, 17);
            this.checkBoxInProgress.TabIndex = 11;
            this.checkBoxInProgress.Text = "In progress";
            this.checkBoxInProgress.UseVisualStyleBackColor = true;
            // 
            // checkBoxCompleted
            // 
            this.checkBoxCompleted.AutoSize = true;
            this.checkBoxCompleted.Checked = true;
            this.checkBoxCompleted.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCompleted.Location = new System.Drawing.Point(10, 68);
            this.checkBoxCompleted.Name = "checkBoxCompleted";
            this.checkBoxCompleted.Size = new System.Drawing.Size(76, 17);
            this.checkBoxCompleted.TabIndex = 12;
            this.checkBoxCompleted.Text = "Completed";
            this.checkBoxCompleted.UseVisualStyleBackColor = true;
            // 
            // checkBoxArchived
            // 
            this.checkBoxArchived.AutoSize = true;
            this.checkBoxArchived.Location = new System.Drawing.Point(10, 91);
            this.checkBoxArchived.Name = "checkBoxArchived";
            this.checkBoxArchived.Size = new System.Drawing.Size(68, 17);
            this.checkBoxArchived.TabIndex = 13;
            this.checkBoxArchived.Text = "Archived";
            this.checkBoxArchived.UseVisualStyleBackColor = true;
            // 
            // pendingCheckBox
            // 
            this.pendingCheckBox.AutoSize = true;
            this.pendingCheckBox.Checked = true;
            this.pendingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.pendingCheckBox.Location = new System.Drawing.Point(10, 23);
            this.pendingCheckBox.Name = "pendingCheckBox";
            this.pendingCheckBox.Size = new System.Drawing.Size(65, 17);
            this.pendingCheckBox.TabIndex = 17;
            this.pendingCheckBox.Text = "Pending";
            this.pendingCheckBox.UseVisualStyleBackColor = true;
            // 
            // detachedCheckBox
            // 
            this.detachedCheckBox.AutoSize = true;
            this.detachedCheckBox.Location = new System.Drawing.Point(10, 114);
            this.detachedCheckBox.Name = "detachedCheckBox";
            this.detachedCheckBox.Size = new System.Drawing.Size(73, 17);
            this.detachedCheckBox.TabIndex = 16;
            this.detachedCheckBox.Text = "Detached";
            this.detachedCheckBox.UseVisualStyleBackColor = true;
            // 
            // deliveryDate_groupBox
            // 
            this.deliveryDate_groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.deliveryDate_groupBox.Controls.Add(this._projectDelDate_checkBox);
            this.deliveryDate_groupBox.Controls.Add(this._startDeliveryDate);
            this.deliveryDate_groupBox.Controls.Add(this._endDeliveryDate);
            this.deliveryDate_groupBox.Controls.Add(this.label_to_deliveryDate);
            this.deliveryDate_groupBox.Location = new System.Drawing.Point(15, 228);
            this.deliveryDate_groupBox.Name = "deliveryDate_groupBox";
            this.deliveryDate_groupBox.Size = new System.Drawing.Size(377, 55);
            this.deliveryDate_groupBox.TabIndex = 28;
            this.deliveryDate_groupBox.TabStop = false;
            // 
            // _projectDelDate_checkBox
            // 
            this._projectDelDate_checkBox.AutoSize = true;
            this._projectDelDate_checkBox.Checked = true;
            this._projectDelDate_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this._projectDelDate_checkBox.Location = new System.Drawing.Point(5, 0);
            this._projectDelDate_checkBox.Name = "_projectDelDate_checkBox";
            this._projectDelDate_checkBox.Size = new System.Drawing.Size(122, 17);
            this._projectDelDate_checkBox.TabIndex = 22;
            this._projectDelDate_checkBox.Text = "Project delivery date";
            this._projectDelDate_checkBox.UseVisualStyleBackColor = true;
            // 
            // _startDeliveryDate
            // 
            this._startDeliveryDate.Location = new System.Drawing.Point(5, 29);
            this._startDeliveryDate.Name = "_startDeliveryDate";
            this._startDeliveryDate.Size = new System.Drawing.Size(165, 20);
            this._startDeliveryDate.TabIndex = 6;
            // 
            // _endDeliveryDate
            // 
            this._endDeliveryDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._endDeliveryDate.Location = new System.Drawing.Point(206, 29);
            this._endDeliveryDate.Name = "_endDeliveryDate";
            this._endDeliveryDate.Size = new System.Drawing.Size(165, 20);
            this._endDeliveryDate.TabIndex = 7;
            // 
            // label_to_deliveryDate
            // 
            this.label_to_deliveryDate.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label_to_deliveryDate.AutoSize = true;
            this.label_to_deliveryDate.Location = new System.Drawing.Point(178, 35);
            this.label_to_deliveryDate.Name = "label_to_deliveryDate";
            this.label_to_deliveryDate.Size = new System.Drawing.Size(16, 13);
            this.label_to_deliveryDate.TabIndex = 8;
            this.label_to_deliveryDate.Text = "to";
            // 
            // publishingDate_groupBox
            // 
            this.publishingDate_groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.publishingDate_groupBox.Controls.Add(this._projectPubDate_checkBox);
            this.publishingDate_groupBox.Controls.Add(this._startPublishingDate);
            this.publishingDate_groupBox.Controls.Add(this._endPublishingDate);
            this.publishingDate_groupBox.Controls.Add(this.label_to_publishDate);
            this.publishingDate_groupBox.Location = new System.Drawing.Point(15, 156);
            this.publishingDate_groupBox.Name = "publishingDate_groupBox";
            this.publishingDate_groupBox.Size = new System.Drawing.Size(377, 55);
            this.publishingDate_groupBox.TabIndex = 27;
            this.publishingDate_groupBox.TabStop = false;
            // 
            // _projectPubDate_checkBox
            // 
            this._projectPubDate_checkBox.AutoSize = true;
            this._projectPubDate_checkBox.Checked = true;
            this._projectPubDate_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this._projectPubDate_checkBox.Location = new System.Drawing.Point(5, 0);
            this._projectPubDate_checkBox.Name = "_projectPubDate_checkBox";
            this._projectPubDate_checkBox.Size = new System.Drawing.Size(133, 17);
            this._projectPubDate_checkBox.TabIndex = 22;
            this._projectPubDate_checkBox.Text = "Project publishing date";
            this._projectPubDate_checkBox.UseVisualStyleBackColor = true;
            // 
            // _startPublishingDate
            // 
            this._startPublishingDate.Location = new System.Drawing.Point(5, 29);
            this._startPublishingDate.Name = "_startPublishingDate";
            this._startPublishingDate.Size = new System.Drawing.Size(165, 20);
            this._startPublishingDate.TabIndex = 1;
            // 
            // _endPublishingDate
            // 
            this._endPublishingDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._endPublishingDate.Location = new System.Drawing.Point(206, 29);
            this._endPublishingDate.Name = "_endPublishingDate";
            this._endPublishingDate.Size = new System.Drawing.Size(165, 20);
            this._endPublishingDate.TabIndex = 2;
            // 
            // label_to_publishDate
            // 
            this.label_to_publishDate.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label_to_publishDate.AutoSize = true;
            this.label_to_publishDate.Location = new System.Drawing.Point(178, 35);
            this.label_to_publishDate.Name = "label_to_publishDate";
            this.label_to_publishDate.Size = new System.Drawing.Size(16, 13);
            this.label_to_publishDate.TabIndex = 5;
            this.label_to_publishDate.Text = "to";
            // 
            // _organization_groupBox
            // 
            this._organization_groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._organization_groupBox.Controls.Add(this._orgComboTreeBox);
            this._organization_groupBox.Controls.Add(this._includeSubOrgs_checkBox);
            this._organization_groupBox.Location = new System.Drawing.Point(15, 84);
            this._organization_groupBox.Name = "_organization_groupBox";
            this._organization_groupBox.Size = new System.Drawing.Size(377, 55);
            this._organization_groupBox.TabIndex = 26;
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
            this._orgComboTreeBox.Size = new System.Drawing.Size(165, 20);
            this._orgComboTreeBox.TabIndex = 0;
            // 
            // _includeSubOrgs_checkBox
            // 
            this._includeSubOrgs_checkBox.AutoSize = true;
            this._includeSubOrgs_checkBox.Location = new System.Drawing.Point(177, 22);
            this._includeSubOrgs_checkBox.Name = "_includeSubOrgs_checkBox";
            this._includeSubOrgs_checkBox.Size = new System.Drawing.Size(143, 17);
            this._includeSubOrgs_checkBox.TabIndex = 23;
            this._includeSubOrgs_checkBox.Text = "Include suborganizations";
            this._includeSubOrgs_checkBox.UseVisualStyleBackColor = true;
            // 
            // _lblProcessingMessage
            // 
            this._lblProcessingMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._lblProcessingMessage.AutoSize = true;
            this._lblProcessingMessage.Location = new System.Drawing.Point(12, 474);
            this._lblProcessingMessage.Name = "_lblProcessingMessage";
            this._lblProcessingMessage.Size = new System.Drawing.Size(119, 13);
            this._lblProcessingMessage.TabIndex = 12;
            this._lblProcessingMessage.Text = "Loading organizations...";
            this._lblProcessingMessage.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this._includePhasesCheckBox);
            this.groupBox1.Location = new System.Drawing.Point(15, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(377, 55);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data to be included";
            // 
            // _includePhasesCheckBox
            // 
            this._includePhasesCheckBox.AutoSize = true;
            this._includePhasesCheckBox.Location = new System.Drawing.Point(5, 32);
            this._includePhasesCheckBox.Name = "_includePhasesCheckBox";
            this._includePhasesCheckBox.Size = new System.Drawing.Size(170, 17);
            this._includePhasesCheckBox.TabIndex = 23;
            this._includePhasesCheckBox.Text = "Include phases with assignees";
            this._phasesInclusionToolTip.SetToolTip(this._includePhasesCheckBox, "Significantly reduces speed; Use in combination with the other filters to reduce " +
        "local processing.");
            this._includePhasesCheckBox.UseVisualStyleBackColor = true;
            // 
            // ProjectForm
            // 
            this.AcceptButton = this._getProjectDataButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(401, 504);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._projectStatus_groupBox);
            this.Controls.Add(this.deliveryDate_groupBox);
            this.Controls.Add(this.publishingDate_groupBox);
            this.Controls.Add(this._lblProcessingMessage);
            this.Controls.Add(this._organization_groupBox);
            this.Controls.Add(this._getProjectDataButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "ProjectForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Filter GroupShare Projects";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProjectForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ProjectForm_KeyDown);
            this._projectStatus_groupBox.ResumeLayout(false);
            this._projectStatus_groupBox.PerformLayout();
            this.deliveryDate_groupBox.ResumeLayout(false);
            this.deliveryDate_groupBox.PerformLayout();
            this.publishingDate_groupBox.ResumeLayout(false);
            this.publishingDate_groupBox.PerformLayout();
            this._organization_groupBox.ResumeLayout(false);
            this._organization_groupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Button _getProjectDataButton;
        private System.Windows.Forms.GroupBox _projectStatus_groupBox;
        private System.Windows.Forms.CheckBox _projectStatus_checkBox;
        private System.Windows.Forms.CheckBox checkBoxInProgress;
        private System.Windows.Forms.CheckBox checkBoxCompleted;
        private System.Windows.Forms.CheckBox checkBoxArchived;
        private System.Windows.Forms.CheckBox pendingCheckBox;
        private System.Windows.Forms.CheckBox detachedCheckBox;
        private System.Windows.Forms.GroupBox deliveryDate_groupBox;
        internal System.Windows.Forms.DateTimePicker _startDeliveryDate;
        internal System.Windows.Forms.DateTimePicker _endDeliveryDate;
        private System.Windows.Forms.Label label_to_deliveryDate;
        private System.Windows.Forms.GroupBox publishingDate_groupBox;
        internal System.Windows.Forms.DateTimePicker _endPublishingDate;
        private System.Windows.Forms.Label label_to_publishDate;
        private System.Windows.Forms.GroupBox _organization_groupBox;
        private System.Windows.Forms.Label _lblProcessingMessage;
        private System.Windows.Forms.CheckBox _includeSubOrgs_checkBox;
        private CustomTreeView _orgComboTreeBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox _includePhasesCheckBox;
        private System.Windows.Forms.ToolTip _phasesInclusionToolTip;
        internal System.Windows.Forms.DateTimePicker _startPublishingDate;
        private System.Windows.Forms.CheckBox _projectDelDate_checkBox;
        private System.Windows.Forms.CheckBox _projectPubDate_checkBox;
    }
}