﻿namespace Sdl.Community.TermExcelerator.Ui
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.mainLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.targetGridView = new System.Windows.Forms.DataGridView();
            this.Target = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Approved = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsTarget = new System.Windows.Forms.BindingSource(this.components);
            this.buttonsLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.confirmBtn = new System.Windows.Forms.Button();
            this.addBtn = new System.Windows.Forms.Button();
            this.btnSync = new System.Windows.Forms.Button();
            this.sourceListView = new BrightIdeasSoftware.FastObjectListView();
            this.sourceColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.deleteLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.deleteBtn = new System.Windows.Forms.Button();
            this.mainLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.targetGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsTarget)).BeginInit();
            this.buttonsLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sourceListView)).BeginInit();
            this.deleteLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainLayoutPanel
            // 
            this.mainLayoutPanel.ColumnCount = 2;
            this.mainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.mainLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
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
            this.mainLayoutPanel.Size = new System.Drawing.Size(538, 548);
            this.mainLayoutPanel.TabIndex = 0;
            // 
            // targetGridView
            // 
            this.targetGridView.AutoGenerateColumns = false;
            this.targetGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.targetGridView.BackgroundColor = System.Drawing.Color.White;
            this.targetGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Target,
            this.Approved});
            this.targetGridView.DataSource = this.bsTarget;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.targetGridView.DefaultCellStyle = dataGridViewCellStyle1;
            this.targetGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.targetGridView.Location = new System.Drawing.Point(191, 3);
            this.targetGridView.Name = "targetGridView";
            this.targetGridView.RowHeadersVisible = false;
            this.targetGridView.Size = new System.Drawing.Size(344, 492);
            this.targetGridView.TabIndex = 0;
            this.targetGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.targetGridView_CellFormatting);
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
            // bsTarget
            // 
            this.bsTarget.CurrentItemChanged += new System.EventHandler(this.bsTarget_CurrentItemChanged);
            // 
            // buttonsLayoutPanel
            // 
            this.buttonsLayoutPanel.ColumnCount = 3;
            this.buttonsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.buttonsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.buttonsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.buttonsLayoutPanel.Controls.Add(this.confirmBtn, 1, 0);
            this.buttonsLayoutPanel.Controls.Add(this.addBtn, 0, 0);
            this.buttonsLayoutPanel.Controls.Add(this.btnSync, 2, 0);
            this.buttonsLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonsLayoutPanel.Location = new System.Drawing.Point(191, 501);
            this.buttonsLayoutPanel.Name = "buttonsLayoutPanel";
            this.buttonsLayoutPanel.RowCount = 1;
            this.buttonsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.buttonsLayoutPanel.Size = new System.Drawing.Size(344, 44);
            this.buttonsLayoutPanel.TabIndex = 2;
            // 
            // confirmBtn
            // 
            this.confirmBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.confirmBtn.Location = new System.Drawing.Point(148, 3);
            this.confirmBtn.Name = "confirmBtn";
            this.confirmBtn.Size = new System.Drawing.Size(75, 23);
            this.confirmBtn.TabIndex = 1;
            this.confirmBtn.Text = "Save Entry";
            this.confirmBtn.UseVisualStyleBackColor = true;
            this.confirmBtn.Click += new System.EventHandler(this.confirmBtn_Click);
            // 
            // addBtn
            // 
            this.addBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addBtn.Location = new System.Drawing.Point(35, 3);
            this.addBtn.Name = "addBtn";
            this.addBtn.Size = new System.Drawing.Size(75, 23);
            this.addBtn.TabIndex = 2;
            this.addBtn.Text = "Add";
            this.addBtn.UseVisualStyleBackColor = true;
            this.addBtn.Click += new System.EventHandler(this.addBtn_Click);
            // 
            // btnSync
            // 
            this.btnSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSync.Location = new System.Drawing.Point(267, 2);
            this.btnSync.Margin = new System.Windows.Forms.Padding(2);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(75, 23);
            this.btnSync.TabIndex = 3;
            this.btnSync.Text = "Sync";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // sourceListView
            // 
            this.sourceListView.AllColumns.Add(this.sourceColumn);
            this.sourceListView.CellEditUseWholeCell = false;
            this.sourceListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.sourceColumn});
            this.sourceListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceListView.EmptyListMsg = "There are no terms available. If there is a large number of terms it might take a" +
    " few seconds to load the terms.";
            this.sourceListView.FullRowSelect = true;
            this.sourceListView.GridLines = true;
            this.sourceListView.HideSelection = false;
            this.sourceListView.HighlightBackgroundColor = System.Drawing.Color.Empty;
            this.sourceListView.HighlightForegroundColor = System.Drawing.Color.Empty;
            this.sourceListView.Location = new System.Drawing.Point(3, 3);
            this.sourceListView.MultiSelect = false;
            this.sourceListView.Name = "sourceListView";
            this.sourceListView.ShowGroups = false;
            this.sourceListView.ShowItemToolTips = true;
            this.sourceListView.Size = new System.Drawing.Size(182, 492);
            this.sourceListView.TabIndex = 3;
            this.sourceListView.UseCompatibleStateImageBehavior = false;
            this.sourceListView.View = System.Windows.Forms.View.Details;
            this.sourceListView.VirtualMode = true;
            this.sourceListView.CellEditFinished += new BrightIdeasSoftware.CellEditEventHandler(this.sourceListView_CellEditFinished);
            this.sourceListView.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.sourceListView_CellEditStarting);
            this.sourceListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.sourceListView_ItemSelectionChanged);
            // 
            // sourceColumn
            // 
            this.sourceColumn.AspectName = "SearchText";
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
            this.deleteLayoutPanel.Size = new System.Drawing.Size(182, 44);
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
            ((System.ComponentModel.ISupportInitialize)(this.bsTarget)).EndInit();
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
        private System.Windows.Forms.BindingSource bsTarget;
        private System.Windows.Forms.TableLayoutPanel deleteLayoutPanel;
        private System.Windows.Forms.Button deleteBtn;
        private System.Windows.Forms.Button btnSync;
    }
}