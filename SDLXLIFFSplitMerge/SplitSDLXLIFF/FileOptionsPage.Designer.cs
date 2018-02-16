using Sdl.Utilities.SplitSDLXLIFF.Wizard;
namespace Sdl.Utilities.SplitSDLXLIFF
{
    partial class FileOptionsPage
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
            this.pSplitOptions = new System.Windows.Forms.Panel();
            this.lvSplitInFiles = new System.Windows.Forms.ListView();
            this.chFiles = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnSplitAddFromProject = new System.Windows.Forms.Button();
            this.btnSplitRemoveAll = new System.Windows.Forms.Button();
            this.btnSplitInFileRemove = new System.Windows.Forms.Button();
            this.btnSplitInFileAdd = new System.Windows.Forms.Button();
            this.btnSplitOutBrowse = new System.Windows.Forms.Button();
            this.tbSplitOut = new System.Windows.Forms.TextBox();
            this.lblSplitOut = new System.Windows.Forms.Label();
            this.lblSplitIn = new System.Windows.Forms.Label();
            this.pMergeOptions = new System.Windows.Forms.Panel();
            this.btnMergeInfoFile = new System.Windows.Forms.Button();
            this.tbMergeInfoFile = new System.Windows.Forms.TextBox();
            this.lblMergeInfoFile = new System.Windows.Forms.Label();
            this.btnMergeSplitFiles = new System.Windows.Forms.Button();
            this.tbMergeSplitFiles = new System.Windows.Forms.TextBox();
            this.lblMergeSpliFiles = new System.Windows.Forms.Label();
            this.btnMergeOrigFile = new System.Windows.Forms.Button();
            this.tbMergeOrigFile = new System.Windows.Forms.TextBox();
            this.lblMergeOrigFile = new System.Windows.Forms.Label();
            this.pSplitOptions.SuspendLayout();
            this.pMergeOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // Banner
            // 
            this.Banner.Dock = System.Windows.Forms.DockStyle.None;
            this.Banner.Size = new System.Drawing.Size(610, 64);
            // 
            // pSplitOptions
            // 
            this.pSplitOptions.Controls.Add(this.lvSplitInFiles);
            this.pSplitOptions.Controls.Add(this.btnSplitAddFromProject);
            this.pSplitOptions.Controls.Add(this.btnSplitRemoveAll);
            this.pSplitOptions.Controls.Add(this.btnSplitInFileRemove);
            this.pSplitOptions.Controls.Add(this.btnSplitInFileAdd);
            this.pSplitOptions.Controls.Add(this.btnSplitOutBrowse);
            this.pSplitOptions.Controls.Add(this.tbSplitOut);
            this.pSplitOptions.Controls.Add(this.lblSplitOut);
            this.pSplitOptions.Controls.Add(this.lblSplitIn);
            this.pSplitOptions.Location = new System.Drawing.Point(3, 68);
            this.pSplitOptions.Name = "pSplitOptions";
            this.pSplitOptions.Size = new System.Drawing.Size(604, 314);
            this.pSplitOptions.TabIndex = 1;
            // 
            // lvSplitInFiles
            // 
            this.lvSplitInFiles.AllowDrop = true;
            this.lvSplitInFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chFiles});
            this.lvSplitInFiles.FullRowSelect = true;
            this.lvSplitInFiles.GridLines = true;
            this.lvSplitInFiles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvSplitInFiles.HideSelection = false;
            this.lvSplitInFiles.Location = new System.Drawing.Point(10, 27);
            this.lvSplitInFiles.Name = "lvSplitInFiles";
            this.lvSplitInFiles.Size = new System.Drawing.Size(583, 197);
            this.lvSplitInFiles.TabIndex = 2;
            this.lvSplitInFiles.UseCompatibleStateImageBehavior = false;
            this.lvSplitInFiles.View = System.Windows.Forms.View.Details;
            this.lvSplitInFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvSplitInFiles_DragDrop);
            this.lvSplitInFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvSplitInFiles_DragEnter);
            this.lvSplitInFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvSplitInFiles_KeyDown);
            // 
            // chFiles
            // 
            this.chFiles.Text = "SDLXLIFF Files";
            this.chFiles.Width = 575;
            // 
            // btnSplitAddFromProject
            // 
            this.btnSplitAddFromProject.Location = new System.Drawing.Point(89, 230);
            this.btnSplitAddFromProject.Name = "btnSplitAddFromProject";
            this.btnSplitAddFromProject.Size = new System.Drawing.Size(135, 23);
            this.btnSplitAddFromProject.TabIndex = 4;
            this.btnSplitAddFromProject.Text = "&Load files from Project...";
            this.btnSplitAddFromProject.UseVisualStyleBackColor = true;
            this.btnSplitAddFromProject.Click += new System.EventHandler(this.btnSplitAddFromProject_Click);
            // 
            // btnSplitRemoveAll
            // 
            this.btnSplitRemoveAll.Location = new System.Drawing.Point(518, 229);
            this.btnSplitRemoveAll.Name = "btnSplitRemoveAll";
            this.btnSplitRemoveAll.Size = new System.Drawing.Size(75, 23);
            this.btnSplitRemoveAll.TabIndex = 6;
            this.btnSplitRemoveAll.Text = "&Remove All";
            this.btnSplitRemoveAll.UseVisualStyleBackColor = true;
            this.btnSplitRemoveAll.Click += new System.EventHandler(this.btnSplitRemoveAll_Click);
            // 
            // btnSplitInFileRemove
            // 
            this.btnSplitInFileRemove.Location = new System.Drawing.Point(407, 229);
            this.btnSplitInFileRemove.Name = "btnSplitInFileRemove";
            this.btnSplitInFileRemove.Size = new System.Drawing.Size(107, 23);
            this.btnSplitInFileRemove.TabIndex = 5;
            this.btnSplitInFileRemove.Text = "Remove &Selected";
            this.btnSplitInFileRemove.UseVisualStyleBackColor = true;
            this.btnSplitInFileRemove.Click += new System.EventHandler(this.btnSplitInFileRemove_Click);
            // 
            // btnSplitInFileAdd
            // 
            this.btnSplitInFileAdd.Location = new System.Drawing.Point(10, 230);
            this.btnSplitInFileAdd.Name = "btnSplitInFileAdd";
            this.btnSplitInFileAdd.Size = new System.Drawing.Size(75, 23);
            this.btnSplitInFileAdd.TabIndex = 3;
            this.btnSplitInFileAdd.Text = "&Add...";
            this.btnSplitInFileAdd.UseVisualStyleBackColor = true;
            this.btnSplitInFileAdd.Click += new System.EventHandler(this.btnSplitInFileAdd_Click);
            // 
            // btnSplitOutBrowse
            // 
            this.btnSplitOutBrowse.Location = new System.Drawing.Point(518, 280);
            this.btnSplitOutBrowse.Name = "btnSplitOutBrowse";
            this.btnSplitOutBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnSplitOutBrowse.TabIndex = 9;
            this.btnSplitOutBrowse.Text = "Br&owse...";
            this.btnSplitOutBrowse.UseVisualStyleBackColor = true;
            this.btnSplitOutBrowse.Click += new System.EventHandler(this.btnSplitOutBrowse_Click);
            // 
            // tbSplitOut
            // 
            this.tbSplitOut.AllowDrop = true;
            this.tbSplitOut.Location = new System.Drawing.Point(10, 282);
            this.tbSplitOut.Name = "tbSplitOut";
            this.tbSplitOut.Size = new System.Drawing.Size(504, 20);
            this.tbSplitOut.TabIndex = 8;
            this.tbSplitOut.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbSplitOut_DragDrop);
            this.tbSplitOut.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbSplitOut_DragEnter);
            // 
            // lblSplitOut
            // 
            this.lblSplitOut.AutoSize = true;
            this.lblSplitOut.Location = new System.Drawing.Point(8, 265);
            this.lblSplitOut.Name = "lblSplitOut";
            this.lblSplitOut.Size = new System.Drawing.Size(161, 13);
            this.lblSplitOut.TabIndex = 7;
            this.lblSplitOut.Text = "Specify &target folder for split files:";
            // 
            // lblSplitIn
            // 
            this.lblSplitIn.AutoSize = true;
            this.lblSplitIn.Location = new System.Drawing.Point(8, 11);
            this.lblSplitIn.Name = "lblSplitIn";
            this.lblSplitIn.Size = new System.Drawing.Size(83, 13);
            this.lblSplitIn.TabIndex = 1;
            this.lblSplitIn.Text = "Add &files to split:";
            // 
            // pMergeOptions
            // 
            this.pMergeOptions.Controls.Add(this.btnMergeInfoFile);
            this.pMergeOptions.Controls.Add(this.tbMergeInfoFile);
            this.pMergeOptions.Controls.Add(this.lblMergeInfoFile);
            this.pMergeOptions.Controls.Add(this.btnMergeSplitFiles);
            this.pMergeOptions.Controls.Add(this.tbMergeSplitFiles);
            this.pMergeOptions.Controls.Add(this.lblMergeSpliFiles);
            this.pMergeOptions.Controls.Add(this.btnMergeOrigFile);
            this.pMergeOptions.Controls.Add(this.tbMergeOrigFile);
            this.pMergeOptions.Controls.Add(this.lblMergeOrigFile);
            this.pMergeOptions.Location = new System.Drawing.Point(3, 68);
            this.pMergeOptions.Name = "pMergeOptions";
            this.pMergeOptions.Size = new System.Drawing.Size(604, 118);
            this.pMergeOptions.TabIndex = 2;
            // 
            // btnMergeInfoFile
            // 
            this.btnMergeInfoFile.Location = new System.Drawing.Point(516, 64);
            this.btnMergeInfoFile.Name = "btnMergeInfoFile";
            this.btnMergeInfoFile.Size = new System.Drawing.Size(75, 23);
            this.btnMergeInfoFile.TabIndex = 9;
            this.btnMergeInfoFile.Text = "Bro&wse...";
            this.btnMergeInfoFile.UseVisualStyleBackColor = true;
            this.btnMergeInfoFile.Click += new System.EventHandler(this.btnMergeInfoFile_Click);
            // 
            // tbMergeInfoFile
            // 
            this.tbMergeInfoFile.AllowDrop = true;
            this.tbMergeInfoFile.Location = new System.Drawing.Point(96, 66);
            this.tbMergeInfoFile.Name = "tbMergeInfoFile";
            this.tbMergeInfoFile.Size = new System.Drawing.Size(416, 20);
            this.tbMergeInfoFile.TabIndex = 8;
            this.tbMergeInfoFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbMergeInfoFile_DragDrop);
            this.tbMergeInfoFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbMergeInfoFile_DragEnter);
            // 
            // lblMergeInfoFile
            // 
            this.lblMergeInfoFile.AutoSize = true;
            this.lblMergeInfoFile.Location = new System.Drawing.Point(10, 69);
            this.lblMergeInfoFile.Name = "lblMergeInfoFile";
            this.lblMergeInfoFile.Size = new System.Drawing.Size(67, 13);
            this.lblMergeInfoFile.TabIndex = 7;
            this.lblMergeInfoFile.Text = "Split &Info file:";
            // 
            // btnMergeSplitFiles
            // 
            this.btnMergeSplitFiles.Location = new System.Drawing.Point(516, 38);
            this.btnMergeSplitFiles.Name = "btnMergeSplitFiles";
            this.btnMergeSplitFiles.Size = new System.Drawing.Size(75, 23);
            this.btnMergeSplitFiles.TabIndex = 6;
            this.btnMergeSplitFiles.Text = "Br&owse...";
            this.btnMergeSplitFiles.UseVisualStyleBackColor = true;
            this.btnMergeSplitFiles.Click += new System.EventHandler(this.btnMergeSplitFiles_Click);
            // 
            // tbMergeSplitFiles
            // 
            this.tbMergeSplitFiles.AllowDrop = true;
            this.tbMergeSplitFiles.Location = new System.Drawing.Point(96, 40);
            this.tbMergeSplitFiles.Name = "tbMergeSplitFiles";
            this.tbMergeSplitFiles.Size = new System.Drawing.Size(416, 20);
            this.tbMergeSplitFiles.TabIndex = 5;
            this.tbMergeSplitFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbMergeSplitFiles_DragDrop);
            this.tbMergeSplitFiles.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbMergeSplitFiles_DragEnter);
            // 
            // lblMergeSpliFiles
            // 
            this.lblMergeSpliFiles.AutoSize = true;
            this.lblMergeSpliFiles.Location = new System.Drawing.Point(10, 43);
            this.lblMergeSpliFiles.Name = "lblMergeSpliFiles";
            this.lblMergeSpliFiles.Size = new System.Drawing.Size(80, 13);
            this.lblMergeSpliFiles.TabIndex = 4;
            this.lblMergeSpliFiles.Text = "S&plit files folder:";
            // 
            // btnMergeOrigFile
            // 
            this.btnMergeOrigFile.Location = new System.Drawing.Point(516, 12);
            this.btnMergeOrigFile.Name = "btnMergeOrigFile";
            this.btnMergeOrigFile.Size = new System.Drawing.Size(75, 23);
            this.btnMergeOrigFile.TabIndex = 3;
            this.btnMergeOrigFile.Text = "B&rowse...";
            this.btnMergeOrigFile.UseVisualStyleBackColor = true;
            this.btnMergeOrigFile.Click += new System.EventHandler(this.btnMergeOrigFile_Click);
            // 
            // tbMergeOrigFile
            // 
            this.tbMergeOrigFile.AllowDrop = true;
            this.tbMergeOrigFile.Location = new System.Drawing.Point(96, 14);
            this.tbMergeOrigFile.Name = "tbMergeOrigFile";
            this.tbMergeOrigFile.Size = new System.Drawing.Size(416, 20);
            this.tbMergeOrigFile.TabIndex = 2;
            this.tbMergeOrigFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbMergeOrigFile_DragDrop);
            this.tbMergeOrigFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbMergeOrigFile_DragEnter);
            // 
            // lblMergeOrigFile
            // 
            this.lblMergeOrigFile.AutoSize = true;
            this.lblMergeOrigFile.Location = new System.Drawing.Point(10, 17);
            this.lblMergeOrigFile.Name = "lblMergeOrigFile";
            this.lblMergeOrigFile.Size = new System.Drawing.Size(61, 13);
            this.lblMergeOrigFile.TabIndex = 1;
            this.lblMergeOrigFile.Text = "Ori&ginal file:";
            // 
            // FileOptionsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pMergeOptions);
            this.Controls.Add(this.pSplitOptions);
            this.Name = "FileOptionsPage";
            this.Size = new System.Drawing.Size(610, 385);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.OptionsPage_SetActive);
            this.WizardNext += new Sdl.Utilities.SplitSDLXLIFF.Wizard.WizardPageEventHandler(this.FileOptionsPage_WizardNext);
            this.WizardPreNext += new Sdl.Utilities.SplitSDLXLIFF.Wizard.WizardPageEventHandler(this.FileOptionsPage_WizardPreNext);
            this.WizardBack += new Sdl.Utilities.SplitSDLXLIFF.Wizard.WizardPageEventHandler(this.FileOptionsPage_WizardBack);
            this.Controls.SetChildIndex(this.pSplitOptions, 0);
            this.Controls.SetChildIndex(this.pMergeOptions, 0);
            this.Controls.SetChildIndex(this.Banner, 0);
            this.pSplitOptions.ResumeLayout(false);
            this.pSplitOptions.PerformLayout();
            this.pMergeOptions.ResumeLayout(false);
            this.pMergeOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pSplitOptions;
        private System.Windows.Forms.Panel pMergeOptions;
        private System.Windows.Forms.Button btnSplitOutBrowse;
        private System.Windows.Forms.TextBox tbSplitOut;
        private System.Windows.Forms.Label lblSplitOut;
        private System.Windows.Forms.Label lblSplitIn;
        private System.Windows.Forms.Button btnSplitInFileAdd;
        private System.Windows.Forms.Button btnSplitInFileRemove;
        private System.Windows.Forms.Button btnMergeInfoFile;
        private System.Windows.Forms.TextBox tbMergeInfoFile;
        private System.Windows.Forms.Label lblMergeInfoFile;
        private System.Windows.Forms.Button btnMergeSplitFiles;
        private System.Windows.Forms.TextBox tbMergeSplitFiles;
        private System.Windows.Forms.Label lblMergeSpliFiles;
        private System.Windows.Forms.Button btnMergeOrigFile;
        private System.Windows.Forms.TextBox tbMergeOrigFile;
        private System.Windows.Forms.Label lblMergeOrigFile;
        private System.Windows.Forms.Button btnSplitRemoveAll;
        private System.Windows.Forms.Button btnSplitAddFromProject;
        private System.Windows.Forms.ListView lvSplitInFiles;
        private System.Windows.Forms.ColumnHeader chFiles;
    }
}
