
namespace Sdl.Community.ExportAnalysisReports.Controls
{
	partial class InformationMessage
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
			this.TitleMessage_textbox = new System.Windows.Forms.TextBox();
			this.ListOfProjects_TextBox = new System.Windows.Forms.TextBox();
			this.Cancel_Button = new System.Windows.Forms.Button();
			this.OK_Button = new System.Windows.Forms.Button();
			this.DontShowThisMessageAgain_CheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// TitleMessage_textbox
			// 
			this.TitleMessage_textbox.BackColor = System.Drawing.SystemColors.Control;
			this.TitleMessage_textbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.TitleMessage_textbox.ForeColor = System.Drawing.SystemColors.WindowText;
			this.TitleMessage_textbox.Location = new System.Drawing.Point(12, 12);
			this.TitleMessage_textbox.Multiline = true;
			this.TitleMessage_textbox.Name = "TitleMessage_textbox";
			this.TitleMessage_textbox.ReadOnly = true;
			this.TitleMessage_textbox.Size = new System.Drawing.Size(277, 57);
			this.TitleMessage_textbox.TabIndex = 2;
			this.TitleMessage_textbox.TabStop = false;
			this.TitleMessage_textbox.Text = "The selected projects cannot be exported; please run the \'Analyze Files\' batch t" +
    "ask for these projects, otherwise they will be excluded from the Export Analysis" +
    " Reports.";
			// 
			// ListOfProjects_TextBox
			// 
			this.ListOfProjects_TextBox.BackColor = System.Drawing.SystemColors.Control;
			this.ListOfProjects_TextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.ListOfProjects_TextBox.ForeColor = System.Drawing.SystemColors.WindowText;
			this.ListOfProjects_TextBox.Location = new System.Drawing.Point(12, 75);
			this.ListOfProjects_TextBox.Multiline = true;
			this.ListOfProjects_TextBox.Name = "ListOfProjects_TextBox";
			this.ListOfProjects_TextBox.ReadOnly = true;
			this.ListOfProjects_TextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.ListOfProjects_TextBox.Size = new System.Drawing.Size(277, 99);
			this.ListOfProjects_TextBox.TabIndex = 2;
			this.ListOfProjects_TextBox.TabStop = false;
			this.ListOfProjects_TextBox.WordWrap = false;
			// 
			// Cancel_Button
			// 
			this.Cancel_Button.Location = new System.Drawing.Point(203, 215);
			this.Cancel_Button.Name = "Cancel_Button";
			this.Cancel_Button.Size = new System.Drawing.Size(86, 24);
			this.Cancel_Button.TabIndex = 3;
			this.Cancel_Button.Text = "&Cancel";
			this.Cancel_Button.UseVisualStyleBackColor = true;
			this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
			// 
			// OK_Button
			// 
			this.OK_Button.Location = new System.Drawing.Point(122, 215);
			this.OK_Button.Name = "OK_Button";
			this.OK_Button.Size = new System.Drawing.Size(75, 24);
			this.OK_Button.TabIndex = 1;
			this.OK_Button.Text = "&OK";
			this.OK_Button.UseVisualStyleBackColor = true;
			this.OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
			// 
			// DontShowThisMessageAgain_CheckBox
			// 
			this.DontShowThisMessageAgain_CheckBox.AutoSize = true;
			this.DontShowThisMessageAgain_CheckBox.Location = new System.Drawing.Point(12, 180);
			this.DontShowThisMessageAgain_CheckBox.Name = "DontShowThisMessageAgain_CheckBox";
			this.DontShowThisMessageAgain_CheckBox.Size = new System.Drawing.Size(130, 17);
			this.DontShowThisMessageAgain_CheckBox.TabIndex = 5;
			this.DontShowThisMessageAgain_CheckBox.Text = "Don\'t show this again.";
			this.DontShowThisMessageAgain_CheckBox.UseVisualStyleBackColor = true;
			// 
			// InformationMessage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(309, 251);
			this.Controls.Add(this.DontShowThisMessageAgain_CheckBox);
			this.Controls.Add(this.OK_Button);
			this.Controls.Add(this.Cancel_Button);
			this.Controls.Add(this.ListOfProjects_TextBox);
			this.Controls.Add(this.TitleMessage_textbox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InformationMessage";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Projects without analysis reports";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox TitleMessage_textbox;
		private System.Windows.Forms.TextBox ListOfProjects_TextBox;
		private System.Windows.Forms.Button Cancel_Button;
		private System.Windows.Forms.Button OK_Button;
		private System.Windows.Forms.CheckBox DontShowThisMessageAgain_CheckBox;
	}
}