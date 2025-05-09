namespace GroupshareExcelAddIn.Controls
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this._authentication_box = new System.Windows.Forms.GroupBox();
            this._rememberCheckBox = new System.Windows.Forms.CheckBox();
            this._passwordTxtBox = new System.Windows.Forms.TextBox();
            this._passwordLabel = new System.Windows.Forms.Label();
            this._userNameTxtBox = new System.Windows.Forms.TextBox();
            this._userNameLabel = new System.Windows.Forms.Label();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._inputInfoLabel = new System.Windows.Forms.Label();
            this._serverNameTxtBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._authentication_box.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _authentication_box
            // 
            this._authentication_box.Controls.Add(this._rememberCheckBox);
            this._authentication_box.Controls.Add(this._passwordTxtBox);
            this._authentication_box.Controls.Add(this._passwordLabel);
            this._authentication_box.Controls.Add(this._userNameTxtBox);
            this._authentication_box.Controls.Add(this._userNameLabel);
            this._authentication_box.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._authentication_box.Location = new System.Drawing.Point(6, 79);
            this._authentication_box.Name = "_authentication_box";
            this._authentication_box.Size = new System.Drawing.Size(279, 159);
            this._authentication_box.TabIndex = 22;
            this._authentication_box.TabStop = false;
            this._authentication_box.Text = "Authentication";
            // 
            // _rememberCheckBox
            // 
            this._rememberCheckBox.AutoSize = true;
            this._rememberCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._rememberCheckBox.Location = new System.Drawing.Point(6, 136);
            this._rememberCheckBox.Name = "_rememberCheckBox";
            this._rememberCheckBox.Size = new System.Drawing.Size(133, 17);
            this._rememberCheckBox.TabIndex = 4;
            this._rememberCheckBox.Text = "Remember user details";
            this._rememberCheckBox.UseVisualStyleBackColor = true;
            // 
            // _passwordTxtBox
            // 
            this._passwordTxtBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._passwordTxtBox.Location = new System.Drawing.Point(6, 101);
            this._passwordTxtBox.Name = "_passwordTxtBox";
            this._passwordTxtBox.PasswordChar = '●';
            this._passwordTxtBox.Size = new System.Drawing.Size(267, 20);
            this._passwordTxtBox.TabIndex = 1;
            this._passwordTxtBox.UseSystemPasswordChar = true;
            this._passwordTxtBox.TextChanged += new System.EventHandler(this.passwordTxtBox_TextChanged);
            // 
            // _passwordLabel
            // 
            this._passwordLabel.AutoSize = true;
            this._passwordLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._passwordLabel.Location = new System.Drawing.Point(6, 81);
            this._passwordLabel.Name = "_passwordLabel";
            this._passwordLabel.Size = new System.Drawing.Size(56, 13);
            this._passwordLabel.TabIndex = 3;
            this._passwordLabel.Text = "Password:";
            // 
            // _userNameTxtBox
            // 
            this._userNameTxtBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._userNameTxtBox.Location = new System.Drawing.Point(6, 49);
            this._userNameTxtBox.Name = "_userNameTxtBox";
            this._userNameTxtBox.Size = new System.Drawing.Size(267, 20);
            this._userNameTxtBox.TabIndex = 0;
            this._userNameTxtBox.TextChanged += new System.EventHandler(this.userNameTxtBox_TextChanged);
            // 
            // _userNameLabel
            // 
            this._userNameLabel.AutoSize = true;
            this._userNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this._userNameLabel.Location = new System.Drawing.Point(6, 29);
            this._userNameLabel.Name = "_userNameLabel";
            this._userNameLabel.Size = new System.Drawing.Size(61, 13);
            this._userNameLabel.TabIndex = 2;
            this._userNameLabel.Text = "User name:";
            // 
            // _okButton
            // 
            this._okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._okButton.Enabled = false;
            this._okButton.Location = new System.Drawing.Point(123, 282);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 21;
            this._okButton.Text = "Ok";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(204, 282);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 23;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this.cancelBtnServerBased_Click);
            // 
            // _inputInfoLabel
            // 
            this._inputInfoLabel.AutoSize = true;
            this._inputInfoLabel.ForeColor = System.Drawing.Color.Red;
            this._inputInfoLabel.Location = new System.Drawing.Point(12, 250);
            this._inputInfoLabel.Name = "_inputInfoLabel";
            this._inputInfoLabel.Size = new System.Drawing.Size(29, 13);
            this._inputInfoLabel.TabIndex = 5;
            this._inputInfoLabel.Text = "Error";
            this._inputInfoLabel.Visible = false;
            // 
            // _serverNameTxtBox
            // 
            this._serverNameTxtBox.Location = new System.Drawing.Point(6, 19);
            this._serverNameTxtBox.Name = "_serverNameTxtBox";
            this._serverNameTxtBox.Size = new System.Drawing.Size(267, 20);
            this._serverNameTxtBox.TabIndex = 25;
            this._serverNameTxtBox.TextChanged += new System.EventHandler(this.serverNameTxtBox_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._serverNameTxtBox);
            this.groupBox1.Location = new System.Drawing.Point(6, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(279, 54);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server address";
            // 
            // LoginForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(292, 317);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._inputInfoLabel);
            this.Controls.Add(this._authentication_box);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoginForm_FormClosing);
            this._authentication_box.ResumeLayout(false);
            this._authentication_box.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox _authentication_box;
        private System.Windows.Forms.TextBox _passwordTxtBox;
        private System.Windows.Forms.Label _passwordLabel;
        private System.Windows.Forms.TextBox _userNameTxtBox;
        private System.Windows.Forms.Label _userNameLabel;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.CheckBox _rememberCheckBox;
        private System.Windows.Forms.Label _inputInfoLabel;
        private System.Windows.Forms.TextBox _serverNameTxtBox;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}