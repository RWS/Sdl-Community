namespace Sdl.Community.Productivity.UI
{
    partial class TwitterAccountSetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TwitterAccountSetup));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lkAuthorizeTwitter = new System.Windows.Forms.LinkLabel();
            this.txtPin = new System.Windows.Forms.TextBox();
            this.lblCopy = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnLater = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.errPin = new System.Windows.Forms.ErrorProvider(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errPin)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.lkAuthorizeTwitter, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtPin, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblCopy, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(600, 173);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lkAuthorizeTwitter
            // 
            this.lkAuthorizeTwitter.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lkAuthorizeTwitter.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lkAuthorizeTwitter.Image = global::Sdl.Community.Productivity.PluginResources.twitter_24;
            this.lkAuthorizeTwitter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lkAuthorizeTwitter.Location = new System.Drawing.Point(86, 0);
            this.lkAuthorizeTwitter.Name = "lkAuthorizeTwitter";
            this.lkAuthorizeTwitter.Size = new System.Drawing.Size(427, 29);
            this.lkAuthorizeTwitter.TabIndex = 0;
            this.lkAuthorizeTwitter.TabStop = true;
            this.lkAuthorizeTwitter.Text = "Please click here to authorize posting on twitter";
            this.lkAuthorizeTwitter.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lkAuthorizeTwitter.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // txtPin
            // 
            this.txtPin.CausesValidation = false;
            this.txtPin.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPin.Location = new System.Drawing.Point(23, 73);
            this.txtPin.Multiline = true;
            this.txtPin.Name = "txtPin";
            this.txtPin.Size = new System.Drawing.Size(554, 54);
            this.txtPin.TabIndex = 2;
            this.txtPin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPin.Validating += new System.ComponentModel.CancelEventHandler(this.txtPin_Validating);
            // 
            // lblCopy
            // 
            this.lblCopy.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblCopy.AutoSize = true;
            this.lblCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCopy.Location = new System.Drawing.Point(41, 40);
            this.lblCopy.Name = "lblCopy";
            this.lblCopy.Size = new System.Drawing.Size(517, 20);
            this.lblCopy.TabIndex = 3;
            this.lblCopy.Text = "After you authorize the app please copy the pin code in the following box";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.btnLater, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnOk, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(23, 133);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(554, 37);
            this.tableLayoutPanel2.TabIndex = 6;
            // 
            // btnLater
            // 
            this.btnLater.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnLater.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnLater.Location = new System.Drawing.Point(379, 3);
            this.btnLater.Name = "btnLater";
            this.btnLater.Size = new System.Drawing.Size(73, 23);
            this.btnLater.TabIndex = 5;
            this.btnLater.Text = "Later";
            this.btnLater.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(102, 3);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(73, 23);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // errPin
            // 
            this.errPin.ContainerControl = this;
            // 
            // TwitterAccountSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(600, 173);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TwitterAccountSetup";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configure Twitter account";
            this.TopMost = true;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errPin)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.LinkLabel lkAuthorizeTwitter;
        private System.Windows.Forms.TextBox txtPin;
        private System.Windows.Forms.Label lblCopy;
        private System.Windows.Forms.ErrorProvider errPin;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnLater;
        private System.Windows.Forms.Button btnOk;
    }
}