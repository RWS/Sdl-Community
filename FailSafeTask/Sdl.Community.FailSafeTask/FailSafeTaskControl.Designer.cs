namespace Sdl.Community.FailSafeTask
{
    partial class FailSafeTaskControl
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
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.pseudoTranslateRadioButton = new System.Windows.Forms.RadioButton();
            this.copyToTargetRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.pseudoTranslateRadioButton);
            this.groupBox.Controls.Add(this.copyToTargetRadioButton);
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox.Location = new System.Drawing.Point(0, 0);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(263, 150);
            this.groupBox.TabIndex = 0;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Select Task";
            // 
            // pseudoTranslateRadioButton
            // 
            this.pseudoTranslateRadioButton.AutoSize = true;
            this.pseudoTranslateRadioButton.Location = new System.Drawing.Point(30, 41);
            this.pseudoTranslateRadioButton.Name = "pseudoTranslateRadioButton";
            this.pseudoTranslateRadioButton.Size = new System.Drawing.Size(159, 16);
            this.pseudoTranslateRadioButton.TabIndex = 1;
            this.pseudoTranslateRadioButton.Text = "Pseudo translate and save";
            this.pseudoTranslateRadioButton.UseVisualStyleBackColor = true;
            // 
            // copyToTargetRadioButton
            // 
            this.copyToTargetRadioButton.AutoSize = true;
            this.copyToTargetRadioButton.Checked = true;
            this.copyToTargetRadioButton.Location = new System.Drawing.Point(30, 18);
            this.copyToTargetRadioButton.Name = "copyToTargetRadioButton";
            this.copyToTargetRadioButton.Size = new System.Drawing.Size(185, 16);
            this.copyToTargetRadioButton.TabIndex = 0;
            this.copyToTargetRadioButton.TabStop = true;
            this.copyToTargetRadioButton.Text = "Copy source to target and save";
            this.copyToTargetRadioButton.UseVisualStyleBackColor = true;
            // 
            // FailSafeTaskControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox);
            this.Name = "FailSafeTaskControl";
            this.Size = new System.Drawing.Size(263, 150);
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.RadioButton copyToTargetRadioButton;
        private System.Windows.Forms.RadioButton pseudoTranslateRadioButton;
    }
}
