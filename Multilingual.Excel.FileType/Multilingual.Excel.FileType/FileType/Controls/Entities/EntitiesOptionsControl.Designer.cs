using Multilingual.XML.FileType.FileType.Settings.Entities;

namespace Multilingual.XML.FileType.FileType.Controls.Entities
{
	partial class EntitiesOptionsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EntitiesOptionsControl));
            this.entityMappingBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.settingNamesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.entitiesGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxEntityMappings = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridViewEntityMappings = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.charAsIntDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.readDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.writeDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.buttonAddEntityMapping = new System.Windows.Forms.Button();
            this.buttonEditEntityMapping = new System.Windows.Forms.Button();
            this.buttonCheckAll = new System.Windows.Forms.Button();
            this.buttonReset = new System.Windows.Forms.Button();
            this.buttonRemoveEntityMapping = new System.Windows.Forms.Button();
            this.buttonCheckNone = new System.Windows.Forms.Button();
            this.checkBoxConvertEntities = new System.Windows.Forms.CheckBox();
            this.groupBoxEntitySets = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.listViewEntitySets = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSearch = new System.Windows.Forms.TextBox();
            this.checkBoxConvertNumericEntityToPlaceholder = new System.Windows.Forms.CheckBox();
            this.checkBoxSkipInsideLocked = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.entityMappingBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingNamesBindingSource)).BeginInit();
            this.entitiesGroupBox.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxEntityMappings.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEntityMappings)).BeginInit();
            this.groupBoxEntitySets.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // entityMappingBindingSource
            // 
            this.entityMappingBindingSource.DataSource = typeof(EntityMapping);
            // 
            // settingNamesBindingSource
            // 
            //this.settingNamesBindingSource.DataSource = typeof(Sdl.FileTypeSupport.Filters.Common.Settings.SettingNames);
            // 
            // entitiesGroupBox
            // 
            this.entitiesGroupBox.Controls.Add(this.tableLayoutPanel1);
            resources.ApplyResources(this.entitiesGroupBox, "entitiesGroupBox");
            this.entitiesGroupBox.Name = "entitiesGroupBox";
            this.entitiesGroupBox.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.groupBoxEntityMappings, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxConvertEntities, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxEntitySets, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxConvertNumericEntityToPlaceholder, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxSkipInsideLocked, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // groupBoxEntityMappings
            // 
            resources.ApplyResources(this.groupBoxEntityMappings, "groupBoxEntityMappings");
            this.groupBoxEntityMappings.Controls.Add(this.tableLayoutPanel2);
            this.groupBoxEntityMappings.Name = "groupBoxEntityMappings";
            this.groupBoxEntityMappings.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.dataGridViewEntityMappings, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonAddEntityMapping, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.buttonEditEntityMapping, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.buttonCheckAll, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.buttonReset, 1, 6);
            this.tableLayoutPanel2.Controls.Add(this.buttonRemoveEntityMapping, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.buttonCheckNone, 1, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // dataGridViewEntityMappings
            // 
            this.dataGridViewEntityMappings.AllowUserToAddRows = false;
            this.dataGridViewEntityMappings.AllowUserToResizeRows = false;
            resources.ApplyResources(this.dataGridViewEntityMappings, "dataGridViewEntityMappings");
            this.dataGridViewEntityMappings.AutoGenerateColumns = false;
            this.dataGridViewEntityMappings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewEntityMappings.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridViewEntityMappings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewEntityMappings.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.charDataGridViewTextBoxColumn,
            this.charAsIntDataGridViewTextBoxColumn,
            this.readDataGridViewCheckBoxColumn,
            this.writeDataGridViewCheckBoxColumn});
            this.dataGridViewEntityMappings.DataSource = this.entityMappingBindingSource;
            this.dataGridViewEntityMappings.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewEntityMappings.Name = "dataGridViewEntityMappings";
            this.dataGridViewEntityMappings.RowHeadersVisible = false;
            this.tableLayoutPanel2.SetRowSpan(this.dataGridViewEntityMappings, 7);
            this.dataGridViewEntityMappings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewEntityMappings.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewEntityMappings_CellClick);
            this.dataGridViewEntityMappings.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewEntityMappings_CellDoubleClick);
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            resources.ApplyResources(this.nameDataGridViewTextBoxColumn, "nameDataGridViewTextBoxColumn");
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            // 
            // charDataGridViewTextBoxColumn
            // 
            this.charDataGridViewTextBoxColumn.DataPropertyName = "Char";
            resources.ApplyResources(this.charDataGridViewTextBoxColumn, "charDataGridViewTextBoxColumn");
            this.charDataGridViewTextBoxColumn.Name = "charDataGridViewTextBoxColumn";
            // 
            // charAsIntDataGridViewTextBoxColumn
            // 
            this.charAsIntDataGridViewTextBoxColumn.DataPropertyName = "CharAsInt";
            resources.ApplyResources(this.charAsIntDataGridViewTextBoxColumn, "charAsIntDataGridViewTextBoxColumn");
            this.charAsIntDataGridViewTextBoxColumn.Name = "charAsIntDataGridViewTextBoxColumn";
            // 
            // readDataGridViewCheckBoxColumn
            // 
            this.readDataGridViewCheckBoxColumn.DataPropertyName = "Read";
            resources.ApplyResources(this.readDataGridViewCheckBoxColumn, "readDataGridViewCheckBoxColumn");
            this.readDataGridViewCheckBoxColumn.Name = "readDataGridViewCheckBoxColumn";
            // 
            // writeDataGridViewCheckBoxColumn
            // 
            this.writeDataGridViewCheckBoxColumn.DataPropertyName = "Write";
            resources.ApplyResources(this.writeDataGridViewCheckBoxColumn, "writeDataGridViewCheckBoxColumn");
            this.writeDataGridViewCheckBoxColumn.Name = "writeDataGridViewCheckBoxColumn";
            // 
            // buttonAddEntityMapping
            // 
            resources.ApplyResources(this.buttonAddEntityMapping, "buttonAddEntityMapping");
            this.buttonAddEntityMapping.Name = "buttonAddEntityMapping";
            this.buttonAddEntityMapping.UseVisualStyleBackColor = true;
            this.buttonAddEntityMapping.Click += new System.EventHandler(this.buttonAddEntityMapping_Click);
            // 
            // buttonEditEntityMapping
            // 
            resources.ApplyResources(this.buttonEditEntityMapping, "buttonEditEntityMapping");
            this.buttonEditEntityMapping.Name = "buttonEditEntityMapping";
            this.buttonEditEntityMapping.UseVisualStyleBackColor = true;
            this.buttonEditEntityMapping.Click += new System.EventHandler(this.buttonEditEntityMapping_Click);
            // 
            // buttonCheckAll
            // 
            resources.ApplyResources(this.buttonCheckAll, "buttonCheckAll");
            this.buttonCheckAll.Name = "buttonCheckAll";
            this.buttonCheckAll.UseVisualStyleBackColor = true;
            this.buttonCheckAll.Click += new System.EventHandler(this.buttonCheckAll_Click);
            // 
            // buttonReset
            // 
            resources.ApplyResources(this.buttonReset, "buttonReset");
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // buttonRemoveEntityMapping
            // 
            resources.ApplyResources(this.buttonRemoveEntityMapping, "buttonRemoveEntityMapping");
            this.buttonRemoveEntityMapping.Name = "buttonRemoveEntityMapping";
            this.buttonRemoveEntityMapping.UseVisualStyleBackColor = true;
            this.buttonRemoveEntityMapping.Click += new System.EventHandler(this.buttonRemoveEntityMapping_Click);
            // 
            // buttonCheckNone
            // 
            resources.ApplyResources(this.buttonCheckNone, "buttonCheckNone");
            this.buttonCheckNone.Name = "buttonCheckNone";
            this.buttonCheckNone.UseVisualStyleBackColor = true;
            this.buttonCheckNone.Click += new System.EventHandler(this.buttonCheckNone_Click);
            // 
            // checkBoxConvertEntities
            // 
            resources.ApplyResources(this.checkBoxConvertEntities, "checkBoxConvertEntities");
            this.checkBoxConvertEntities.Name = "checkBoxConvertEntities";
            this.checkBoxConvertEntities.UseVisualStyleBackColor = true;
            this.checkBoxConvertEntities.CheckedChanged += new System.EventHandler(this.checkBoxConvertEntities_CheckedChanged);
            // 
            // groupBoxEntitySets
            // 
            resources.ApplyResources(this.groupBoxEntitySets, "groupBoxEntitySets");
            this.groupBoxEntitySets.Controls.Add(this.tableLayoutPanel3);
            this.groupBoxEntitySets.Name = "groupBoxEntitySets";
            this.groupBoxEntitySets.TabStop = false;
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.listViewEntitySets, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // listViewEntitySets
            // 
            this.listViewEntitySets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName});
            resources.ApplyResources(this.listViewEntitySets, "listViewEntitySets");
            this.listViewEntitySets.ForeColor = System.Drawing.SystemColors.WindowText;
            this.listViewEntitySets.HideSelection = false;
            this.listViewEntitySets.Name = "listViewEntitySets";
            this.listViewEntitySets.UseCompatibleStateImageBehavior = false;
            this.listViewEntitySets.View = System.Windows.Forms.View.Details;
            this.listViewEntitySets.SelectedIndexChanged += new System.EventHandler(this.listViewEntitySets_SelectedIndexChanged);
            // 
            // columnHeaderName
            // 
            resources.ApplyResources(this.columnHeaderName, "columnHeaderName");
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.textBoxSearch, 1, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBoxSearch
            // 
            resources.ApplyResources(this.textBoxSearch, "textBoxSearch");
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.TextChanged += new System.EventHandler(this.textBoxSearch_TextChanged);
            // 
            // checkBoxConvertNumericEntityToPlaceholder
            // 
            resources.ApplyResources(this.checkBoxConvertNumericEntityToPlaceholder, "checkBoxConvertNumericEntityToPlaceholder");
            this.checkBoxConvertNumericEntityToPlaceholder.Name = "checkBoxConvertNumericEntityToPlaceholder";
            this.checkBoxConvertNumericEntityToPlaceholder.UseVisualStyleBackColor = true;
            // 
            // checkBoxSkipInsideLocked
            // 
            resources.ApplyResources(this.checkBoxSkipInsideLocked, "checkBoxSkipInsideLocked");
            this.checkBoxSkipInsideLocked.Name = "checkBoxSkipInsideLocked";
            this.checkBoxSkipInsideLocked.UseVisualStyleBackColor = true;
            // 
            // EntitiesOptionsControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.entitiesGroupBox);
            this.Name = "EntitiesOptionsControl";
            ((System.ComponentModel.ISupportInitialize)(this.entityMappingBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingNamesBindingSource)).EndInit();
            this.entitiesGroupBox.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBoxEntityMappings.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEntityMappings)).EndInit();
            this.groupBoxEntitySets.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion
        private System.Windows.Forms.BindingSource entityMappingBindingSource;
        private System.Windows.Forms.BindingSource settingNamesBindingSource;
        private System.Windows.Forms.GroupBox entitiesGroupBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBoxEntityMappings;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridView dataGridViewEntityMappings;
        private System.Windows.Forms.Button buttonAddEntityMapping;
        private System.Windows.Forms.Button buttonEditEntityMapping;
        private System.Windows.Forms.Button buttonCheckAll;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Button buttonRemoveEntityMapping;
        private System.Windows.Forms.Button buttonCheckNone;
        private System.Windows.Forms.CheckBox checkBoxConvertEntities;
        private System.Windows.Forms.GroupBox groupBoxEntitySets;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.ListView listViewEntitySets;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.CheckBox checkBoxConvertNumericEntityToPlaceholder;
        private System.Windows.Forms.TextBox textBoxSearch;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn charDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn charAsIntDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn readDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn writeDataGridViewCheckBoxColumn;
        private System.Windows.Forms.CheckBox checkBoxSkipInsideLocked;
    }
}
