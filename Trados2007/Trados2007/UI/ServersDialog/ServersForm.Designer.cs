namespace Sdl.Community.Trados2007.UI
{
    partial class ServersForm
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripAddButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripEditButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripDeleteButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripCheckServerButton = new System.Windows.Forms.ToolStripButton();
            this.cancelButton = new System.Windows.Forms.Button();
            this.serversGridView = new System.Windows.Forms.DataGridView();
            this._name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.serversGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripAddButton,
            this.toolStripEditButton,
            this.toolStripDeleteButton,
            this.toolStripSeparator1,
            this.toolStripCheckServerButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(516, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripAddButton
            // 
            this.toolStripAddButton.Image = global::Sdl.Community.Trados2007.PluginResources.server_add;
            this.toolStripAddButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripAddButton.Name = "toolStripAddButton";
            this.toolStripAddButton.Size = new System.Drawing.Size(58, 22);
            this.toolStripAddButton.Text = "&Add...";
            this.toolStripAddButton.Click += new System.EventHandler(this.OnAddButtonClick);
            // 
            // toolStripEditButton
            // 
            this.toolStripEditButton.Image = global::Sdl.Community.Trados2007.PluginResources.server_edit;
            this.toolStripEditButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripEditButton.Name = "toolStripEditButton";
            this.toolStripEditButton.Size = new System.Drawing.Size(56, 22);
            this.toolStripEditButton.Text = "&Edit...";
            this.toolStripEditButton.Click += new System.EventHandler(this.OnEditButtonClick);
            // 
            // toolStripDeleteButton
            // 
            this.toolStripDeleteButton.Image = global::Sdl.Community.Trados2007.PluginResources.server_delete;
            this.toolStripDeleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDeleteButton.Name = "toolStripDeleteButton";
            this.toolStripDeleteButton.Size = new System.Drawing.Size(60, 22);
            this.toolStripDeleteButton.Text = "&Delete";
            this.toolStripDeleteButton.Click += new System.EventHandler(this.OnDeleteButtonClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripCheckServerButton
            // 
            this.toolStripCheckServerButton.Image = global::Sdl.Community.Trados2007.PluginResources.server_connect;
            this.toolStripCheckServerButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripCheckServerButton.Name = "toolStripCheckServerButton";
            this.toolStripCheckServerButton.Size = new System.Drawing.Size(156, 22);
            this.toolStripCheckServerButton.Text = "Check &Server Availability";
            this.toolStripCheckServerButton.Click += new System.EventHandler(this.OnCheckServerButtonClick);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(429, 227);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "&Close";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // serversGridView
            // 
            this.serversGridView.AllowUserToAddRows = false;
            this.serversGridView.AllowUserToResizeRows = false;
            this.serversGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serversGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.serversGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.serversGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.serversGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.serversGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._name,
            this._status});
            this.serversGridView.Location = new System.Drawing.Point(12, 28);
            this.serversGridView.MultiSelect = false;
            this.serversGridView.Name = "serversGridView";
            this.serversGridView.ReadOnly = true;
            this.serversGridView.RowHeadersVisible = false;
            this.serversGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.serversGridView.Size = new System.Drawing.Size(492, 193);
            this.serversGridView.TabIndex = 2;
            this.serversGridView.SelectionChanged += new System.EventHandler(this.serversGridView_SelectionChanged);
            // 
            // _name
            // 
            this._name.HeaderText = "Name";
            this._name.Name = "_name";
            this._name.ReadOnly = true;
            // 
            // _status
            // 
            this._status.HeaderText = "Status";
            this._status.Name = "_status";
            this._status.ReadOnly = true;
            // 
            // ServersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(516, 262);
            this.Controls.Add(this.serversGridView);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.toolStrip1);
            this.MinimumSize = new System.Drawing.Size(532, 300);
            this.Name = "ServersForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SDL Trados 2007 Servers";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.serversGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripAddButton;
        private System.Windows.Forms.ToolStripButton toolStripEditButton;
        private System.Windows.Forms.ToolStripButton toolStripDeleteButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripCheckServerButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.DataGridView serversGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn _name;
        private System.Windows.Forms.DataGridViewTextBoxColumn _status;

    }
}