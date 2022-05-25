﻿using Sdl.Community.TermExcelerator.Model;

namespace Sdl.Community.TermExcelerator.Ui
{
    partial class Settings
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
            this.components = new System.ComponentModel.Container();
            this.mainTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.headerLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.descriptionLbl = new System.Windows.Forms.Label();
            this.settingsLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.customSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.customSettingsLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.targetLanguageComboBox = new System.Windows.Forms.ComboBox();
            this.sourceLanguageComboBox = new System.Windows.Forms.ComboBox();
            this.approvedBox = new System.Windows.Forms.TextBox();
            this.targetBox = new System.Windows.Forms.TextBox();
            this.sourceBox = new System.Windows.Forms.TextBox();
            this.hasHeader = new System.Windows.Forms.CheckBox();
            this.customSourceLbl = new System.Windows.Forms.Label();
            this.customTargetLbl = new System.Windows.Forms.Label();
            this.customApprovedLbl = new System.Windows.Forms.Label();
            this.sourceLanguageLbl = new System.Windows.Forms.Label();
            this.targetLanguageLbl = new System.Windows.Forms.Label();
            this.textSeparatorLbl = new System.Windows.Forms.Label();
            this.separatorTextBox = new System.Windows.Forms.TextBox();
            this.pathLbl = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.submitBtn = new System.Windows.Forms.Button();
            this.browseBtn = new System.Windows.Forms.Button();
            this.chkIsReadOnly = new System.Windows.Forms.CheckBox();
            this.providerSettingsBindingSource6 = new System.Windows.Forms.BindingSource(this.components);
            this.providerSettingsBindingSource2 = new System.Windows.Forms.BindingSource(this.components);
            this.providerSettingsBindingSource5 = new System.Windows.Forms.BindingSource(this.components);
            this.providerSettingsBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.providerSettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.providerSettingsBindingSource4 = new System.Windows.Forms.BindingSource(this.components);
            this.providerSettingsBindingSource7 = new System.Windows.Forms.BindingSource(this.components);
            this.providerSettingsBindingSource8 = new System.Windows.Forms.BindingSource(this.components);
            this.providerSettingsBindingSource3 = new System.Windows.Forms.BindingSource(this.components);
            this.mainTableLayout.SuspendLayout();
            this.headerLayoutPanel.SuspendLayout();
            this.settingsLayoutPanel.SuspendLayout();
            this.customSettingsGroupBox.SuspendLayout();
            this.customSettingsLayoutPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.providerSettingsBindingSource6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.providerSettingsBindingSource2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.providerSettingsBindingSource5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.providerSettingsBindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.providerSettingsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.providerSettingsBindingSource4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.providerSettingsBindingSource7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.providerSettingsBindingSource8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.providerSettingsBindingSource3)).BeginInit();
            this.SuspendLayout();
            // 
            // mainTableLayout
            // 
            this.mainTableLayout.BackColor = System.Drawing.Color.White;
            this.mainTableLayout.ColumnCount = 1;
            this.mainTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.mainTableLayout.Controls.Add(this.headerLayoutPanel, 0, 0);
            this.mainTableLayout.Controls.Add(this.settingsLayoutPanel, 0, 1);
            this.mainTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayout.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayout.Name = "mainTableLayout";
            this.mainTableLayout.RowCount = 2;
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.mainTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainTableLayout.Size = new System.Drawing.Size(659, 357);
            this.mainTableLayout.TabIndex = 0;
            // 
            // headerLayoutPanel
            // 
            this.headerLayoutPanel.BackColor = System.Drawing.Color.White;
            this.headerLayoutPanel.ColumnCount = 1;
            this.headerLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.headerLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.headerLayoutPanel.Controls.Add(this.descriptionLbl, 0, 0);
            this.headerLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerLayoutPanel.Location = new System.Drawing.Point(3, 3);
            this.headerLayoutPanel.Name = "headerLayoutPanel";
            this.headerLayoutPanel.RowCount = 1;
            this.headerLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.headerLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.headerLayoutPanel.Size = new System.Drawing.Size(653, 64);
            this.headerLayoutPanel.TabIndex = 0;
            // 
            // descriptionLbl
            // 
            this.descriptionLbl.AutoSize = true;
            this.descriptionLbl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.descriptionLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.descriptionLbl.Location = new System.Drawing.Point(3, 0);
            this.descriptionLbl.Name = "descriptionLbl";
            this.descriptionLbl.Size = new System.Drawing.Size(647, 64);
            this.descriptionLbl.TabIndex = 0;
            this.descriptionLbl.Text = "label1";
            this.descriptionLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // settingsLayoutPanel
            // 
            this.settingsLayoutPanel.BackColor = System.Drawing.Color.White;
            this.settingsLayoutPanel.ColumnCount = 1;
            this.settingsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.settingsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.settingsLayoutPanel.Controls.Add(this.customSettingsGroupBox, 0, 0);
            this.settingsLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsLayoutPanel.Location = new System.Drawing.Point(3, 73);
            this.settingsLayoutPanel.Name = "settingsLayoutPanel";
            this.settingsLayoutPanel.RowCount = 1;
            this.settingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.settingsLayoutPanel.Size = new System.Drawing.Size(653, 283);
            this.settingsLayoutPanel.TabIndex = 1;
            // 
            // customSettingsGroupBox
            // 
            this.customSettingsGroupBox.AutoSize = true;
            this.customSettingsGroupBox.BackColor = System.Drawing.Color.White;
            this.customSettingsGroupBox.Controls.Add(this.customSettingsLayoutPanel);
            this.customSettingsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customSettingsGroupBox.Location = new System.Drawing.Point(3, 3);
            this.customSettingsGroupBox.Name = "customSettingsGroupBox";
            this.customSettingsGroupBox.Size = new System.Drawing.Size(647, 277);
            this.customSettingsGroupBox.TabIndex = 1;
            this.customSettingsGroupBox.TabStop = false;
            this.customSettingsGroupBox.Text = "Settings";
            // 
            // customSettingsLayoutPanel
            // 
            this.customSettingsLayoutPanel.ColumnCount = 2;
            this.customSettingsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.customSettingsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 545F));
            this.customSettingsLayoutPanel.Controls.Add(this.targetLanguageComboBox, 1, 6);
            this.customSettingsLayoutPanel.Controls.Add(this.sourceLanguageComboBox, 1, 5);
            this.customSettingsLayoutPanel.Controls.Add(this.approvedBox, 1, 4);
            this.customSettingsLayoutPanel.Controls.Add(this.targetBox, 1, 3);
            this.customSettingsLayoutPanel.Controls.Add(this.sourceBox, 1, 2);
            this.customSettingsLayoutPanel.Controls.Add(this.hasHeader, 0, 0);
            this.customSettingsLayoutPanel.Controls.Add(this.customSourceLbl, 0, 2);
            this.customSettingsLayoutPanel.Controls.Add(this.customTargetLbl, 0, 3);
            this.customSettingsLayoutPanel.Controls.Add(this.customApprovedLbl, 0, 4);
            this.customSettingsLayoutPanel.Controls.Add(this.sourceLanguageLbl, 0, 5);
            this.customSettingsLayoutPanel.Controls.Add(this.targetLanguageLbl, 0, 6);
            this.customSettingsLayoutPanel.Controls.Add(this.textSeparatorLbl, 0, 7);
            this.customSettingsLayoutPanel.Controls.Add(this.separatorTextBox, 1, 7);
            this.customSettingsLayoutPanel.Controls.Add(this.pathLbl, 0, 8);
            this.customSettingsLayoutPanel.Controls.Add(this.tableLayoutPanel1, 1, 8);
            this.customSettingsLayoutPanel.Controls.Add(this.chkIsReadOnly, 1, 0);
            this.customSettingsLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.customSettingsLayoutPanel.Location = new System.Drawing.Point(3, 16);
            this.customSettingsLayoutPanel.Name = "customSettingsLayoutPanel";
            this.customSettingsLayoutPanel.RowCount = 9;
            this.customSettingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.customSettingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.customSettingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.customSettingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.customSettingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.customSettingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.customSettingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.customSettingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.customSettingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.customSettingsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.customSettingsLayoutPanel.Size = new System.Drawing.Size(641, 258);
            this.customSettingsLayoutPanel.TabIndex = 0;
            // 
            // targetLanguageComboBox
            // 
            this.targetLanguageComboBox.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.providerSettingsBindingSource6, "TargetLanguage", true));
            this.targetLanguageComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.targetLanguageComboBox.FormattingEnabled = true;
            this.targetLanguageComboBox.Location = new System.Drawing.Point(99, 131);
            this.targetLanguageComboBox.Name = "targetLanguageComboBox";
            this.targetLanguageComboBox.Size = new System.Drawing.Size(539, 21);
            this.targetLanguageComboBox.TabIndex = 1;
            // 
            // sourceLanguageComboBox
            // 
            this.sourceLanguageComboBox.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.providerSettingsBindingSource2, "SourceLanguage", true));
            this.sourceLanguageComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceLanguageComboBox.FormattingEnabled = true;
            this.sourceLanguageComboBox.Location = new System.Drawing.Point(99, 104);
            this.sourceLanguageComboBox.Name = "sourceLanguageComboBox";
            this.sourceLanguageComboBox.Size = new System.Drawing.Size(539, 21);
            this.sourceLanguageComboBox.TabIndex = 1;
            // 
            // approvedBox
            // 
            this.approvedBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.providerSettingsBindingSource5, "ApprovedColumn", true));
            this.approvedBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.approvedBox.Location = new System.Drawing.Point(99, 78);
            this.approvedBox.Name = "approvedBox";
            this.approvedBox.Size = new System.Drawing.Size(539, 20);
            this.approvedBox.TabIndex = 1;
            // 
            // targetBox
            // 
            this.targetBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.providerSettingsBindingSource1, "TargetColumn", true));
            this.targetBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.targetBox.Location = new System.Drawing.Point(99, 52);
            this.targetBox.Name = "targetBox";
            this.targetBox.Size = new System.Drawing.Size(539, 20);
            this.targetBox.TabIndex = 1;
            // 
            // sourceBox
            // 
            this.sourceBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.providerSettingsBindingSource, "SourceColumn", true));
            this.sourceBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceBox.Location = new System.Drawing.Point(99, 26);
            this.sourceBox.Name = "sourceBox";
            this.sourceBox.Size = new System.Drawing.Size(539, 20);
            this.sourceBox.TabIndex = 1;
            // 
            // hasHeader
            // 
            this.hasHeader.AutoSize = true;
            this.hasHeader.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.providerSettingsBindingSource4, "HasHeader", true));
            this.hasHeader.Location = new System.Drawing.Point(3, 3);
            this.hasHeader.Name = "hasHeader";
            this.hasHeader.Size = new System.Drawing.Size(81, 17);
            this.hasHeader.TabIndex = 6;
            this.hasHeader.Text = "Has header";
            this.hasHeader.UseVisualStyleBackColor = true;
            // 
            // customSourceLbl
            // 
            this.customSourceLbl.AutoSize = true;
            this.customSourceLbl.Location = new System.Drawing.Point(3, 23);
            this.customSourceLbl.Name = "customSourceLbl";
            this.customSourceLbl.Size = new System.Drawing.Size(78, 13);
            this.customSourceLbl.TabIndex = 8;
            this.customSourceLbl.Text = "Source column";
            // 
            // customTargetLbl
            // 
            this.customTargetLbl.AutoSize = true;
            this.customTargetLbl.Location = new System.Drawing.Point(3, 49);
            this.customTargetLbl.Name = "customTargetLbl";
            this.customTargetLbl.Size = new System.Drawing.Size(75, 13);
            this.customTargetLbl.TabIndex = 9;
            this.customTargetLbl.Text = "Target column";
            // 
            // customApprovedLbl
            // 
            this.customApprovedLbl.AutoSize = true;
            this.customApprovedLbl.Location = new System.Drawing.Point(3, 75);
            this.customApprovedLbl.Name = "customApprovedLbl";
            this.customApprovedLbl.Size = new System.Drawing.Size(90, 13);
            this.customApprovedLbl.TabIndex = 10;
            this.customApprovedLbl.Text = "Approved column";
            // 
            // sourceLanguageLbl
            // 
            this.sourceLanguageLbl.AutoSize = true;
            this.sourceLanguageLbl.Location = new System.Drawing.Point(3, 101);
            this.sourceLanguageLbl.Name = "sourceLanguageLbl";
            this.sourceLanguageLbl.Size = new System.Drawing.Size(88, 13);
            this.sourceLanguageLbl.TabIndex = 11;
            this.sourceLanguageLbl.Text = "Source language";
            // 
            // targetLanguageLbl
            // 
            this.targetLanguageLbl.AutoSize = true;
            this.targetLanguageLbl.Location = new System.Drawing.Point(3, 128);
            this.targetLanguageLbl.Name = "targetLanguageLbl";
            this.targetLanguageLbl.Size = new System.Drawing.Size(85, 13);
            this.targetLanguageLbl.TabIndex = 12;
            this.targetLanguageLbl.Text = "Target language";
            // 
            // textSeparatorLbl
            // 
            this.textSeparatorLbl.AutoSize = true;
            this.textSeparatorLbl.Location = new System.Drawing.Point(3, 155);
            this.textSeparatorLbl.Name = "textSeparatorLbl";
            this.textSeparatorLbl.Size = new System.Drawing.Size(75, 13);
            this.textSeparatorLbl.TabIndex = 13;
            this.textSeparatorLbl.Text = "Text separator";
            // 
            // separatorTextBox
            // 
            this.separatorTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.providerSettingsBindingSource7, "Separator", true));
            this.separatorTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.separatorTextBox.Location = new System.Drawing.Point(99, 158);
            this.separatorTextBox.Name = "separatorTextBox";
            this.separatorTextBox.Size = new System.Drawing.Size(539, 20);
            this.separatorTextBox.TabIndex = 1;
            // 
            // pathLbl
            // 
            this.pathLbl.AutoSize = true;
            this.pathLbl.Location = new System.Drawing.Point(3, 181);
            this.pathLbl.Name = "pathLbl";
            this.pathLbl.Size = new System.Drawing.Size(29, 13);
            this.pathLbl.TabIndex = 14;
            this.pathLbl.Text = "Path";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.Controls.Add(this.pathTextBox, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.submitBtn, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.browseBtn, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(99, 184);
            this.tableLayoutPanel1.MinimumSize = new System.Drawing.Size(0, 52);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(539, 71);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // pathTextBox
            // 
            this.pathTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.providerSettingsBindingSource8, "TermFilePath", true));
            this.pathTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pathTextBox.Location = new System.Drawing.Point(0, 0);
            this.pathTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.pathTextBox.Multiline = true;
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.pathTextBox.Size = new System.Drawing.Size(377, 71);
            this.pathTextBox.TabIndex = 1;
            // 
            // submitBtn
            // 
            this.submitBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.submitBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.submitBtn.Location = new System.Drawing.Point(464, 48);
            this.submitBtn.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.submitBtn.Name = "submitBtn";
            this.submitBtn.Size = new System.Drawing.Size(67, 23);
            this.submitBtn.TabIndex = 1;
            this.submitBtn.Text = "Submit";
            this.submitBtn.UseVisualStyleBackColor = true;
            // 
            // browseBtn
            // 
            this.browseBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.browseBtn.Location = new System.Drawing.Point(384, 48);
            this.browseBtn.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.browseBtn.Name = "browseBtn";
            this.browseBtn.Size = new System.Drawing.Size(66, 23);
            this.browseBtn.TabIndex = 1;
            this.browseBtn.Text = "Browse";
            this.browseBtn.UseVisualStyleBackColor = true;
            this.browseBtn.Click += new System.EventHandler(this.browseBtn_Click);
            // 
            // chkIsReadOnly
            // 
            this.chkIsReadOnly.AutoSize = true;
            this.chkIsReadOnly.Location = new System.Drawing.Point(98, 2);
            this.chkIsReadOnly.Margin = new System.Windows.Forms.Padding(2);
            this.chkIsReadOnly.Name = "chkIsReadOnly";
            this.chkIsReadOnly.Size = new System.Drawing.Size(87, 17);
            this.chkIsReadOnly.TabIndex = 16;
            this.chkIsReadOnly.Text = "Is Read Only";
            this.chkIsReadOnly.UseVisualStyleBackColor = true;
            // 
            // providerSettingsBindingSource6
            // 
            this.providerSettingsBindingSource6.DataSource = typeof(ProviderSettings);
            // 
            // providerSettingsBindingSource2
            // 
            this.providerSettingsBindingSource2.DataSource = typeof(ProviderSettings);
            // 
            // providerSettingsBindingSource5
            // 
            this.providerSettingsBindingSource5.DataSource = typeof(ProviderSettings);
            // 
            // providerSettingsBindingSource1
            // 
            this.providerSettingsBindingSource1.DataSource = typeof(ProviderSettings);
            // 
            // providerSettingsBindingSource
            // 
            this.providerSettingsBindingSource.DataSource = typeof(ProviderSettings);
            // 
            // providerSettingsBindingSource4
            // 
            this.providerSettingsBindingSource4.DataSource = typeof(ProviderSettings);
            // 
            // providerSettingsBindingSource7
            // 
            this.providerSettingsBindingSource7.DataSource = typeof(ProviderSettings);
            // 
            // providerSettingsBindingSource8
            // 
            this.providerSettingsBindingSource8.DataSource = typeof(ProviderSettings);
            // 
            // providerSettingsBindingSource3
            // 
            this.providerSettingsBindingSource3.DataSource = typeof(ProviderSettings);
            // 
            // Settings
            // 
            this.AcceptButton = this.submitBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 357);
            this.Controls.Add(this.mainTableLayout);
            this.MinimumSize = new System.Drawing.Size(675, 396);
            this.Name = "Settings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.mainTableLayout.ResumeLayout(false);
            this.headerLayoutPanel.ResumeLayout(false);
            this.headerLayoutPanel.PerformLayout();
            this.settingsLayoutPanel.ResumeLayout(false);
            this.settingsLayoutPanel.PerformLayout();
            this.customSettingsGroupBox.ResumeLayout(false);
            this.customSettingsLayoutPanel.ResumeLayout(false);
            this.customSettingsLayoutPanel.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.providerSettingsBindingSource6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.providerSettingsBindingSource2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.providerSettingsBindingSource5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.providerSettingsBindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.providerSettingsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.providerSettingsBindingSource4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.providerSettingsBindingSource7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.providerSettingsBindingSource8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.providerSettingsBindingSource3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainTableLayout;
        private System.Windows.Forms.TableLayoutPanel headerLayoutPanel;
        private System.Windows.Forms.TableLayoutPanel settingsLayoutPanel;
        private System.Windows.Forms.GroupBox customSettingsGroupBox;
        private System.Windows.Forms.TableLayoutPanel customSettingsLayoutPanel;
        private System.Windows.Forms.TextBox sourceBox;
        private System.Windows.Forms.TextBox targetBox;
        private System.Windows.Forms.TextBox approvedBox;
        private System.Windows.Forms.ComboBox sourceLanguageComboBox;
        private System.Windows.Forms.ComboBox targetLanguageComboBox;
        private System.Windows.Forms.BindingSource providerSettingsBindingSource;
        private System.Windows.Forms.BindingSource providerSettingsBindingSource1;
        private System.Windows.Forms.BindingSource providerSettingsBindingSource5;
        private System.Windows.Forms.BindingSource providerSettingsBindingSource2;
        private System.Windows.Forms.BindingSource providerSettingsBindingSource6;
        private System.Windows.Forms.CheckBox hasHeader;
        private System.Windows.Forms.BindingSource providerSettingsBindingSource4;
        private System.Windows.Forms.BindingSource providerSettingsBindingSource7;
        private System.Windows.Forms.BindingSource providerSettingsBindingSource3;
        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.BindingSource providerSettingsBindingSource8;
        private System.Windows.Forms.Button browseBtn;
        private System.Windows.Forms.Button submitBtn;
        private System.Windows.Forms.Label targetLanguageLbl;
        private System.Windows.Forms.Label sourceLanguageLbl;
        private System.Windows.Forms.Label customApprovedLbl;
        private System.Windows.Forms.Label customTargetLbl;
        private System.Windows.Forms.Label customSourceLbl;
        private System.Windows.Forms.TextBox separatorTextBox;
        private System.Windows.Forms.Label textSeparatorLbl;
        private System.Windows.Forms.Label pathLbl;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox chkIsReadOnly;
		private System.Windows.Forms.Label descriptionLbl;
	}
}
