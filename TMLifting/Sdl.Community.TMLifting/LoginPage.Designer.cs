namespace Sdl.Community.TMLifting
{
	partial class LoginPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginPage));
            this.panel3 = new System.Windows.Forms.Panel();
            this.serverNameTxtBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.authentication_box = new System.Windows.Forms.GroupBox();
            this.passwordTxtBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.userNameTxtBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOkServerBased = new System.Windows.Forms.Button();
            this.cancelBtnServerBased = new System.Windows.Forms.Button();
            this.panel3.SuspendLayout();
            this.authentication_box.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.serverNameTxtBox);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.authentication_box);
            this.panel3.Controls.Add(this.btnOkServerBased);
            this.panel3.Controls.Add(this.cancelBtnServerBased);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(493, 241);
            this.panel3.TabIndex = 8;
            // 
            // serverNameTxtBox
            // 
            this.serverNameTxtBox.Location = new System.Drawing.Point(170, 9);
            this.serverNameTxtBox.Name = "serverNameTxtBox";
            this.serverNameTxtBox.Size = new System.Drawing.Size(249, 20);
            this.serverNameTxtBox.TabIndex = 14;
            this.serverNameTxtBox.TextChanged += new System.EventHandler(this.serverNameTxtBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(47, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Server Address:";
            // 
            // authentication_box
            // 
            this.authentication_box.Controls.Add(this.passwordTxtBox);
            this.authentication_box.Controls.Add(this.label3);
            this.authentication_box.Controls.Add(this.userNameTxtBox);
            this.authentication_box.Controls.Add(this.label2);
            this.authentication_box.Location = new System.Drawing.Point(50, 43);
            this.authentication_box.Name = "authentication_box";
            this.authentication_box.Size = new System.Drawing.Size(397, 125);
            this.authentication_box.TabIndex = 8;
            this.authentication_box.TabStop = false;
            this.authentication_box.Text = "Authentication";
            // 
            // passwordTxtBox
            // 
            this.passwordTxtBox.Location = new System.Drawing.Point(120, 73);
            this.passwordTxtBox.Name = "passwordTxtBox";
            this.passwordTxtBox.PasswordChar = '●';
            this.passwordTxtBox.Size = new System.Drawing.Size(249, 20);
            this.passwordTxtBox.TabIndex = 1;
            this.passwordTxtBox.UseSystemPasswordChar = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Password:";
            // 
            // userNameTxtBox
            // 
            this.userNameTxtBox.Location = new System.Drawing.Point(120, 36);
            this.userNameTxtBox.Name = "userNameTxtBox";
            this.userNameTxtBox.Size = new System.Drawing.Size(249, 20);
            this.userNameTxtBox.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "User name:";
            // 
            // btnOkServerBased
            // 
            this.btnOkServerBased.Location = new System.Drawing.Point(291, 193);
            this.btnOkServerBased.Name = "btnOkServerBased";
            this.btnOkServerBased.Size = new System.Drawing.Size(75, 23);
            this.btnOkServerBased.TabIndex = 3;
            this.btnOkServerBased.Text = "Ok";
            this.btnOkServerBased.UseVisualStyleBackColor = true;
            this.btnOkServerBased.Click += new System.EventHandler(this.btnOkServerBased_Click);
            // 
            // cancelBtnServerBased
            // 
            this.cancelBtnServerBased.Location = new System.Drawing.Point(372, 193);
            this.cancelBtnServerBased.Name = "cancelBtnServerBased";
            this.cancelBtnServerBased.Size = new System.Drawing.Size(75, 23);
            this.cancelBtnServerBased.TabIndex = 10;
            this.cancelBtnServerBased.Text = "Cancel";
            this.cancelBtnServerBased.UseVisualStyleBackColor = true;
            this.cancelBtnServerBased.Click += new System.EventHandler(this.cancelBtnServerBased_Click);
            // 
            // LoginPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 241);
            this.Controls.Add(this.panel3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginPage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Login";
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.authentication_box.ResumeLayout(false);
            this.authentication_box.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.TextBox serverNameTxtBox;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox authentication_box;
		private System.Windows.Forms.TextBox passwordTxtBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox userNameTxtBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnOkServerBased;
		private System.Windows.Forms.Button cancelBtnServerBased;
	}
}