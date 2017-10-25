namespace Sdl.Community.XmlReader.View
{
    partial class XmlReaderForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XmlReaderForm));
            this.groupBoxXmlFiles = new System.Windows.Forms.GroupBox();
            this.treeViewXmlFiles = new Sdl.Community.XmlReader.View.TreeViewMultipleSelection();
            this.contextMenuStripMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageListXmlFiles = new System.Windows.Forms.ImageList(this.components);
            this.buttonClean = new System.Windows.Forms.Button();
            this.labelReports = new System.Windows.Forms.Label();
            this.textBoxInstructions = new System.Windows.Forms.TextBox();
            this.buttonConvertToExcel = new System.Windows.Forms.Button();
            this.labelInstructions = new System.Windows.Forms.Label();
            this.groupBoxXmlFiles.SuspendLayout();
            this.contextMenuStripMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxXmlFiles
            // 
            this.groupBoxXmlFiles.AutoSize = true;
            this.groupBoxXmlFiles.Controls.Add(this.treeViewXmlFiles);
            this.groupBoxXmlFiles.Location = new System.Drawing.Point(12, 30);
            this.groupBoxXmlFiles.Name = "groupBoxXmlFiles";
            this.groupBoxXmlFiles.Size = new System.Drawing.Size(245, 248);
            this.groupBoxXmlFiles.TabIndex = 0;
            this.groupBoxXmlFiles.TabStop = false;
            this.groupBoxXmlFiles.Text = "  Group by languages  ";
            // 
            // treeViewXmlFiles
            // 
            this.treeViewXmlFiles.AllowDrop = true;
            this.treeViewXmlFiles.BackColor = System.Drawing.SystemColors.Control;
            this.treeViewXmlFiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewXmlFiles.ContextMenuStrip = this.contextMenuStripMenu;
            this.treeViewXmlFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewXmlFiles.HideSelection = false;
            this.treeViewXmlFiles.ImageIndex = 0;
            this.treeViewXmlFiles.ImageList = this.imageListXmlFiles;
            this.treeViewXmlFiles.Location = new System.Drawing.Point(3, 16);
            this.treeViewXmlFiles.Name = "treeViewXmlFiles";
            this.treeViewXmlFiles.SelectedImageIndex = 0;
            this.treeViewXmlFiles.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("treeViewXmlFiles.SelectedNodes")));
            this.treeViewXmlFiles.Size = new System.Drawing.Size(239, 229);
            this.treeViewXmlFiles.TabIndex = 0;
            this.treeViewXmlFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeViewXmlFiles_DragDrop);
            this.treeViewXmlFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeViewXmlFiles_DragEnter);
            // 
            // contextMenuStripMenu
            // 
            this.contextMenuStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.contextMenuStripMenu.Name = "contextMenuStripMenu";
            this.contextMenuStripMenu.Size = new System.Drawing.Size(108, 26);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // imageListXmlFiles
            // 
            this.imageListXmlFiles.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListXmlFiles.ImageStream")));
            this.imageListXmlFiles.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListXmlFiles.Images.SetKeyName(0, "default.ico");
            // 
            // buttonClean
            // 
            this.buttonClean.FlatAppearance.BorderSize = 0;
            this.buttonClean.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClean.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonClean.ForeColor = System.Drawing.SystemColors.Window;
            this.buttonClean.Location = new System.Drawing.Point(12, 288);
            this.buttonClean.Name = "buttonClean";
            this.buttonClean.Size = new System.Drawing.Size(245, 23);
            this.buttonClean.TabIndex = 1;
            this.buttonClean.Text = "Clean all";
            this.buttonClean.UseVisualStyleBackColor = true;
            this.buttonClean.Click += new System.EventHandler(this.buttonClean_Click);
            // 
            // labelReports
            // 
            this.labelReports.AutoSize = true;
            this.labelReports.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelReports.Location = new System.Drawing.Point(11, 11);
            this.labelReports.Name = "labelReports";
            this.labelReports.Size = new System.Drawing.Size(36, 16);
            this.labelReports.TabIndex = 1;
            this.labelReports.Text = "label1";
            // 
            // textBoxInstructions
            // 
            this.textBoxInstructions.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxInstructions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxInstructions.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxInstructions.Location = new System.Drawing.Point(270, 39);
            this.textBoxInstructions.Multiline = true;
            this.textBoxInstructions.Name = "textBoxInstructions";
            this.textBoxInstructions.Size = new System.Drawing.Size(79, 203);
            this.textBoxInstructions.TabIndex = 2;
            // 
            // buttonConvertToExcel
            // 
            this.buttonConvertToExcel.FlatAppearance.BorderSize = 0;
            this.buttonConvertToExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonConvertToExcel.Font = new System.Drawing.Font("Arial Narrow", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonConvertToExcel.ForeColor = System.Drawing.SystemColors.Window;
            this.buttonConvertToExcel.Location = new System.Drawing.Point(271, 266);
            this.buttonConvertToExcel.Name = "buttonConvertToExcel";
            this.buttonConvertToExcel.Size = new System.Drawing.Size(109, 45);
            this.buttonConvertToExcel.TabIndex = 3;
            this.buttonConvertToExcel.Text = "Excel";
            this.buttonConvertToExcel.UseVisualStyleBackColor = true;
            // 
            // labelInstructions
            // 
            this.labelInstructions.AutoSize = true;
            this.labelInstructions.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelInstructions.Location = new System.Drawing.Point(268, 13);
            this.labelInstructions.Name = "labelInstructions";
            this.labelInstructions.Size = new System.Drawing.Size(36, 16);
            this.labelInstructions.TabIndex = 4;
            this.labelInstructions.Text = "label1";
            // 
            // XmlReaderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 320);
            this.Controls.Add(this.buttonClean);
            this.Controls.Add(this.labelInstructions);
            this.Controls.Add(this.buttonConvertToExcel);
            this.Controls.Add(this.textBoxInstructions);
            this.Controls.Add(this.labelReports);
            this.Controls.Add(this.groupBoxXmlFiles);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "XmlReaderForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "XML Reader";
            this.Load += new System.EventHandler(this.XmlReaderForm_Load);
            this.groupBoxXmlFiles.ResumeLayout(false);
            this.contextMenuStripMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxXmlFiles;
        private System.Windows.Forms.ImageList imageListXmlFiles;
        private System.Windows.Forms.Label labelReports;
        private System.Windows.Forms.TextBox textBoxInstructions;
        private System.Windows.Forms.Button buttonConvertToExcel;
        private System.Windows.Forms.Label labelInstructions;
        private TreeViewMultipleSelection treeViewXmlFiles;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.Button buttonClean;
    }
}