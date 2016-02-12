namespace Sdl.Community.ExcelTerminology.Ui
{
    partial class TermsList
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
            this.mainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.targetGridView = new System.Windows.Forms.DataGridView();
            this.Target = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Approved = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonsLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.confirmBtn = new System.Windows.Forms.Button();
            this.addBtn = new System.Windows.Forms.Button();
            this.sourceListView = new BrightIdeasSoftware.FastObjectListView();
            this.sourceColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.deleteLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.deleteBtn = new System.Windows.Forms.Button();
            this.mainLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.targetGridView)).BeginInit();
            this.buttonsLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sourceListView)).BeginInit();
            this.deleteLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainLayoutPanel
            // 
            this.mainLayoutPanel.ColumnCount = 2;
            this.mainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainLayoutPanel.Controls.Add(this.targetGridView, 1, 0);
            this.mainLayoutPanel.Controls.Add(this.buttonsLayoutPanel, 1, 1);
            this.mainLayoutPanel.Controls.Add(this.sourceListView, 0, 0);
            this.mainLayoutPanel.Controls.Add(this.deleteLayoutPanel, 0, 1);
            this.mainLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mainLayoutPanel.Name = "mainLayoutPanel";
            this.mainLayoutPanel.RowCount = 2;
            this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.mainLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainLayoutPanel.Size = new System.Drawing.Size(538, 548);
            this.mainLayoutPanel.TabIndex = 0;
            // 
            // targetGridView
            // 
            this.targetGridView.AllowUserToAddRows = false;
            this.targetGridView.BackgroundColor = System.Drawing.Color.White;
            this.targetGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.targetGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Target,
            this.Approved});
            this.targetGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.targetGridView.Location = new System.Drawing.Point(272, 3);
            this.targetGridView.Name = "targetGridView";
            this.targetGridView.RowHeadersVisible = false;
            this.targetGridView.Size = new System.Drawing.Size(263, 492);
            this.targetGridView.TabIndex = 0;
            // 
            // Target
            // 
            this.Target.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Target.DataPropertyName = "Term";
            this.Target.HeaderText = "Target";
            this.Target.Name = "Target";
            // 
            // Approved
            // 
            this.Approved.DataPropertyName = "Approved";
            this.Approved.HeaderText = "Approved";
            this.Approved.Name = "Approved";
            // 
            // buttonsLayoutPanel
            // 
            this.buttonsLayoutPanel.ColumnCount = 2;
            this.buttonsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.buttonsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.buttonsLayoutPanel.Controls.Add(this.confirmBtn, 1, 0);
            this.buttonsLayoutPanel.Controls.Add(this.addBtn, 0, 0);
            this.buttonsLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonsLayoutPanel.Location = new System.Drawing.Point(272, 501);
            this.buttonsLayoutPanel.Name = "buttonsLayoutPanel";
            this.buttonsLayoutPanel.RowCount = 1;
            this.buttonsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.buttonsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.buttonsLayoutPanel.Size = new System.Drawing.Size(263, 44);
            this.buttonsLayoutPanel.TabIndex = 2;
            // 
            // confirmBtn
            // 
            this.confirmBtn.Location = new System.Drawing.Point(134, 3);
            this.confirmBtn.Name = "confirmBtn";
            this.confirmBtn.Size = new System.Drawing.Size(75, 23);
            this.confirmBtn.TabIndex = 1;
            this.confirmBtn.Text = "Save";
            this.confirmBtn.UseVisualStyleBackColor = true;
            this.confirmBtn.Click += new System.EventHandler(this.confirmBtn_Click);
            // 
            // addBtn
            // 
            this.addBtn.Location = new System.Drawing.Point(3, 3);
            this.addBtn.Name = "addBtn";
            this.addBtn.Size = new System.Drawing.Size(75, 23);
            this.addBtn.TabIndex = 2;
            this.addBtn.Text = "Add";
            this.addBtn.UseVisualStyleBackColor = true;
            this.addBtn.Click += new System.EventHandler(this.addBtn_Click);
            // 
            // sourceListView
            // 
            this.sourceListView.AllColumns.Add(this.sourceColumn);
            this.sourceListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.sourceColumn});
            this.sourceListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceListView.EmptyListMsg = "There are no terms available. If there is a large number of terms it might take a" +
    " few seconds to load the terms.";
            this.sourceListView.FullRowSelect = true;
            this.sourceListView.GridLines = true;
            this.sourceListView.HideSelection = false;
            this.sourceListView.Location = new System.Drawing.Point(3, 3);
            this.sourceListView.MultiSelect = false;
            this.sourceListView.Name = "sourceListView";
            this.sourceListView.ShowGroups = false;
            this.sourceListView.Size = new System.Drawing.Size(263, 492);
            this.sourceListView.TabIndex = 3;
            this.sourceListView.UseCompatibleStateImageBehavior = false;
            this.sourceListView.View = System.Windows.Forms.View.Details;
            this.sourceListView.VirtualMode = true;
            this.sourceListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.sourceListView_ItemSelectionChanged);
            // 
            // sourceColumn
            // 
            this.sourceColumn.AspectName = "SearchText";
            this.sourceColumn.CellPadding = null;
            this.sourceColumn.FillsFreeSpace = true;
            this.sourceColumn.Width = 197;
            // 
            // deleteLayoutPanel
            // 
            this.deleteLayoutPanel.ColumnCount = 1;
            this.deleteLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.deleteLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.deleteLayoutPanel.Controls.Add(this.deleteBtn, 0, 0);
            this.deleteLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deleteLayoutPanel.Location = new System.Drawing.Point(3, 501);
            this.deleteLayoutPanel.Name = "deleteLayoutPanel";
            this.deleteLayoutPanel.RowCount = 1;
            this.deleteLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.deleteLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.deleteLayoutPanel.Size = new System.Drawing.Size(263, 44);
            this.deleteLayoutPanel.TabIndex = 4;
            // 
            // deleteBtn
            // 
            this.deleteBtn.Location = new System.Drawing.Point(3, 3);
            this.deleteBtn.Name = "deleteBtn";
            this.deleteBtn.Size = new System.Drawing.Size(75, 23);
            this.deleteBtn.TabIndex = 0;
            this.deleteBtn.Text = "Delete";
            this.deleteBtn.UseVisualStyleBackColor = true;
            this.deleteBtn.Click += new System.EventHandler(this.deleteBtn_Click);
            // 
            // TermsList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainLayoutPanel);
            this.Name = "TermsList";
            this.Size = new System.Drawing.Size(538, 548);
            this.mainLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.targetGridView)).EndInit();
            this.buttonsLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sourceListView)).EndInit();
            this.deleteLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView targetGridView;
        private System.Windows.Forms.TableLayoutPanel mainLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel buttonsLayoutPanel;
        private System.Windows.Forms.Button confirmBtn;
        private BrightIdeasSoftware.FastObjectListView sourceListView;
        private BrightIdeasSoftware.OLVColumn sourceColumn;
        private System.Windows.Forms.Button addBtn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Target;
        private System.Windows.Forms.DataGridViewTextBoxColumn Approved;
        private System.Windows.Forms.TableLayoutPanel deleteLayoutPanel;
        private System.Windows.Forms.Button deleteBtn;
    }
}